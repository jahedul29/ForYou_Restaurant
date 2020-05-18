using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForYou.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        //GET: Admin/MenuItem/Index
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }
    }
}