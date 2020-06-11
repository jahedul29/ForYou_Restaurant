using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models;
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

        private int PageSize = 2;

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


        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            OrderHeaderAndOrderDetailsListViewModel OrderAndPaging = new OrderHeaderAndOrderDetailsListViewModel()
            {
                OrderList = new List<OrderHeaderAndOrderDetailsViewModel>()
            };

            List<OrderHeader> OrderHeaderList = await _db.OrderHeaders.Include(u => u.ApplicationUser).Where(u => u.ApplicationUserId == claim).ToListAsync();

            foreach (var orderHeader in OrderHeaderList)
            {
                OrderHeaderAndOrderDetailsViewModel invidualOrder = new OrderHeaderAndOrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetailsList = await _db.OrderDetails.Where(u => u.OrderId == orderHeader.OrderHeaderId).ToListAsync()
                };
                OrderAndPaging.OrderList.Add(invidualOrder);
            }

            var count = OrderAndPaging.OrderList.Count;

            OrderAndPaging.OrderList = OrderAndPaging.OrderList.OrderByDescending(p => p.OrderHeader.OrderHeaderId)
                .Skip((productPage - 1) * PageSize).Take(PageSize)
                .ToList();

            OrderAndPaging.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                UrlParam = "/Customer/Order/OrderHistory?productPage=:"
            };

            return View(OrderAndPaging);
        }

        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderHeaderAndOrderDetailsViewModel orderDetailsVM = new OrderHeaderAndOrderDetailsViewModel()
            { 
                OrderHeader = await _db.OrderHeaders.Include(u=>u.ApplicationUser).Where(u=>u.OrderHeaderId == Id).FirstOrDefaultAsync(),
                OrderDetailsList = await _db.OrderDetails.Where(u=>u.OrderId == Id).ToListAsync()
            };

            return PartialView("_IndividualOrderDetailsPartial", orderDetailsVM);
        }

        public async Task<IActionResult> GetOrderStatus(int id)
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == id);

            return PartialView("_OrderStatusPartial", orderHeader);
        }
    }
}