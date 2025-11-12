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
                System.Diagnostics.Debug.WriteLine($"[Session Restore] Path: {context.Request.Path}, Has Session: {context.Session != null}, UserId in Session: {context.Session["UserId"]}");

                if (context.Session["UserId"] == null)
                {
                    var authCookie = context.Request.Cookies["AuthToken"];
                    var userCookie = context.Request.Cookies["UserInfo"];

                    System.Diagnostics.Debug.WriteLine($"[Session Restore] AuthCookie: {authCookie?.Value}, UserCookie: {userCookie?.Value}");

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

                                    System.Diagnostics.Debug.WriteLine($"[Session Restore] SUCCESS - Restored session for UserId: {userId}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"[Session Restore] FAILED - User not found in DB for UserId: {userId}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Session Restore] Session already has UserId: {context.Session["UserId"]}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Session Restore] SKIPPED - Context or Session is null. Path: {context?.Request.Path}");
            }
        }
    }
}
