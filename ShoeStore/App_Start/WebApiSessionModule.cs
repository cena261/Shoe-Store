using System;
using System.Web;
using System.Web.SessionState;

namespace ShoeStore.App_Start
{
    public class WebApiSessionModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostAuthorizeRequest += (sender, e) =>
            {
                if (HttpContext.Current.Request.Path.StartsWith("/api/"))
                {
                    HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
                }
            };
        }

        public void Dispose()
        {
        }
    }
}
