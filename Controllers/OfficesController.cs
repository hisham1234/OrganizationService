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
    public class OfficesController : ControllerBase
    {
        private readonly OrganizationContext _context;

        public OfficesController(OrganizationContext context)
        {
            _context = context;
        }

        // GET: api/Offices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfficeDTO>>> GetOffices()
        {
            //return await _context.Offices.ToListAsync();
            return await _context.Offices.Select(x => ItemToDTO(x)).ToListAsync();
        }

        // GET: api/Offices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDTO>> GetOffice(int id)
        {
            var office = await _context.Offices.FindAsync(id);

            if (office == null)
            {
                return NotFound();
            }

            //return office;
            return ItemToDTO(office);
        }

        // PUT: api/Offices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffice(int id, OfficeDTO officeDTO)
        {
            if (id != officeDTO.ID)
            {
                return BadRequest();
            }

            //_context.Entry(office).State = EntityState.Modified;
            
            var office = await _context.Offices.FindAsync(id);
            if (office == null)
            {
                return NotFound();
            }

            office.ID = officeDTO.ID;
            office.OfficeName = officeDTO.OfficeName;
            office.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfficeExists(id))
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

        // POST: api/Offices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OfficeDTO>> PostOffice(OfficeDTO officeDTO)
        {
            var office = new Office
            {
                ID = officeDTO.ID,
                OfficeName = officeDTO.OfficeName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Offices.Add(office);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOffice), new { id = office.ID }, ItemToDTO(office));
        }

        // DELETE: api/Offices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            var office = await _context.Offices.FindAsync(id);
            if (office == null)
            {
                return NotFound();
            }

            _context.Offices.Remove(office);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OfficeExists(int id)
        {
            return _context.Offices.Any(e => e.ID == id);
        }

        private static OfficeDTO ItemToDTO(Office office) => new OfficeDTO
        {
            ID = office.ID,
            OfficeName = office.OfficeName
        };
    }
}
