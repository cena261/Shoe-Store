using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShoeStore.Models;
using ShoeStore.Areas.Admin.Models.DTOs;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private ShoeStoreDBContext db = new ShoeStoreDBContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            var userRoles = Session["UserRoles"] as string[];
            if (userRoles == null || !userRoles.Contains("Admin"))
            {
                filterContext.Result = new RedirectResult("/Home/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(string status = "", string search = "", int page = 1, int pageSize = 20)
        {
            ViewBag.Title = "Orders";
            ViewBag.PageTitle = "Orders Management";
            ViewBag.Status = status;
            ViewBag.Search = search;
            ViewBag.Page = page;

            var query = db.Orders
                .Include(o => o.Users)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.ShippingName.Contains(search) ||
                    o.ShippingPhone.Contains(search) ||
                    o.Users.Email.Contains(search) ||
                    o.OrderID.ToString().Contains(search)
                );
            }

            var totalOrders = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            var orders = query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderListItemDTO
                {
                    OrderID = o.OrderID,
                    UserID = o.UserID,
                    CustomerName = o.ShippingName,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    ShippingTinhThanh = o.ShippingTinhThanh,
                    UserEmail = o.Users != null ? o.Users.Email : "Guest"
                })
                .ToList();

            ViewBag.Orders = orders;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalOrders = totalOrders;

            ViewBag.PendingCount = db.Orders.Count(o => o.OrderStatus == "Pending");
            ViewBag.ConfirmedCount = db.Orders.Count(o => o.OrderStatus == "Confirmed");
            ViewBag.OnDeliveryCount = db.Orders.Count(o => o.OrderStatus == "On Delivery");
            ViewBag.SuccessCount = db.Orders.Count(o => o.OrderStatus == "Success");
            ViewBag.CancelledCount = db.Orders.Count(o => o.OrderStatus == "Cancelled");
            ViewBag.FailedCount = db.Orders.Count(o => o.OrderStatus == "Failed");

            return View();
        }

        public JsonResult GetOrderDetails(int id)
        {
            var order = db.Orders
                .Include(o => o.Users)
                .Include(o => o.OrderItems.Select(oi => oi.ProductVariant.Product))
                .Include(o => o.OrderItems.Select(oi => oi.ProductVariant.Product.ProductImages))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return Json(new { Success = false, Message = "Order not found" }, JsonRequestBehavior.AllowGet);
            }

            var model = new OrderDetailDTO
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                UserEmail = order.Users != null ? order.Users.Email : "Guest",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingTenDuong = order.ShippingTenDuong,
                ShippingXaQuan = order.ShippingXaQuan,
                ShippingTinhThanh = order.ShippingTinhThanh,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemID = oi.OrderItemID,
                    VariantID = oi.VariantID,
                    ProductName = oi.ProductVariant.Product.ProductName,
                    ColorName = oi.ProductVariant.ColorName,
                    Size = oi.ProductVariant.SizeValue,
                    SKU = oi.ProductVariant.SKU,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal,
                    ImageUrl = oi.ProductVariant.Product.ProductImages
                        .Where(pi => pi.StyleColor == oi.ProductVariant.StyleColor)
                        .OrderBy(pi => pi.ImageID)
                        .Select(pi => pi.ImageURL)
                        .FirstOrDefault()
                }).ToList()
            };

            return Json(new { Success = true, Data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStatus(int orderId, string newStatus)
        {
            try
            {
                var order = db.Orders
                    .Include(o => o.OrderItems.Select(oi => oi.ProductVariant))
                    .FirstOrDefault(o => o.OrderID == orderId);

                if (order == null)
                {
                    return Json(new OrderResponseDTO
                    {
                        Success = false,
                        Message = "Order not found"
                    });
                }

                string oldStatus = order.OrderStatus;

                if (newStatus == "Confirmed" && oldStatus == "Pending")
                {
                    foreach (var item in order.OrderItems)
                    {
                        if (item.ProductVariant.StockQty < item.Quantity)
                        {
                            return Json(new OrderResponseDTO
                            {
                                Success = false,
                                Message = $"Insufficient stock for {item.ProductVariant.Product.ProductName} - {item.ProductVariant.ColorName} - Size {item.ProductVariant.SizeValue}"
                            });
                        }
                    }

                    foreach (var item in order.OrderItems)
                    {
                        item.ProductVariant.StockQty -= item.Quantity;
                    }
                }
                else if ((newStatus == "Cancelled" || newStatus == "Failed") && (oldStatus == "Confirmed" || oldStatus == "On Delivery"))
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.ProductVariant.StockQty += item.Quantity;
                    }
                }

                order.OrderStatus = newStatus;
                db.SaveChanges();

                return Json(new OrderResponseDTO
                {
                    Success = true,
                    Message = $"Order status updated to {newStatus}",
                    Data = new { OrderID = orderId, NewStatus = newStatus }
                });
            }
            catch (Exception ex)
            {
                return Json(new OrderResponseDTO
                {
                    Success = false,
                    Message = "Error updating order status: " + ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult CancelOrder(int orderId)
        {
            return UpdateStatus(orderId, "Cancelled");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
