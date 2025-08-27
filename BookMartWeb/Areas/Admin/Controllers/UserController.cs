using BookMart.DataAccess.Data;
using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using BookMart.Models.ViewModels;
using BookMart.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        
       
        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objApplicationUserList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
            
            var userRoles = _db.UserRoles.ToList();
            var Roles = _db.Roles.ToList();


            foreach(var user in objApplicationUserList)
            {
                var roleId = userRoles.FirstOrDefault(u=>u.UserId==user.Id)?.RoleId ;
                user.Role =  (roleId != null)?_db.Roles.FirstOrDefault(u=> u.Id==roleId).Name:"Role Not Assigned";

                if (user.Company == null)
                {
                    user.Company = new() {Name = ""};
                }
            }
            
            return Json(new { data = objApplicationUserList });

        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string? id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u=>u.Id==id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });

            }
            if (objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
            {//user is currently locked and we need to unlock
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success=true , message ="Successful" });

        }
        #endregion
    }
}
