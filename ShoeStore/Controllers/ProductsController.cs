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
    public class ProductsController : Controller
    {
        private ShoeStoreDBContext db = new ShoeStoreDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
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