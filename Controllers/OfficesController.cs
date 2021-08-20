﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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


namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            _mapper = mapper;
            logHelp = new LoggerHelper();


        }

        // GET: api/Offices
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetOffices()
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetOffices)));
            try
            {
                var findOffices = _mapper.Map<IEnumerable<ResponseOfficeDTO>>(await _context.Office.ToListAsync());
                if (findOffices == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffices), "Offices were not Found"));
                    return NotFound();
                }

                _logger.LogInformation(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status200OK));
               
                return Ok(findOffices);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetOffices), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetOffices), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Offices/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetOffice(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetOffice)));

            try
            {
                var office = _mapper.Map<ResponseOfficeDTO>(await _context.Office.FindAsync(id));
                
                if (office == null || !OfficeExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetOffice), "Office was not found"));

                    return NotFound();
                }
                
                _logger.LogInformation(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status200OK));
                
                return Ok(office);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetOffice), ex.Message));
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Offices/5/users
        [HttpGet("{id}/users")]
        [Authorize]
        public async Task<ActionResult> GetSpecificOfficeUsers(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetSpecificOfficeUsers)));

            try
            {
                if (OfficeExists(id) == false)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), "Office was not found"));

                    return NotFound();
                }
                
                var findUsers = await _context.User.Where(u => u.OfficeID == id).Include(u => u.Roles).ToListAsync();

                if (findUsers == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetSpecificOfficeUsers), "Users was not found"));

                    return NotFound();
                }

                _logger.LogInformation(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status200OK));
               
                return Ok(_mapper.Map<IEnumerable<ResponseOfficeDTO>>(findUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetSpecificOfficeUsers), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetSpecificOfficeUsers), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Offices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutOffice(int id, UpdateOfficeDTO officeToUpdate)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PutOffice)));
            
            try
            {
                var office = await _context.Office.FindAsync(id);

                if (office == null || !OfficeExists(id))
                {

                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status404NotFound));
                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), "Office was not Found"));

                    return NotFound();
                }
                else if (id != officeToUpdate.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutOffice), "Office was not Found"));

                     return BadRequest();
                }
                else
                {
                    office.ID = officeToUpdate.ID;
                    office.OfficeName = String.IsNullOrWhiteSpace(officeToUpdate.OfficeName) == false ? officeToUpdate.OfficeName : office.OfficeName;
                    office.ParentOfficeID = officeToUpdate.ParentOfficeID != null ? officeToUpdate.ParentOfficeID : office.ParentOfficeID;
                    office.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status204NoContent));
                                
                    return Ok(_mapper.Map<ResponseOfficeDTO>(office));
                    //return NoContent();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(logHelp.getMessage(nameof(PutOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutOffice), ex.Message));
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Offices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostOffice(NewOfficeDTO newOffice)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PostOffice)));
            try
            {
                var office = new OfficeEntity
                {
                    OfficeName = newOffice.OfficeName,
                    ParentOfficeID = newOffice.ParentOfficeID ?? null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Office.Add(office);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation(logHelp.getMessage(nameof(PostOffice), StatusCodes.Status201Created));
                return CreatedAtAction(nameof(GetOffice), new { id = office.ID }, _mapper.Map<ResponseOfficeDTO>(office));
            }
            catch(Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PostOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PostOffice), ex.Message));
                
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Offices/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(DeleteOffice)));
            try
            {
                var office = await _context.Office.FindAsync(id);

                if (office == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteOffice), "Office was not found"));
                                        
                    return NotFound();
                }
                else
                {
                    _context.Office.Remove(office);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status204NoContent));
                                 
                    return Ok(_mapper.Map<ResponseOfficeDTO>(office));
                    //return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(DeleteOffice), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(DeleteOffice), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool OfficeExists(int id)
        {
            return _context.Office.Any(e => e.ID == id);
        }
    }
}
