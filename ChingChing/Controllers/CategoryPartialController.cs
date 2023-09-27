using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;

namespace ClothingShop.Controllers
{
    public class CategoryPartialController : Controller
    {
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        // GET: CategoryPartial
        public ActionResult PartialCate()
        {
            var listCate = db.CATEGORies.ToList();
            return PartialView(listCate);
        }

    }
}