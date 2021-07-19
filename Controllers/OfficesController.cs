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
    public class OfficesController : ControllerBase
    {
        // Inject telemetry and logger is necessary in order to add
        // specific log in Azure ApplicationInsights
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;
        private readonly OrganizationContext _context;
        private LoggerHelper logHelp;

        public OfficesController(OrganizationContext context, ILogger<OfficesController> logger, TelemetryClient telemetry)
        {
            _telemetry = telemetry;
            _logger = logger;
            _context = context;
            logHelp = new LoggerHelper();
        }

        // GET: api/Offices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfficeDTO>>> GetOffices()
        {
            logHelp.Log(logHelp.getMessage("GetOffices"));
            _logger.LogInformation(logHelp.getMessage("GetOffices"));
            try
            {
                var findOffices = await _context.Office.Select(x => ItemToDTO(x)).ToListAsync();
                if (findOffices == null)
                {
                    logHelp.Log(logHelp.getMessage("GetOffices", 404));
                    _logger.LogWarning(logHelp.getMessage("GetOffices", 404));

                    return NotFound();
                }    
                var result = new
                {
                    response = findOffices,
                };
                logHelp.Log(logHelp.getMessage("GetOffices", 200));
                _logger.LogInformation(logHelp.getMessage("GetOffices", 200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetOffices", 500));
                logHelp.Log(logHelp.getMessage("GetOffices", ex.Message));

                _logger.LogError(logHelp.getMessage("GetOffices", 500));
                _logger.LogError(logHelp.getMessage("GetOffices", ex.Message));

                return StatusCode(500);
            }
        }

        // GET: api/Offices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDTO>> GetOffice(int id)
        {
            logHelp.Log(logHelp.getMessage("GetOffice"));
            _logger.LogInformation(logHelp.getMessage("GetOffice"));
            try
            {
                var office = await _context.Office.FindAsync(id);
                if (office == null|| !OfficeExists(id))
                {
                    logHelp.Log(logHelp.getMessage("GetOffice", 404));
                    _logger.LogWarning(logHelp.getMessage("GetOffice", 404));

                    return NotFound();
                }
                var result = new
                {
                    response = office,
                };
                logHelp.Log(logHelp.getMessage("GetOffice", 200));
                _logger.LogInformation(logHelp.getMessage("GetOffice", 200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetOffice", 500));
                logHelp.Log(logHelp.getMessage("GetOffice", ex.Message));
                _logger.LogError(logHelp.getMessage("GetOffice", 500));
                _logger.LogError(logHelp.getMessage("GetOffice", ex.Message));
                return StatusCode(500);
            }
        }

        // PUT: api/Offices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffice(int id, OfficeDTO officeDTO)
        {
            logHelp.Log(logHelp.getMessage("PutOffice"));
            _logger.LogInformation(logHelp.getMessage("PutOffice"));
            var office = await _context.Office.FindAsync(id);
            try
            {
                if (office == null|| !OfficeExists(id))
                {
                    logHelp.Log(logHelp.getMessage("PutOffice", 500));
                    logHelp.Log(logHelp.getMessage("PutOffice", "Office was not Found"));
                    _logger.LogError(logHelp.getMessage("PutOffice", 500));
                    _logger.LogError(logHelp.getMessage("PutOffice", "Office was not Found"));

                    return NotFound();
                }
                else if (id != officeDTO.ID)
                {
                    logHelp.Log(logHelp.getMessage("PutOffice", 500));
                    logHelp.Log(logHelp.getMessage("PutOffice", "Office was not Found"));
                    _logger.LogError(logHelp.getMessage("PutOffice", 500));
                    _logger.LogError(logHelp.getMessage("PutOffice", "Office was not Found"));
                    return BadRequest();
                }
                else
                {
                    office.ID = officeDTO.ID;
                    office.OfficeName = String.IsNullOrWhiteSpace(officeDTO.OfficeName) == false ? officeDTO.OfficeName : office.OfficeName;
                    office.ParentOfficeID = officeDTO.ParentOfficeID != null ? officeDTO.ParentOfficeID : office.ParentOfficeID;
                    office.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation(logHelp.getMessage("PutOffice", 200));

                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage("PutOffice", 500));
                logHelp.Log(logHelp.getMessage("PutOffice", ex.Message));
                _logger.LogError(logHelp.getMessage("PutOffice", 500));
                _logger.LogError(logHelp.getMessage("PutOffice", ex.Message));
                return StatusCode(500);
            }
            return NoContent();
        }

        // POST: api/Offices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OfficeDTO>> PostOffice(OfficeDTO officeDTO)
        {
            logHelp.Log(logHelp.getMessage("PostOffice"));
            _logger.LogInformation(logHelp.getMessage("PostOffice"));

            var office = new Office
            {
                ID = officeDTO.ID,
                OfficeName = officeDTO.OfficeName,
                ParentOfficeID = officeDTO.ParentOfficeID ?? null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            try
            {
                _context.Office.Add(office);
                await _context.SaveChangesAsync();
                logHelp.Log(logHelp.getMessage("PostOffice", 200));
                _logger.LogInformation(logHelp.getMessage("PostOffice", 200));
            }
            catch(Exception ex)
            {
                logHelp.Log(logHelp.getMessage("PostOffice", 500));
                logHelp.Log(logHelp.getMessage("PostOffice", ex.Message));
                _logger.LogError(logHelp.getMessage("PostOffice", 500));
                _logger.LogError(logHelp.getMessage("PostOffice", ex.Message));
                return StatusCode(500);
            }

            return CreatedAtAction(nameof(GetOffice), new { id = office.ID }, ItemToDTO(office));
        }

        // DELETE: api/Offices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            logHelp.Log(logHelp.getMessage("DeleteOffice"));
            _logger.LogInformation(logHelp.getMessage("DeleteOffice"));

            try
            {
                var office = await _context.Office.FindAsync(id);
                if (office == null)
                {
                    _logger.LogWarning(logHelp.getMessage("DeleteOffice", 404));
                    logHelp.Log(logHelp.getMessage("DeleteOffice", 404));
                    return NotFound();
                }
                else
                {
                    _context.Office.Remove(office);
                    _logger.LogInformation(logHelp.getMessage("DeleteOffice", 200));
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("DeleteOffice", 500));
                logHelp.Log(logHelp.getMessage("DeleteOffice", ex.Message));
                _logger.LogError(logHelp.getMessage("DeleteOffice", 500));
                _logger.LogError(logHelp.getMessage("DeleteOffice", ex.Message));
                return StatusCode(500);
            }
            return NoContent();
        }

        private bool OfficeExists(int id)
        {
            return _context.Office.Any(e => e.ID == id);
        }

        private static OfficeDTO ItemToDTO(Office office) => new OfficeDTO
        {
            ID = office.ID,
            OfficeName = office.OfficeName,
            ParentOfficeID = office.ParentOfficeID
        };
    }
}
