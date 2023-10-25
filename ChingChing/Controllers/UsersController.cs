using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
            Console.OutputEncoding = Encoding.UTF8;
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
            return RedirectToAction("Login", "Users");
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
            string phoneCus = account["PhoneCus"].ToString();
            if (pass != rePass)
            {
                ViewBag.Notif = "Mật khẩu và nhập lại mật khẩu không khớp";
                ViewBag.EmailCus = emailCus;
                ViewBag.nameCus = nameCus;
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
                //CUSTOMER customer = new CUSTOMER();
                //customer.CUSNAME = nameCus;
                //customer.EMAILCUS = emailCus;
                //customer.MATKHAU = pass;
                //customer.MAROLE = 2;
                //db.CUSTOMERs.Add(customer);
                //db.SaveChanges();
                //TempData["Email"] = emailCus;
                //TempData["Password"] = pass;
                //return RedirectToAction("Login", "Users");

                //Chuyển hướng đến trang VerifyPage
                return RedirectToAction("VerificationPage", "Users", new { name = nameCus, email = emailCus, matkhau = pass, phone = phoneCus });
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

        public ActionResult VerificationPage(string name, string email, string matkhau, string phone)
        {
            //Kiểm tra trong table đã có xuất hiện gmail đó chưa
            //Nếu chưa thì tạo mã mới và lưu vào bảng
            //Rồi thì cập nhật mã mới
            //Thực hiện lấy random dãy số từ 100000 đến 999999
            Random random = new Random();
            int getRandomNumber = random.Next(100000, 999999);

            //Tìm kiếm email đã được gửi mã chưa
            var item = db.VERIFYCODEs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            if (item == null)
            {
                
                //Thực hiện lưu mã xác thực vào bảng
                VERIFYCODE newCode = new VERIFYCODE();
                newCode.EMAILCUS = email;
                newCode.CODE = getRandomNumber;
                db.VERIFYCODEs.Add(newCode);
                
            } else
            {
                //Cập nhật dữ liệu mới
                item.CODE = getRandomNumber;
            }
            //Lưu
            db.SaveChanges();

            //Thực hiện gửi mã code qua gmail cho khách hàng
            SendEmail(getRandomNumber, email);

            ViewBag.Email = email;
            ViewBag.Pass = matkhau;
            ViewBag.Name = name;
            ViewBag.Phone = phone;
            return View();
        }
        [HttpPost]
        public JsonResult checkVerifyCode(string code, string email, string pass, string name, string phone)
        {
            var checkFromDatabase = db.VERIFYCODEs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            if (checkFromDatabase != null && checkFromDatabase.CODE == int.Parse(code))
            {
                CUSTOMER customer = new CUSTOMER();
                customer.CUSNAME = name;
                customer.EMAILCUS = email;
                customer.MATKHAU = pass;
                customer.MAROLE = 2;
                customer.PHONE = phone;
                db.CUSTOMERs.Add(customer);
                db.SaveChanges();


                TempData["Email"] = email;
                TempData["Password"] = pass;

                return Json(true);
            } else
            {
                return Json(false);
            }
        }

        public void SendEmail(int getRandomNumber, string getGmail)
        {
            //bpuh fkkn nhyf kmnv
            string _fromGmail = "phuoctestsender@gmail.com";
            string _fromPassword = "bpuhfkknnhyfkmnv";
            string htmlString =
                "<html>" +
                "   <body>" +
                $"       Mã xác thực của bạn là <b>{getRandomNumber}</b>" +
                "   </body>" +
                "</html>";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_fromGmail);
            message.Subject = "Gửi mã xác thực từ web ChinhChinh Store";
            message.To.Add(new MailAddress(getGmail.Trim()));
            message.Body = htmlString;
            message.IsBodyHtml = true;

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromGmail, _fromPassword),
                EnableSsl = true,
            };

            smtp.Send(message);
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult SendMessage(string message, string email, string name)
        {
            CONTACT newContact = new CONTACT();
            newContact.MESSAGE = message;
            newContact.EMAIL = email;
            newContact.NAME = name;
            newContact.DATE_SEND = DateTime.Now;
            db.CONTACTs.Add(newContact);
            db.SaveChanges();
            string mess = $"Tin nhắn của bạn đã được gửi đi với địa chỉ email là {email}. Chúng tôi sẽ phản hồi" +
                " trong thời gian sớm nhất";
            TempData["Noti"] = mess;
            return RedirectToAction("Contact", "Users");
        }

    }
}