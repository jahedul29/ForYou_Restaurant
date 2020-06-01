using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models.ViewModel;
using ForYou.Utility;
using Microsoft.AspNetCore.Http;
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
                DetailsCartVM.OrderHeader.OrderTotal = DetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price*list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);

                if (list.MenuItem.Description.Length > 60)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 59) + "....";
                }
            }
            DetailsCartVM.OrderHeader.OrderTotalOriginal = DetailsCartVM.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var coupon = _db.Coupons.FirstOrDefault(u => u.CouponName.ToLower() == DetailsCartVM.OrderHeader.CouponCode.ToLower());
                DetailsCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(coupon, DetailsCartVM.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailsCartVM);
        }

        public IActionResult AddCoupon()
        {
            if (DetailsCartVM.OrderHeader.CouponCode == null)
            {
                DetailsCartVM.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, DetailsCartVM.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await _db.ShoppingCart.FirstOrDefaultAsync(u => u.Id == cartId);
            cartFromDb.Count += 1;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartFromDb = await _db.ShoppingCart.FirstOrDefaultAsync(u => u.Id == cartId);
            if (cartFromDb.Count == 1)
            {
                _db.ShoppingCart.Remove(cartFromDb);

                var cartCount = _db.ShoppingCart.Where(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cartCount);
            }
            else
            {
                cartFromDb.Count -= 1;
            }
            
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await _db.ShoppingCart.FirstOrDefaultAsync(u => u.Id == cartId);
            _db.ShoppingCart.Remove(cartFromDb);

            var cartCount = _db.ShoppingCart.Where(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cartCount);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}