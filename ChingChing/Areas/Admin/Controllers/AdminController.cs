using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChingChing.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public ActionResult QLDonHang()
        {
            return View();
        }
        public ActionResult QLTaiKhoan()
        {
            return View();
        }
        public ActionResult QLHoaDon()
        {
            return View();
        }
        public ActionResult QLSanPham()
        {
            return View();
        }
        public ActionResult QLLoaiSanPham()
        {
            return View();
        }
    }
}