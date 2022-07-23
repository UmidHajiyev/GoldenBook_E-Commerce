using GoldenBook.DataAccess.Repository;
using GoldenBook.DataAccess.Repository.IRepository;
using GoldenBook.Models;
using GoldenBook.Models.ViewModels;
using GoldenBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace GoldenBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderfromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id);
            orderHeaderfromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderfromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderfromDb.Address = orderVM.OrderHeader.Address;
            orderHeaderfromDb.State = orderVM.OrderHeader.State;
            orderHeaderfromDb.City = orderVM.OrderHeader.City;
            orderHeaderfromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            if (orderVM.OrderHeader.TrackingId !=null)
            {
                orderHeaderfromDb.TrackingId = orderVM.OrderHeader.TrackingId;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderfromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details uptaded successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderHeaderfromDb.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {            
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id,SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order status uptaded successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeaderfromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id);
            orderHeaderfromDb.TrackingId = orderVM.OrderHeader.TrackingId;
            orderHeaderfromDb.ShippingDate = DateTime.Now;
            orderHeaderfromDb.OrderStatus = SD.StatusShipped;
            _unitOfWork.OrderHeader.Update(orderHeaderfromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order status uptaded successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeaderfromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id);

            _unitOfWork.OrderHeader.UpdateStatus(orderHeaderfromDb.Id,SD.StatusCancelled,SD.StatusRefunded);
            _unitOfWork.Save();
            TempData["success"] = "Order cancelled successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPayNow(int orderId)
        {
            orderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser");
            orderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product");
            var domain = HttpContext.Request.GetDisplayUrl().Substring(0, 24);
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
            };
            foreach (var item in orderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100 / 1.7),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
           orderVM.OrderHeader.SessionId = session.Id;
            orderVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderheader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderId);
            if (orderheader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderheader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderheader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            return View(orderHeaderId);
        }

        public IActionResult Details(int orderId)
        {
            
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u=>u.Id==orderId,includeProperties:"ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u=>u.OrderId==orderId,includeProperties:"Product")
            };

            return View(orderVM);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_Admin)||User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId==claim.Value,includeProperties: "ApplicationUser");
            }



            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u=>u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }



            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
