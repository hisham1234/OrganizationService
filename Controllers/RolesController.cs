using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organization_Service.Helpers;
using Organization_Service.Models;

namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        // Inject telemetry and logger is necessary in order to add
        // specific log in Azure ApplicationInsights
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;
        private readonly OrganizationContext _context;
        private LoggerHelper logHelp;
        public RolesController(OrganizationContext context, ILogger<RolesController> logger, TelemetryClient telemetry)
        {
            _telemetry = telemetry;
            _logger = logger;
            _context = context;
            logHelp = new LoggerHelper();
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            logHelp.Log(logHelp.getMessage("GetRoles"));
            _logger.LogInformation(logHelp.getMessage("GetRoles"));

            try
            {
                var findRoles = await _context.Role.Select(x => ItemToDTO(x)).ToListAsync();
                if (findRoles == null)
                {
                    logHelp.Log(logHelp.getMessage("GetRoles", 404));
                    _logger.LogWarning(logHelp.getMessage("GetRoles", 404));
                    return NotFound();
                }
                var result = new
                {
                    response = findRoles,
                };
                logHelp.Log(logHelp.getMessage("GetRoles", 200));
                _logger.LogInformation(logHelp.getMessage("GetRoles",200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetRoles", 500));
                logHelp.Log(logHelp.getMessage("GetRoles", ex.Message));
                _logger.LogError(logHelp.getMessage("GetRoles", 500));
                _logger.LogError(logHelp.getMessage("GetRoles", ex.Message));
                return StatusCode(500);
            }
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRole(int id)
        {
            logHelp.Log(logHelp.getMessage("GetRole"));
            _logger.LogInformation(logHelp.getMessage("GetRole"));
            try
            {
                var role = await _context.Role.FindAsync(id);
                if (role == null || !RoleExists(id))
                {
                    logHelp.Log(logHelp.getMessage("GetRole", 404));
                    _logger.LogWarning(logHelp.getMessage("GetRole",404));
                    return NotFound();
                }
                var result = new
                {
                    response = role,
                };
                logHelp.Log(logHelp.getMessage("GetRole", 200));
                _logger.LogInformation(logHelp.getMessage("GetRole",200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetRole", 500));
                logHelp.Log(logHelp.getMessage("GetRole", ex.Message));
                _logger.LogError(logHelp.getMessage("GetRole", 500));
                _logger.LogError(logHelp.getMessage("GetRole", ex.Message));
                return StatusCode(500);
            }
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDTO roleDTO)
        {
            logHelp.Log(logHelp.getMessage("PutRole"));
            _logger.LogInformation(logHelp.getMessage("PutRole"));
            var role = await _context.Role.FindAsync(id);
            try
            {
                if (role == null || !RoleExists(id))
                {
                    logHelp.Log(logHelp.getMessage("PutRole", 500));
                    logHelp.Log(logHelp.getMessage("PutRole", "Role was not Found"));
                    _logger.LogError(logHelp.getMessage("PutRole", 500));
                    _logger.LogError(logHelp.getMessage("PutRole", "Role was not Found"));
                    return NotFound();
                }
                else if (id != roleDTO.ID)
                {
                    logHelp.Log(logHelp.getMessage("PutRole", 500));
                    logHelp.Log(logHelp.getMessage("PutRole", "Role was not Found"));
                    _logger.LogError(logHelp.getMessage("PutRole", 500));
                    _logger.LogError(logHelp.getMessage("PutRole", "Role was not Found"));
                    return BadRequest();
                }
                else
                {
                    role.ID = roleDTO.ID;
                    role.RoleName = roleDTO.RoleName;
                    role.UpdatedAt = DateTime.Now;
                    _logger.LogInformation(logHelp.getMessage("PutRole", 200));

                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                logHelp.Log(logHelp.getMessage("PutRole", 500));
                logHelp.Log(logHelp.getMessage("PutRole", ex.Message));
                _logger.LogError(logHelp.getMessage("PutRole", 500));
                _logger.LogError(logHelp.getMessage("PutRole", ex.Message));
                return StatusCode(500);
            }
            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> PostRole(RoleDTO roleDTO)
        {
            logHelp.Log(logHelp.getMessage("PostRole"));
            _logger.LogInformation(logHelp.getMessage("PostRole"));
            var role = new Role
            {
                ID = roleDTO.ID,
                RoleName = roleDTO.RoleName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            try
            {
                _context.Role.Add(role);
                await _context.SaveChangesAsync();
                _logger.LogInformation(logHelp.getMessage("PostRole", 200));
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("PostRole", 500));
                logHelp.Log(logHelp.getMessage("PostRole", ex.Message));
                _logger.LogError(logHelp.getMessage("PostRole", 500));
                _logger.LogError(logHelp.getMessage("PostRole", ex.Message));
                return StatusCode(500);
            }
            return CreatedAtAction(nameof(GetRole), new { id = role.ID }, ItemToDTO(role));
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            logHelp.Log(logHelp.getMessage("DeleteRole"));
            _logger.LogInformation(logHelp.getMessage("DeleteRole"));


            try
            {
                var role = await _context.Role.FindAsync(id);
                if (role == null)
                {
                    _logger.LogWarning(logHelp.getMessage("DeleteRole", 404));
                    logHelp.Log(logHelp.getMessage("DeleteRole", 404));
                    return NotFound();
                }
                else
                {
                    _context.Role.Remove(role);
                    _logger.LogInformation(logHelp.getMessage("DeleteRole", 200));
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("DeleteRole", 500));
                logHelp.Log(logHelp.getMessage("DeleteRole", ex.Message));
                _logger.LogError(logHelp.getMessage("DeleteRole", 500));
                _logger.LogError(logHelp.getMessage("DeleteRole", ex.Message));
                return StatusCode(500);
            }
            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Role.Any(e => e.ID == id);
        }

        private static RoleDTO ItemToDTO(Role role) => new RoleDTO
        {
            ID = role.ID,
            RoleName = role.RoleName
        };
    }
}
