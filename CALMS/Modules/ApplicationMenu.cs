using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CALMS.Modules
{
    public class ApplicationMenu
    {
        [Key]
        public string Id { get; set; }
        public string PageName { get; set; }
        public string PageDesc { get; set; }
        public bool IsActive { get; set; }
        public string MenuHref { get; set; }
    }
    public class ApplicationMenuAssignment
    {
        public string ApplicationMenuId { get; set; }
        public string IdentityRoleId { get; set; }
        public ApplicationMenu ApplicationMenu { get; set; }
        public IdentityRole IdentityRole { get; set; }
    }
    public class ApplicationMenuView
    {
        public string PageName { get; set; }
    }

    public class ApplicationMenuAssignmentViewModel
    {
        public string IdentityRoleId { get; set; }
        public string[] ApplicationMenuId { get; set; } 
    }
}
