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
                        //Get email customer into Session
                        Session["Email"] = checkEmail;
                        if (checkEmail.MAROLE == 1)
                        {
                            return RedirectToAction("QLDonHang", "Admin", new { area = "Admin"});
                        } else
                        {
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
                //Chuyển hướng đến trang VerifyPage
                return RedirectToAction("VerificationPage", "Users", new { name = nameCus, email = emailCus, matkhau = pass, phone = phoneCus });
            }

        }

        public ActionResult UserDetail()
        {
            UpdateSessionAccount();
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
            UpdateSessionAccount();
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
            newContact.STATUS = "Not Reply Yet";
            db.CONTACTs.Add(newContact);
            db.SaveChanges();
            TempData["Noti"] = HttpUtility.HtmlEncode("Nội dung tin nhắn của bạn đã được gửi đi với địa chỉ email là " + email + ". Chúng tôi sẽ phản hồi trong thời gian sớm nhất bằng tiếng Việt.");
            return RedirectToAction("Contact", "Users");
        }

        public ActionResult LayLaiMatKhau()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LayLaiMatKhau(string email)
        {
            CUSTOMER getCustomer = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            if  (getCustomer != null)
            {
                //Lấy ra thông tin người gửi
                string _fromGmail = "phuoctestsender@gmail.com";
                string _fromPassword = "bpuhfkknnhyfkmnv";

                //Tạo random mật khẩu mới
                Random random = new Random();
                int getRandomNumber = random.Next(100000, 999999);

                //Nội dung của email
                string htmlString =
                    "<html>" +
                    "   <body>" +
                    $"       Mật khẩu mới của bạn là: <b>{getRandomNumber}</b>" +
                    "   </body>" +
                    "</html>";

                //Thực hiện tạo một email
                MailMessage message = new MailMessage();
                message.From = new MailAddress(_fromGmail); //Nhập địa chỉ người gửi
                message.Subject = "Xác nhận gửi mật khẩu mới"; //Nhập chủ đề
                message.To.Add(new MailAddress(email.Trim())); //Nhập địa chỉ người nhận
                message.Body = htmlString;
                message.IsBodyHtml = true;

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_fromGmail, _fromPassword),
                    EnableSsl = true,
                };

                smtp.Send(message);  //Xác nhận gửi

                //Gửi thông báo kiểm tra hòm thư
                TempData["GetNewPass_Noti"] = $"Vui lòng kiểm tra trong hộp thư của email {email}.";

                //Đổi mật khẩu customer dựa trên chuỗi đã tạo

                getCustomer.MATKHAU = getRandomNumber.ToString();
                db.SaveChanges();

                //Trở về trang Login thông qua hàm Logout
                return RedirectToAction("Logout", "Users");
            } else
            {
                //Không tìm thấy tài khoản nào chứa email này
                ViewBag.Noti = "*Không có tài khoản nào sử dụng email này.";
                return View();
            }
        }

        public ActionResult CheckDone(string idorder)
        {
            int converted = int.Parse(idorder);
            ORDER getOrder = db.ORDERs.Where(x => x.IDORDER == converted).FirstOrDefault();
            getOrder.DATERECEIVE = DateTime.Now;

            //Tạo hoá đơn
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            BILL newBill = new BILL();
            newBill.DATEBILL = DateTime.Now;
            newBill.EMAILCUS = getCustomer.EMAILCUS;
            newBill.TOTAL_PRICE = getOrder.TOTAL_PRICE;
            newBill.ADDRESS = getOrder.ADDRESS;
            db.BILLs.Add(newBill);

            //Tạo chi tiết hoá đơn
            List<ORDERDETAIL> getListOrderDetail = db.ORDERDETAILs.Where(x => x.IDORDER == getOrder.IDORDER).ToList();
            foreach (var item in getListOrderDetail)
            {
                BILLDETAIL newBillDetail = new BILLDETAIL();
                newBillDetail.IDBILL = newBill.IDBILL;
                newBillDetail.IDPRO = item.IDPRO;
                newBillDetail.QUANTITY = item.QUANTITY;
                db.BILLDETAILs.Add(newBillDetail);
            }
            db.SaveChanges();

            CheckAndUpdateCustomerType(getCustomer.EMAILCUS);

            UpdateSessionAccount();
            TempData["NhanHangThanhCong"] = "Bạn đã nhận hàng thành công!! Cảm ơn vì đã sử dụng dịch vụ của ChinhChinh Store.";
            return RedirectToAction("UserDetail", "Users");
        }

        public bool CheckAndUpdateCustomerType(string email)
        {
            bool check = false;
            CUSTOMER getCustomer = db.CUSTOMERs.Where(x => x.EMAILCUS == email).FirstOrDefault();
            double totalBillPrice = 0.0;
            List<BILL> getListBill = db.BILLs.Where(x => x.EMAILCUS == getCustomer.EMAILCUS).ToList();
            foreach (var item in getListBill)
            {
                totalBillPrice += Convert.ToDouble(item.TOTAL_PRICE) ;
            }

            if (totalBillPrice > 2000000 && getCustomer.MAROLE != 3)
            {
                getCustomer.MAROLE = 3;
                check = true;
                TempData["UpdateTypeAccount"] = "Chúc mừng, bạn đã được nâng cấp lên khách hàng VIP của ChinhChinh Store.";
            }
            db.SaveChanges();
            return check;
        }

        public void UpdateSessionAccount()
        {
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            Session["Email"] = db.CUSTOMERs.Where(x => x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
        }

        public ActionResult ShowOrderDetail(string idorder)
        {
            int converted = int.Parse(idorder);
            List<ORDERDETAIL> getListOrderDetail = db.ORDERDETAILs.Where(x => x.IDORDER == converted).ToList();
            return PartialView(getListOrderDetail);
        }
    }
}