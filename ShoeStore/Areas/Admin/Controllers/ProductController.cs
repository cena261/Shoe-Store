using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeStore.Models;
using ShoeStore.Areas.Admin.Models.DTOs;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class ProductController : Controller
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

        public ActionResult Index(string search = "", int page = 1, int pageSize = 20)
        {
            ViewBag.Title = "Products";
            ViewBag.PageTitle = "Products Management";
            ViewBag.Search = search;
            ViewBag.Page = page;

            var query = db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || p.Slug.Contains(search));
            }

            var totalProducts = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemDTO
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Slug = p.Slug,
                    Price = p.Price,
                    PromotionPrice = p.PromotionPrice,
                    CategoryName = p.Category.CategoryName,
                    TotalVariants = p.ProductVariants.Count,
                    TotalStock = p.ProductVariants.Sum(v => v.StockQty),
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    FirstImageURL = p.ProductImages.Where(i => i.DisplayOrder == 1).Select(i => i.ImageURL).FirstOrDefault()
                })
                .ToList();

            ViewBag.Products = products;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;

            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Create Product";
            ViewBag.PageTitle = "Create New Product";
            ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductFormDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
                    return View(model);
                }

                var existingSlug = db.Products.Any(p => p.Slug == model.Slug);
                if (existingSlug)
                {
                    ModelState.AddModelError("Slug", "Slug already exists");
                    ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
                    return View(model);
                }

                var product = new Products
                {
                    ProductName = model.ProductName,
                    Slug = model.Slug,
                    Description = model.Description,
                    Price = model.Price,
                    PromotionPrice = model.PromotionPrice,
                    CategoryID = model.CategoryID,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.Now
                };

                db.Products.Add(product);
                db.SaveChanges();

                if (model.Variants != null && model.Variants.Any())
                {
                    foreach (var variantDTO in model.Variants)
                    {
                        var existingSKU = db.ProductVariants.Any(v => v.SKU == variantDTO.SKU);
                        if (existingSKU)
                        {
                            continue;
                        }

                        var variant = new ProductVariants
                        {
                            ProductID = product.ProductID,
                            SizeValue = variantDTO.SizeValue,
                            ColorName = variantDTO.ColorName,
                            StyleColor = variantDTO.StyleColor,
                            SKU = variantDTO.SKU,
                            StockQty = variantDTO.StockQty,
                            PriceOverride = variantDTO.PriceOverride,
                            IsActive = variantDTO.IsActive
                        };
                        db.ProductVariants.Add(variant);
                    }
                    db.SaveChanges();
                }

                if (model.Images != null && model.Images.Any())
                {
                    foreach (var imageDTO in model.Images)
                    {
                        var image = new ProductImages
                        {
                            ProductID = product.ProductID,
                            StyleColor = imageDTO.StyleColor,
                            ImageURL = imageDTO.ImageURL,
                            DisplayOrder = imageDTO.DisplayOrder
                        };
                        db.ProductImages.Add(image);
                    }
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating product: " + ex.Message);
                ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            var product = db.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var colorGroups = product.ProductVariants
                .GroupBy(v => new { v.ColorName, v.StyleColor })
                .Select(g => new ProductColorGroupDTO
                {
                    ColorName = g.Key.ColorName,
                    StyleColor = g.Key.StyleColor,
                    ImageURL = product.ProductImages
                        .FirstOrDefault(i => i.StyleColor == g.Key.StyleColor && i.DisplayOrder == 1)?.ImageURL,
                    Variants = g.Select(v => new ProductVariantDTO
                    {
                        VariantID = v.VariantID,
                        SizeValue = v.SizeValue,
                        ColorName = v.ColorName,
                        StyleColor = v.StyleColor,
                        SKU = v.SKU,
                        StockQty = v.StockQty,
                        PriceOverride = v.PriceOverride,
                        IsActive = v.IsActive
                    }).ToList()
                }).ToList();

            var model = new ProductFormDTO
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                PromotionPrice = product.PromotionPrice,
                CategoryID = product.CategoryID,
                IsActive = product.IsActive,
                ColorGroups = colorGroups
            };

            ViewBag.Title = "Edit Product";
            ViewBag.PageTitle = "Edit Product: " + product.ProductName;
            ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductFormDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
                    return View(model);
                }

                var product = db.Products
                    .Include(p => p.ProductVariants)
                    .Include(p => p.ProductImages)
                    .FirstOrDefault(p => p.ProductID == model.ProductID);

                if (product == null)
                {
                    return HttpNotFound();
                }

                var existingSlug = db.Products.Any(p => p.Slug == model.Slug && p.ProductID != model.ProductID);
                if (existingSlug)
                {
                    ModelState.AddModelError("Slug", "Slug already exists");
                    ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
                    return View(model);
                }

                product.ProductName = model.ProductName;
                product.Slug = model.Slug;
                product.Description = model.Description;
                product.Price = model.Price;
                product.PromotionPrice = model.PromotionPrice;
                product.CategoryID = model.CategoryID;
                product.IsActive = model.IsActive;

                var existingVariantIds = model.Variants.Where(v => v.VariantID.HasValue).Select(v => v.VariantID.Value).ToList();
                var variantsToDelete = product.ProductVariants.Where(v => !existingVariantIds.Contains(v.VariantID)).ToList();
                foreach (var variant in variantsToDelete)
                {
                    db.ProductVariants.Remove(variant);
                }

                foreach (var variantDTO in model.Variants)
                {
                    if (variantDTO.VariantID.HasValue)
                    {
                        var existingVariant = product.ProductVariants.FirstOrDefault(v => v.VariantID == variantDTO.VariantID);
                        if (existingVariant != null)
                        {
                            existingVariant.SizeValue = variantDTO.SizeValue;
                            existingVariant.ColorName = variantDTO.ColorName;
                            existingVariant.StyleColor = variantDTO.StyleColor;
                            existingVariant.SKU = variantDTO.SKU;
                            existingVariant.StockQty = variantDTO.StockQty;
                            existingVariant.PriceOverride = variantDTO.PriceOverride;
                            existingVariant.IsActive = variantDTO.IsActive;
                        }
                    }
                    else
                    {
                        var existingSKU = db.ProductVariants.Any(v => v.SKU == variantDTO.SKU);
                        if (!existingSKU)
                        {
                            var newVariant = new ProductVariants
                            {
                                ProductID = product.ProductID,
                                SizeValue = variantDTO.SizeValue,
                                ColorName = variantDTO.ColorName,
                                StyleColor = variantDTO.StyleColor,
                                SKU = variantDTO.SKU,
                                StockQty = variantDTO.StockQty,
                                PriceOverride = variantDTO.PriceOverride,
                                IsActive = variantDTO.IsActive
                            };
                            db.ProductVariants.Add(newVariant);
                        }
                    }
                }

                var existingImageIds = model.Images.Where(i => i.ImageID.HasValue).Select(i => i.ImageID.Value).ToList();
                var imagesToDelete = product.ProductImages.Where(i => !existingImageIds.Contains(i.ImageID)).ToList();
                foreach (var image in imagesToDelete)
                {
                    db.ProductImages.Remove(image);
                }

                foreach (var imageDTO in model.Images)
                {
                    if (imageDTO.ImageID.HasValue)
                    {
                        var existingImage = product.ProductImages.FirstOrDefault(i => i.ImageID == imageDTO.ImageID);
                        if (existingImage != null)
                        {
                            existingImage.StyleColor = imageDTO.StyleColor;
                            existingImage.ImageURL = imageDTO.ImageURL;
                            existingImage.DisplayOrder = imageDTO.DisplayOrder;
                        }
                    }
                    else
                    {
                        var newImage = new ProductImages
                        {
                            ProductID = product.ProductID,
                            StyleColor = imageDTO.StyleColor,
                            ImageURL = imageDTO.ImageURL,
                            DisplayOrder = imageDTO.DisplayOrder
                        };
                        db.ProductImages.Add(newImage);
                    }
                }

                db.SaveChanges();

                TempData["SuccessMessage"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating product: " + ex.Message);
                ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var product = db.Products
                    .Include(p => p.ProductVariants)
                    .Include(p => p.ProductImages)
                    .FirstOrDefault(p => p.ProductID == id);

                if (product == null)
                {
                    return Json(new ProductResponseDTO { Status = false, Message = "Product not found" });
                }

                db.Products.Remove(product);
                db.SaveChanges();

                return Json(new ProductResponseDTO { Status = true, Message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new ProductResponseDTO { Status = false, Message = "Error deleting product: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteVariant(int id)
        {
            try
            {
                var variant = db.ProductVariants.Find(id);
                if (variant == null)
                {
                    return Json(new ProductResponseDTO { Status = false, Message = "Variant not found" });
                }

                db.ProductVariants.Remove(variant);
                db.SaveChanges();

                return Json(new ProductResponseDTO { Status = true, Message = "Variant deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new ProductResponseDTO { Status = false, Message = "Error deleting variant: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteImage(int id)
        {
            try
            {
                var image = db.ProductImages.Find(id);
                if (image == null)
                {
                    return Json(new ProductResponseDTO { Status = false, Message = "Image not found" });
                }

                db.ProductImages.Remove(image);
                db.SaveChanges();

                return Json(new ProductResponseDTO { Status = true, Message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new ProductResponseDTO { Status = false, Message = "Error deleting image: " + ex.Message });
            }
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
