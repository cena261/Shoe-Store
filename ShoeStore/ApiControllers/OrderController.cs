using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ShoeStore.Models;

namespace ShoeStore.ApiControllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        [HttpGet]
        [Route("history")]
        public IHttpActionResult GetOrderHistory()
        {
            try
            {
                int? userId = null;

                if (System.Web.HttpContext.Current != null &&
                    System.Web.HttpContext.Current.Session != null &&
                    System.Web.HttpContext.Current.Session["UserId"] != null)
                {
                    userId = (int)System.Web.HttpContext.Current.Session["UserId"];
                }

                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                using (var db = new ShoeStoreDBContext())
                {
                    var orders = db.Orders
                        .Where(o => o.UserID == userId.Value)
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => new
                        {
                            o.OrderID,
                            o.OrderDate,
                            o.TotalAmount,
                            o.OrderStatus,
                            ItemCount = o.OrderItems.Count()
                        })
                        .ToList();

                    return Ok(new
                    {
                        Status = true,
                        Orders = orders
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order History] ERROR: {ex.Message}");
                return Ok(new { Status = false, Message = "Failed to load order history" });
            }
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                int? userId = null;

                if (System.Web.HttpContext.Current != null &&
                    System.Web.HttpContext.Current.Session != null &&
                    System.Web.HttpContext.Current.Session["UserId"] != null)
                {
                    userId = (int)System.Web.HttpContext.Current.Session["UserId"];
                }

                if (!userId.HasValue && request.RequireAuth)
                {
                    return Unauthorized();
                }

                using (var db = new ShoeStoreDBContext())
                {
                    var order = new Orders
                    {
                        UserID = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = 0,
                        OrderStatus = "Pending",
                        ShippingName = request.ShippingName,
                        ShippingPhone = request.ShippingPhone,
                        ShippingTenDuong = request.ShippingTenDuong,
                        ShippingXaQuan = request.ShippingXaQuan,
                        ShippingTinhThanh = request.ShippingTinhThanh
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    decimal totalAmount = 0;

                    foreach (var item in request.Items)
                    {
                        var subtotal = item.Quantity * item.UnitPrice;

                        var orderItem = new OrderItems
                        {
                            OrderID = order.OrderID,
                            VariantID = item.VariantID,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            Subtotal = subtotal
                        };

                        db.OrderItems.Add(orderItem);
                        totalAmount += subtotal;
                    }

                    db.SaveChanges();

                    order.TotalAmount = totalAmount;
                    db.SaveChanges();

                    if (userId.HasValue)
                    {
                        var cartItems = db.Cart.Where(c => c.UserID == userId.Value).ToList();
                        db.Cart.RemoveRange(cartItems);
                        db.SaveChanges();
                    }

                    return Ok(new
                    {
                        Status = true,
                        Message = "Order created successfully",
                        OrderID = order.OrderID
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Order Create] ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Order Create] Stack trace: {ex.StackTrace}");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Order Create] Inner Exception: {innerEx.Message}");
                    innerEx = innerEx.InnerException;
                }

                return Ok(new { Status = false, Message = ex.Message });
            }
        }
    }

    public class CreateOrderRequest
    {
        public bool RequireAuth { get; set; }
        public string ShippingName { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingTenDuong { get; set; }
        public string ShippingXaQuan { get; set; }
        public string ShippingTinhThanh { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        public int VariantID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
