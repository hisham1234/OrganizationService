using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private LoggerHelper logHelp;

        public OfficesController(OrganizationContext context, ILogger<OfficesController> logger, TelemetryClient telemetry, IMapper mapper)
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
            _logger.LogInformation(logHelp.getMessage(nameof(GetOffices)));
            logHelp.Log(logHelp.getMessage(nameof(GetOffices)));

            try
            {
                var findOffices = await _context.Office.Select(x => ItemToDTO(x)).ToListAsync();
                
                if (findOffices == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffices), "Offices were not Found"));

                    logHelp.Log(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetOffices), "Offices were not Found"));

                    return NotFound();
                }

                var result = new
                {
                    response = findOffices
                };

                _logger.LogInformation(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status200OK));
                logHelp.Log(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status200OK));
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetOffices), ex.Message));

                logHelp.Log(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetOffices), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Offices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDTO>> GetOffice(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetOffice)));
            logHelp.Log(logHelp.getMessage(nameof(GetOffice)));

            try
            {
                var office = await _context.Office.FindAsync(id);
                
                if (office == null || !OfficeExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffice), "Office was not found"));

                    logHelp.Log(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetOffice), "Office was not Found"));

                    return NotFound();
                }

                var result = new
                {
                    response = ItemToDTO(office)
                };
                
                _logger.LogInformation(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status200OK));
                logHelp.Log(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status200OK));
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetOffice), ex.Message));
                
                logHelp.Log(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetOffice), ex.Message));
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Offices/5/users
        [HttpGet("{id}/users")]
        public async Task<ActionResult<IEnumerable<OfficeDTO>>> GetSpecificOfficeUsers(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetSpecificOfficeUsers)));

            logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers)));

            try
            {
                if (OfficeExists(id) == false)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), "Office was not found"));

                    logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), "Office was not Found"));

                    return NotFound();
                }
                
                var findUsers = await _context.User.Where(u => u.OfficeID == id).Include(u => u.Roles).Select(u => ItemToDTO(u)).ToListAsync();

                if (findUsers == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), "Users was not found"));

                    logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), "Users was not Found"));

                    return NotFound();
                }

                var result = new
                {
                    response = _mapper.Map <IEnumerable<UserDTOOutput>>(findUsers)
                };

                _logger.LogInformation(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status200OK));
                logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status200OK));
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetSpecificOfficeUsers), ex.Message));

                logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetSpecificOfficeUsers), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Offices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffice(int id, OfficeDTO officeDTO)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PutOffice)));
            logHelp.Log(logHelp.getMessage(nameof(PutOffice)));
            
            try
            {
                var office = await _context.Office.FindAsync(id);

                if (office == null || !OfficeExists(id))
                {

                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status404NotFound));
                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), "Office was not Found"));

                    logHelp.Log(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(PutOffice), "Office was not Found"));

                    return NotFound();
                }
                else if (id != officeDTO.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), "Office was not Found"));

                    logHelp.Log(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status400BadRequest));
                    logHelp.Log(logHelp.getMessage(nameof(PutOffice), "OfficeID does not match"));
                    
                    return BadRequest();
                }
                else
                {
                    office.ID = officeDTO.ID;
                    office.OfficeName = String.IsNullOrWhiteSpace(officeDTO.OfficeName) == false ? officeDTO.OfficeName : office.OfficeName;
                    office.ParentOfficeID = officeDTO.ParentOfficeID != null ? officeDTO.ParentOfficeID : office.ParentOfficeID;
                    office.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status204NoContent));
                    logHelp.Log(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status204NoContent));

                    return NoContent();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutOffice), ex.Message));
                
                logHelp.Log(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PutOffice), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Offices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OfficeDTO>> PostOffice(OfficeDTO officeDTO)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PostOffice)));
            logHelp.Log(logHelp.getMessage(nameof(PostOffice)));

            try
            {
                var office = new Office
                {
                    ID = officeDTO.ID,
                    OfficeName = officeDTO.OfficeName,
                    ParentOfficeID = officeDTO.ParentOfficeID ?? null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Office.Add(office);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation(logHelp.getMessage(nameof(PostOffice), StatusCodes.Status201Created));
                logHelp.Log(logHelp.getMessage(nameof(PostOffice), StatusCodes.Status201Created));

                return CreatedAtAction(nameof(GetOffice), new { id = office.ID }, ItemToDTO(office));
            }
            catch(Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PostOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PostOffice), ex.Message));

                logHelp.Log(logHelp.getMessage(nameof(PostOffice), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PostOffice), ex.Message));
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Offices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(DeleteOffice)));
            logHelp.Log(logHelp.getMessage(nameof(DeleteOffice)));
            
            try
            {
                var office = await _context.Office.FindAsync(id);

                if (office == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteOffice), "Office was not found"));
                    
                    logHelp.Log(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(DeleteOffice), "Office was not Found"));
                    
                    return NotFound();
                }
                else
                {
                    _context.Office.Remove(office);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status204NoContent));
                    logHelp.Log(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status204NoContent));

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(DeleteOffice), ex.Message));
                
                logHelp.Log(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(DeleteOffice), ex.Message));
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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

        private static UserDTO ItemToDTO(User user) => new UserDTO
        {
            ID = user.ID,
            Email = user.Email,
            Password = user.Password,
            FirstName = user.FirstName,
            LastName = user.LastName,
            OfficeID = user.OfficeID,
            RolesID = user.Roles.Select(r => r.ID).ToList()
        };
    }
}
