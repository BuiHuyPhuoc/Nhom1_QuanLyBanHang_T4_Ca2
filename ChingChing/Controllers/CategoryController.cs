using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
using Unidecode.NET;

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

        public ActionResult Search(string inputSearch)
        {

            // Chuyển từ khóa tìm kiếm thành dạng không dấu và chuyển thành chữ thường
            string normalizedSearch = inputSearch.Unidecode().ToLower();

            var allProducts = db.PRODUCTs.ToList();

            // Tìm sản phẩm dựa trên tên sản phẩm không dấu và chữ thường
            var normalizedProducts = allProducts
                .Where(n => n.NAMEPRO.Unidecode().ToLower().Contains(normalizedSearch) || n.NAMEPRO.ToLower().Contains(normalizedSearch))
                .ToList();

            return View(normalizedProducts.OrderBy(n => n.NAMEPRO));

        }
    }

}