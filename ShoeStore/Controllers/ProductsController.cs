using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeStore.Models;
using ShoeStore.Models.DTOs;
using System.Data.Entity;

namespace ShoeStore.Controllers
{
    public class ProductsController : BaseUserController
    {
        private ShoeStoreDBContext db = new ShoeStoreDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string slug, string styleColor = null)
        {
            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Slug == slug && p.IsActive);

            if (product == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrEmpty(styleColor))
            {
                var firstColor = product.ProductVariants
                    .Where(v => v.IsActive)
                    .Select(v => v.StyleColor)
                    .Distinct()
                    .FirstOrDefault();

                if (firstColor != null)
                {
                    return RedirectToAction("Details", new { slug = slug, styleColor = firstColor });
                }
            }

            ViewBag.Slug = slug;
            ViewBag.StyleColor = styleColor;

            return View();
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