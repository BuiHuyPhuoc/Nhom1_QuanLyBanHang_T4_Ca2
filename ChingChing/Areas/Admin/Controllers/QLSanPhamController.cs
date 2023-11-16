using ChingChing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ChingChing.Areas.Admin.Controllers
{
    public class QLSanPhamController : Controller
    {
        // GET: Admin/QLSanPham
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult EditProduct(int id)
        {
            var product = db.PRODUCTs.Find(id);
            ViewBag.Categories = new SelectList(db.CATEGORies, "IDCATE", "TENCATE", product.IDCATE);
            return View(product);
        }

        // Trong EditProduct ActionResult POST
        [HttpPost]
        public ActionResult EditProduct(PRODUCT product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    file.SaveAs(path);
                    product.IMAGEPRO = "/Images/" + fileName;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToAction("EditProduct","QLSanPham");
            }

            ViewBag.Categories = new SelectList(db.CATEGORies, "IDCATE", "TENCATE", product.IDCATE);
            return View(product);
        }

        // Trong AddProduct ActionResult POST
        // Trong AddProduct ActionResult GET
        public ActionResult AddProduct()
        {
            ViewBag.Categories = new SelectList(db.CATEGORies, "IDCATE", "TENCATE");
            return View();
        }

        // Trong AddProduct ActionResult POST
        [HttpPost]
        public ActionResult AddProduct(PRODUCT product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    file.SaveAs(path);
                    product.IMAGEPRO = "/Images/" + fileName;
                }

                db.PRODUCTs.Add(product);
                db.SaveChanges();
                return RedirectToAction("QLSanPham");
            }

            ViewBag.Categories = new SelectList(db.CATEGORies, "IDCATE", "TENCATE");
            return View(product);
        }

        // Trong DeleteProductConfirmed ActionResult
        [HttpPost, ActionName("DeleteProduct")]
        public ActionResult DeleteProductConfirmed(int id)
        {
            var product = db.PRODUCTs.Find(id);
            db.PRODUCTs.Remove(product);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
            return RedirectToAction("DeleteProduct", "QLSanPham");
        }
    }
}