using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoeStore.Controllers
{
    public class OrderController : BaseUserController
    {
        // GET: Order
        public ActionResult Checkout()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
    }
}