using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models;
using ForYou.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET:Admin/SubCategory/Index
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategories.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        //GET:Admin/SubCategory/Index
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryNameList = await _db.SubCategories.OrderBy(p=>p.SubCategoryName).Select(p=>p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(viewModel);
        }


    }
}