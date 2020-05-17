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
        [TempData]

        public string StatusMessage { get; set; }

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
                SubCategoryNameList = await _db.SubCategories.OrderBy(p => p.SubCategoryName).Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(viewModel);
        }

        //POST: Admin/SubCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var doesCategoryExist = _db.SubCategories.Include(s => s.Category).Where(s => s.SubCategoryName == viewModel.SubCategory.SubCategoryName && s.CategoryId == viewModel.SubCategory.CategoryId);
                if (doesCategoryExist.Count() > 0)
                {
                    StatusMessage = "Error : This sub category under " + doesCategoryExist.First().Category.CategoryName + " exist. Please use another name.";
                }
                else
                {
                    _db.SubCategories.Add(viewModel.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryNameList = await _db.SubCategories.OrderBy(p => p.SubCategoryName).Select(p => p.SubCategoryName).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(model);

        }

    }
}