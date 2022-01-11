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
using Microsoft.EntityFrameworkCore;

namespace CALMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly ApplicationDbContext _applicationDbContext;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
        }
        public class ApplicationUserRegisterModel
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long, MinimumLength = 4")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class ApplicationUserViewModel
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string[] Roles { get; set; }
        }
        //POST: api/Users/Register
        [HttpPost]
        [Route("Register")]
        public async Task<Object> Register(ApplicationUserRegisterModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName };
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
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        // POST: api/Users/Login
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(ApplicationUserLoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("Id", user.Id.ToString()));
                claims.Add(new Claim("UserName", user.UserName));
                var roles = await _userManager.GetRolesAsync(user);
                IdentityOptions identityOptions = new IdentityOptions();
                foreach (string role in roles) {
                    claims.Add(new Claim(identityOptions.ClaimsIdentity.RoleClaimType, role));
                }
                var securityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims.ToArray()),
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(0.5),
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
                user.UserName
            };
        }
        //GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<ApplicationUserViewModel>>> Users()
        {
            List<ApplicationUserViewModel> applicationUserViewModels = new List<ApplicationUserViewModel>();
            List<ApplicationUser> applicationUsers = await _applicationDbContext.Users.ToListAsync();
            foreach (ApplicationUser applicationUser in applicationUsers) {
                applicationUserViewModels.Add(new ApplicationUserViewModel()
                {
                    Id = applicationUser.Id,
                    UserName = applicationUser.UserName,
                    Roles = _userManager.GetRolesAsync(applicationUser).Result.ToArray()
                }
                );

            }
            return applicationUserViewModels;

        }
        //GET: api/Users/id
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApplicationUserViewModel>> GetUser(string id)
        {
            var applicationUser = await _applicationDbContext.Users.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            ApplicationUserViewModel applicationUserViewModel = new ApplicationUserViewModel()
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName,
                Roles = _userManager.GetRolesAsync(applicationUser).Result.ToArray()
            };
            return applicationUserViewModel;
        }
        //Delete: api/Users 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApplicationUserViewModel>> DeleteUser(string id)
        {
            var applicationUser = await _applicationDbContext.Users.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            _applicationDbContext.Users.Remove(applicationUser);
            await _applicationDbContext.SaveChangesAsync();

            return new ApplicationUserViewModel()
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName,
                Roles = _userManager.GetRolesAsync(applicationUser).Result.ToArray()
            };
        }

        //PUT: api/Users/
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApplicationUserViewModel>> PutUser(string id, ApplicationUserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            var applicationUser = await _applicationDbContext.Users.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            _applicationDbContext.Entry(applicationUser).State = EntityState.Modified;
            try
            {
                var userRoles = await _userManager.GetRolesAsync(applicationUser);
                await _userManager.RemoveFromRolesAsync(applicationUser, userRoles.ToArray());
                await _userManager.AddToRolesAsync(applicationUser, model.Roles);
                await _applicationDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (applicationUser == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        //GET: api/Users/GetRoles
        [HttpGet]
        [Route("GetRoles")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
        {
            return await _applicationDbContext.Roles.ToListAsync();
        }

        public class IdentityRoleView
         {
            public string Name { get; set; }
        }
        //POST:api/Users/AddRoles
        [HttpPost]
        [Route("AddRole")]
        [Authorize(Roles ="Administrator")]
        public async Task<Object> AddRole(string role)
        {
            var _identityRole = new IdentityRole()
            {
                Name = role,
                NormalizedName = role.ToUpper()
            };
            try
            {
                var result = await _roleManager.CreateAsync(_identityRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete: api/Users/DeleteRole/5
        [HttpDelete]
        [Route("DeleteRole")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IdentityRole>> DeleteRole(string role)
        {
            var identityRole = await _roleManager.FindByNameAsync(role);
            if (identityRole == null)
            {
                return NotFound();
            }
            _applicationDbContext.Roles.Remove(identityRole);
            await _applicationDbContext.SaveChangesAsync();

            return new IdentityRole()
            {
                Id = identityRole.Id,
                Name = identityRole.Name,
            };
        }



    }
}
