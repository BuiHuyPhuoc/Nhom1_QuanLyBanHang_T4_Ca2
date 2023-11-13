using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
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
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
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
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
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

            if (getCustomer.ROLE.CHIETKHAU != 0)
            {
                totalPrice -= totalPrice * Convert.ToDecimal(getCustomer.ROLE.CHIETKHAU) ;
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

        [HttpPost]
        public ActionResult Pay(string address)
        {
            CUSTOMER getCustomer = Session["Email"] as CUSTOMER;
            if (getCustomer != null)
            {
                ORDER order = new ORDER
                {
                    DATEORDER = DateTime.Now,
                    ADDRESS = address,
                    EMAILCUS = getCustomer.EMAILCUS,
                };
                db.ORDERs.Add(order);
                db.SaveChanges();
                List<CART> getCartFromCustomer = db.CARTs.Where(x => x.EMAILCUS == getCustomer.EMAILCUS).ToList();
                foreach (var item in getCartFromCustomer)
                {
                    ORDERDETAIL orderdetail = new ORDERDETAIL
                    {
                        IDORDER = order.IDORDER,
                        IDPRO = item.IDPRO,
                        QUANTITY = item.QUANTITY
                    };
                    db.ORDERDETAILs.Add(orderdetail);
                }
                db.SaveChanges();

                //Xóa giỏ hàng
                foreach(var item in getCartFromCustomer)
                {
                    db.CARTs.Remove(item);
                }
                db.SaveChanges();
                return RedirectToAction("ThanksPage", "Cart");
            } else
            {
                return RedirectToAction("YourCart", "Cart");
            }
        }
        public ActionResult ThanksPage()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("ErrorPage", "Home", new { area = "" });
            }
            return View();
        }

        public JsonResult ChangeQuantity(string value, string idpro)
        {
            var getCustomer = Session["Email"] as CUSTOMER;
            int convertToInt = int.Parse(idpro);
            var getCart = db.CARTs.Where(x => x.IDPRO == convertToInt && x.EMAILCUS == getCustomer.EMAILCUS).FirstOrDefault();
            bool result = true;
            if (value  == "UP")
            {
                if (getCart.QUANTITY + 1 > getCart.PRODUCT.QUANTITY)
                {
                    result = false;
                } else
                {
                    getCart.QUANTITY += 1;
                }
            } else
            {
                if (getCart.QUANTITY - 1 <= 0)
                {
                    result = false;
                }
                else
                {
                    getCart.QUANTITY -= 1;
                }
            }
            db.SaveChanges();
            int totalQuantity = GetTotalQuantity();
            decimal getTotalPrice = GetTotalPrice();
            var data = new {    result = result, 
                                value =  getCart.QUANTITY, 
                                idpro = idpro, 
                                totalQuant = totalQuantity, 
                                totalPrice = getTotalPrice  };
            return Json(data);
        }
    }
}