using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ForYou.Data;
using ForYou.Models;
using ForYou.Models.ViewModel;
using ForYou.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        private int PageSize = 2;

        public OrderController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
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
        public async Task<IActionResult> OrderHistory(int productPage = 1)
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
                OrderHeader = await _db.OrderHeaders.Include(u => u.ApplicationUser).Where(u => u.OrderHeaderId == Id).FirstOrDefaultAsync(),
                OrderDetailsList = await _db.OrderDetails.Where(u => u.OrderId == Id).ToListAsync()
            };

            return PartialView("_IndividualOrderDetailsPartial", orderDetailsVM);
        }

        public async Task<IActionResult> GetOrderStatus(int id)
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == id);

            return PartialView("_OrderStatusPartial", orderHeader);
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitcheUser)]
        public async Task<IActionResult> ManageOrder()
        {
            List<OrderHeaderAndOrderDetailsViewModel> orderDetailsVM = new List<OrderHeaderAndOrderDetailsViewModel>();

            List<OrderHeader> OrderHeaderList = await _db.OrderHeaders
                .Where(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess)
                .OrderByDescending(o => o.PickUpTime).ToListAsync();

            foreach (var orderHeader in OrderHeaderList)
            {
                OrderHeaderAndOrderDetailsViewModel invidualOrder = new OrderHeaderAndOrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetailsList = await _db.OrderDetails.Where(u => u.OrderId == orderHeader.OrderHeaderId).ToListAsync()
                };
                orderDetailsVM.Add(invidualOrder);
            }

            return View(orderDetailsVM.OrderBy(o => o.OrderHeader.PickUpTime));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitcheUser)]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitcheUser)]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusReady;
            await _db.SaveChangesAsync();

            //Email notiication for order Pickup
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.ApplicationUserId).FirstOrDefault().Email,
                   "Your order is ready for pickup completed OrderId: " + orderHeader.OrderHeaderId.ToString(),
                   "Your order is ready for pickup. <br /> <strong class =\"text-info\">THANK YOU!!</strong>");


            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.KitcheUser)]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusCencelled;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.ApplicationUserId).FirstOrDefault().Email,
                   "Your order is ready for pickup OrderId: " + orderHeader.OrderHeaderId.ToString(),
                   "Your order is cencelled anyway. <br /> <strong class =\"text-info\">THANK YOU!!</strong>");

            return RedirectToAction(nameof(ManageOrder));
        }


        [Authorize]
        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchName = null, string searchPhone = null, string searchEmail = null)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            OrderHeaderAndOrderDetailsListViewModel OrderAndPaging = new OrderHeaderAndOrderDetailsListViewModel()
            {
                OrderList = new List<OrderHeaderAndOrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchName=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }


            List<OrderHeader> OrderHeaderList = new List<OrderHeader>();
            ApplicationUser user = new ApplicationUser();

            if (searchName != null || searchPhone != null || searchEmail != null)
            {
                if (searchName != null)
                {
                    OrderHeaderList = _db.OrderHeaders
                       .Include(o => o.ApplicationUser)
                        .AsEnumerable()
                        .Where(u => u.PickUpName.ToLower().Contains(searchName.ToString().ToLower()))
                        .OrderByDescending(o => o.PickUpDate).ToList();

                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUsers.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                        OrderHeaderList = _db.OrderHeaders
                            .Include(o => o.ApplicationUser)
                            .AsEnumerable()
                            .Where(o => o.ApplicationUserId == user.Id)
                            .OrderByDescending(o => o.PickUpDate).ToList();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderHeaderList = _db.OrderHeaders
                                .AsEnumerable()
                                .Where(o => o.PhoneNumber == searchPhone)
                                .OrderByDescending(o => o.PickUpDate).ToList();
                        }
                    }
                }
            }
            else
            {
                OrderHeaderList = await _db.OrderHeaders.Include(u => u.ApplicationUser).Where(u => u.Status == SD.StatusReady).ToListAsync();
            }

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
                UrlParam = param.ToString()
            };

            return View(OrderAndPaging);
        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.FrontDestUser)]
        [HttpPost, ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            OrderHeader orderHeader = await _db.OrderHeaders.FindAsync(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.ApplicationUserId).FirstOrDefault().Email,
                   "Your order is completed OrderId: " + orderHeader.OrderHeaderId.ToString(),
                   "Your order Completed succesfully. <br /> <strong class =\"text-info\">THANK YOU!!</strong>");

            return RedirectToAction(nameof(OrderPickup));
        }
    }
}