using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models;
using ForYou.Models.ViewModel;
using ForYou.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]        

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

        //GET:Admin/SubCategory/Create
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

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategories
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "SubCategoryId", "SubCategoryName"));
        }

        //GET:Admin/SubCategory/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.SubCategoryId == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryNameList = await _db.SubCategories.OrderBy(p => p.SubCategoryName).Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(viewModel);
        }

        //POST: Admin/SubCategory/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel viewModel)
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
                    var subCatFromDb = await _db.SubCategories.FindAsync(viewModel.SubCategory.SubCategoryId);
                    subCatFromDb.SubCategoryName = viewModel.SubCategory.SubCategoryName;

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

        //GET:Admin/SubCategory/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.SubCategoryId == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryNameList = await _db.SubCategories.OrderBy(p => p.SubCategoryName).Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(viewModel);
        }

        //GET:Admin/SubCategory/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.SubCategoryId == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryNameList = await _db.SubCategories.OrderBy(p => p.SubCategoryName).Select(p => p.SubCategoryName).Distinct().ToListAsync()
            };
            return View(viewModel);
        }

        //POST: Admin/SubCategory/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.FindAsync(id);

            if (subCategory == null)
            {
                return NotFound();
            }

            _db.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}