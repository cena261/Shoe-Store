using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShoeStore.Models;
using ShoeStore.Models.DTOs;

namespace ShoeStore.ApiControllers
{
    public class ProductsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(
            string search = "",
            string gender = "",
            string sizes = "",
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "newest",
            int page = 1,
            int pageSize = 15)
        {
            using (var db = new ShoeStoreDBContext())
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                try
                {
                    var query = db.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductVariants)
                        .Where(p => p.IsActive)
                        .AsNoTracking();

                    if (!string.IsNullOrEmpty(search))
                    {
                        query = query.Where(p =>
                            p.ProductName.Contains(search) ||
                            p.ProductImages.Any(img => img.StyleColor.Contains(search)));
                    }

                    if (!string.IsNullOrEmpty(gender))
                    {
                        var genderList = gender.Split(',').Select(g => g.Trim()).ToList();
                        query = query.Where(p => genderList.Contains(p.Category.CategoryName));
                    }

                    if (!string.IsNullOrEmpty(sizes))
                    {
                        var sizeList = sizes.Split(',').Select(s => s.Trim()).ToList();
                        query = query.Where(p => p.ProductVariants.Any(v => sizeList.Contains(v.SizeValue)));
                    }

                    if (minPrice.HasValue)
                    {
                        query = query.Where(p =>
                            (p.PromotionPrice.HasValue ? p.PromotionPrice.Value : p.Price) >= minPrice.Value);
                    }

                    if (maxPrice.HasValue)
                    {
                        query = query.Where(p =>
                            (p.PromotionPrice.HasValue ? p.PromotionPrice.Value : p.Price) <= maxPrice.Value);
                    }

                    var totalProducts = query.Count();
                    var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                    switch (sortBy?.ToLower())
                    {
                        case "price-low":
                            query = query.OrderBy(p => p.PromotionPrice ?? p.Price);
                            break;
                        case "price-high":
                            query = query.OrderByDescending(p => p.PromotionPrice ?? p.Price);
                            break;
                        case "name":
                            query = query.OrderBy(p => p.ProductName);
                            break;
                        default:
                            query = query.OrderByDescending(p => p.CreatedAt);
                            break;
                    }

                    var products = query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    var productDTOs = products.Select(p =>
                    {
                        var colorVariants = p.ProductImages
                            .Where(img => img.DisplayOrder == 1)
                            .GroupBy(img => img.StyleColor)
                            .Select(g => new ColorVariantDTO
                            {
                                StyleColor = g.Key ?? "",
                                ColorName = p.ProductVariants
                                    .FirstOrDefault(v => v.StyleColor == g.Key)?.ColorName ?? "",
                                MainImageURL = g.FirstOrDefault()?.ImageURL ?? "/Content/images/placeholder.png"
                            })
                            .ToList();

                        if (colorVariants.Count == 0)
                        {
                            colorVariants.Add(new ColorVariantDTO
                            {
                                StyleColor = "",
                                ColorName = "",
                                MainImageURL = "/Content/images/placeholder.png"
                            });
                        }

                        return new ProductListItemDTO
                        {
                            ProductID = p.ProductID,
                            ProductName = p.ProductName,
                            Slug = p.Slug,
                            Price = p.Price,
                            PromotionPrice = p.PromotionPrice,
                            CategoryName = p.Category.CategoryName,
                            Gender = p.Category.CategoryName,
                            ColorVariants = colorVariants
                        };
                    }).ToList();

                    return Ok(new ProductListResponse
                    {
                        Status = true,
                        Products = productDTOs,
                        TotalProducts = totalProducts,
                        CurrentPage = page,
                        TotalPages = totalPages,
                        PageSize = pageSize
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new ProductListResponse
                    {
                        Status = false,
                        Products = new List<ProductListItemDTO>(),
                        TotalProducts = 0,
                        CurrentPage = 1,
                        TotalPages = 0,
                        PageSize = pageSize
                    });
                }
            }
        }
    }
}
