using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization_Service.Entities;
using Organization_Service.Models.DTO;
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
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
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
                var result = new
                {
                    response = _mapper.Map<IEnumerable<UserResponseDTO>>(findUsers)
                };

                _logger.LogInformation(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status200OK));

                return Ok(result);
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
        public async Task<ActionResult<UserResponseDTO>> GetUser(int id)
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

                var result = new
                {
                    response = _mapper.Map<UserResponseDTO>(findUser)
                };

                _logger.LogInformation(logHelp.getMessage(nameof(GetUser),StatusCodes.Status200OK));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetUser), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PutUser)));

            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null || !UserExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(PutUser),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(PutUser), "User was not Found"));

                    return NotFound();
                }

                if (id != userDTO.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutUser), "User was not Found"));


                    return BadRequest();
                }

                if (userDTO.RolesID != null)
                {
                    userDTO.RolesID.Sort();

                    var findRoles = await _context.Role.AsNoTracking().Where(r => userDTO.RolesID.Contains(r.ID)).OrderBy(r => r.ID).ToListAsync();

                    if (findRoles == null)
                    {
                        _logger.LogWarning(logHelp.getMessage(nameof(PutUser), StatusCodes.Status404NotFound));
                        _logger.LogWarning(logHelp.getMessage(nameof(PutUser), "Roles was not Found"));

                        return NotFound();
                    }

                    if (userDTO.RolesID.SequenceEqual(findRoles.Select(r => r.ID).ToList()) == false)
                    {
                        _logger.LogError(logHelp.getMessage(nameof(PutUser), StatusCodes.Status400BadRequest));
                        _logger.LogError(logHelp.getMessage(nameof(PutUser), "RolesID does not match"));

                        return BadRequest();
                    }

                    user.Roles = findRoles;
                }
                else
                {
                    user.Roles = user.Roles;
                }
                
                user.Salt = SaltedHashedHelper.GetSalt();
                user.ID = userDTO.ID;
                user.Email = String.IsNullOrWhiteSpace(userDTO.Email) == false ? userDTO.Email : user.Email;
                user.Password = String.IsNullOrWhiteSpace(userDTO.Password) == false ? SaltedHashedHelper.StringEncrypt(userDTO.Password, user.Salt) : user.Password;     // Password Encryption
                user.FirstName = String.IsNullOrWhiteSpace(userDTO.FirstName) == false ? userDTO.FirstName : user.FirstName;
                user.LastName = String.IsNullOrWhiteSpace(userDTO.LastName) == false ? userDTO.LastName : user.LastName;
                user.OfficeID = userDTO.OfficeID != null ? userDTO.OfficeID : user.OfficeID;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation(logHelp.getMessage(nameof(PutUser), StatusCodes.Status204NoContent));
                
                var result = new
                {
                    response = _mapper.Map<UserResponseDTO>(user)
                };                
                return Ok(result);             
                //return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutUser), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(PostUser)));
            var salt = SaltedHashedHelper.GetSalt();
            try
            {
                var user = new UserEntity
                {
                    Email = userDTO.Email,
                    Password = SaltedHashedHelper.StringEncrypt(userDTO.Password, salt),      // Password Encryption
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    OfficeID = userDTO.OfficeID ?? null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Salt = salt
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation(logHelp.getMessage(nameof(PostUser), StatusCodes.Status201Created));
             
                return CreatedAtAction(nameof(GetUser), new { id = user.ID }, _mapper.Map<UserResponseDTO>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PostUser), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PostUser), ex.Message));

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
                   
                    var result = new
                    {
                        response = _mapper.Map<UserResponseDTO>(user)
                    };                  
                    return Ok(result);
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

        private static UserDTO ItemToDTO(UserEntity user) => new UserDTO
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
