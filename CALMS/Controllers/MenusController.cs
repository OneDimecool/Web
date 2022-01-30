using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CALMS.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CALMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MenusController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        public MenusController(ApplicationDbContext context)
        {
            applicationDbContext = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationMenu>>> GetMenuList()
        {
            return await applicationDbContext.Menus.ToListAsync();
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApplicationMenuAssignment>> PutMenuAssignment(ApplicationMenuAssignmentViewModel model)
        {
            string roleId = model.IdentityRoleId;
            List <ApplicationMenuAssignment> applicationMenuAssignments= new List<ApplicationMenuAssignment>();
            
                applicationMenuAssignments = await applicationDbContext.MenuRoles.ToListAsync();
                var _applicationMenuAssignments = applicationMenuAssignments.FindAll(e => e.IdentityRoleId == roleId);
            try
            {
                foreach (var a in _applicationMenuAssignments) { 
                    applicationDbContext.MenuRoles.Remove(a);
                }
                List<ApplicationMenuAssignment> applicationMenuAssignments_add = new List<ApplicationMenuAssignment>();
                foreach (var menuId in model.ApplicationMenuId) {
                    var applicationMenuAssignment_add = new ApplicationMenuAssignment();
                    applicationMenuAssignment_add.IdentityRoleId = roleId;
                    applicationMenuAssignments_add.Add(applicationMenuAssignment_add);
                }
                await applicationDbContext.AddRangeAsync(applicationMenuAssignments_add);
                applicationDbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) {
                if (_applicationMenuAssignments == null)
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

        [HttpGet("{id}")]
        [Route("GetMenusById")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IdentityRoleModel>> GetMenusById(string id)
        {
            var applicationRole= await applicationDbContext.Roles.FindAsync(id);
            if (applicationRole == null)
            {
                return NotFound();
            }
            IdentityRoleModel roleModel = new IdentityRoleModel() 
            { 
                Id=applicationRole.Id,
                RoleName=applicationRole.Name,
                MenuNames= GetMenuName(id),
                MenuId= GetMenuId(id)
            };

            return roleModel;
        }

        public string[] GetMenuName(string id)
        {
            var Menus = applicationDbContext.MenuRoles.ToList();
            var _Menus = Menus.FindAll(e => e.IdentityRoleId == id);
            string[] MenuArray = new string[_Menus.Count];
            int i = 0;
            foreach (var a in _Menus)
            {
                MenuArray[i] = GetMenuNameById(a.ApplicationMenuId);
            }
            return MenuArray;
        }

        public string GetMenuNameById(string id)
        {
            var MenuName = applicationDbContext.Menus.Find(id);
            return MenuName.PageName;
        }
        public string[] GetMenuId(string id)
        {
            var Menus = applicationDbContext.MenuRoles.ToList();
            var _Menus = Menus.FindAll(e => e.IdentityRoleId == id);
            string[] MenuList = new string[_Menus.Count];
            int i = 0;
            foreach (var a in _Menus)
            {
                MenuList[i] = a.ApplicationMenuId;
            }
            return MenuList;
        }
    }
}
