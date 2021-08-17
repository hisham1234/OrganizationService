using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Organization_Service.Helpers;
using Organization_Service.Models;

namespace Organization_Service.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;
        private readonly OrganizationContext _context;
        private readonly IMapper _mapper;
        private LoggerHelper logHelp;

        public AccountsController(OrganizationContext context, ILogger<UsersController> logger, TelemetryClient telemetry, IMapper mapper)
        {
            _logger = logger;
            _telemetry = telemetry;
            _context = context;
            _mapper = mapper;
           logHelp = new LoggerHelper();
        }

        // GET api/account/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponseDTO>> Me()
        {
            /**
             * Get user email from token to return all information about the user.
             */

            var currentUser = HttpContext.User;
            string email = currentUser.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            var findUser = await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();
            var result = new {
                response = _mapper.Map<UserResponseDTO>(findUser)
            };

            return Ok(result);    
        }

        // POST api/account/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginAnswerModel>> Login(UserLoginModel user)
        {
            // Get user with email passed in
            var findUser = await _context.User.Where(u => u.Email == user.Email).FirstOrDefaultAsync();

            // Check if user linked to the token exist
            if(findUser == null)
            {
                return StatusCode((int)System.Net.HttpStatusCode.Unauthorized, "Bad Login / Password");
            }

            // Password verification.
            var encriptedLoginPw = SaltedHashedHelper.StringEncrypt(user.Password, findUser.Salt);

            if (encriptedLoginPw == findUser.Password)
            {
                return Ok(new LoginAnswerModel
                {
                    Message = "Authentication success",
                    Token = SaltedHashedHelper.GenerateJSONWebToken(findUser) // Generated token
                });
            }
            else
            {
                return StatusCode((int)System.Net.HttpStatusCode.Unauthorized, "Bad Login / Password");

            }
        }

        // PUT api/account/me
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> PutMe(UserDTO userDTO)
        {
            /**
             * Get user email from token and update the related user with the information passed in the request body.
             */
            _logger.LogInformation(logHelp.getMessage(nameof(PutMe)));
            var currentUser = HttpContext.User;
           

            try
            {

                string email = currentUser.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                var findUser = await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();

                // Test if the user got from the email has the same ID that the user in parameter
                if (findUser.ID != userDTO.ID)
                {
                    _logger.LogError(logHelp.getMessage(nameof(PutMe), StatusCodes.Status400BadRequest));
                    _logger.LogError(logHelp.getMessage(nameof(PutMe), "Id found missmatch the Id related to the token"));


                    return BadRequest();
                }

                // If the user have roles
                if (userDTO.RolesID != null)
                {
                    userDTO.RolesID.Sort();

                    var findRoles = await _context.Role.AsNoTracking().Where(r => userDTO.RolesID.Contains(r.ID)).OrderBy(r => r.ID).ToListAsync();

                    if (findRoles == null)
                    {
                        _logger.LogWarning(logHelp.getMessage(nameof(PutMe), StatusCodes.Status404NotFound));
                        _logger.LogWarning(logHelp.getMessage(nameof(PutMe), "Roles was not Found"));

                        return NotFound();
                    }

                    if (userDTO.RolesID.SequenceEqual(findRoles.Select(r => r.ID).ToList()) == false)
                    {
                        _logger.LogError(logHelp.getMessage(nameof(PutMe), StatusCodes.Status400BadRequest));
                        _logger.LogError(logHelp.getMessage(nameof(PutMe), "RolesID does not match"));

                        return BadRequest();
                    }

                    findUser.Roles = findRoles;
                }
                else
                {
                    findUser.Roles = findUser.Roles;
                }

                if(String.IsNullOrWhiteSpace(userDTO.Password) == false )
                {
                    findUser.Salt = SaltedHashedHelper.GetSalt();
                    findUser.Password = SaltedHashedHelper.StringEncrypt(userDTO.Password, findUser.Salt);
                }
                
                findUser.ID = userDTO.ID;
                findUser.Email = String.IsNullOrWhiteSpace(userDTO.Email) == false ? userDTO.Email : findUser.Email;
                findUser.FirstName = String.IsNullOrWhiteSpace(userDTO.FirstName) == false ? userDTO.FirstName : findUser.FirstName;
                findUser.LastName = String.IsNullOrWhiteSpace(userDTO.LastName) == false ? userDTO.LastName : findUser.LastName;
                findUser.OfficeID = userDTO.OfficeID != null ? userDTO.OfficeID : findUser.OfficeID;
                findUser.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var result = new {
                    response = _mapper.Map<UserResponseDTO>(findUser)
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(PutMe), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(PutMe), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE api/account/me
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult>  DeleteMe()
        {

            /**
             * Get email from token and delete related user.
             */
            _logger.LogInformation(logHelp.getMessage(nameof(DeleteMe)));
            try
            {
               var currentUser = HttpContext.User;
               string email = currentUser.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
               var findUser = await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();
                _context.User.Remove(findUser);

                await _context.SaveChangesAsync();

                _logger.LogInformation(logHelp.getMessage(nameof(DeleteMe), StatusCodes.Status204NoContent));

                var result = new
                {
                    response = _mapper.Map<UserResponseDTO>(findUser)
                };
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(logHelp.getMessage(nameof(DeleteMe), StatusCodes.Status500InternalServerError));
                _logger.LogError(logHelp.getMessage(nameof(DeleteMe), ex.Message));

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST api/account/verify
        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> Verify()
        {

            /**
             * Get user email from token and check if the user exist.
             * Return code 200 if the user is existing
             * Return code 401 if the user is not existing (unauthorized token).
             */

            var currentUser = HttpContext.User;
            string email = currentUser.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            var findUser = await _context.User.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (findUser == null)
                return StatusCode((int)System.Net.HttpStatusCode.Unauthorized, "User has not been found");

            return Ok();
        }
    }
}
