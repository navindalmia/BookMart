using BookMart.DataAccess.Repository;
using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll("Category").ToList();
            return View(objProductList);
            
        }
        public IActionResult Details(int id)
        {
            Product product = _unitOfWork.ProductRepository.Get(u=>u.Id==id,"Category");
            return View(product);

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
