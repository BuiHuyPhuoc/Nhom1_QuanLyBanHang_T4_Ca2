using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
namespace ChingChing.Controllers
{
    public class UsersController : Controller
        
    {
        // GET: Users
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CUSTOMER cus)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = db.CUSTOMERs.FirstOrDefault(x => x.EMAILCUS == cus.EMAILCUS);
                if (checkEmail != null)
                {
                    if (checkEmail.MATKHAU == cus.MATKHAU)
                    {
                        if (checkEmail.MAROLE == 1)
                        {
                            return RedirectToAction("Index", "Admin/Orders");
                        } else
                        {
                            //Get email customer into Session
                            Session["Email"] = checkEmail;
                            //Get full-name customer
                            string[] fullnamecus = checkEmail.CUSNAME.Split(' ');
                            //Get name from full-name to show in View
                            Session["Account"] = fullnamecus[fullnamecus.Length - 1];
                            return RedirectToAction("Index", "Home");
                        }
                    } else
                    {
                        ViewBag.ThongBao = "Sai mật khẩu!";
                    }
                } else
                {
                    ViewBag.ThongBao = "Email chưa được đăng kí!";
                }
              
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["Email"] = null;
            Session["Account"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(FormCollection account)
        {
            string pass = account["PassCus"].ToString();
            string rePass = account["Repass"].ToString();
            string emailCus = account["EmailCus"].ToString();
            string nameCus = account["NameCus"].ToString();
            if (pass != rePass)
            {
                ViewBag.Notif = "Mật khẩu và nhập lại mật khẩu không khớp";
                return View();
            }

            var cus = db.CUSTOMERs.FirstOrDefault(k => k.EMAILCUS == emailCus);
            if (cus != null)
            {
                ViewBag.Warning = "Đã có người đăng ký bằng email này";
                return View();

            }
            else
            {
                CUSTOMER customer = new CUSTOMER();
                customer.CUSNAME = nameCus;
                customer.EMAILCUS = emailCus;
                customer.MATKHAU = pass;
                customer.MAROLE = 2;
                db.CUSTOMERs.Add(customer);
                db.SaveChanges();
                TempData["Email"] = emailCus;
                TempData["Password"] = pass;
                return RedirectToAction("Login", "Users");
            }

        }
    }
}