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
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RolesController(ApplicationDbContext context)
        {
            _context=context;
        }

    }
}
