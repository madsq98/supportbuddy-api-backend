using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SB.WebAPI.DTO;
using SB.WebAPI.DTO.AttachmentDTO;

namespace SB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _service;
        private IConfiguration Configuration { get; }

        public AttachmentController(IAttachmentService service, IConfiguration configuration)
        {
            _service = service;
            Configuration = configuration;
        }
        
        [HttpPost]
        public async Task<ActionResult<Attachment_DTO_Out>> Upload(IFormFile file)
        {
            try
            {
                if (file.Length <= 0) return BadRequest("File is null");

                var fileExtension = Path.GetExtension(file.FileName);

                var folderName = Path.Combine("Uploads");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var fileName = Path.GetRandomFileName() + fileExtension;
                var fullPath = Path.Combine(pathToSave, fileName);

                await using var stream = System.IO.File.Create(fullPath);
                await file.CopyToAsync(stream);

                return Ok(Conversion(new Attachment
                {
                    Url = Configuration["SupportBuddySettings:Url"] + "/attachments/" + fileName
                }));
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

        private Attachment_DTO_Out Conversion(Attachment obj)
        {
            return new Attachment_DTO_Out
            {
                Id = obj.Id,
                Url = obj.Url
            };
        }
    }
}