using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SB.WebAPI.DTO;
using SB.WebAPI.DTO.LiveChatDTO;
using SB.WebAPI.Util;
using SB.WebAPI.Utilities;

namespace SB.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LiveChatController : ControllerBase
    {
        private readonly ILogger<LiveChatController> _logger;
        private readonly ILiveChatService _service;
        private readonly ISupporterService _supporterService;

        private readonly BasicAuthorizationReader _authorizationReader;

        private readonly IWebsocketHandler _websocketHandler;

        public LiveChatController(ILogger<LiveChatController> logger, ILiveChatService service, ISupporterService supporterService, IWebsocketHandler websocketHandler)
        {
            _logger = logger;
            _service = service;
            _supporterService = supporterService;
            _websocketHandler = websocketHandler;

            _authorizationReader = new BasicAuthorizationReader();
        }

        [HttpGet("/api/livechat")]
        public ActionResult<IEnumerable<LiveChat_DTO_Out>> GetAll()
        {
            try
            {
                return Ok(_service.GetAll().Select(Conversion));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }

        [HttpPost("/api/livechat")]
        public ActionResult<LiveChat_DTO_Out> Create([FromBody] LiveChat_DTO_In data)
        {
            try
            {
                return Ok(Conversion(_service.Store(new LiveChat
                {
                    Author = new UserInfo
                    {
                        Email = data.Email,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        PhoneNumber = data.PhoneNumber
                    }
                })));
            }
            catch (InvalidDataException e)
            {
                return BadRequest(new Error_DTO_Out(400, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }

        [HttpGet("/ws/{id:int}")]
        public async Task Get(int id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    var liveChatObject = _service.GetOneById(id);

                    using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    _logger.Log(LogLevel.Information, "WebSocket connection established");
                    
                    var supporterUsername = _authorizationReader.GetUsername(HttpContext);
                    var supporterPassword = _authorizationReader.GetPassword(HttpContext);

                    try
                    {
                        var supporterId = _supporterService.CheckLogin(supporterUsername, supporterPassword);
                        var supporterObject = _supporterService.GetOneById(supporterId);

                        await _websocketHandler.Handle(Guid.NewGuid(), webSocket, _service, liveChatObject,
                            supporterObject);
                    }
                    catch (InvalidDataException e)
                    {
                        await _websocketHandler.Handle(Guid.NewGuid(), webSocket, _service, liveChatObject, null);
                    }
                }
                catch (InvalidDataException e)
                {
                    HttpContext.Response.StatusCode = 400;
                }
                catch (FileNotFoundException e)
                {
                    HttpContext.Response.StatusCode = 404;
                }
                catch (Exception e)
                {
                    HttpContext.Response.StatusCode = 500;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        /*
        private async Task Echo(WebSocket webSocket, LiveChat obj, Supporter supporter = null)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            _logger.Log(LogLevel.Information, "Message recieved from Client");

            while (!result.CloseStatus.HasValue)
            {
                var msgToSend = Encoding.UTF8.GetString(buffer);

                string newMessage;
                if (supporter != null)
                {
                    _service.AddMessage(obj, new Message {Text = msgToSend}, supporter.Id);
                    newMessage = supporter.UserInfo.FirstName + " " + supporter.UserInfo.LastName + ": " + msgToSend;
                }
                else
                {
                    _service.AddMessage(obj, new Message {Text = msgToSend});
                    newMessage = obj.Author.FirstName + " " + obj.Author.LastName + ": " + msgToSend;
                }


                var serverMsg = Encoding.UTF8.GetBytes(newMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType,
                    result.EndOfMessage, CancellationToken.None);

                _logger.Log(LogLevel.Information, "Message sent to Client");

                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message recieved from Client");
            }
            
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.Log(LogLevel.Information, "WebSocket connection closed");
        }
        */

        private LiveChat_DTO_Out Conversion(LiveChat obj)
        {
            return new LiveChat_DTO_Out
            {
                Id = obj.Id,
                FirstName = obj.Author.FirstName,
                LastName = obj.Author.LastName,
                Email = obj.Author.Email,
                PhoneNumber = obj.Author.PhoneNumber,
                Status = (obj.Open) ? "Open" : "Closed",
                Messages = obj.Messages.Select(msg => new Message_DTO_Out
                {
                    Id = msg.Id,
                    FirstName = msg.Author.FirstName,
                    LastName = msg.Author.LastName,
                    Email = msg.Author.Email,
                    PhoneNumber = msg.Author.PhoneNumber,
                    Message = msg.Text,
                    Timestamp = msg.Timestamp
                }).ToList()
            };
        }
    }
}