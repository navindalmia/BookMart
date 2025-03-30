using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using BookMart.Models.ViewModels;
using BookMart.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookMartWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
                _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details( int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")

            };

            return View(orderVM);
        }
        #region API Calls

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();


            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where( u=> u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = objOrderHeaders });

        }
        
        #endregion
    }
}
