using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
namespace ChingChing.Areas.Admin.Controllers
{
    public class QLDonHangController : Controller
    {
        // GET: Admin/QLDonHang
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult ChiTietDonHang(string idorder)
        {
            int id = int.Parse(idorder);
            List<ORDERDETAIL> getListOrderDetails = db.ORDERDETAILs
                                                    .Where(x=> x.IDORDER == id)
                                                    .OrderBy(x => x.ORDER.DATEORDER)
                                                    .ToList();
            return View(getListOrderDetails);
        }

        public ActionResult ChangeStatus(string idorder, string status)
        {
            string getStatus = returnStatus(status);
            int getIdOrder = int.Parse(idorder);
            if (getStatus != "Error")
            {
                var getItem = db.ORDERs.Where(x => x.IDORDER == getIdOrder).FirstOrDefault();
                if (getStatus == "Đã xác nhận")
                {
                    getItem.DATEACCEPT = DateTime.Now;
                } else if (getStatus == "Đang giao")
                {
                    getItem.DATESHIPPING = DateTime.Now;
                    //Khi đơn hàng đang giao, thì sẽ trừ vô kho
                    int convertToInt = int.Parse(idorder);
                    var getOrderDetails = db.ORDERDETAILs.Where(x => x.IDORDER == convertToInt).ToList();
                    foreach (var item in getOrderDetails)
                    {
                        PRODUCT product = db.PRODUCTs.Where(x => x.IDPRO == item.IDPRO).FirstOrDefault();
                        product.QUANTITY = Convert.ToInt16(product.QUANTITY - item.QUANTITY);
                    }
                    db.SaveChanges();
                } else if (getStatus == "Đã nhận")
                {
                    getItem.DATERECEIVE = DateTime.Now;
                    //Khi đơn hàng là đã nhận, thì sẽ lưu vô bảng hóa đơn
                    BILL bill = new BILL() {
                        DATEBILL = DateTime.Now,
                        ADDRESS = getItem.ADDRESS
                    };
                    db.BILLs.Add(bill);
                    db.SaveChanges();
                    int convertToInt = int.Parse(idorder);
                    var getOrderDetails = db.ORDERDETAILs.Where(x => x.IDORDER == convertToInt).ToList();
                    foreach(var item in getOrderDetails)
                    {
                        BILLDETAIL billdetail = new BILLDETAIL() {
                            IDBILL = bill.IDBILL,
                            IDPRO = item.IDPRO,
                            QUANTITY = item.QUANTITY
                        };
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("ChiTietDonHang", "QLDonHang", new { idorder = idorder });
        }
        private string returnStatus(string value)
        {
            switch (int.Parse(value))
            {
                case 1:
                    return "Chờ xác nhận";  
                case 2:
                    return "Đã xác nhận";
                case 3:
                    return "Đang giao";
                case 4:
                    return "Đã nhận";
                default:
                    return "Error";
            }
        }
    }
}