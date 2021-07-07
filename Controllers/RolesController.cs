using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization_Service.Models;

namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly OrganizationContext _context;

        public RolesController(OrganizationContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            //return await _context.Roles.ToListAsync();
            return await _context.Role.Select(x => ItemToDTO(x)).ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRole(int id)
        {
            var role = await _context.Role.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            //return role;
            return ItemToDTO(role);
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDTO roleDTO)
        {
            if (id != roleDTO.ID)
            {
                return BadRequest();
            }

            //_context.Entry(role).State = EntityState.Modified;

            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.ID = roleDTO.ID;
            role.RoleName = roleDTO.RoleName;
            role.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> PostRole(RoleDTO roleDTO)
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

            return CreatedAtAction(nameof(GetRole), new { id = role.ID }, ItemToDTO(role));
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();

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
