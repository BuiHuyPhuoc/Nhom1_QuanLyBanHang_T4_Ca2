using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChingChing.Models;
namespace ChingChing.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        DB_ChinhChinhEntities db = new DB_ChinhChinhEntities();
        public ActionResult AddToCart(int idProduct)
        {
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            var itemCart = db.CARTs.Where(x => x.IDPRO == idProduct && x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
            if (itemCart == null)
            {
                CART newData = new CART();
                newData.EMAILCUS = getCustomer.EMAILCUS;
                newData.IDPRO = idProduct;
                newData.QUANTITY = 1;
                db.CARTs.Add(newData);
                db.SaveChanges();
            } else
            {
                //Trường hợp trong giỏ hàng đã có sản phẩm này rồi
                //Kiểm số lượng trong giỏ hàng có bé hơn số lượng tồn kho hay không
                //Không thì tăng số lượng lên 1
                //Có thì không tăng và giữ nguyên số lượng trong giỏ hàng
                if (itemCart.QUANTITY < itemCart.PRODUCT.QUANTITY)
                {
                    itemCart.QUANTITY += 1;
                    db.Entry(itemCart).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("YourCart", "Cart");
            

        }
        public ActionResult YourCart()
        {
            if (Session["Email"] != null)
            {
                CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
                string getEmail = getCustomer.EMAILCUS;
                List<CART> listCart = db.CARTs.Where(x => x.EMAILCUS == getEmail).ToList();
                ViewBag.TotalPrice = GetTotalPrice();
                ViewBag.TotalQuantity = GetTotalQuantity();
                return View(listCart);
            } else
            {
                return RedirectToAction("Login", "Users");
            }
          
        }
        public ActionResult DeleteData(int idProduct)
        {

            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            // Thực hiện cập nhật dữ liệu vào cơ sở dữ liệu ở đây (ví dụ: lưu vào biến data)
            var itemCart = db.CARTs.Where(x => x.IDPRO == idProduct && x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
            if (itemCart != null)
            {
                // Xóa dữ liệu
                db.CARTs.Remove(itemCart);
                db.SaveChanges();
                
            }
            return RedirectToAction("YourCart", "Cart"); // Trả về dữ liệu đã được cập nhật
        }
        public ActionResult DecreaseData(int idProduct)
        {

            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            // Thực hiện cập nhật dữ liệu vào cơ sở dữ liệu ở đây (ví dụ: lưu vào biến data)
            var itemCart = db.CARTs.Where(x => x.IDPRO == idProduct && x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
            int newData = Convert.ToByte(itemCart.QUANTITY - 1);
            if (newData < 1)
            {
                ViewBag.OutOfQuantity = "*Quá số lượng tồn kho";
            }
            else
            {
                itemCart.QUANTITY = Convert.ToByte(itemCart.QUANTITY - 1);
                db.Entry(itemCart).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("YourCart", "Cart"); // Trả về dữ liệu đã được cập nhật
        }
        public ActionResult IncreaseData(int idProduct)
        {
            
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            // Thực hiện cập nhật dữ liệu vào cơ sở dữ liệu ở đây (ví dụ: lưu vào biến data)
            var itemCart = db.CARTs.Where(x => x.IDPRO == idProduct && x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
            int newData = Convert.ToByte(itemCart.QUANTITY + 1);
            if ( newData > itemCart.PRODUCT.QUANTITY)
            {
                ViewBag.OutOfQuantity = "*Quá số lượng tồn kho";
            } else
            {
                itemCart.QUANTITY = Convert.ToByte(itemCart.QUANTITY + 1);
                db.Entry(itemCart).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("YourCart", "Cart"); // Trả về dữ liệu đã được cập nhật
        }

        public decimal GetTotalPrice()
        {
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            
            List<CART> listCart = db.CARTs.Where(x => x.EMAILCUS == getCustomer.EMAILCUS).ToList();
            decimal totalPrice = 0;
            
            foreach(var item in listCart)
            {
                if (item.PRODUCT.QUANTITY > 0)
                {
                    totalPrice = totalPrice + Convert.ToDecimal(item.PRODUCT.PRICE * item.QUANTITY);
                }
               
            }
            return totalPrice;
        }
        public int GetTotalQuantity()
        {
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            
            List<CART> listCart = db.CARTs.Where(x => x.EMAILCUS == getCustomer.EMAILCUS).ToList();
            int totalQuan = 0;
            
            foreach(var item in listCart)
            {
                if (item.PRODUCT.QUANTITY > 0)
                {
                    totalQuan = totalQuan + (int)item.QUANTITY;
                }
               
            }
            return totalQuan;
        }
    }
}