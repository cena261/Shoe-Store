using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoeStore.Controllers
{
    public class HomeController : BaseUserController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}