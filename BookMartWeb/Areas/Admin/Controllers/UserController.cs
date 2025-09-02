using BookMart.DataAccess.Data;
using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using BookMart.Models.ViewModels;
using BookMart.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookMartWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserController> _logger;

        //for trial using dbcontext instead od UnitofWork
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _db = db;

        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult ManageUserRole(string? id)
        {
            _logger.LogDebug("Create invoked");
            //CompanyList
            var companies = _db.Companies.ToList();
            //RoleList
            var roles = _db.Roles.ToList(); //AspNetRoles
            //UserRole Mapping
            var userRoles = _db.UserRoles.ToList();

            ApplicationUser objFromDb = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == id);
            //User specific role
            var userRoleId = userRoles.FirstOrDefault(u => u.UserId == objFromDb.Id)?.RoleId;
            var roleName = (userRoleId != null) ? _db.Roles.FirstOrDefault(u => u.Id == userRoleId).Name : "Role Not Assigned";
            objFromDb.Role = roleName;
            UserRoleVM userRoleVM = new()
            {
                ApplicationUser = objFromDb,

                SelectedRoleId = userRoleId,
                RoleList = roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id,
                    Selected = roleName == r.Name ? true : false
                }
                ),
                CompanyList = companies.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id.ToString(),
                    Selected = objFromDb.CompanyId == r.Id ? true : false
                }
                ),
                SelectedCompanyId = objFromDb.CompanyId

            };



            return View(userRoleVM);



        }
        [HttpPost]
        public IActionResult UpdateUserRole(UserRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                //return RedirectToAction("ManageUserRole",model.ApplicationUser.Id);
                return RedirectToAction("Index");
            }

            //get user from database
            ApplicationUser objFromDb = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == model.ApplicationUser.Id);


            //update roles if required
            var existingUserRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (existingUserRole != null)
            {
                if (existingUserRole.RoleId != model.SelectedRoleId)
                {
                    //remove existing role
                    _db.UserRoles.Remove(existingUserRole);

                    //add new role

                    _db.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                    {
                        UserId = objFromDb.Id,
                        RoleId = model.SelectedRoleId
                    });

                }
            }
            else //existing role does not exist, hence add new role
            {
                //add new role
                _db.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                {
                    UserId = objFromDb.Id,
                    RoleId = model.SelectedRoleId
                });
            }
            //Update Company of required
            var companyRoleId = _db.Roles.FirstOrDefault(u => u.Name == SD.Role_Company)?.Id;

            if(model.SelectedRoleId == companyRoleId)
            {
                if(model.SelectedCompanyId!= objFromDb.CompanyId)
                {
                    objFromDb.CompanyId = model.SelectedCompanyId;
                }


            }
            else
            {

                if (objFromDb.CompanyId != null)
                {
                    objFromDb.CompanyId = null;
                }
            }
            _db.SaveChanges();
            TempData["success"] = "User role updated successfully";
            return RedirectToAction("Index");



        }




        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objApplicationUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();//to access [AspNetUserRoles]
            var Roles = _db.Roles.ToList(); //AspNetRoles


            foreach (var user in objApplicationUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = (roleId != null) ? _db.Roles.FirstOrDefault(u => u.Id == roleId).Name : "Role Not Assigned";

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = objApplicationUserList });

        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });

            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {//user is currently locked and we need to unlock
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Successful" });

        }
        #endregion
    }
}
