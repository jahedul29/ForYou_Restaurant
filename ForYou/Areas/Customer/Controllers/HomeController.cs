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
