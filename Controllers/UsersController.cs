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
            logHelp.Log(logHelp.getMessage("GetUsers"));
            try
            {
                var findUsers = await _context.User.Select(x => ItemToDTO(x)).ToListAsync();
                if (findUsers == null)
                {
                    logHelp.Log(logHelp.getMessage("GetUsers", 404));
                    return NotFound();
                }
                var result = new
                {
                    response = findUsers,
                };
                logHelp.Log(logHelp.getMessage("GetUsers", 200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetUsers", 500));
                logHelp.Log(logHelp.getMessage("GetUsers", ex.Message));
                return StatusCode(500);
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            logHelp.Log(logHelp.getMessage("GetUser"));
            try
            {
                var user = await _context.Office.FindAsync(id);
                if (user == null || !UserExists(id))
                {
                    logHelp.Log(logHelp.getMessage("GetUser", 404));
                    return NotFound();
                }
                var result = new
                {
                    response = user,
                };
                logHelp.Log(logHelp.getMessage("GetUser", 200));
                return Ok(result);
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("GetUser", 500));
                logHelp.Log(logHelp.getMessage("GetUser", ex.Message));
                return StatusCode(500);
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            logHelp.Log(logHelp.getMessage("PutUser"));
            var user = await _context.User.FindAsync(id);
            try
            {
                if (user == null|| !UserExists(id))
                {
                    logHelp.Log(logHelp.getMessage("PutUser", 500));
                    logHelp.Log(logHelp.getMessage("PutUser", "User was not Found"));
                    return NotFound();
                }
                else if (id != userDTO.ID)
                {
                    logHelp.Log(logHelp.getMessage("PutUser", 500));
                    logHelp.Log(logHelp.getMessage("PutUser", "User was not Found"));
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

                    await _context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {

                logHelp.Log(logHelp.getMessage("PutUser", 500));
                logHelp.Log(logHelp.getMessage("PutUser", ex.Message));
                return StatusCode(500);
            }
            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
        {
            logHelp.Log(logHelp.getMessage("PostUser"));
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

            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                logHelp.Log(logHelp.getMessage("PostUser", 500));
                logHelp.Log(logHelp.getMessage("Error while creating new user"));
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("PostUser", 500));
                logHelp.Log(logHelp.getMessage("PostUser", ex.Message));
                return StatusCode(500);
            }
            return CreatedAtAction(nameof(GetUser), new { id = user.ID }, ItemToDTO(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            logHelp.Log(logHelp.getMessage("DeleteUser"));
            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    logHelp.Log(logHelp.getMessage("DeleteUser", 404));
                    return NotFound();
                }
                else
                {
                    _context.User.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logHelp.Log(logHelp.getMessage("DeleteUser", 500));
                logHelp.Log(logHelp.getMessage("DeleteUser", ex.Message));
                return StatusCode(500);
            }
            return NoContent();
        }

        private bool UserExists(int id)
        {
            logHelp.Log(logHelp.getMessage("UserExists"));
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
            logHelp.Log(logHelp.getMessage("StringEncryption"));
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
