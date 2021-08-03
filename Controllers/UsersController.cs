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

namespace Organization_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly OrganizationContext _context;
        private LoggerHelper logHelp;

        public UsersController(OrganizationContext context)
        {
            _context = context;
            logHelp = new LoggerHelper();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            logHelp.Log(logHelp.getMessage(nameof(GetUsers)));

            try
            {
                var findUsers = await _context.User.AsNoTracking().Include(u => u.Roles).Select(x => ItemToDTO(x)).ToListAsync();
                
                if (findUsers == null)
                {
                    logHelp.Log(logHelp.getMessage(nameof(GetUsers), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetUsers), "Users were not Found"));
                    return NotFound();
                }
                
                var result = new
                {
                    response = findUsers
                };

                logHelp.Log(logHelp.getMessage(nameof(GetUsers), StatusCodes.Status200OK));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(GetUsers), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(GetUsers), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            logHelp.Log(logHelp.getMessage(nameof(GetUser)));

            try
            {
                var findUser = await _context.User.Where(u => u.ID == id).Include(u => u.Roles).Select(u => ItemToDTO(u)).FirstOrDefaultAsync();

                if (findUser == null || !UserExists(id))
                {
                    logHelp.Log(logHelp.getMessage(nameof(GetUser), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(GetUser), "User was not Found"));
                    return NotFound();
                }

                var result = new
                {
                    response = findUser
                };

                logHelp.Log(logHelp.getMessage(nameof(GetUser), StatusCodes.Status200OK));
                return Ok(result);
            }
            catch (Exception ex)
            {
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
            
            try
            {
                var user = await _context.User.FindAsync(id);

                if (user == null || !UserExists(id))
                {
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), "User was not Found"));
                    return NotFound();
                }

                if (id != userDTO.ID)
                {
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status400BadRequest));
                    logHelp.Log(logHelp.getMessage(nameof(PutUser), "UserID does not match"));
                    return BadRequest();
                }

                if (userDTO.RolesID != null)
                {
                    userDTO.RolesID.Sort();

                    var findRoles = await _context.Role.AsNoTracking().Where(r => userDTO.RolesID.Contains(r.ID)).OrderBy(r => r.ID).ToListAsync();

                    if (findRoles == null)
                    {
                        logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status404NotFound));
                        logHelp.Log(logHelp.getMessage(nameof(PutUser), "Roles was not Found"));
                        return NotFound();
                    }

                    if (userDTO.RolesID.SequenceEqual(findRoles.Select(r => r.ID).ToList()) == false)
                    {
                        logHelp.Log(logHelp.getMessage(nameof(PutUser), StatusCodes.Status400BadRequest));
                        logHelp.Log(logHelp.getMessage(nameof(PutUser), "RoleID does not match"));
                        return BadRequest();
                    }

                    user.Roles = findRoles;
                }
                else
                {
                    user.Roles = user.Roles;
                }

                user.ID = userDTO.ID;
                user.Email = String.IsNullOrWhiteSpace(userDTO.Email) == false ? userDTO.Email : user.Email;
                user.Password = String.IsNullOrWhiteSpace(userDTO.Password) == false ? StringEncryption(userDTO.Password) : user.Password;     // Password Encryption
                user.FirstName = String.IsNullOrWhiteSpace(userDTO.FirstName) == false ? userDTO.FirstName : user.FirstName;
                user.LastName = String.IsNullOrWhiteSpace(userDTO.LastName) == false ? userDTO.LastName : user.LastName;
                user.OfficeID = userDTO.OfficeID != null ? userDTO.OfficeID : user.OfficeID;
                user.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

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

            try
            {
                var user = new User
                {
                    Email = userDTO.Email,
                    Password = StringEncryption(userDTO.Password),      // Password Encryption
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    OfficeID = userDTO.OfficeID ?? null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.User.Add(user);
                await _context.SaveChangesAsync();
                logHelp.Log(logHelp.getMessage(nameof(PostUser), StatusCodes.Status201Created));
                return CreatedAtAction(nameof(GetUser), new { id = user.ID }, ItemToDTO(user));
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(PostUser), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(PostUser), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            logHelp.Log(logHelp.getMessage(nameof(DeleteUser)));

            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    logHelp.Log(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status404NotFound));
                    logHelp.Log(logHelp.getMessage(nameof(DeleteUser), "User was not Found"));
                    return NotFound();
                }
                else
                {
                    _context.User.Remove(user);
                    await _context.SaveChangesAsync();
                    logHelp.Log(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status204NoContent));
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage(nameof(DeleteUser), StatusCodes.Status500InternalServerError));
                logHelp.Log(logHelp.getMessage(nameof(DeleteUser), ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool UserExists(int id)
        {
            logHelp.Log(logHelp.getMessage(nameof(UserExists)));
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
            RolesID = user.Roles.Select(r => r.ID).ToList()
        };

        private string StringEncryption(string target)
        {
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
