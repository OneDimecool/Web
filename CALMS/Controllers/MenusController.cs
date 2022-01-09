using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CALMS.Modules;
using Microsoft.EntityFrameworkCore;

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
    }
}
