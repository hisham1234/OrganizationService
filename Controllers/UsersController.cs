using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Organization_Service.Models;
using Organization_Service.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;
        private readonly OrganizationContext _context;
        private LoggerHelper logHelp;

        public UsersController(OrganizationContext context, ILogger<UsersController> logger, TelemetryClient telemetry)
        {
            // Inject telemetry and logger is necessary in order to add
            // specific log in Azure ApplicationInsights
            _telemetry = telemetry;
            _logger = logger;
            _context = context;
            logHelp = new LoggerHelper();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            logHelp.Log(logHelp.getMessage(nameof(GetUsers)));
            _logger.LogInformation(logHelp.getMessage(nameof(GetUsers)));

            try
            {
                var findUsers = await _context.User.Select(x => ItemToDTO(x)).ToListAsync();
                
                if (findUsers == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUsers),"Users were not Found"));

                    logHelp.Log(logHelp.getMessage(nameof(GetUsers), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetUsers), "Users were not Found"));
                    return NotFound();
                }
                
                var result = new
                {
                    response = findUsers
                };

                _logger.LogInformation(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status200OK));
                logHelp.Log(logHelp.getMessage(nameof(GetUsers), StatusCodes.Status200OK));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetUsers),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetUsers), ex.Message));

                logHelp.Log(logHelp.getMessage(nameof(GetUsers), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetUsers), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(GetUser)));
            logHelp.Log(logHelp.getMessage(nameof(GetUser)));

            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null || !UserExists(id))
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUser),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(GetUser), "User was not Found"));
                    logHelp.Log(logHelp.getMessage(nameof(GetUser), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetUser), "User was not Found"));
                    return NotFound();
                }

                var result = new
                {
                    response = ItemToDTO(user)
                };
                
                _logger.LogInformation(logHelp.getMessage(nameof(GetUser),StatusCodes.Status200OK));
                logHelp.Log(logHelp.getMessage(nameof(GetUser), StatusCodes.Status200OK));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(GetUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(GetUser), ex.Message));
                logHelp.Log(logHelp.getMessage(nameof(GetUser), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetUser), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            logHelp.Log(logHelp.getMessage(nameof(PutUser)));
            _logger.LogInformation(logHelp.getMessage(nameof(PutUser)));
            
            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null || !UserExists(id))
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status404NotFound));
                    _logger.LogError(logHelp.getMessage(nameof(PutUser), "User was not Found"));
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), "User was not Found"));
                    return NotFound();
                }
                else if (id != userDTO.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutUser), "User was not Found"));
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status400BadRequest));
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), "UserID does not match"));
                    return BadRequest();
                }
                else
                {
                    user.ID = userDTO.ID;
                    user.Email = String.IsNullOrWhiteSpace(userDTO.Email) == false ? userDTO.Email : user.Email;
                    user.Password = String.IsNullOrWhiteSpace(userDTO.Password) == false ? StringEncryption(userDTO.Password) : user.Password;     // Password Encryption
                    user.FirstName = String.IsNullOrWhiteSpace(userDTO.FirstName) == false ? userDTO.FirstName : user.FirstName;
                    user.LastName = String.IsNullOrWhiteSpace(userDTO.LastName) == false ? userDTO.LastName : user.LastName;
                    user.OfficeID = userDTO.OfficeID != null ? userDTO.OfficeID : user.OfficeID;
                    user.Roles = userDTO.Roles;
                    //user.Roles = userDTO.RolesID;
                    //user.Role_ID = userDTO.Role_ID != null ? userDTO.Role_ID : user.Role_ID;
                    user.UpdatedAt = DateTime.Now;

                    _logger.LogInformation(logHelp.getMessage(nameof(PutUser),StatusCodes.Status200OK));

                    await _context.SaveChangesAsync();
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PutUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutUser), ex.Message));
                logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PutUser), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
        {
            logHelp.Log(logHelp.getMessage(nameof(PostUser)));
            _logger.LogInformation(logHelp.getMessage(nameof(PostUser)));

            try
            {
                var user = new User
                {
                    Email = userDTO.Email,
                    Password = StringEncryption(userDTO.Password),      // Password Encryption
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    OfficeID = userDTO.OfficeID ?? null,
                    Roles = userDTO.Roles,
                    //Roles = userDTO.RolesID,
                    //Role_ID = userDTO.Role_ID ?? null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();
                logHelp.Log(logHelp.getMessage(nameof(PostUser), StatusCodes.Status201Created));
                _logger.LogInformation(logHelp.getMessage(nameof(PostUser),StatusCodes.Status201Created));

                return CreatedAtAction(nameof(GetUser), new { id = user.ID }, ItemToDTO(user));
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(PostUser), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PostUser), ex.Message));
                _logger.LogError(logHelp.getMessage(nameof(PostUser), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PostUser), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(DeleteUser)));
            logHelp.Log(logHelp.getMessage(nameof(DeleteUser)));

            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteUser),StatusCodes.Status404NotFound));
                    _logger.LogWarning(logHelp.getMessage(nameof(DeleteUser),"User was not Found"));

                    logHelp.Log(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(DeleteUser), "User was not Found"));
                    return NotFound();
                }
                else
                {
                    _context.User.Remove(user);

                    await _context.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status204NoContent));
                    _logger.LogInformation(logHelp.getMessage(nameof(DeleteUser),StatusCodes.Status204NoContent));

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(DeleteUser),StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(DeleteUser), ex.Message));
                logHelp.Log(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(DeleteUser), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }

        private static UserDTO ItemToDTO(User user) => new UserDTO
        {
            ID = user.ID,
            Email = user.Email,
            Password = user.Password,
            FirstName = user.FirstName,
            LastName = user.LastName,
            OfficeID = user.OfficeID,
            Roles = user.Roles
            //RolesID = user.Roles
        };

        private string StringEncryption(string target)
        {
            _logger.LogInformation(logHelp.getMessage(nameof(StringEncryption)));
            logHelp.Log(logHelp.getMessage(nameof(StringEncryption)));
            byte[] salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // https://docs.microsoft.com/ja-jp/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-5.0
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: target,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: 10000,
              numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
