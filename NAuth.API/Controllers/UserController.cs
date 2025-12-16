using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Services.Interfaces;
using NAuth.DTO.Domain;
using NAuth.DTO.User;
using NTools.ACL.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NAuth.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IFileClient _fileClient;
        private readonly IUserDomainFactory _userFactory;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IFileClient fileClient,
            IUserDomainFactory userFactory
        )
        {
            _logger = logger;
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
                    _logger.LogError("No file uploaded");
                    return BadRequest("No file uploaded");
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    _logger.LogError("Not Authorized");
                    return StatusCode(401, "Not Authorized");
                }

                //var fileName = await _fileClient.UploadFileAsync(_userService.GetBucketName(), file.OpenReadStream(), userSession.UserId);
                var fileName = await _fileClient.UploadFileAsync(_userService.GetBucketName(), file);
                _logger.LogInformation("File upload successfully, filename: {@filename}", fileName);
                return new StringResult()
                {
                    Value = await _fileClient.GetFileUrlAsync(_userService.GetBucketName(), fileName)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("Not Authorized");
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    _logger.LogError("User Not Found with ID {@userId}", userSession.UserId);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                _logger.LogInformation("getMe() = User(UserId: {@ID}, Name: {@name})", user.UserId, user.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User with token not found {@token}", token);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                _logger.LogInformation("GetByToken(token: {@token}) = User(UserId: {@ID}, Email: {@email}, Name: {@name})", token, user.UserId, user.Email, user.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User Not Found with ID {@userId}", userId);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                _logger.LogInformation("GetById(userId: {@userId}) = User(UserId: {@ID}, Email: {@email}, Name: {@name})", userId, user.UserId, user.Email, user.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User with email not found {@email}", email);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with email not found" };
                }

                _logger.LogInformation("GetByEmail(email: {@email}) = User(UserId: {@ID}, Email: {@email}, Name: {@name})", email, user.UserId, user.Email, user.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User with slug not found {@slug}", slug);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User with slug not found" };
                }

                _logger.LogInformation("GetBySlug(slug: {@slug}) = User(UserId: {@ID}, Email: {@email}, Name: {@name})", slug, user.UserId, user.Email, user.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User is empty");
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var newUser = await _userService.Insert(user);

                _logger.LogInformation("User sucessfully inserted (UserId: {@ID}, Email: {@email}, Name: {@name})", newUser.UserId, newUser.Email, newUser.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(newUser)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User is empty");
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User is empty" };
                }
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    _logger.LogError("Not Authorized");
                    return StatusCode(401, "Not Authorized");
                }
                if (userSession.UserId != user.UserId)
                {
                    _logger.LogError("Only can update your user ({@userSession} != {@userId})", userSession.UserId, user.UserId);
                    throw new Exception("Only can update your user");
                }

                var updatedUser = await _userService.Update(user);

                _logger.LogInformation("User sucessfully updated (UserId: {@ID}, Email: {@email}, Name: {@name})", updatedUser.UserId, updatedUser.Email, updatedUser.Name);

                return new UserResult()
                {
                    User = await _userService.GetUserInfoFromModel(updatedUser)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("loginWithEmail")]
        public async Task<ActionResult<UserTokenResult>> LoginWithEmail([FromBody] LoginParam param)
        {
            try
            {
                var user = _userService.LoginWithEmail(param.Email, param.Password);
                if (user == null)
                {
                    _logger.LogTrace("Email: {@email}, Password: {@password}", param.Email, param.Password);
                    _logger.LogError("Email or password is wrong");
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

                _logger.LogInformation("Token sucessfully created (Token: {@token})", token);

                return new UserTokenResult()
                {
                    Token = token.Token,
                    User = await _userService.GetUserInfoFromModel(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("Not Authorized");
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    _logger.LogError("User with ID not found {@userId}", userSession.UserId);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "User Not Found" };
                }

                var hasPassword = _userService.HasPassword(user.UserId);
                _logger.LogInformation("User has password: {@hasPassword}", hasPassword);

                return new StatusResult
                {
                    Sucesso = hasPassword,
                    Mensagem = "Password verify successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changePassword")]
        public ActionResult<StatusResult> ChangePassword([FromBody] ChangePasswordParam param)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null)
                {
                    _logger.LogError("Not Authorized");
                    return StatusCode(401, "Not Authorized");
                }
                var user = _userService.GetUserByID(userSession.UserId);
                if (user == null)
                {
                    _logger.LogError("User with ID not found {@userId}", userSession.UserId);
                    return new UserResult() { User = null, Sucesso = false, Mensagem = "Email or password is wrong" };
                }

                _userService.ChangePassword(user.UserId, param.OldPassword, param.NewPassword);

                _logger.LogInformation("Password successfully changed, UserId: {@userId}, Email: {@email}, Name: {@name}", user.UserId, user.Email, user.Name);

                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
                    _logger.LogError("User with email not found {@email}", email);
                    return new StatusResult
                    {
                        Sucesso = false,
                        Mensagem = "Email not exist"
                    };
                }

                await _userService.SendRecoveryEmail(email);
                _logger.LogInformation("Send recovery email, Email: {@email}", email);

                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Recovery email sent successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changePasswordUsingHash")]
        public ActionResult<StatusResult> ChangePasswordUsingHash([FromBody] ChangePasswordUsingHashParam param)
        {
            try
            {
                _userService.ChangePasswordUsingHash(param.RecoveryHash, param.NewPassword);
                _logger.LogInformation("Change password using hash, Hash: {@hash}", param.RecoveryHash);

                return new StatusResult
                {
                    Sucesso = true,
                    Mensagem = "Password changed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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

                _logger.LogInformation("list(take: {@take}) successfully", take);

                return new UserListResult
                {
                    Sucesso = true,
                    Users = userInfos.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
