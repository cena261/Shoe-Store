using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ShoeStore.Models;
using ShoeStore.Repositories;
using ShoeStore.Security;

namespace ShoeStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _users = new UserRepository();

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var user = _users.GetByEmail(model.Email);
            if (user.userId == 0 || !PasswordHasher.Verify(model.Password, user.passwordHash))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var existing = _users.GetByEmail(model.Email);
                if (existing.userId != 0)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(model);
                }
                var userId = _users.CreateUser(model.Email, model.FullName, model.Phone, model.Password, "web");
                // Gán role CUSTOMER nếu tồn tại
                TryAssignCustomerRole(userId);

                FormsAuthentication.SetAuthCookie(model.Email, false);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void TryAssignCustomerRole(long userId)
        {
            try
            {
                using (var conn = Data.OracleDb.GetOpenConnection())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DECLARE
                        v_role_id NUMBER;
                    BEGIN
                        SELECT ROLE_ID INTO v_role_id FROM ROLE WHERE CODE = 'CUSTOMER';
                        INSERT INTO USER_ROLE(USER_ID, ROLE_ID) VALUES(:p_user, v_role_id);
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                        WHEN OTHERS THEN NULL;
                    END;";
                    cmd.Parameters.Add(Data.OracleDb.Param(":p_user", userId, Oracle.ManagedDataAccess.Client.OracleDbType.Int64));
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
}
