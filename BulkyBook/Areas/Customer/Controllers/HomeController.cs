using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                .GetAll(sc => sc.ApplicationUserId == claim.Value)
                .ToList().Count();

            HttpContext.Session.SetInt32(SD.ssShoppringCart, count);
            }

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.
                    GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if (!ModelState.IsValid)
            {
                var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(p => p.Id == cartObject.Id, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cartObject.ApplicationUserId = claim.Value;
            
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                sc => sc.ApplicationUserId == cartObject.ApplicationUserId && sc.ProductId == cartObject.ProductId
                , includeProperties: "Product"
            );

            if (cartFromDb == null)
            {
                //no records exists in database for that product for that user
                _unitOfWork.ShoppingCart.Add(cartObject);
            }
            else
            {
                cartFromDb.Count += cartObject.Count;
                // _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();

            var count = _unitOfWork.ShoppingCart
                .GetAll(sc => sc.ApplicationUserId == cartObject.ApplicationUserId)
                .ToList().Count();

            // HttpContext.Session.SetObject(SD.ssShoppringCart, cartObject);
            HttpContext.Session.SetInt32(SD.ssShoppringCart, count);

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
