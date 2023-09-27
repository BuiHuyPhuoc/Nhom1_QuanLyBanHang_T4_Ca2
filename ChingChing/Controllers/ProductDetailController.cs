using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;

namespace ClothingShop.Controllers
{
    public class ProductDetailController : Controller
    {
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        // GET: ProductDetail
        public ActionResult Detail(int id)
        {
            var product = db.PRODUCTs.FirstOrDefault(p => p.IDPRO == id);
            ViewBag.ProdList = db.PRODUCTs.Where(x => x.IDCATE == product.IDCATE).Take(4);
            return View(product);
        }
    }
}