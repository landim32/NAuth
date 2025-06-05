using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NAuth.Domain.Impl.Services;
using NAuth.Domain.Interfaces.Services;
using NAuth.DTO.Domain;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NAuth.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public ImageController(
            IUserService userService,
            IImageService imageService
        ) { 
            _userService = userService;
            _imageService = imageService;
        }

        /*
        [Authorize]
        [HttpPost("uploadImage")]
        public ActionResult<StringResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var fileName = _imageService.InsertFromStream(file.OpenReadStream(), file.FileName);
                return new StringResult()
                {
                    Value = fileName
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        */

        [Authorize]
        [HttpPost("uploadImageUser")]
        public ActionResult<StringResult> UploadImageUser(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                var fileName = _imageService.InsertToUser(file.OpenReadStream(), userSession.UserId);
                return new StringResult()
                {
                    Value = _imageService.GetImageUrl(fileName)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
