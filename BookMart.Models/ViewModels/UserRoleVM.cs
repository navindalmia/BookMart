using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMart.Models.ViewModels
{
    public class UserRoleVM
    {

        public ApplicationUser ApplicationUser {  get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public string SelectedRoleId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CompanyList { get; set; }
        public int? SelectedCompanyId { get; set; }
    }
}
