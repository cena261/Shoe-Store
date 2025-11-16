using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShoeStore.Models;
using ShoeStore.Areas.Admin.Models.DTOs;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private ShoeStoreDBContext db = new ShoeStoreDBContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
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

        public ActionResult Index(string search = "", int page = 1, int pageSize = 20)
        {
            ViewBag.Title = "Users";
            ViewBag.PageTitle = "Users Management";
            ViewBag.Search = search;
            ViewBag.Page = page;

            var query = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search) || u.FullName.Contains(search) || u.Phone.Contains(search));
            }

            var totalUsers = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            var users = query
                .OrderByDescending(u => u.createdAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserListItemDTO
                {
                    UserId = u.userId,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    CreatedAt = u.createdAt,
                    LastLogin = u.lastLogin,
                    IsActive = u.IsActive,
                    Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                    TotalOrders = u.Orders.Count()
                })
                .ToList();

            ViewBag.Users = users;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }

        public ActionResult Details(int id)
        {
            var user = db.Users
                .Include(u => u.UserRoles.Select(ur => ur.Role))
                .Include(u => u.Orders)
                .Include(u => u.Addresses)
                .FirstOrDefault(u => u.userId == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserDetailDTO
            {
                UserId = user.userId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                CreatedAt = user.createdAt,
                LastLogin = user.lastLogin,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                Orders = user.Orders.OrderByDescending(o => o.OrderDate).Select(o => new UserOrderDTO
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    TotalItems = o.OrderItems.Count()
                }).ToList(),
                Addresses = user.Addresses.Select(a => new UserAddressDTO
                {
                    AddressID = a.AddressID,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    TenDuong = a.TenDuong,
                    XaQuan = a.XaQuan,
                    TinhThanh = a.TinhThanh,
                    IsDefault = a.IsDefault
                }).ToList()
            };

            ViewBag.Title = "User Details";
            ViewBag.PageTitle = "User Details: " + user.FullName;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new UserEditDTO
            {
                UserId = user.userId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                IsActive = user.IsActive
            };

            ViewBag.Title = "Edit User";
            ViewBag.PageTitle = "Edit User: " + user.FullName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = db.Users.Find(model.UserId);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var existingEmail = db.Users.Any(u => u.Email == model.Email && u.userId != model.UserId);
                if (existingEmail)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                user.Email = model.Email;
                user.FullName = model.FullName;
                user.Phone = model.Phone;
                user.IsActive = model.IsActive;

                db.SaveChanges();

                TempData["SuccessMessage"] = "User updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update user error: {ex}");

                ModelState.AddModelError("", "An error occurred while updating user. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var currentUserId = (int)Session["UserId"];
                if (id == currentUserId)
                {
                    return Json(new UserResponseDTO { Status = false, Message = "Cannot delete your own account" });
                }

                var user = db.Users.Find(id);
                if (user == null)
                {
                    return Json(new UserResponseDTO { Status = false, Message = "User not found" });
                }

                user.IsActive = false;
                db.SaveChanges();

                return Json(new UserResponseDTO { Status = true, Message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Delete user error: {ex}");

                return Json(new UserResponseDTO { Status = false, Message = "An error occurred while deactivating user. Please try again." });
            }
        }

        public ActionResult ManageRoles(int id)
        {
            var user = db.Users
                .Include(u => u.UserRoles.Select(ur => ur.Role))
                .FirstOrDefault(u => u.userId == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var allRoles = db.Roles.ToList();
            var userRoleIds = user.UserRoles.Select(ur => ur.RoleID).ToList();

            var model = new UserRoleDTO
            {
                UserId = user.userId,
                Email = user.Email,
                FullName = user.FullName,
                CurrentRoles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                AvailableRoles = allRoles.Select(r => new RoleOptionDTO
                {
                    RoleID = r.RoleID,
                    RoleName = r.RoleName,
                    IsAssigned = userRoleIds.Contains(r.RoleID)
                }).ToList()
            };

            ViewBag.Title = "Manage Roles";
            ViewBag.PageTitle = "Manage Roles: " + user.FullName;
            return View(model);
        }

        [HttpPost]
        public JsonResult AssignRole(int userId, int roleId)
        {
            try
            {
                var existingRole = db.UserRoles.FirstOrDefault(ur => ur.UserID == userId && ur.RoleID == roleId);
                if (existingRole != null)
                {
                    return Json(new UserResponseDTO { Status = false, Message = "Role already assigned" });
                }

                var userRole = new UserRoles
                {
                    UserID = userId,
                    RoleID = roleId,
                    AssignedAt = DateTime.Now
                };

                db.UserRoles.Add(userRole);
                db.SaveChanges();

                return Json(new UserResponseDTO { Status = true, Message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Assign role error: {ex}");

                return Json(new UserResponseDTO { Status = false, Message = "An error occurred while assigning role. Please try again." });
            }
        }

        [HttpPost]
        public JsonResult RemoveRole(int userId, int roleId)
        {
            try
            {
                var currentUserId = (int)Session["UserId"];
                if (userId == currentUserId)
                {
                    var role = db.Roles.Find(roleId);
                    if (role != null && role.RoleName == "Admin")
                    {
                        return Json(new UserResponseDTO { Status = false, Message = "Cannot remove Admin role from your own account" });
                    }
                }

                var userRole = db.UserRoles.FirstOrDefault(ur => ur.UserID == userId && ur.RoleID == roleId);
                if (userRole == null)
                {
                    return Json(new UserResponseDTO { Status = false, Message = "Role not found" });
                }

                db.UserRoles.Remove(userRole);
                db.SaveChanges();

                return Json(new UserResponseDTO { Status = true, Message = "Role removed successfully" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Remove role error: {ex}");

                return Json(new UserResponseDTO { Status = false, Message = "An error occurred while removing role. Please try again." });
            }
        }

        [HttpGet]
        public JsonResult GetUserRoles(int id)
        {
            try
            {
                var user = db.Users
                    .Include(u => u.UserRoles.Select(ur => ur.Role))
                    .FirstOrDefault(u => u.userId == id);

                if (user == null)
                {
                    return Json(new UserResponseDTO { Status = false, Message = "User not found" }, JsonRequestBehavior.AllowGet);
                }

                var roles = user.UserRoles.Select(ur => new
                {
                    RoleID = ur.RoleID,
                    RoleName = ur.Role.RoleName
                }).ToList();

                return Json(new UserResponseDTO { Status = true, Data = roles }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Get user roles error: {ex}");

                return Json(new UserResponseDTO { Status = false, Message = "An error occurred while retrieving user roles. Please try again." }, JsonRequestBehavior.AllowGet);
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
