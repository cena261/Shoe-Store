using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeStore.Models;
using ShoeStore.Models.DTOs;
using System.Data.Entity;
using BCrypt.Net;

namespace ShoeStore.Controllers
{
    public class AccountController : Controller
    {
        private ShoeStoreDBContext db = new ShoeStoreDBContext();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register API
        [HttpPost]
        [ActionName("Register")]
        public JsonResult RegisterPost(RegisterRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new RegisterResponse
                    {
                        Status = false,
                        Message = string.Join(", ", errors)
                    });
                }

                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return Json(new RegisterResponse
                    {
                        Status = false,
                        Message = "Email already exists"
                    });
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var newUser = new Users
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    IsActive = true,
                    createdAt = DateTime.Now
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                var userRole = db.Roles.FirstOrDefault(r => r.RoleName == "User");
                if (userRole == null)
                {
                    db.Users.Remove(newUser);
                    db.SaveChanges();

                    return Json(new RegisterResponse
                    {
                        Status = false,
                        Message = "User role not found in database. Please contact administrator."
                    });
                }

                var userRoleAssignment = new UserRoles
                {
                    UserID = newUser.userId,
                    RoleID = userRole.RoleID,
                    AssignedAt = DateTime.Now
                };

                db.UserRoles.Add(userRoleAssignment);
                db.SaveChanges();

                newUser.lastLogin = DateTime.Now;
                db.SaveChanges();

                Session["UserId"] = newUser.userId;
                Session["UserEmail"] = newUser.Email;
                Session["UserName"] = newUser.FullName;
                Session["UserRoles"] = new string[] { "User" };

                var authCookie = new HttpCookie("AuthToken")
                {
                    Value = "authenticated",
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection
                };
                Response.Cookies.Add(authCookie);

                var userCookie = new HttpCookie("UserInfo")
                {
                    Value = newUser.userId.ToString(),
                    Expires = DateTime.Now.AddDays(7)
                };
                Response.Cookies.Add(userCookie);

                return Json(new RegisterResponse
                {
                    Status = true,
                    Message = "Registration successful",
                    User = new UserData
                    {
                        UserId = newUser.userId,
                        Email = newUser.Email,
                        FullName = newUser.FullName,
                        Phone = newUser.Phone,
                        CreatedAt = newUser.createdAt,
                        Roles = new string[] { "User" }
                    }
                });
            }
            catch (Exception ex)
            {
                var innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }

                return Json(new RegisterResponse
                {
                    Status = false,
                    Message = "An error occurred during registration: " + innerException.Message
                });
            }
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login API
        [HttpPost]
        [ActionName("Login")]
        public JsonResult LoginPost(LoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new LoginResponse
                    {
                        Status = false,
                        Message = string.Join(", ", errors)
                    });
                }

                var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return Json(new LoginResponse
                    {
                        Status = false,
                        Message = "Invalid email or password"
                    });
                }

                if (!user.IsActive)
                {
                    return Json(new LoginResponse
                    {
                        Status = false,
                        Message = "Your account has been deactivated. Please contact administrator."
                    });
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    return Json(new LoginResponse
                    {
                        Status = false,
                        Message = "Invalid email or password"
                    });
                }

                var userRoles = db.UserRoles
                    .Where(ur => ur.UserID == user.userId)
                    .Select(ur => ur.Role.RoleName)
                    .ToArray();

                user.lastLogin = DateTime.Now;
                db.SaveChanges();

                Session["UserId"] = user.userId;
                Session["UserEmail"] = user.Email;
                Session["UserName"] = user.FullName;
                Session["UserRoles"] = userRoles;

                var authCookie = new HttpCookie("AuthToken")
                {
                    Value = "authenticated",
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection
                };
                Response.Cookies.Add(authCookie);

                var userCookie = new HttpCookie("UserInfo")
                {
                    Value = user.userId.ToString(),
                    Expires = DateTime.Now.AddDays(7)
                };
                Response.Cookies.Add(userCookie);

                return Json(new LoginResponse
                {
                    Status = true,
                    Message = "Login successful",
                    User = new UserData
                    {
                        UserId = user.userId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Phone = user.Phone,
                        CreatedAt = user.createdAt,
                        Roles = userRoles
                    }
                });
            }
            catch (Exception ex)
            {
                var innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }

                return Json(new LoginResponse
                {
                    Status = false,
                    Message = "An error occurred during login: " + innerException.Message
                });
            }
        }

        // GET: Account/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null)
            {
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
                return RedirectToAction("Login");
            }

            int userId = (int)Session["UserId"];
            var user = db.Users.FirstOrDefault(u => u.userId == userId);

            if (user == null || !user.IsActive)
            {
                Session.Clear();
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetUserInfo()
        {
            if (Session["UserId"] == null)
            {
                return Json(new { Status = false, Message = "Not authenticated" }, JsonRequestBehavior.AllowGet);
            }

            int userId = (int)Session["UserId"];
            var user = db.Users.FirstOrDefault(u => u.userId == userId);

            if (user == null)
            {
                return Json(new { Status = false, Message = "User not found" }, JsonRequestBehavior.AllowGet);
            }

            var userRoles = db.UserRoles
                .Where(ur => ur.UserID == userId)
                .Select(ur => ur.Role.RoleName)
                .ToArray();

            return Json(new
            {
                Status = true,
                UserId = user.userId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                CreatedAt = user.createdAt,
                Roles = userRoles
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordRequest model)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { Status = false, Message = "Not authenticated" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new { Status = false, Message = string.Join(", ", errors) });
                }

                int userId = (int)Session["UserId"];
                var user = db.Users.FirstOrDefault(u => u.userId == userId);

                if (user == null)
                {
                    return Json(new { Status = false, Message = "User not found" });
                }

                bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash);
                if (!isCurrentPasswordValid)
                {
                    return Json(new { Status = false, Message = "Current password is incorrect" });
                }

                string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.PasswordHash = newHashedPassword;
                db.SaveChanges();

                return Json(new { Status = true, Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                var innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }

                return Json(new { Status = false, Message = "An error occurred: " + innerException.Message });
            }
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