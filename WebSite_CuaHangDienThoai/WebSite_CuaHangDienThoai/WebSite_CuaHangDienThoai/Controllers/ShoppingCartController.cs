using System;
using System.Collections.Generic;
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
                                        ProductId = (int)cartItem.ProductDetailId,
                                        ProductName = cartItem.tb_ProductDetail.tb_Products.Title.ToString(),
                                        CategoryName = cartItem.tb_ProductDetail.tb_Products.tb_ProductCategory.Title.ToString(),
                                        Alias = cartItem.tb_ProductDetail.tb_Products.Alias.ToString(),
                                        SoLuong = cartItem.Quantity,
                                        Capcity= (int)checkSanPham.Capacity,
                                        Color= checkSanPham.Color
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