using BookMart.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BookMartWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public IActionResult Index()
        {
            //List<Sho> objProductList = _unitOfWork.ProductRepository.GetAll("Category").ToList();
            return View();

        }

    }
}
