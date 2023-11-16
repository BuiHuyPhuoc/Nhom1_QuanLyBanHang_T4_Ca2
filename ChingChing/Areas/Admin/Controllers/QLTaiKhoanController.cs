using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
using System.Text.RegularExpressions;
namespace ChingChing.Areas.Admin.Controllers
{
    public class QLTaiKhoanController : Controller
    {
        // GET: Admin/QLTaiKhoan
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        [HttpPost]
        public ActionResult UpdateAccount(string email, string name, string address)
        {
            var getAccount = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();

            if (getAccount != null)
            {
                getAccount.CUSNAME = name;
                getAccount.ADDRESS = address;
                db.SaveChanges();
                TempData["Notification"] = "Cập nhật tài khoản thành công";
            }
            else
            {
                TempData["Notification"] = "Không tìm thấy tài khoản";
            }

            return RedirectToAction("QLTaiKhoan", "Admin");
        }
        public JsonResult changeButtonStatus(string email, string name, string address)
        {
            var getAccount = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            return (name == getAccount.CUSNAME && address == getAccount.ADDRESS) ? Json(true) : Json(false);
        }

        public ActionResult AdminCreateAccount(string name, string email, string phone, string address, string pass, List<int> roles)
        {
            CUSTOMER customer = new CUSTOMER
            {
                EMAILCUS = email,
                CUSNAME = name,
                PHONE = phone,
                ADDRESS = address,
                MATKHAU = pass
            };

            //foreach (int roleId in roles)
            //{
            //    customer.ROLE.Add(new ROLE
            //    {
            //        MAROLE = roleId
            //    });
            //}

            db.CUSTOMERs.Add(customer);
            TempData["NotificationCreate"] = "Tạo thành công";
            db.SaveChanges();
            return RedirectToAction("QLTaiKhoan", "Admin");
        }

        public JsonResult checkEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            if (Regex.IsMatch(email, emailPattern))
            {
                var getAccount = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
                return Json((getAccount == null));
            }
            else
            {
                return Json(false);
            }

        }
        [HttpPost]
        public ActionResult UpdateRole(string email, int newRole)
        {
            var customer = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            customer.MAROLE = newRole;
            db.SaveChanges();

            TempData["Notification"] = "Cập nhật quyền hạn thành công";
            return RedirectToAction("QLTaiKhoan", "Admin");
        }

        public ActionResult AddCustomerType(string ten, string chietkhau)
        {
            ROLE getRole = db.ROLEs.Where(x => x.TENROLE == ten).FirstOrDefault();
            if (getRole != null)
            {
                ViewBag.Noti = "Name of role existed.";
                return RedirectToAction("QLTaiKhoan", "Admin");
            }

            ROLE newRole = new ROLE();
            newRole.TENROLE = ten;
            newRole.CHIETKHAU = float.Parse(chietkhau)/100;
            db.ROLEs.Add(newRole);
            db.SaveChanges();
            ViewBag.Noti = "Success";
            return RedirectToAction("QLTaiKhoan", "Admin");
        }
        [HttpPost]
        public ActionResult DeleteRole(int roleId)
        {
            var roleToDelete = db.ROLEs.Find(roleId);
            if (roleToDelete != null)
            {
                CUSTOMER getCustomer = db.CUSTOMERs.Where(x => x.MAROLE == roleToDelete.MAROLE).FirstOrDefault();
                if (getCustomer != null)
                {
                    TempData["NotificationDeleteRole"] = "Quyền hạn có người sử dụng, không thể xoá";
                    return RedirectToAction("QLTaiKhoan", "Admin");
                }
                db.ROLEs.Remove(roleToDelete);
                db.SaveChanges();
                TempData["NotificationDeleteRole"] = "Xóa quyền hạn thành công";
            }
            else
            {
                TempData["NotificationDeleteRole"] = "Không thể xóa quyền hạn";
            }

            return RedirectToAction("QLTaiKhoan", "Admin");
        }

    }
}