using GoldenBook.DataAccess.Repository.IRepository;
using GoldenBook.Models;
using GoldenBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace _1KitabWeb.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofwork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitofwork)
        {
            _logger = logger;
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitofwork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId= productId,
                Product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType")
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claims.Value;

            ShoppingCart shopcart = _unitofwork.ShoppingCart.GetFirstOrDefault(u => u.ProductId == cart.ProductId && u.ApplicationUserId == claims.Value);

            if (shopcart==null)
            {
                _unitofwork.ShoppingCart.Add(cart);
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value).ToList().Count);
            }
            else
            {
                _unitofwork.ShoppingCart.IncrementCount(shopcart, cart.Count);
            }
            
            _unitofwork.Save();

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