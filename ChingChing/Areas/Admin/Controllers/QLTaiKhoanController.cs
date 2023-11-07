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
            getAccount.CUSNAME = name;
            getAccount.ADDRESS = address;
            db.SaveChanges();
            TempData["Notification"] = "Cập nhật tài khoản thành công";
            return RedirectToAction("QLTaiKhoan", "Admin");
        }
        public JsonResult changeButtonStatus(string email, string name, string address)
        {
            var getAccount = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            return (name == getAccount.CUSNAME && address == getAccount.ADDRESS) ? Json(true) : Json(false);
        }

        public ActionResult AdminCreateAccount(string name, string email, string phone, string address, string pass, string role)
        {
            CUSTOMER customer = new CUSTOMER
            {
                EMAILCUS = email,
                CUSNAME = name,
                PHONE = phone,
                ADDRESS = address,
                MATKHAU = pass,
                MAROLE = int.Parse(role)
            };
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
    }
}