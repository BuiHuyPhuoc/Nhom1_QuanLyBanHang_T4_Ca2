using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
namespace ChingChing.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult QLDonHang()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = ""});
            }
            List<ORDER> getListOrder = db.ORDERs.OrderBy(x => x.DATEORDER).ToList();
            return View(getListOrder);
        }
        public ActionResult QLTaiKhoan()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            List<CUSTOMER> getListCustomer = db.CUSTOMERs.ToList();
            ViewBag.ListRole = db.ROLEs.ToList();
            return View(getListCustomer);
        }
        public ActionResult QLHoaDon()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            return View();
        }
        public ActionResult QLSanPham()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            return View();
        }
        public ActionResult QLLoaiSanPham()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            return View();
        }
        public ActionResult QLLienHe()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            List<CONTACT> listContact = db.CONTACTs.ToList();
            return View(listContact);
        }
        [HttpPost]
        public ActionResult QLLienHe(string name)
        {
            List<CONTACT> listContact = db.CONTACTs.Where(x => x.NAME.Contains(name)).ToList();
            return View(listContact);
        }
    }
}