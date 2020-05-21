using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models;
using ForYou.Models.ViewModel;
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

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            MenuItemVM = new MenuItemViewModel()
            {
                CategoryList = _db.Categories,
                MenuItem = new MenuItem()
            };
        }

        //GET: Admin/MenuItem/Index
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }

        //GET: Admin/MenuItem/Create
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }
    }
}