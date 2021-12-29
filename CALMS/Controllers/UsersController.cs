using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CALMS.Modules;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace CALMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public class ApplicationUserRegisterModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long, MinimumLength = 4")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
        //POST: api/Users/Register
        [HttpPost]
        [Route("Register")]
        public async Task<Object> Register(ApplicationUserRegisterModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email
            };
            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class ApplicationUserLoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        // POST: api/Users/Login
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(ApplicationUserLoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("Id", user.Id.ToString()));
                claims.Add(new Claim("Email", user.Email));
                var roles = await _userManager.GetRolesAsync(user);
                IdentityOptions identityOptions = new IdentityOptions();
                foreach(string role in roles){
                    claims.Add(new Claim(identityOptions.ClaimsIdentity.RoleClaimType, role));
                }
                var securityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims.ToArray()),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["JWTkey"].ToString())),
                    SecurityAlgorithms.HmacSha256Signature)
                };
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
                var token = jwtSecurityTokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return BadRequest("Invalid login attempt");
            }
        }

        // GET: api/Users/GetAuthorizedUserInfo
        [HttpGet]
        [Authorize]
        [Route("GetAuthorizedUserInfo")]
        public async Task<object> GetAuthorizedUserInfo()
        {
            string userId = User.Claims.First(c => c.Type == "Id").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user.Email
            };
        }
    }
}
