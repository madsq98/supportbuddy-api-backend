using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.WebAPI.DTO;
using SB.WebAPI.DTO.TicketDTO;

namespace SB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly I_RW_Service<Ticket> _service;

        public TicketController(I_RW_Service<Ticket> service)
        {
            _service = service;
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
                return StatusCode(500, new Error_DTO_Out(500, e.Message));
            }
        }

        private Ticket_DTO_Out Conversion(Ticket obj)
        {
            return new Ticket_DTO_Out
            {
                Id = obj.Id,
                Subject = obj.Subject,
                Message = obj.Message,
                Email = obj.UserInfo.Email,
                FirstName = obj.UserInfo.FirstName,
                LastName = obj.UserInfo.LastName,
                PhoneNumber = obj.UserInfo.PhoneNumber
            };
        }
    }
}
