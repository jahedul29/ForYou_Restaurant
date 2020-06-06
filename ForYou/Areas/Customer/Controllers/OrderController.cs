using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            OrderHeaderAndOrderDetailsViewModel detailsVM = new OrderHeaderAndOrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeaders.Include(u => u.ApplicationUser).Where(u => u.OrderHeaderId == id && u.ApplicationUserId == claim).FirstOrDefaultAsync(),
                OrderDetailsList = await _db.OrderDetails.Where(u => u.OrderId == id).ToListAsync()
            };

            return View(detailsVM);
        }
    }
}