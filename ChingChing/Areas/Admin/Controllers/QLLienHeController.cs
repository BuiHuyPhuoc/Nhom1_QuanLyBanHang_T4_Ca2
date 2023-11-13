using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
namespace ChingChing.Areas.Admin.Controllers
{
    public class QLLienHeController : Controller
    {
        // GET: Admin/QLLienHe
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult ShowContact(string idContact = "")
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            if (idContact == "")
            {
                return RedirectToAction("ErrorPage", "Home", new {area = ""});
            } else
            {
                int convertidContact = int.Parse(idContact);
                CONTACT getContact = db.CONTACTs.Where(x => x.ID_CONTACT == convertidContact).FirstOrDefault();
                return View(getContact);
            }
           
        }
        public ActionResult SendReply(string subject, string content, string to_email, string idContact)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            string _fromGmail = "phuoctestsender@gmail.com";
            string _fromPassword = "bpuhfkknnhyfkmnv";
            string htmlString =
                "<html>" +
                "   <body>" +
                $"       {content}</b>" +
                "   </body>" +
                "</html>";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_fromGmail);
            message.Subject = subject;
            message.To.Add(new MailAddress(to_email.Trim()));
            message.Body = htmlString;
            message.IsBodyHtml = true;

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromGmail, _fromPassword),
                EnableSsl = true,
            };

            smtp.Send(message);

            TempData["SendReply_Noti"] = $"Gửi tin nhắn đến email {to_email} thành công.";

            int convertidContact = int.Parse(idContact);
            CONTACT getContact = db.CONTACTs.Where(x => x.ID_CONTACT == convertidContact).FirstOrDefault();
            getContact.STATUS = "Replied";
            db.SaveChanges();
            
            return RedirectToAction("QLLienHe", "Admin");
        }
    }
}