using ChingChing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;

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

                // Lấy thông tin sản phẩm hiện tại từ cơ sở dữ liệu
                var existingProduct = db.PRODUCTs.Find(product.IDPRO);

                // Cập nhật thuộc tính của sản phẩm hiện tại
                existingProduct.NAMEPRO = product.NAMEPRO;
                existingProduct.DESCRIPTIONPRO = product.DESCRIPTIONPRO;
                existingProduct.PRICE = product.PRICE;
                existingProduct.IDCATE = product.IDCATE;
                existingProduct.QUANTITY = product.QUANTITY;

                if (file != null && file.ContentLength > 0)
                {
                    // Cập nhật hình ảnh chỉ khi có tệp mới được tải lên
                    existingProduct.IMAGEPRO = product.IMAGEPRO;
                }

                db.Entry(existingProduct).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToAction("QLSanPham", "Admin");
            }

            ViewBag.Categories = new SelectList(db.CATEGORies, "IDCATE", "TENCATE", product.IDCATE);
            return View(product);
        }

        // Trong AddProduct ActionResult POST
        // Trong AddProduct ActionResult GET
        public ActionResult AddProduct()
        {
            ViewBag.Categories = db.CATEGORies.ToList();
            return View();
        }

        // Trong AddProduct ActionResult POST
        [HttpPost]
        public ActionResult AddProduct(int idcate, int sl, HttpPostedFileBase anhmota, string gia, string dessp, string tensp)
        {
            PRODUCT _newPro = new PRODUCT();
            if (ModelState.IsValid)
            {
                
                _newPro.NAMEPRO = tensp;
                _newPro.DESCRIPTIONPRO = dessp;
                decimal getPrice = Convert.ToDecimal(gia);
                _newPro.PRICE = getPrice;
                
                if (anhmota != null && anhmota.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(anhmota.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    _newPro.IMAGEPRO = anhmota.FileName;
                    anhmota.SaveAs(path);
                } else
                {
                    ViewBag.Noti = "Failed";
                    return RedirectToAction("QLSanPham", "Admin");
                }
                _newPro.IDCATE = idcate;
                short _getQuant = Convert.ToInt16(sl);
                _newPro.QUANTITY = _getQuant;
                db.PRODUCTs.Add(_newPro);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm thành công!";
                return RedirectToAction("QLSanPham", "Admin");
               
            }
            ViewBag.Categories = new SelectList(db.CATEGORies, "IDCATE", "TENCATE");
            return View(_newPro);

        }

        // Trong DeleteProductConfirmed ActionResult
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            var product = db.PRODUCTs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // Trong DeleteProductConfirmed ActionResult
        [HttpPost]
        public ActionResult DeleteProductConfirmed(int id)
        {
            var product = db.PRODUCTs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            db.PRODUCTs.Remove(product);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
            return RedirectToAction("QLSanPham", "Admin");
        }
    }
}