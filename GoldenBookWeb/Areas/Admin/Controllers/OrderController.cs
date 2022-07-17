using GoldenBook.DataAccess.Repository.IRepository;
using GoldenBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders;

            orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
