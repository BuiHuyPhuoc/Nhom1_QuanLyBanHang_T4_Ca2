using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;

namespace ClothingShop.Controllers
{
    
    public class CategoryController : Controller
    {
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        // GET: Category
        public ActionResult Products()
        {
            var prodList = db.PRODUCTs.ToList();
            return View(prodList);
        }

        public ActionResult ListProductCate(int id)
        {
            var prodList = db.PRODUCTs.Where(p => p.IDCATE == id).ToList();
            return View(prodList);
        }
        //public ActionResult Search(string inputSearch)
        //{
        //    var result = db.Products.Where(s => s.ProductName.Contains(inputSearch)).ToList();

        //    return View(result);
        //}
    }
}