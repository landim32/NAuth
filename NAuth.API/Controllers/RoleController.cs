using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NAuth.Domain.Exceptions;
using NAuth.Domain.Services.Interfaces;
using NAuth.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAuth.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private const string NotAuthorizedMessage = "Not Authorized";
        private const string ExceptionOccurredMessage = "An exception occurred: {Message}";

        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public RoleController(
            ILogger<RoleController> logger,
            IRoleService roleService,
            IUserService userService
        )
        {
            _logger = logger;
            _roleService = roleService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("list")]
        public ActionResult<List<RoleInfo>> List()
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null || !userSession.IsAdmin)
                {
                    _logger.LogError(NotAuthorizedMessage);
                    return Unauthorized(NotAuthorizedMessage);
                }

                var roles = _roleService.ListRoles();

                var roleInfos = roles.Select(x => new RoleInfo
                {
                    RoleId = x.RoleId,
                    Slug = x.Slug,
                    Name = x.Name
                }).ToList();

                _logger.LogInformation("List roles successfully, count: {Count}", roleInfos.Count);

                return Ok(roleInfos);
            }
            catch (UserValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getById/{roleId}")]
        public ActionResult<RoleInfo> GetById(long roleId)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null || !userSession.IsAdmin)
                {
                    _logger.LogError(NotAuthorizedMessage);
                    return Unauthorized(NotAuthorizedMessage);
                }

                var role = _roleService.GetById(roleId);

                var roleInfo = new RoleInfo
                {
                    RoleId = role.RoleId,
                    Slug = role.Slug,
                    Name = role.Name
                };

                _logger.LogInformation("GetById(roleId: {RoleId}) = Role(RoleId: {ID}, Slug: {Slug}, Name: {Name})",
                    roleId, role.RoleId, role.Slug, role.Name);

                return Ok(roleInfo);
            }
            catch (UserValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getBySlug/{slug}")]
        public ActionResult<RoleInfo> GetBySlug(string slug)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null || !userSession.IsAdmin)
                {
                    _logger.LogError(NotAuthorizedMessage);
                    return Unauthorized(NotAuthorizedMessage);
                }

                var role = _roleService.GetBySlug(slug);

                var roleInfo = new RoleInfo
                {
                    RoleId = role.RoleId,
                    Slug = role.Slug,
                    Name = role.Name
                };

                _logger.LogInformation("GetBySlug(slug: {Slug}) = Role(RoleId: {ID}, Name: {Name})", slug, role.RoleId, role.Name);

                return Ok(roleInfo);
            }
            catch (UserValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("insert")]
        public async Task<ActionResult<RoleInfo>> Insert([FromBody] RoleInfo roleInfo)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null || !userSession.IsAdmin)
                {
                    _logger.LogError(NotAuthorizedMessage);
                    return Unauthorized(NotAuthorizedMessage);
                }

                var newRole = await _roleService.Insert(roleInfo);

                var result = new RoleInfo
                {
                    RoleId = newRole.RoleId,
                    Slug = newRole.Slug,
                    Name = newRole.Name
                };

                _logger.LogInformation("Role successfully inserted (RoleId: {RoleId}, Slug: {Slug}, Name: {Name})",
                    newRole.RoleId, newRole.Slug, newRole.Name);

                return Ok(result);
            }
            catch (UserValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<ActionResult<RoleInfo>> Update([FromBody] RoleInfo roleInfo)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null || !userSession.IsAdmin)
                {
                    _logger.LogError(NotAuthorizedMessage);
                    return Unauthorized(NotAuthorizedMessage);
                }

                var updatedRole = await _roleService.Update(roleInfo);

                var result = new RoleInfo
                {
                    RoleId = updatedRole.RoleId,
                    Slug = updatedRole.Slug,
                    Name = updatedRole.Name
                };

                _logger.LogInformation("Role successfully updated (RoleId: {RoleId}, Slug: {Slug}, Name: {Name})",
                    updatedRole.RoleId, updatedRole.Slug, updatedRole.Name);

                return Ok(result);
            }
            catch (UserValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete/{roleId}")]
        public ActionResult Delete(long roleId)
        {
            try
            {
                var userSession = _userService.GetUserInSession(HttpContext);
                if (userSession == null || !userSession.IsAdmin)
                {
                    _logger.LogError(NotAuthorizedMessage);
                    return Unauthorized(NotAuthorizedMessage);
                }

                _roleService.Delete(roleId);

                _logger.LogInformation("Role successfully deleted (RoleId: {RoleId})", roleId);

                return Ok("Role deleted successfully");
            }
            catch (UserValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
