using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoeStore.Models;
using ShoeStore.Models.DTOs;
using System.Data.Entity;
using BCrypt.Net;
using ShoeStore.Services;

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

                var userRole = db.Roles.FirstOrDefault(r => r.RoleName == "User");
                if (userRole == null)
                {
                    return Json(new RegisterResponse
                    {
                        Status = false,
                        Message = "User role not found in database. Please contact administrator."
                    });
                }

                Users newUser;

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                        newUser = new Users
                        {
                            Email = model.Email,
                            PasswordHash = hashedPassword,
                            FullName = model.FullName,
                            Phone = model.Phone,
                            IsActive = true,
                            createdAt = DateTime.Now,
                            lastLogin = DateTime.Now
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        var userRoleAssignment = new UserRoles
                        {
                            UserID = newUser.userId,
                            RoleID = userRole.RoleID,
                            AssignedAt = DateTime.Now
                        };

                        db.UserRoles.Add(userRoleAssignment);
                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

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
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex}");

                return Json(new RegisterResponse
                {
                    Status = false,
                    Message = "An error occurred during registration. Please try again later."
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
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");

                return Json(new LoginResponse
                {
                    Status = false,
                    Message = "An error occurred during login. Please try again later."
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
                System.Diagnostics.Debug.WriteLine($"Change password error: {ex}");

                return Json(new { Status = false, Message = "An error occurred while changing password. Please try again later." });
            }
        }

        [HttpGet]
        public JsonResult GetUserAddresses()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { Status = false, Message = "Not authenticated" }, JsonRequestBehavior.AllowGet);
                }

                int userId = (int)Session["UserId"];
                var addresses = db.Addresses
                    .Where(a => a.UserID == userId)
                    .OrderByDescending(a => a.IsDefault)
                    .Select(a => new AddressData
                    {
                        AddressID = a.AddressID,
                        FullName = a.FullName,
                        Phone = a.Phone,
                        TenDuong = a.TenDuong,
                        XaQuan = a.XaQuan,
                        TinhThanh = a.TinhThanh,
                        IsDefault = a.IsDefault
                    })
                    .ToList();

                return Json(new { Status = true, Addresses = addresses }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get user addresses error: {ex}");

                return Json(new { Status = false, Message = "An error occurred while retrieving addresses. Please try again later." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddAddress(AddressRequest model)
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

                if (model.IsDefault)
                {
                    var existingDefaultAddress = db.Addresses
                        .Where(a => a.UserID == userId && a.IsDefault)
                        .ToList();

                    foreach (var address in existingDefaultAddress)
                    {
                        address.IsDefault = false;
                    }
                }

                var newAddress = new Address
                {
                    UserID = userId,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    TenDuong = model.TenDuong,
                    XaQuan = model.XaQuan,
                    TinhThanh = model.TinhThanh,
                    IsDefault = model.IsDefault
                };

                db.Addresses.Add(newAddress);
                db.SaveChanges();

                return Json(new
                {
                    Status = true,
                    Message = "Address added successfully",
                    Address = new AddressData
                    {
                        AddressID = newAddress.AddressID,
                        FullName = newAddress.FullName,
                        Phone = newAddress.Phone,
                        TenDuong = newAddress.TenDuong,
                        XaQuan = newAddress.XaQuan,
                        TinhThanh = newAddress.TinhThanh,
                        IsDefault = newAddress.IsDefault
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add address error: {ex}");

                return Json(new { Status = false, Message = "An error occurred while adding address. Please try again later." });
            }
        }

        // GET: Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ActionName("ForgotPassword")]
        public JsonResult ForgotPasswordPost(ForgotPasswordRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = string.Join(", ", errors)
                    });
                }

                var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = "Email not found."
                    });
                }

                if (!user.IsActive)
                {
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = "This account is inactive. Please contact support."
                    });
                }

                string verificationCode = EmailService.GenerateVerificationCode();

                var resetToken = new PasswordResetToken
                {
                    Email = model.Email,
                    Code = verificationCode,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(15),
                    IsUsed = false
                };

                db.PasswordResetTokens.Add(resetToken);
                db.SaveChanges();

                var emailService = new EmailService();
                bool emailSent = emailService.SendPasswordResetCode(model.Email, verificationCode, user.FullName);

                if (!emailSent)
                {
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = "Failed to send email. Please try again later."
                    });
                }

                return Json(new ForgotPasswordResponse
                {
                    Status = true,
                    Message = "Verification code sent to your email. Please check your inbox."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Forgot password error: {ex}");

                return Json(new ForgotPasswordResponse
                {
                    Status = false,
                    Message = "An error occurred while processing your request. Please try again later."
                });
            }
        }

        // POST: Account/VerifyResetCode
        [HttpPost]
        public JsonResult VerifyResetCode(VerifyResetCodeRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = string.Join(", ", errors)
                    });
                }

                // Find valid token
                var token = db.PasswordResetTokens
                    .Where(t => t.Email == model.Email
                             && t.Code == model.Code
                             && !t.IsUsed
                             && t.ExpiresAt > DateTime.Now)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefault();

                if (token == null)
                {
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = "Invalid or expired verification code."
                    });
                }

                return Json(new ForgotPasswordResponse
                {
                    Status = true,
                    Message = "Code verified successfully. You can now reset your password."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Verify reset code error: {ex}");

                return Json(new ForgotPasswordResponse
                {
                    Status = false,
                    Message = "An error occurred while verifying code. Please try again later."
                });
            }
        }

        // POST: Account/ResetPassword
        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = string.Join(", ", errors)
                    });
                }

                // Find valid token
                var token = db.PasswordResetTokens
                    .Where(t => t.Email == model.Email
                             && t.Code == model.Code
                             && !t.IsUsed
                             && t.ExpiresAt > DateTime.Now)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefault();

                if (token == null)
                {
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = "Invalid or expired verification code."
                    });
                }

                // Get user
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return Json(new ForgotPasswordResponse
                    {
                        Status = false,
                        Message = "User not found."
                    });
                }

                // Hash new password using BCrypt (same as registration)
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                // Update user password
                user.PasswordHash = hashedPassword;

                // Mark token as used
                token.IsUsed = true;
                token.UsedAt = DateTime.Now;

                db.SaveChanges();

                return Json(new ForgotPasswordResponse
                {
                    Status = true,
                    Message = "Password reset successfully. You can now login with your new password."
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Reset password error: {ex}");

                return Json(new ForgotPasswordResponse
                {
                    Status = false,
                    Message = "An error occurred while resetting password. Please try again later."
                });
            }
        }

        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                Session.Clear();
                Session.Abandon();

                if (Response.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddDays(-1);
                }

                if (Response.Cookies["UserInfo"] != null)
                {
                    Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
                }

                return Json(new { Status = true, Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = "Logout error: " + ex.Message });
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