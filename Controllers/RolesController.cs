using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organization_Service.Helpers;
using Organization_Service.Entities;
using Organization_Service.Models;
using Organization_Service.Models.DTO;
using AutoMapper;
namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        // Inject telemetry and logger is necessary in order to add
        // specific log in Azure ApplicationInsights
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;
        private readonly OrganizationContext _context;
        private readonly IMapper _mapper;
        private LoggerHelper logHelp;
        public RolesController(OrganizationContext context, ILogger<RolesController> logger, TelemetryClient telemetry, IMapper mapper)
        {
            _telemetry = telemetry;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            logHelp = new LoggerHelper();
        }

        // GET: api/Roles
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetRoles()
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetRoles)));

            try
            {
                var findRoles = _mapper.Map<IEnumerable<ResponseRoleDTO>>(await _context.Role.ToListAsync());
                
                if (findRoles == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetRoles),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetRoles),"Roles were not Found"));
              
                    return NotFound();
                }
                
                _logger.LogInformation(logHelp.getMessage(nameof(GetRoles),StatusCodes.Status200OK));

                return Ok(findRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetRoles),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetRoles), ex.Message));
                       
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetRole(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetRole)));

            try
            {
                var role = _mapper.Map<ResponseRoleDTO>(await _context.Role.FindAsync(id));

                if (role == null || !RoleExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetRole),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetRole),StatusCodes.Status404NotFound));
  
                    return NotFound();
                }

                _logger.LogInformation(logHelp.getMessage(nameof(GetRole),StatusCodes.Status200OK));

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetRole),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetRole), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRole(int id, UpdateRoleDTO role)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PutRole)));

            try
            {
                var roleToSave = await _context.Role.FindAsync(id);

                if (roleToSave == null || !RoleExists(id))
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutRole),StatusCodes.Status404NotFound));
                    _logger.LogError(logHelp.getMessage(nameof(PutRole), "Role was not Found"));
                  return NotFound();
                }
                else if (id != role.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutRole),StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutRole), "Role was not Found"));
                          
                    return BadRequest();
                }
                else
                {
                    roleToSave.ID = role.ID;
                    roleToSave.RoleName = role.RoleName;
                    roleToSave.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation(logHelp.getMessage(nameof(PutRole), StatusCodes.Status204NoContent));
                                
                    return Ok(_mapper.Map<ResponseRoleDTO>(roleToSave));
                    //return NoContent();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PutRole),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutRole), ex.Message));
              
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostRole(NewRoleDTO newRole)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PostRole)));
          try
            {
                var role = new RoleEntity
                {
                    RoleName = newRole.RoleName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Role.Add(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation(logHelp.getMessage(nameof(PostRole), StatusCodes.Status201Created));
             return CreatedAtAction(nameof(GetRole), new { id = role.ID }, _mapper.Map<ResponseRoleDTO>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PostRole), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PostRole), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRole(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(DeleteRole)));
          try
            {
                var role = await _context.Role.FindAsync(id);

                if (role == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteRole),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteRole), "Role was not Found"));
                    return NotFound();
                }
                else
                {
                    _context.Role.Remove(role);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(logHelp.getMessage(nameof(DeleteRole), StatusCodes.Status204NoContent));
                                
                    return Ok(_mapper.Map<ResponseRoleDTO>(role));
                    //return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(logHelp.getMessage(nameof(DeleteRole), StatusCodes.Status500InternalServerError));
                _logger.LogInformation(logHelp.getMessage(nameof(DeleteRole), ex.Message));                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool RoleExists(int id)
        {
            return _context.Role.Any(e => e.ID == id);
        }
    }
}
