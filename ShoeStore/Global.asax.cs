using ShoeStore.App_Start;
using ShoeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace ShoeStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            if (context != null && context.Session != null)
            {
                if (context.Session["UserId"] == null)
                {
                    var authCookie = context.Request.Cookies["AuthToken"];
                    var userCookie = context.Request.Cookies["UserInfo"];

                    var logoutCookie = context.Request.Cookies["LoggedOut"];
                    if (logoutCookie != null && logoutCookie.Value == "true")
                    {
                        return;
                    }

                    if (authCookie != null && authCookie.Value == "authenticated" && userCookie != null)
                    {
                        int userId;
                        if (int.TryParse(userCookie.Value, out userId))
                        {
                            using (var db = new ShoeStoreDBContext())
                            {
                                var user = db.Users.FirstOrDefault(u => u.userId == userId);
                                if (user != null)
                                {
                                    var userRoles = db.UserRoles
                                        .Where(ur => ur.UserID == userId)
                                        .Select(ur => ur.Role.RoleName)
                                        .ToArray();

                                    context.Session["UserId"] = user.userId;
                                    context.Session["UserEmail"] = user.Email;
                                    context.Session["UserName"] = user.FullName;
                                    context.Session["UserRoles"] = userRoles;
                                    context.Session["SessionToken"] = Guid.NewGuid().ToString();
                                    context.Session["LoginTime"] = DateTime.Now;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
