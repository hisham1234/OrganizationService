using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization_Service.Entities;
using Organization_Service.Models.DTO;
using Organization_Service.Models;
using Organization_Service.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using AutoMapper;
using static Organization_Service.Helpers.SaltedHashedHelper;
using Microsoft.AspNetCore.Authorization;

namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;
        private readonly OrganizationContext _context;
        private readonly IMapper _mapper;
        private LoggerHelper logHelp;

        public UsersController(OrganizationContext context, ILogger<UsersController> logger, TelemetryClient telemetry, IMapper mapper)
        {
            // Inject telemetry and logger is necessary in order to add
            // specific log in Azure ApplicationInsights
            _telemetry = telemetry;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            logHelp = new LoggerHelper();
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ResponseUserDTO>>> GetUsers()
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetUsers)));

            try
            {
                var findUsers = await _context.User.AsNoTracking().Include(u => u.Roles).ToListAsync();

                if (findUsers == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUsers),"Users were not Found"));

                    return NotFound();
                }

                _logger.LogInformation(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status200OK));

                return Ok(_mapper.Map<IEnumerable<ResponseUserDTO>>(findUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetUsers), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ResponseUserDTO>> GetUser(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetUser)));

            try
            {
                var findUser = await _context.User.Where(u => u.ID == id).FirstOrDefaultAsync();

                if (findUser == null || !UserExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUser),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUser), "User was not Found"));

                    return NotFound();
                }

                _logger.LogInformation(logHelp.getMessage(nameof(GetUser),StatusCodes.Status200OK));

                return Ok(_mapper.Map<ResponseUserDTO>(findUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetUser), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseUserDTO>> PostUser(NewUserDTO newUser)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PostUser)));
            var salt = SaltedHashedHelper.GetSalt();
            try
            {
                UserEntity userToSave = new UserEntity
                {
                    Email = newUser.Email,
                    Password = SaltedHashedHelper.StringEncrypt(newUser.Password, salt),      // Password Encryption
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    OfficeID = newUser.OfficeID ?? null,
                    RefreshRate = newUser.RefreshRate,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Salt = salt
                };

                _context.User.Add(userToSave);
                await _context.SaveChangesAsync();

                _logger.LogInformation(logHelp.getMessage(nameof(PostUser), StatusCodes.Status201Created));

                return CreatedAtAction(nameof(GetUser), new { id = userToSave.ID }, _mapper.Map<ResponseUserDTO>(userToSave));
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PostUser), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PostUser), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, UpdateUserDTO user)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PutUser)));

            try
            {
                var userToSaved = await _context.User.FindAsync(id);

                if (userToSaved == null || !UserExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(PutUser),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(PutUser), "User was not Found"));

                    return NotFound();
                }

                if (id != user.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutUser), "User was not Found"));


                    return BadRequest();
                }

                if (user.RolesID != null)
                {
                    //user.RolesID.Sort();

                    var findRoles = await _context.Role.AsNoTracking().Where(r => user.RolesID.Contains(r.ID)).OrderBy(r => r.ID).ToListAsync();

                    if (findRoles == null)
                    {
                        _logger.LogWarning(logHelp.getMessage(nameof(PutUser), StatusCodes.Status404NotFound));
                        _logger.LogWarning(logHelp.getMessage(nameof(PutUser), "Roles was not Found"));

                        return NotFound();
                    }

                    if (user.RolesID.SequenceEqual(findRoles.Select(r => r.ID).ToList()) == false)
                    {
                        _logger.LogError(logHelp.getMessage(nameof(PutUser), StatusCodes.Status400BadRequest));
                        _logger.LogError(logHelp.getMessage(nameof(PutUser), "RolesID does not match"));

                        return BadRequest();
                    }

                    userToSaved.Roles = findRoles;
                }
                else
                {
                    userToSaved.Roles = userToSaved.Roles;
                }

                userToSaved.Salt = SaltedHashedHelper.GetSalt();
                userToSaved.ID = user.ID;
                userToSaved.Email = String.IsNullOrWhiteSpace(user.Email) == false ? user.Email : userToSaved.Email;
                userToSaved.Password = String.IsNullOrWhiteSpace(user.Password) == false ? SaltedHashedHelper.StringEncrypt(user.Password, userToSaved.Salt) : userToSaved.Password;     // Password Encryption
                userToSaved.FirstName = String.IsNullOrWhiteSpace(user.FirstName) == false ? user.FirstName : userToSaved.FirstName;
                userToSaved.LastName = String.IsNullOrWhiteSpace(user.LastName) == false ? user.LastName : userToSaved.LastName;
                userToSaved.RefreshRate = user.RefreshRate > 0 ? user.RefreshRate : userToSaved.RefreshRate;
                userToSaved.OfficeID = user.OfficeID != null ? user.OfficeID : userToSaved.OfficeID;
                userToSaved.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation(logHelp.getMessage(nameof(PutUser), StatusCodes.Status204NoContent));
                               
                return Ok(_mapper.Map<ResponseUserDTO>(userToSaved));             
                //return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutUser), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(DeleteUser)));
            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteUser),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteUser),"User was not Found"));

                   return NotFound();
                }
                else
                {
                    _context.User.Remove(user);

                    await _context.SaveChangesAsync();

                    _logger.LogInformation(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status204NoContent));
                
                    return Ok(_mapper.Map<ResponseUserDTO>(user));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(DeleteUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(DeleteUser), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
