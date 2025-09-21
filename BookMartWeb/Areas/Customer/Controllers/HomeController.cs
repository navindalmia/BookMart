using BookMart.DataAccess.Repository;
using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using BookMart.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
namespace BookMartWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var claimsIdentiy = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentiy.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                   _unitOfWork.ShoppingCartRepository.GetAll(
               x => x.ApplicationUserId == claim.Value).Count()
                   );
            }

            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category,ProductImages").ToList();
           
            return View(objProductList);

        }
        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.ProductRepository.Get(u => u.Id == productId, "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };
            return View(shoppingCart);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentiy = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentiy.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            var shoppingCartFromDb = _unitOfWork.ShoppingCartRepository.Get(
                x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);
            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
                
                _unitOfWork.ShoppingCartRepository.Update(shoppingCartFromDb);
                _unitOfWork.Save();

            }
            else
            {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCartRepository.GetAll(
                x => x.ApplicationUserId == userId).Count()
                    );
            }
            TempData["success"] = "Cart updated successfully";
            
            
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
