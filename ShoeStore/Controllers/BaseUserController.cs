using System.Linq;
using System.Web.Mvc;

namespace ShoeStore.Controllers
{
    public class BaseUserController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] != null)
            {
                var userRoles = Session["UserRoles"] as string[];
                if (userRoles != null && userRoles.Contains("Admin"))
                {
                    filterContext.Result = new RedirectResult("/Admin/Product/Index");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
