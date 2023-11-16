using ChingChing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;

namespace ChingChing.Areas.Admin.Controllers
{
    public class QLSanPhamController : Controller
    {
        // GET: Admin/QLSanPham
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult EditProduct(int id)
        {
            var product = db.PRODUCTs.Find(id);
            return View(product);
        }

        // Action để xử lý cập nhật sản phẩm
        [HttpPost]
        public ActionResult EditProduct(PRODUCT product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QLSanPham");
            }
            return View(product);
        }

        // Action để hiển thị form thêm sản phẩm
        public ActionResult AddProduct()
        {
            return View();
        }

        // Action để xử lý thêm sản phẩm
        [HttpPost]
        public ActionResult AddProduct(PRODUCT product)
        {
            if (ModelState.IsValid)
            {
                db.PRODUCTs.Add(product);
                db.SaveChanges();
                return RedirectToAction("QLSanPham");
            }
            return View(product);
        }

        // Action để hiển thị form xóa sản phẩm
        public ActionResult DeleteProduct(int id)
        {
            var product = db.PRODUCTs.Find(id);
            return View(product);
        }

        // Action để xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteProduct")]
        public ActionResult DeleteProductConfirmed(int id)
        {
            var product = db.PRODUCTs.Find(id);
            db.PRODUCTs.Remove(product);
            db.SaveChanges();
            return RedirectToAction("QLSanPham");
        }
    }
}