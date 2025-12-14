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
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddYears(-1));
            Response.AppendHeader("Pragma", "no-cache");

            var logoutCookie = Request.Cookies["LoggedOut"];
            if (logoutCookie != null && logoutCookie.Value == "true")
            {
                Session.Clear();
                Session.Abandon();

                Response.Cookies["LoggedOut"].Expires = DateTime.Now.AddYears(-1);
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddYears(-1);
                Response.Cookies["UserInfo"].Expires = DateTime.Now.AddYears(-1);

                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            if (Session["UserId"] == null || Session["SessionToken"] == null)
            {
                Session.Clear();
                Session.Abandon();
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

        public ActionResult Index()
        {
            return View();
        }
    }
}