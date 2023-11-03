using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
namespace ChingChing.Controllers
{
    public class HomeController : Controller
    {
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult Index()
        {
            var prod = db.PRODUCTs.ToList().Take(8);
            return View(prod);
        }
        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}