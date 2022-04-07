using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.WebAPI.DTO;
using SB.WebAPI.DTO.TicketDTO;
using SB.WebAPI.DTO.TicketDTO.AnswerDTO;
using SB.WebAPI.Utilities;

namespace SB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;
        private readonly ISupporterService _supporterService;

        private readonly BasicAuthorizationReader _authorizationReader;

        public TicketController(ITicketService service, ISupporterService supporterService)
        {
            _service = service;
            _supporterService = supporterService;

            _authorizationReader = new BasicAuthorizationReader();
        }

        // GET: api/Ticket
        [HttpGet]
        public ActionResult<IEnumerable<Ticket_DTO_Out>> Get()
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

        // GET: api/Ticket/5
        [HttpGet("{id:int}", Name = "Get")]
        public ActionResult<Ticket_DTO_Out> Get(int id)
        {
            try
            {
                return Ok(Conversion(_service.GetOneById(id)));
            }
            catch (InvalidDataException e)
            {
                return BadRequest(new Error_DTO_Out(400, e.Message));
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new Error_DTO_Out(404, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }

        // POST: api/Ticket
        [HttpPost]
        public ActionResult<Ticket_DTO_Out> Post([FromBody] Ticket_DTO_In value)
        {
            try
            {
                return Ok(Conversion(_service.Store(new Ticket
                {
                    Subject = value.Subject,
                    Message = value.Message,
                    UserInfo = new UserInfo
                    {
                        Email = value.Email,
                        FirstName = value.FirstName ?? "",
                        LastName = value.LastName ?? "",
                        PhoneNumber = value.PhoneNumber
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

        // PUT: api/Ticket/5
        [HttpPut("{id:int}")]
        public ActionResult<Ticket_DTO_Out> Put(int id, [FromBody] Ticket_DTO_In value)
        {
            try
            {
                return Ok(Conversion(_service.Update(new Ticket
                {
                    Id = id,
                    Subject = value.Subject,
                    Message = value.Message,
                    UserInfo = new UserInfo
                    {
                        Email = value.Email,
                        FirstName = value.FirstName,
                        LastName = value.LastName,
                        PhoneNumber = value.PhoneNumber
                    }
                })));
            }
            catch (InvalidDataException e)
            {
                return BadRequest(new Error_DTO_Out(400, e.Message));
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new Error_DTO_Out(404, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }

        // DELETE: api/Ticket/5
        [HttpDelete("{id:int}")]
        public ActionResult<Ticket_DTO_Out> Delete(int id)
        {
            try
            {
                return Ok(Conversion(_service.Delete(new Ticket {Id = id})));
            }
            catch (InvalidDataException e)
            {
                return BadRequest(new Error_DTO_Out(400, e.Message));
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new Error_DTO_Out(404, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }
        
        // POST: api/Ticket/6
        [HttpPost("{id:int}")]
        public ActionResult<Ticket_DTO_Out> PostAnswer(int id, [FromBody] Answer_DTO_In obj)
        {
            try
            {
                var supporterUsername = _authorizationReader.GetUsername(HttpContext);
                var supporterPassword = _authorizationReader.GetPassword(HttpContext);

                try
                {
                    var supporterUserId = _supporterService.CheckLogin(supporterUsername, supporterPassword);
                    var supporterUserInfo = _supporterService.GetOneById(supporterUserId).UserInfo;

                    return Ok(Conversion(_service.AddAnswer(new Ticket {Id = id}, new Answer {Message = obj.Message}, supporterUserInfo.Id)));
                }
                catch (InvalidDataException e)
                {
                    return Ok(Conversion(_service.AddAnswer(new Ticket {Id = id}, new Answer {Message = obj.Message})));
                }
            }
            catch (InvalidDataException e)
            {
                return BadRequest(new Error_DTO_Out(400, e.Message));
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new Error_DTO_Out(404, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }
        
        // POST: api/ticket/6/close
        [HttpPost("{id:int}/close")]
        public ActionResult<Ticket_DTO_Out> CloseTicket(int id)
        {
            try
            {
                return Ok(Conversion(_service.CloseTicket(new Ticket {Id = id})));
            }
            catch (InvalidDataException e)
            {
                return BadRequest(new Error_DTO_Out(400, e.Message));
            }
            catch (FileNotFoundException e)
            {
                return NotFound(new Error_DTO_Out(404, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }

        private Ticket_DTO_Out Conversion(Ticket obj)
        {
            return new Ticket_DTO_Out
            {
                Id = obj.Id,
                Status = (obj.Status == TicketStatus.Open) ? "Open" : "Closed",
                Subject = obj.Subject,
                Message = obj.Message,
                Email = obj.UserInfo.Email,
                FirstName = obj.UserInfo.FirstName,
                LastName = obj.UserInfo.LastName,
                PhoneNumber = obj.UserInfo.PhoneNumber,
                Answers = obj.Answers.Select(answer => new Answer_DTO_Out
                {
                    Id = answer.Id,
                    AuthorFirstName = answer.Author.FirstName,
                    AuthorLastName = answer.Author.LastName,
                    Message = answer.Message,
                    TimeStamp = answer.TimeStamp
                }).ToList(),
                TimeStamp = obj.TimeStamp
            };
        }
    }
}
