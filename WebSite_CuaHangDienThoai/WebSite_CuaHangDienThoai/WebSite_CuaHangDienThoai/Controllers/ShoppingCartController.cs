using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Client;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   
        public ActionResult Index()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];

                var checkIdCart = db.tb_Cart.SingleOrDefault(x => x.CustomerId == idKhach);

                if (checkIdCart != null)
                {
                    int checkId = checkIdCart.CartId;

                    var cartItem = db.tb_CartItem.Where(row => row.CartId == checkId);
                    if (cartItem != null && cartItem.Any())
                    {
                        ViewBag.Cart = cartItem;
                    }

                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public ActionResult Partial_ItemCart()
        {

            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                //tb_Cart sessCart = (tb_Cart)idKhach;
                var checkIdCart = db.tb_Cart.FirstOrDefault(x => x.CustomerId == idKhach);
                if (checkIdCart != null)
                {
                    int checkId = checkIdCart.CartId;

                    var cartItem = db.tb_CartItem.OrderByDescending(row => row.CartId == checkId);
                    return PartialView(cartItem);
                }
                else
                {

                }

            }
            return PartialView();
        }



        public ActionResult AddtoCart()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("index");

        }

        [HttpPost]
        public ActionResult AddtoCart(int id, int soluong)
        {
            var code = new { Success = false, msg = "", code = -1 };

            if (Session["CustomerId"] != null)
            {

                int idKhach = (int)Session["CustomerId"];

                var checkIdCart = db.tb_Cart.SingleOrDefault(x => x.CustomerId == idKhach);

                if (checkIdCart != null)
                {
                    
                    int checkId = checkIdCart.CartId;
                    var checkIdCartItem = db.tb_CartItem.SingleOrDefault(ci => ci.CartId == checkId && ci.ProductDetailId == id);

                    if (checkIdCartItem != null)
                    {
                        checkIdCartItem.Quantity += soluong;
                        checkIdCartItem.TemPrice = checkIdCartItem.Price * checkIdCartItem.Quantity;
                        db.SaveChanges();
                    }
                    else
                    {
                        var productDetail = db.tb_ProductDetail.Find(id);
                        if (productDetail != null)
                        {
                            tb_CartItem cartitem = new tb_CartItem
                            {
                                CartId = checkId,
                                ProductDetailId = productDetail.ProductDetailId,
                                Quantity = soluong,
                                Price = (decimal)productDetail.Price,
                                TemPrice = (decimal)productDetail.Price * soluong,
                                PriceTotal = (decimal)productDetail.Price
                            };
                            db.tb_CartItem.Add(cartitem);
                            db.SaveChanges();
                        }
                        else
                        {
                            code = new { Success = false, msg = "Sản phẩm không tồn tại!", code = -1 };
                            return Json(code);
                        }
                    }

                    code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!", code = 1 };

                }
                else
                {
                    return PartialView("Cartnull");
                }

            }
            else
            {
                code = new { Success = false, msg = "", code = -2 };
            }
            return Json(code);

        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, Message = "", Code = -1 };

            try
            {

                int? idKhach = Session["CustomerId"] as int?;
                if (idKhach == null)
                {
                    code = new { Success = false, Message = "Không có phiên làm việc (session) cho khách hàng", Code = -1 };
                    return Json(code);
                }


                var cart = db.tb_Cart.FirstOrDefault(x => x.CustomerId == idKhach);
                if (cart == null)
                {
                    code = new { Success = false, Message = "Không tìm thấy giỏ hàng", Code = -2 };
                    return Json(code);
                }


                var cartItem = db.tb_CartItem.FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductDetailId == id);
                if (cartItem == null)
                {
                    code = new { Success = false, Message = "Sản phẩm không tồn tại trong giỏ hàng", Code = -3 };
                    return Json(code);
                }


                db.tb_CartItem.Remove(cartItem);
                db.SaveChanges();

                code = new { Success = true, Message = "Xóa sản phẩm thành công", Code = 1 };
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                code = new { Success = false, Message = "Lỗi xóa sản phẩm: " + ex.Message, Code = -1 };
            }

            return Json(code);
        }



        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1 };

            try
            {
                if (Session["CustomerId"] != null)
                {
                    int idKhach = (int)Session["CustomerId"];

                    var checkIdCart = db.tb_Cart.FirstOrDefault(x => x.CustomerId == idKhach);

                    if (checkIdCart != null)
                    {
                        int checkId = checkIdCart.CartId;

                        var checkIdCartItem = db.tb_CartItem.FirstOrDefault(ci => ci.CartId == checkId && ci.ProductDetailId == id);

                        if (checkIdCartItem != null)
                        {
                            checkIdCartItem.Quantity = quantity;
                            db.SaveChanges();

                            code = new { Success = true, msg = "ok", code = 1 };
                        }
                        else
                        {
                            code = new { Success = false, msg = "Sản phẩm không tồn tại trong giỏ hàng", code = 0 };
                        }
                    }
                    else
                    {
                        code = new { Success = false, msg = "Không tìm thấy giỏ hàng", code = -1 };
                    }
                }
                else
                {
                    code = new { Success = false, msg = "Không có phiên làm việc (session) cho khách hàng", code = -1 };
                }
            }
            catch (Exception ex)
            {
                code = new { Success = false, msg = "Lỗi cập nhật số lượng sản phẩm: " + ex.Message, code = -1 };
            }

            return Json(code);
        }


        [HttpPost]
        public ActionResult DatHang(List<int> productIds)
        {
            var code = new { Success = false, msg = "", code = -1 };
            if (productIds != null && productIds.Any())
            {
                if (Session["CustomerId"] != null)
                {
                    int idKhach = (int)Session["CustomerId"];
                    var checkIdCart = db.tb_Cart.SingleOrDefault(x => x.CustomerId == idKhach);
                    if (checkIdCart != null)
                    {
                        int checkId = checkIdCart.CartId;

                        ShoppingCart cart = (ShoppingCart)Session[""];
                        if (cart == null)
                        {
                            cart = new ShoppingCart();
                        }

                        foreach (var productId in productIds)
                        {
                            var cartItem = db.tb_CartItem.SingleOrDefault(row => row.CartId == checkId && row.ProductDetailId == productId);
                            if (cartItem != null)
                            {
                                var checkSanPham = db.tb_ProductDetail.FirstOrDefault(row => row.ProductDetailId == productId);
                                if (checkSanPham != null)
                                {
                                    ShoppingCartItem item = new ShoppingCartItem
                                    {
                                        ProductDetailId = (int)cartItem.ProductDetailId,
                                        ProductName = cartItem.tb_ProductDetail.tb_Products.Title.ToString(),
                                        CategoryName = cartItem.tb_ProductDetail.tb_Products.tb_ProductCategory.Title.ToString(),
                                        Alias = cartItem.tb_ProductDetail.tb_Products.Alias.ToString(),
                                        SoLuong = cartItem.Quantity,
                                        Capcity = (int)checkSanPham.Capacity,
                                        Color = checkSanPham.Color
                                    };

                                    if (cartItem.tb_ProductDetail.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                                    {
                                        item.ProductImg = cartItem.tb_ProductDetail.tb_ProductDetailImage.FirstOrDefault(row => row.IsDefault).Image;
                                    }

                                    item.Price = (decimal)checkSanPham.Price;
                                    if (checkSanPham.PriceSale > 0)
                                    {
                                        item.Price = (decimal)checkSanPham.PriceSale;
                                    }
                                    item.PriceTotal = item.SoLuong * item.Price;
                                    cart.AddToCart(item, cartItem.Quantity);
                                }
                            }
                        }

                        Session["Cart"] = cart;
                        code = new
                        {
                            Success = true,
                            msg = "",
                            code = 1
                        };
                        //return RedirectToAction("CheckOut");
                    }
                }
            }
            else
            {
                code = new { Success = false, msg = "", code = -2 };
            }
            return Json(code);
        }

        public ActionResult Partial_CheckOut()
        {
            return PartialView();
        }
        public ActionResult Partial_ThongTinKhach()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var khachHang = db.tb_Customer.Find(idKhach);
                return PartialView(khachHang);
            }
            return PartialView();
        }

        public ActionResult Partial_ChiTietSanPhamMua()
        {

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                int count = cart.Items.Count;
                ViewBag.Count = count;
                return PartialView(cart.Items);
            }

            return PartialView();
        }


        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.Cart = cart;
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewVNPay req, tb_Products model)
        {

            try
            {

                var code = new { Success = false, Code = -1, Url = "" };
                if (ModelState.IsValid)
                {

                    if (Session["CustomerId"] != null)
                    {
                        int idKhach = (int)Session["CustomerId"];
                        var inforKhachHang = db.tb_Customer.FirstOrDefault(x => x.CustomerId == idKhach);
                        ShoppingCart cart = (ShoppingCart)Session["Cart"];
                        if (cart != null)
                        {

                            bool isTransactionSuccessful = false;///goc
                  
                          


                            // dang chua xuat so luong
                            //foreach (var item in cart.Items)
                            //{
                            //    var checkQuantityPro = db.tb_ProductDetail.Find(item.ProductDetailId);

                            //    if (checkQuantityPro != null)
                            //    {
                            //        if (checkQuantityPro.Quan >= item.SoLuong)
                            //        {
                            //            checkQuantityPro.Quantity -= item.SoLuong;

                            //            DeleteCartSucces(idKhach, item.ProductDetailId);

                            //            db.Entry(checkQuantityPro).State = System.Data.Entity.EntityState.Modified;
                            //            db.SaveChanges();
                            //            isTransactionSuccessful = true;
                            //        }
                            //        else
                            //        {
                            //            ViewBag.error = "Số lượng sản phẩm không đủ";
                            //            code = new { Success = false, Code = -7, Url = "" };//Số lượng sản phẩm hiện không đủ 
                            //        }
                            //    }
                            //}


                            if (isTransactionSuccessful)
                            {
                                tb_Order order = new tb_Order();
                                order.CustomerName = inforKhachHang.CustomerName;
                                order.Phone = inforKhachHang.PhoneNumber;
                                order.Address = inforKhachHang.Loaction;
                                order.Email = inforKhachHang.Email;
                                order.typeOrder = false;





                                cart.Items.ForEach(row => order.tb_OrderDetail.Add(new tb_OrderDetail
                                {

                                    ProductDetailId = row.ProductDetailId,
                                    Quantity = row.SoLuong,
                                    Price = row.Price,
                                }));
                                order.TotalAmount = cart.Items.Sum(x => (x.Price * x.SoLuong));
                                order.TypePayment = req.TypePayment;
                                //inforKhachHang.TypePayment = req.TypePayment;   

                                order.CreatedDate = DateTime.Now;
                                order.ModifiedDate = DateTime.Now;
                                order.Confirm = false;
                                order.typeOrder = false;
                                order.Status = null;
                                order.typeReturn = false;
                                order.Success = false;
                                Random ran = new Random();
                                order.Code = "DH" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);


                                //var checkSanPham=db.tb_Products.SingleOrDefault(row=>row.ProductId==cart.Imm)




                                db.tb_Order.Add(order);
                                //db.tb_KhachHang.Add(inforKhachHang);
                                db.SaveChanges();

                                var SanPham = "";
                                var thanhTien = decimal.Zero;
                                var tongTien = decimal.Zero;
                                foreach (var item in cart.Items)
                                {
                                    SanPham += "<tr>";
                                    SanPham += "<td>" + item.ProductName + "</td>";
                                    SanPham += "<td>" + item.SoLuong + "</td>";
                                    SanPham += "<td>" + WebSite_CuaHangDienThoai.Common.Common.FormatNumber(item.PriceTotal, 0) + "</td>";
                                    SanPham += "</tr>";
                                    thanhTien += item.Price * item.SoLuong;
                                }
                                tongTien = thanhTien;
                                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                                contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                                contentCustomer = contentCustomer.Replace("{{SanPham}}", SanPham);
                                contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                                contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                                contentCustomer = contentCustomer.Replace("{{Email}}", inforKhachHang.Email);
                                contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                                contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(thanhTien, 0));
                                contentCustomer = contentCustomer.Replace("{{TongTien}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(tongTien, 0));
                                WebSite_CuaHangDienThoai.Common.Common.SendMail("ShopOnline", "Đơn hàng #" + order.Code, contentCustomer.ToString(), inforKhachHang.Email);

                                string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                                contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                                contentAdmin = contentAdmin.Replace("{{SanPham}}", SanPham);
                                contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                                contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                                contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                                contentAdmin = contentAdmin.Replace("{{Email}}", inforKhachHang.Email);
                                contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                                contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(thanhTien, 0));
                                contentAdmin = contentAdmin.Replace("{{TongTien}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(tongTien, 0));
                                WebSite_CuaHangDienThoai.Common.Common.SendMail("ShopOnline", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                                cart.ClearCart();
                                code = new { Success = true, Code = req.TypePayment, Url = "" };
                                //if (req.TypePayment == 2)
                                //{
                                //    var url = UrlPayment(req.TypePaymentVN, order.Code);
                                //    code = new { Success = true, Code = req.TypePayment, Url = url };
                                //}
                            }
                            else
                            {
                                code = new { Success = false, Code = -5, Url = "" };//Số lượng sản phẩm hiện không đủ 

                            }


                        }
                    }
                }

                return Json(code);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Cartnull");
            }
        }





        public ActionResult ShowCount()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];

                var checkIdCart = db.tb_Cart.SingleOrDefault(x => x.CustomerId == idKhach);

                if (checkIdCart != null)
                {
                    int checkId = checkIdCart.CartId;

                    var cartItem = db.tb_CartItem.Count(row => row.CartId == checkId);
                    return Json(new { Count = cartItem }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }




    }
}