using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NAuth.Domain.Factory.Interfaces;
using NAuth.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private const string ExceptionOccurredMessage = "An exception occurred: {Message}";

        private readonly ILogger<RoleController> _logger;
        private readonly IRoleDomainFactory _roleFactory;

        public RoleController(
            ILogger<RoleController> logger,
            IRoleDomainFactory roleFactory
        )
        {
            _logger = logger;
            _roleFactory = roleFactory;
        }

        [HttpGet("list/{take}")]
        public ActionResult<List<RoleInfo>> List(int take)
        {
            try
            {
                var roleModel = _roleFactory.BuildRoleModel();
                var roles = roleModel.ListRoles(take, _roleFactory);

                var roleInfos = roles.Select(x => new RoleInfo
                {
                    RoleId = x.RoleId,
                    Slug = x.Slug,
                    Name = x.Name
                }).ToList();

                _logger.LogInformation("List roles successfully, count: {Count}", roleInfos.Count);

                return Ok(roleInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getById/{roleId}")]
        public ActionResult<RoleInfo> GetById(long roleId)
        {
            try
            {
                var roleModel = _roleFactory.BuildRoleModel();
                var role = roleModel.GetById(roleId, _roleFactory);

                if (role == null)
                {
                    _logger.LogError("Role not found with ID {RoleId}", roleId);
                    return NotFound("Role not found");
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getBySlug/{slug}")]
        public ActionResult<RoleInfo> GetBySlug(string slug)
        {
            try
            {
                var roleModel = _roleFactory.BuildRoleModel();
                var role = roleModel.GetBySlug(slug, _roleFactory);

                if (role == null)
                {
                    _logger.LogError("Role not found with slug {Slug}", slug);
                    return NotFound("Role not found");
                }

                var roleInfo = new RoleInfo
                {
                    RoleId = role.RoleId,
                    Slug = role.Slug,
                    Name = role.Name
                };

                _logger.LogInformation("GetBySlug(slug: {Slug}) = Role(RoleId: {ID}, Slug: {Slug}, Name: {Name})", 
                    slug, role.RoleId, role.Slug, role.Name);

                return Ok(roleInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionOccurredMessage, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
