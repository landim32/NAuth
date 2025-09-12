using DB.Infra.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAuth.Domain.Impl.Models;
using NAuth.Domain.Impl.Services;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using NAuth.Domain.Interfaces.Services;
using NAuth.DTO.Domain;
using NAuth.DTO.User;
using NTools.ACL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NAuth.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IFileClient _fileClient;
        private readonly IUserDomainFactory _userFactory;

        public UserController(IUserService userService, IFileClient fileClient, IUserDomainFactory userFactory)
        {
            _userService = userService;
            _fileClient = fileClient;
            _userFactory = userFactory;
        }

        [Authorize]
        [HttpPost("uploadImageUser")]
        public async Task<ActionResult<StringResult>> UploadImageUser(IFormFile file)
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

                //var fileName = await _fileClient.UploadFileAsync(_userService.GetBucketName(), file.OpenReadStream(), userSession.UserId);
                var fileName = await _fileClient.UploadFileAsync(_userService.GetBucketName(), file);
                return new StringResult()
                {
                    Value = await _fileClient.GetFileUrlAsync(_userService.GetBucketName(), fileName)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getMe")]
        [Authorize]
        public async Task<ActionResult<UserResult>> GetMe()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getByToken/{token}")]
        public async Task<ActionResult<UserResult>> GetByToken(string token)
        {
            try
            {
                var user = _userService.GetUserByToken(token);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getById/{userId}")]
        public async Task<ActionResult<UserResult>> GetById(long userId)
        {
            try
            {
                var user = _userService.GetUserByID(userId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getByEmail/{email}")]
        public async Task<ActionResult<UserResult>> GetByEmail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with email not found" };
                }
                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getBySlug/{slug}")]
        public async Task<ActionResult<UserResult>> GetBySlug(string slug)
        {
            try
            {
                var user = _userService.GetBySlug(slug);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with email not found" };
                }
                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("insert")]
        public async Task<ActionResult<UserResult>> Insert([FromBody] UserInfo user)
        {
            try
            {
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var newUser = await _userService.Insert(user);
                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(newUser)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<ActionResult<UserResult>> Update([FromBody] UserInfo user)
        {
            try
            {
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                if (userSession.UserId != user.UserId)
                {
                    throw new Exception("Only can update your user");
                }

                var updatedUser = await _userService.Update(user);
                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(updatedUser)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("loginWithEmail")]
        public async Task<ActionResult<UserTokenResult>> LoginWithEmail([FromBody]LoginParam param)
        {
            try
            {
                var user = _userService.LoginWithEmail(param.Email, param.Password);
                if (user == null)
                {
                    return new UserTokenResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                var fingerprint = Request.Headers["X-Device-Fingerprint"].FirstOrDefault();
                var userAgent = Request.Headers["User-Agent"].FirstOrDefault();

                var ipAddr = Request.HttpContext.Connection?.RemoteIpAddress?.ToString();

                if (Request.Headers?.ContainsKey("X-Forwarded-For") == true)
                {
                    ipAddr = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                }
                var token = await _userService.CreateToken(user.UserId, ipAddr, userAgent, fingerprint);
                return new UserTokenResult()
                {
                    Token = token.Token,
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("hasPassword")]
        public ActionResult<StatusResult> HasPassword()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }
                return new StatusResult
                {
                    Sucesso = _userService.HasPassword(user.UserId),
                    Mensagem = "Password verify successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changePassword")]
        public ActionResult<StatusResult> ChangePassword([FromBody]ChangePasswordParam param)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }
                _userService.ChangePassword(user.UserId, param.OldPassword, param.NewPassword);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("sendRecoveryMail/{email}")]
        public async Task<ActionResult<StatusResult>> SendRecoveryMail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new StatusResult
                    {
                        Sucesso = false,
                        Mensagem = "Email not exist"
                    };
                }
                await _userService.SendRecoveryEmail(email);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Recovery email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changePasswordUsingHash")]
        public ActionResult<StatusResult> ChangePasswordUsingHash([FromBody] ChangePasswordUsingHashParam param)
        {
            try
            {
                _userService.ChangePasswordUsingHash(param.RecoveryHash, param.NewPassword);
                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("list/{take}")]
        public async Task<ActionResult<UserListResult>> list(int take)
        {
            try
            {
                var userModels = _userService.ListUsers(take);
                var userInfos = await Task.WhenAll(userModels.Select(x => _userService.GetUserInfoFromModel(x)));
                return new UserListResult
                {
                    Sucesso = true,
                    Users = userInfos.ToList()
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
