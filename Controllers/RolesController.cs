using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization_Service.Helpers;
using Organization_Service.Models;

namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly OrganizationContext _context;
        private LoggerHelper logHelp;
        public RolesController(OrganizationContext context)
        {
            _context = context;
            logHelp = new LoggerHelper();
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            logHelp.Log(logHelp.getMessage(nameof(GetRoles)));

            try
            {
                var findRoles = await _context.Role.Select(x => ItemToDTO(x)).ToListAsync();
                
                if (findRoles == null)
                {
                    logHelp.Log(logHelp.getMessage(nameof(GetRoles), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetRoles), "Roles were not Found"));
                    return NotFound();
                }
                
                var result = new
                {
                    response = findRoles
                };

                logHelp.Log(logHelp.getMessage(nameof(GetRoles), StatusCodes.Status200OK));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(GetRoles), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetRoles), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRole(int id)
        {
            logHelp.Log(logHelp.getMessage(nameof(GetRole)));

            try
            {
                var role = await _context.Role.FindAsync(id);

                if (role == null || !RoleExists(id))
                {
                    logHelp.Log(logHelp.getMessage(nameof(GetRole), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetRole), "Role was not Found"));
                    return NotFound();
                }

                var result = new
                {
                    response = ItemToDTO(role)
                };

                logHelp.Log(logHelp.getMessage("GetRole", StatusCodes.Status200OK));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(GetRole), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetRole), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDTO roleDTO)
        {
            logHelp.Log(logHelp.getMessage(nameof(PutRole)));
            
            try
            {
                var role = await _context.Role.FindAsync(id);

                if (role == null || !RoleExists(id))
                {
                    logHelp.Log(logHelp.getMessage(nameof(PutRole), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(PutRole), "Role was not Found"));
                    return NotFound();
                }
                else if (id != roleDTO.ID)
                {
                    logHelp.Log(logHelp.getMessage(nameof(PutRole), StatusCodes.Status400BadRequest));
                    logHelp.Log(logHelp.getMessage(nameof(PutRole), "RoleID does not match"));
                    return BadRequest();
                }
                else
                {
                    role.ID = roleDTO.ID;
                    role.RoleName = roleDTO.RoleName;
                    role.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(PutRole), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PutRole), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> PostRole(RoleDTO roleDTO)
        {
            logHelp.Log(logHelp.getMessage(nameof(PostRole)));

            try
            {
                var role = new Role
                {
                    ID = roleDTO.ID,
                    RoleName = roleDTO.RoleName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Role.Add(role);
                await _context.SaveChangesAsync();
                logHelp.Log(logHelp.getMessage(nameof(PostRole), StatusCodes.Status201Created));
                return CreatedAtAction(nameof(GetRole), new { id = role.ID }, ItemToDTO(role));
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(PostRole), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PostRole), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            logHelp.Log(logHelp.getMessage(nameof(DeleteRole)));

            try
            {
                var role = await _context.Role.FindAsync(id);

                if (role == null)
                {
                    logHelp.Log(logHelp.getMessage(nameof(DeleteRole), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(DeleteRole), "Role was not Found"));
                    return NotFound();
                }
                else
                {
                    _context.Role.Remove(role);
                    await _context.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(nameof(DeleteRole), StatusCodes.Status204NoContent));
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(DeleteRole), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(DeleteRole), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool RoleExists(int id)
        {
            logHelp.Log(logHelp.getMessage(nameof(RoleExists)));
            return _context.Role.Any(e => e.ID == id);
        }

        private static RoleDTO ItemToDTO(Role role) => new RoleDTO
        {
            ID = role.ID,
            RoleName = role.RoleName
        };
    }
}
