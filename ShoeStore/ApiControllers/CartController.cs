using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using ShoeStore.Models;
using ShoeStore.Models.DTOs;
using System.Collections.Generic;

namespace ShoeStore.ApiControllers
{
    [RoutePrefix("api/Cart")]
    public class CartController : ApiController
    {

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetCart()
        {
            try
            {
                if (System.Web.HttpContext.Current == null ||
                    System.Web.HttpContext.Current.Session == null ||
                    System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return Unauthorized();
                }

                int userId = (int)System.Web.HttpContext.Current.Session["UserId"];

                using (var db = new ShoeStoreDBContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;

                    var cartItems = db.Cart
                        .Where(c => c.UserID == userId)
                        .Include(c => c.ProductVariant.Product)
                        .Include(c => c.ProductVariant.Product.ProductImages)
                        .ToList()
                        .Select(c => new CartItemDTO
                        {
                            VariantID = c.VariantID,
                            ProductID = c.ProductVariant.ProductID,
                            ProductName = c.ProductVariant.Product.ProductName,
                            Slug = c.ProductVariant.Product.Slug,
                            StyleColor = c.ProductVariant.StyleColor,
                            ColorName = c.ProductVariant.ColorName,
                            Size = c.ProductVariant.SizeValue,
                            Price = c.ProductVariant.Product.Price,
                            PromotionPrice = c.ProductVariant.Product.PromotionPrice,
                            Quantity = c.Quantity,
                            StockQty = c.ProductVariant.StockQty,
                            ImageURL = c.ProductVariant.Product.ProductImages
                                .Where(img => img.StyleColor == c.ProductVariant.StyleColor && img.DisplayOrder == 1)
                                .Select(img => img.ImageURL)
                                .FirstOrDefault() ?? "/Content/images/placeholder.png"
                        })
                        .ToList();

                    var response = new CartResponseDTO
                    {
                        Items = cartItems,
                        TotalItems = cartItems.Sum(i => i.Quantity),
                        TotalPrice = cartItems.Sum(i => (i.PromotionPrice ?? i.Price) * i.Quantity)
                    };

                    return Ok(new { Status = true, Cart = response });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (System.Web.HttpContext.Current == null ||
                    System.Web.HttpContext.Current.Session == null ||
                    System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return Unauthorized();
                }

                int userId = (int)System.Web.HttpContext.Current.Session["UserId"];

                using (var db = new ShoeStoreDBContext())
                {
                    var variant = db.ProductVariants
                        .FirstOrDefault(v => v.VariantID == request.VariantID && v.IsActive);

                    if (variant == null)
                    {
                        return Ok(new { Status = false, Message = "Product variant not found" });
                    }

                    if (variant.StockQty < request.Quantity)
                    {
                        return Ok(new { Status = false, Message = "Insufficient stock" });
                    }

                    var existingCart = db.Cart
                        .FirstOrDefault(c => c.UserID == userId && c.VariantID == request.VariantID);

                    if (existingCart != null)
                    {
                        existingCart.Quantity += request.Quantity;

                        if (existingCart.Quantity > variant.StockQty)
                        {
                            return Ok(new { Status = false, Message = "Quantity exceeds available stock" });
                        }
                    }
                    else
                    {
                        db.Cart.Add(new Cart
                        {
                            UserID = userId,
                            VariantID = request.VariantID,
                            Quantity = request.Quantity,
                            AddedAt = DateTime.Now
                        });
                    }

                    db.SaveChanges();

                    return Ok(new { Status = true, Message = "Added to cart successfully" });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult UpdateCart([FromBody] UpdateCartRequest request)
        {
            try
            {
                if (System.Web.HttpContext.Current == null ||
                    System.Web.HttpContext.Current.Session == null ||
                    System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return Unauthorized();
                }

                int userId = (int)System.Web.HttpContext.Current.Session["UserId"];

                using (var db = new ShoeStoreDBContext())
                {
                    var cartItem = db.Cart
                        .Include(c => c.ProductVariant)
                        .FirstOrDefault(c => c.UserID == userId && c.VariantID == request.VariantID);

                    if (cartItem == null)
                    {
                        return Ok(new { Status = false, Message = "Cart item not found" });
                    }

                    if (request.Quantity <= 0)
                    {
                        db.Cart.Remove(cartItem);
                    }
                    else
                    {
                        if (request.Quantity > cartItem.ProductVariant.StockQty)
                        {
                            return Ok(new { Status = false, Message = "Quantity exceeds available stock" });
                        }
                        cartItem.Quantity = request.Quantity;
                    }

                    db.SaveChanges();

                    return Ok(new { Status = true, Message = "Cart updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("remove/{variantId}")]
        public IHttpActionResult RemoveFromCart(int variantId)
        {
            try
            {
                if (System.Web.HttpContext.Current == null ||
                    System.Web.HttpContext.Current.Session == null ||
                    System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return Unauthorized();
                }

                int userId = (int)System.Web.HttpContext.Current.Session["UserId"];

                using (var db = new ShoeStoreDBContext())
                {
                    var cartItem = db.Cart
                        .FirstOrDefault(c => c.UserID == userId && c.VariantID == variantId);

                    if (cartItem == null)
                    {
                        return Ok(new { Status = false, Message = "Cart item not found" });
                    }

                    db.Cart.Remove(cartItem);
                    db.SaveChanges();

                    return Ok(new { Status = true, Message = "Item removed from cart" });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("sync")]
        public IHttpActionResult SyncCart([FromBody] List<AddToCartRequest> items)
        {
            try
            {
                if (System.Web.HttpContext.Current == null ||
                    System.Web.HttpContext.Current.Session == null ||
                    System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return Unauthorized();
                }

                int userId = (int)System.Web.HttpContext.Current.Session["UserId"];

                System.Diagnostics.Debug.WriteLine($"[Cart Sync] UserId: {userId}, Items count: {items?.Count ?? 0}");

                using (var db = new ShoeStoreDBContext())
                {
                    foreach (var item in items)
                    {
                        var variant = db.ProductVariants
                            .FirstOrDefault(v => v.VariantID == item.VariantID && v.IsActive);

                        if (variant == null)
                        {
                            continue;
                        }

                        var existingCartItem = db.Cart
                            .FirstOrDefault(c => c.UserID == userId && c.VariantID == item.VariantID);

                        if (existingCartItem != null)
                        {
                            int newQuantity = existingCartItem.Quantity + item.Quantity;

                            if (newQuantity > variant.StockQty)
                            {
                                existingCartItem.Quantity = variant.StockQty;
                            }
                            else
                            {
                                existingCartItem.Quantity = newQuantity;
                            }
                        }
                        else
                        {
                            if (variant.StockQty < item.Quantity)
                            {
                                continue;
                            }

                            var cartItem = new Cart
                            {
                                UserID = userId,
                                VariantID = item.VariantID,
                                Quantity = item.Quantity,
                                AddedAt = DateTime.Now
                            };

                            db.Cart.Add(cartItem);
                        }
                    }

                    db.SaveChanges();

                    return Ok(new { Status = true, Message = "Cart synced successfully" });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Cart Sync] ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[Cart Sync] Stack trace: {ex.StackTrace}");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Cart Sync] Inner Exception: {innerEx.Message}");
                    innerEx = innerEx.InnerException;
                }

                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("clear")]
        public IHttpActionResult ClearCart()
        {
            try
            {
                if (System.Web.HttpContext.Current == null ||
                    System.Web.HttpContext.Current.Session == null ||
                    System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return Unauthorized();
                }

                int userId = (int)System.Web.HttpContext.Current.Session["UserId"];

                using (var db = new ShoeStoreDBContext())
                {
                    var cartItems = db.Cart.Where(c => c.UserID == userId).ToList();
                    db.Cart.RemoveRange(cartItems);
                    db.SaveChanges();

                    return Ok(new { Status = true, Message = "Cart cleared successfully" });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
