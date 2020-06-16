using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models;
using ForYou.Models.ViewModel;
using ForYou.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace ForYou.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public OrderDetailsCartViewModel DetailsCartVM { get; set; }

        public CartController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [Authorize]
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
                DetailsCartVM.OrderHeader.OrderTotal = DetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
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

        [Authorize]
        public async Task<IActionResult> Summery()
        {
            DetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == claim);

            var cartList = await _db.ShoppingCart.Where(u => u.ApplicationUserId == claim).ToListAsync();

            if (claim != null)
            {
                DetailsCartVM.ShoppingCartList = cartList;
            }

            foreach (var list in cartList)
            {
                list.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(u => u.MenuItemId == list.MenuItemId);
                DetailsCartVM.OrderHeader.OrderTotal = DetailsCartVM.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);

            }
            DetailsCartVM.OrderHeader.OrderTotalOriginal = DetailsCartVM.OrderHeader.OrderTotal;

            DetailsCartVM.OrderHeader.PickUpName = user.Name;
            DetailsCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            DetailsCartVM.OrderHeader.PickUpTime = DateTime.Now;
            DetailsCartVM.OrderHeader.PickUpDate = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var coupon = _db.Coupons.FirstOrDefault(u => u.CouponName.ToLower() == DetailsCartVM.OrderHeader.CouponCode.ToLower());
                DetailsCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(coupon, DetailsCartVM.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailsCartVM);
        }

        [HttpPost, ActionName(nameof(Summery))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummeryPost(string stripeToken)
        {
            //Saving OrderHeader To database
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
             var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            DetailsCartVM.ShoppingCartList = await _db.ShoppingCart.Where(u => u.ApplicationUserId == claim).ToListAsync();

            DetailsCartVM.OrderHeader.PaymentStatus = SD.StatusPending;
            DetailsCartVM.OrderHeader.Status = SD.StatusPending;
            DetailsCartVM.OrderHeader.ApplicationUserId = claim;
            DetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            DetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(DetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + " " + DetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeaders.Add(DetailsCartVM.OrderHeader);
            await _db.SaveChangesAsync();

            DetailsCartVM.OrderHeader.OrderTotalOriginal = 0;

            foreach (var item in DetailsCartVM.ShoppingCartList)
            {
                item.MenuItem = await _db.MenuItems.FirstOrDefaultAsync(u => u.MenuItemId == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    OrderId = DetailsCartVM.OrderHeader.OrderHeaderId,
                    MenuItemId = item.MenuItemId,
                    Count = item.Count,
                    ItemName = item.MenuItem.MenuItemName,
                    Price = item.MenuItem.Price
                };
                DetailsCartVM.OrderHeader.OrderTotalOriginal += item.Count * item.MenuItem.Price;
                _db.OrderDetails.Add(orderDetails);
            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var coupon = _db.Coupons.FirstOrDefault(u => u.CouponName.ToLower() == DetailsCartVM.OrderHeader.CouponCode.ToLower());
                DetailsCartVM.OrderHeader.OrderTotal = SD.DiscountedPrice(coupon, DetailsCartVM.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                DetailsCartVM.OrderHeader.OrderTotal = DetailsCartVM.OrderHeader.OrderTotalOriginal;
            }
            DetailsCartVM.OrderHeader.CouponCodeDiscount = DetailsCartVM.OrderHeader.OrderTotalOriginal - DetailsCartVM.OrderHeader.OrderTotal;

            _db.ShoppingCart.RemoveRange(DetailsCartVM.ShoppingCartList);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32((DetailsCartVM.OrderHeader.OrderTotal / 80) * 100),
                Currency = "usd",
                Description = "Order Id : " + DetailsCartVM.OrderHeader.OrderHeaderId,
                Source = stripeToken
            };

            var service = new ChargeService();

            Charge charge = service.Create(options);

            
            if (charge.Status.ToLower() == "succeeded")
            {
                DetailsCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;

                await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == claim).FirstOrDefault().Email,
                    "ForYou payment completed OrderId: " + DetailsCartVM.OrderHeader.OrderHeaderId.ToString(),
                    "You have paid " + DetailsCartVM.OrderHeader.OrderTotal.ToString() + " for your Order<br /> <strong class =\"text-info\">THANK YOU!!</strong>");
                 
                DetailsCartVM.OrderHeader.PaymentStatus = SD.StatusApproved;
                DetailsCartVM.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == claim).FirstOrDefault().Email,
                    "ForYou payment completed OrderId: " + DetailsCartVM.OrderHeader.OrderHeaderId.ToString(),
                    "Your payment request failed anyway. Please contuct us for more information.<br /> <strong class =\"text-info\">THANK YOU!!</strong>");

                DetailsCartVM.OrderHeader.PaymentStatus = SD.StatusRejected;
            }

            await _db.SaveChangesAsync();
            //return RedirectToAction("Index", "Cart");
            return RedirectToAction("Confirm", "Order", new { id = DetailsCartVM.OrderHeader.OrderHeaderId });
        }

    }

}