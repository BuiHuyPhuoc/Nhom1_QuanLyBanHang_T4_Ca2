using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
using Microsoft.Ajax.Utilities;

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
                            return RedirectToAction("QLDonHang", "Admin", new { area = "Admin"});
                        } else
                        {
                            //Get email customer into Session
                            Session["Email"] = checkEmail;
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

        public ActionResult UserDetail()
        {
            
            return View();
        }
        public ActionResult resetPage()
        {
            return RedirectToAction("UserDetail", "Users");
        }
        public JsonResult updateCustomer(string email, string name, string address)
        {
            CUSTOMER getCustomer = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            getCustomer.CUSNAME = name;
            getCustomer.ADDRESS = address;
            db.SaveChanges();
            Session["email"] = db.CUSTOMERs.Where(x => x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
            var data = new { Check = true, Email = email, Name = name, Address = address };
            return Json(data);

        }

    }
}