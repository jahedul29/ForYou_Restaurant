using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForYou.Models;
using ForYou.Data;
using ForYou.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ForYou.Utility;

namespace ForYou.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexVM = new IndexViewModel
            {
                MenuItemList = await _db.MenuItems.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync(),
                CategoryList = await _db.Categories.ToListAsync(),
                CouponList = await _db.Coupons.Where(m=>m.IsActive ==true).ToListAsync()
            };
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim!=null)
            {
                int count = _db.ShoppingCart.Where(m => m.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }

            return View(indexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItemFromDb = await _db.MenuItems.Include(u => u.Category).Include(u => u.SubCategory).FirstOrDefaultAsync(u => u.MenuItemId == id);
            if (menuItemFromDb == null)
            {
                return NotFound();
            }

            ShoppingCart cartObj = new ShoppingCart {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.MenuItemId
            };

            return View(cartObj);
        }

        //POST: Customer/Home/Details
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                cartObj.ApplicationUserId = claim.Value;

                var cartFromDb = await _db.ShoppingCart.
                    FirstOrDefaultAsync(u => u.ApplicationUserId == cartObj.ApplicationUserId && u.MenuItemId == cartObj.MenuItemId);

                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(cartObj);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cartObj.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(u => u.ApplicationUserId == cartObj.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemFromDb = await _db.MenuItems.Include(u => u.Category).Include(u => u.SubCategory).FirstOrDefaultAsync(u => u.MenuItemId == cartObj.MenuItemId);
                if (menuItemFromDb == null)
                {
                    return NotFound();
                }

                ShoppingCart cartObjPrevious = new ShoppingCart
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.MenuItemId
                };

                return View(cartObjPrevious);
            }
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
