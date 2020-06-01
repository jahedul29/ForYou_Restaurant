using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models.ViewModel;
using ForYou.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsCartViewModel DetailsCartVM { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            DetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartList = await _db.ShoppingCart.Where(u => u.ApplicationUserId == claim).ToListAsync();

            if (claim != null)
            {
                DetailsCartVM.ShoppingCartList = cartList;
            }

            foreach (var list in cartList)
            {
                list.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(u => u.MenuItemId == list.MenuItemId);
                DetailsCartVM.OrderHeader.OrderTotal = DetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);

                if (list.MenuItem.Description.Length > 60)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 59) + "....";
                }
            }
            DetailsCartVM.OrderHeader.OrderTotalOriginal = DetailsCartVM.OrderHeader.OrderTotal;

            return View(DetailsCartVM);
        }
    }
}