using BookMart.DataAccess.Repository;
using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
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
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);

        }
        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.ProductRepository.Get(u => u.Id == productId, "Category"),
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
                
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                
            }
            TempData["success"] = "Cart updated successfully";
            _unitOfWork.Save();
            
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
