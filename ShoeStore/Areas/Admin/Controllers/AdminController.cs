using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.PageTitle = "Dashboard Overview";
            return View();
        }

        public ActionResult Products()
        {
            ViewBag.Title = "Products";
            ViewBag.PageTitle = "Products Management";
            return View();
        }

        public ActionResult Orders()
        {
            ViewBag.Title = "Orders";
            ViewBag.PageTitle = "Orders Management";
            return View();
        }

        public ActionResult Customers()
        {
            ViewBag.Title = "Customers";
            ViewBag.PageTitle = "Customers Management";
            return View();
        }

        public ActionResult Inventory()
        {
            ViewBag.Title = "Inventory";
            ViewBag.PageTitle = "Inventory Management";
            return View();
        }
    }
}