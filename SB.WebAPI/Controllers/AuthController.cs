using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using SB.WebAPI.DTO;
using SB.WebAPI.DTO.AuthDTO;

namespace SB.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISupporterService _service;

        public AuthController(ISupporterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Checks if specified username and password is correct
        /// </summary>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<Login_DTO_Out> Login([FromBody] Login_DTO_In data)
        {
            try
            {
                if (_service.CheckLogin(data.Username, data.Password) > 0)
                {
                    var returnObj = new Login_DTO_Out
                    {
                        Status = "OK",
                        BasicAuthString =
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(data.Username + ":" + data.Password))
                    };
                    return Ok(returnObj);
                }

                return Unauthorized(new Error_DTO_Out(401, "Incorrect username and/or password!"));
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

        /// <summary>
        /// Gets a list of all supporters
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Supporter_DTO_Out>> GetAll()
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

        /// <summary>
        /// Gets one supporter - specified by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public ActionResult<Supporter_DTO_Out> GetOne(int id)
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

        /// <summary>
        /// Create a supporter
        /// </summary>
        [HttpPost]
        public ActionResult<Supporter_DTO_Out> CreateSupporter([FromBody] Supporter_DTO_In data)
        {
            try
            {
                return Ok(Conversion(_service.Store(new Supporter
                {
                    Username = data.Username,
                    Password = data.Password,
                    UserInfo = new UserInfo
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

        /// <summary>
        /// Update details for a supporter
        /// </summary>
        [HttpPut("{id:int}")]
        public ActionResult<Supporter_DTO_Out> UpdateSupporter(int id, [FromBody] Supporter_DTO_In data)
        {
            try
            {
                return Ok(Conversion(_service.Update(new Supporter
                {
                    Id = id,
                    Username = data.Username,
                    Password = data.Password,
                    UserInfo = new UserInfo
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
            catch (FileNotFoundException e)
            {
                return NotFound(new Error_DTO_Out(404, e.Message));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error_DTO_Out(500, ApiStrings.InternalServerError));
            }
        }

        /// <summary>
        /// Delete a supporter
        /// </summary>
        [HttpDelete("{id:int}")]
        public ActionResult<Supporter_DTO_Out> Delete(int id)
        {
            try
            {
                return Ok(Conversion(_service.Delete(new Supporter {Id = id})));
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

        private Supporter_DTO_Out Conversion(Supporter obj)
        {
            return new Supporter_DTO_Out
            {
                Id = obj.Id,
                Username = obj.Username,
                Email = obj.UserInfo.Email,
                FirstName = obj.UserInfo.FirstName,
                LastName = obj.UserInfo.LastName,
                PhoneNumber = obj.UserInfo.PhoneNumber
            };
        }
    }
}