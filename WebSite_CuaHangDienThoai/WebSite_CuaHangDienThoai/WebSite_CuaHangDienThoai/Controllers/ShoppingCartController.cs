using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Client;
using WebSite_CuaHangDienThoai.Models.Payment;
using Newtonsoft.Json;
using WebSite_CuaHangDienThoai.Models.Token.Admin;
using ImageProcessor.Processors;
using DynamicData.Kernel;
using System.Data.Entity;
using System.Data.SqlClient;
using DynamicData;
using System.Transactions;

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

                var checkIdCart = db.tb_Cart.FirstOrDefault(x => x.CustomerId == idKhach);

                if (checkIdCart != null)
                {
                    int checkId = checkIdCart.CartId;

                    var item = db.tb_Cart.Find(checkId);
                    ViewBag.item = item.CustomerId;

                    return View(item);

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
                var checkCart = db.tb_Cart.FirstOrDefault(x => x.CartId == idKhach);
                if (checkCart != null)
                {
                    var cartItem = db.tb_CartItem.Where(row => row.CartId == checkCart.CartId).OrderByDescending(x => x.CartItem).ToList();
                    if (cartItem != null)
                    {
                        ViewBag.Count = cartItem.Count;
                        return PartialView(cartItem);
                    }
                    else
                    {
                        return PartialView();
                    }
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

                var checkIdCart = db.tb_Cart.FirstOrDefault(x => x.CustomerId == idKhach);

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
                                TemPrice = (decimal)productDetail.PriceSale,
                                PriceTotal = (decimal)productDetail.Price * soluong
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
                                    item.PriceSale = (decimal)checkSanPham.PriceSale;
                                    if (checkSanPham.PriceSale > 0)
                                    {
                                        item.PriceSale = (decimal)checkSanPham.PriceSale;
                                        item.PriceTotal = item.SoLuong * item.PriceSale;
                                    }
                                    else
                                    {
                                        item.PriceTotal = item.SoLuong * item.Price;
                                    }


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


        public static int Oderid;


        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.Cart = cart;
            }

            return View();
        }
        public ActionResult Partial_CheckOut()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewVNPay req, tb_Products model)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                var code = new { Success = false, Code = -1, Url = "" };
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (Session["CustomerId"] != null)
                        {
                            int idKhach = (int)Session["CustomerId"];
                            var inforKhachHang = db.tb_Customer.FirstOrDefault(x => x.CustomerId == idKhach);
                            ShoppingCart cart = (ShoppingCart)Session["Cart"];

                            if (cart != null)
                            {
                                var insufficientItems = ProcessCartItems(cart, idKhach);
                                if (insufficientItems.Any())
                                {
                                    return Json(new { Success = false, Code = -2, InsufficientItems = insufficientItems });
                                }

                                var order = CreateOrder(cart, inforKhachHang, req.TypePayment);

                                if (order == null)
                                {
                                    return Json(new { Success = false, Code = -5, Url = "" });
                                }
                                

                                db.tb_Order.Add(order);
                                db.SaveChanges();
                                int OrderId = order.OrderId;
                                inforKhachHang.NumberofPurchases += 1;
                                db.Entry(inforKhachHang).State = EntityState.Modified;
                                db.SaveChanges();


                                foreach (var item in cart.Items)
                                {
                                    if (item.PercentPriceReduction.HasValue && item.PercentPriceReduction > 0)
                                    {
                                        if (!UpdateVoucherDetail(item.CodeVoucher, order))
                                        {
                                            return Json(new { Success = false, Code = -7, Url = "" }); // Áp dụng voucher thất bại
                                        }
                                    }
                                }





                                if (req.TypePayment == 2)
                                {
                                    dbContextTransaction.Commit();
                                    var url = UrlPayment(req.TypePaymentVN, order.Code);
                                    code = new { Success = true, Code = req.TypePayment, Url = url };
                                }
                                else
                                {
                                    SendConfirmationEmails(cart, order, inforKhachHang);


                                    cart.ClearCart();

                                    dbContextTransaction.Commit();
                                   





                                    code = new { Success = true, Code = 1, Url = "" };
                                }
                            }
                            else
                            {
                                code = new { Success = false, Code = -6, Url = "" };
                            }
                        }
                        else
                        {
                            code = new { Success = false, Code = -3, Url = "" };
                        }
                    }
                    else
                    {
                        code = new { Success = false, Code = -4, Url = "" };
                    }

                    return Json(code);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    code = new { Success = false, Code = -100, Url = "" };
                    return Json(code);
                }
            }
        }
        private bool UpdateVoucherDetail(string codeVoucher, tb_Order order)
        {
            try
            {
                var voucherDetail = db.tb_VoucherDetail.FirstOrDefault(x => x.Code == codeVoucher && x.Status == false);
                if (voucherDetail != null)
                {
                    var existingOrder = db.tb_Order.FirstOrDefault(o => o.OrderId == order.OrderId);
                    if (existingOrder != null)
                    {

                        if (voucherDetail.VoucherId.HasValue) 
                        {
                            var existingVoucher = db.tb_Voucher.FirstOrDefault(v => v.VoucherId == voucherDetail.VoucherId);
                            if (existingVoucher != null)
                            {
                                voucherDetail.OrderId = order.OrderId;
                                voucherDetail.UsedDate = DateTime.Now;
                                voucherDetail.Status = true;
                                db.SaveChanges();
                                return true; // Cập nhật voucher thành công
                            }
                            else
                            {
                                return false; // Không tìm thấy voucher tương ứng
                            }
                        }
                        else
                        {
                            return false; // Không tìm thấy đơn hàng
                        }
                    }
                    else
                    {
                        return false; // Không tìm thấy đơn hàng
                    }
                }
                else
                {
                    UpdateWarehouseQuantity(order);
                    return false; // Không tìm thấy voucher hoặc đã được sử dụng
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu cần
                return false; // Có lỗi khi cập nhật voucher
            }
        }






        //private void updatevoucherdetail(string code, int orderid, tb_customer customer)
        //{
        //    // tìm kiếm chi tiết voucher dựa trên mã code
        //    var voucherdetail = db.tb_voucherdetail.firstordefault(v => v.code == code);

        //    if (voucherdetail != null)
        //    {
        //        if (voucherdetail.tb_voucher == null)
        //        {
        //            console.writeline("voucher detail not found or already used.");
        //        }
        //        else 
        //        {
        //            voucherdetail.orderid = orderid;
        //            voucherdetail.usedby = customer.customername; // sử dụng tên khách hàng
        //            voucherdetail.useddate = datetime.now; // thời gian sử dụng voucher
        //            voucherdetail.status = true; // đánh dấu voucher đã được sử dụng

        //            // lưu thay đổi vào cơ sở dữ liệu
        //            db.savechanges();
        //        }
        //        // cập nhật các trường thích hợp

        //    }
        //}

        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        public ActionResult Cartnull()
        {
            return View();
        }
        #region/* Thanh toán vnpay*/
        //public string UrlPayment(int TypePaymentVN, string orderCode)
        //{
        //    var urlPayment = "";
        //    var order = db.tb_Order.FirstOrDefault(x => x.Code == orderCode);
        //    //Get Config Info
        //    string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
        //    string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
        //    string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
        //    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

        //    //Build URL for VNPAY
        //    VnPayLibrary vnpay = new VnPayLibrary();
        //    var Price = (long)order.TotalAmount * 100;
        //    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        //    vnpay.AddRequestData("vnp_Command", "pay");
        //    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        //    vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
        //    if (TypePaymentVN == 1)
        //    {
        //        vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
        //    }
        //    else if (TypePaymentVN == 2)
        //    {
        //        vnpay.AddRequestData("vnp_BankCode", "VNBANK");
        //    }
        //    else if (TypePaymentVN == 3)
        //    {
        //        vnpay.AddRequestData("vnp_BankCode", "INTCARD");
        //    }

        //    vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
        //    vnpay.AddRequestData("vnp_CurrCode", "VND");
        //    vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
        //    vnpay.AddRequestData("vnp_Locale", "vn");
        //    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
        //    vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

        //    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        //    vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

        //    //Add Params of 2.1.0 Version
        //    //Billing

        //    urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        //    //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
        //    return urlPayment;
        //}





        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.tb_Order.FirstOrDefault(x => x.Code == orderCode);

            if (order == null) // Kiểm tra xem đơn hàng có tồn tại không
            {
                return ""; // Trả về chuỗi rỗng nếu không tìm thấy đơn hàng
            }

            // Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; // URL nhận kết quả trả về 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; // URL thanh toán của VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; // Mã định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; // Secret Key

            // Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString());
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code);

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return urlPayment;
        }

        #endregion

        public ActionResult VnpayReturn()
        {
            try
            {
                if (Request.QueryString.Count > 0)
                {
                    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; // Chuỗi bí mật
                    var vnpayData = Request.QueryString;
                    VnPayLibrary vnpay = new VnPayLibrary();

                    foreach (string s in vnpayData)
                    {
                        // Lấy tất cả dữ liệu truy vấn querystring
                        if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(s, vnpayData[s]);
                        }
                    }

                    string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                    string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                    string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                    String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                    String TerminalID = Request.QueryString["vnp_TmnCode"];
                    long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                    String bankCode = Request.QueryString["vnp_BankCode"];

                    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                    if (checkSignature)
                    {
                        var itemOrder = db.tb_Order.FirstOrDefault(x => x.Code == orderCode);
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            if (itemOrder != null)
                            {
                                itemOrder.TypePayment = 2; // Đã thanh toán
                                db.tb_Order.Attach(itemOrder);
                                db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                            }
                            ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        }
                        else
                        {
                            if (itemOrder != null)
                            {
                                UpdateWarehouseQuantity(itemOrder);
                                return RedirectToAction("CheckOutFailVnpay", "ShoppingCart");
                            }
                            ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                        }
                        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND): " + vnp_Amount.ToString();
                        return View();
                    }
                }
                return RedirectToAction("Index", "Error"); // Redirect đến trang lỗi nếu không có query string hoặc xác thực không thành công
            }
            catch (Exception ex)
            {
                // Redirect đến trang lỗi nếu có ngoại lệ xảy ra
                return RedirectToAction("Index", "Error");
            }
        }






        // Hàm cập nhật số lượng sản phẩm trong kho
        private void UpdateWarehouseQuantity(tb_Order itemOrder)
        {
            var orderDetails = db.tb_OrderDetail.Where(od => od.OrderId == itemOrder.OrderId).ToList();

            foreach (var orderDetail in orderDetails)
            {
                // Lấy thông tin sản phẩm chi tiết từ đơn hàng
                int productDetailId = (int)orderDetail.ProductDetailId;
                int quantity = orderDetail.Quantity;

                // Cập nhật số lượng sản phẩm trong kho
                var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == productDetailId);
                if (warehouseDetail != null)
                {
                    // Trả lại số lượng sản phẩm vào kho
                    warehouseDetail.QuanTity += quantity;
                    db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // Xử lý khi không tìm thấy sản phẩm trong kho (nếu cần)
                }
            }
            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            RestoreCartItems(itemOrder);

            DeleteOrderAndOrderDetails(itemOrder);


        }

        private void RestoreCartItems(tb_Order itemOrder)
        {
            var orderDetails = db.tb_OrderDetail.Where(od => od.OrderId == itemOrder.OrderId).ToList();
            int customerId = (int)itemOrder.CustomerId;


            var cart = db.tb_Cart.FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null)
            {
                // Tạo mới giỏ hàng nếu chưa tồn tại
                cart = new tb_Cart { CustomerId = customerId };
                db.tb_Cart.Add(cart);
                db.SaveChanges();
            }
            foreach (var orderDetail in orderDetails)
            {
                var cartItem = new tb_CartItem
                {
                    CartId = cart.CartId,
                    ProductDetailId = (int)orderDetail.ProductDetailId,
                    Quantity = orderDetail.Quantity,
                    Price = orderDetail.Price,
                    PriceTotal = orderDetail.Price * orderDetail.Quantity,
                    TemPrice = orderDetail.Price // Giá tạm thời nếu cần thiết
                };

                db.tb_CartItem.Add(cartItem);
            }
            db.SaveChanges();
        }
        private void DeleteOrderAndOrderDetails(tb_Order itemOrder)
        {
            var orderDetails = db.tb_OrderDetail.Where(od => od.OrderId == itemOrder.OrderId).ToList();
            foreach (var orderDetail in orderDetails)
            {
                db.tb_OrderDetail.Remove(orderDetail);
            }
            db.tb_Order.Remove(itemOrder);
            db.SaveChanges();
        }

        private List<ShoppingCartItem> ProcessCartItems(ShoppingCart cart, int customerId)
        {
            List<ShoppingCartItem> insufficientItems = new List<ShoppingCartItem>();
            List<int> rollbackQuantities = new List<int>();
            List<ShoppingCartItem> removedItems = new List<ShoppingCartItem>();

            try
            {
                var itemsCopy = new List<ShoppingCartItem>(cart.Items);

                foreach (var item in itemsCopy)
                {
                    var warehouseDetail = db.tb_WarehouseDetail.SingleOrDefault(w => w.ProductDetailId == item.ProductDetailId);

                    if (warehouseDetail != null && warehouseDetail.QuanTity >= item.SoLuong)
                    {
                        rollbackQuantities.Add((int)warehouseDetail.QuanTity);

                        warehouseDetail.QuanTity -= item.SoLuong;
                        db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;

                        DeleteCartSucces(customerId, item.ProductDetailId);

                        removedItems.Add(item);
                    }
                    else
                    {
                        insufficientItems.Add(item);
                    }
                }

                // Kiểm tra nếu có sản phẩm không đủ hàng
                if (insufficientItems.Any())
                {
                    // Rollback lại số lượng sản phẩm đã giảm
                    foreach (var removedItem in removedItems)
                    {
                        var originalQuantity = rollbackQuantities[removedItems.IndexOf(removedItem)];
                        var currentWarehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == removedItem.ProductDetailId);
                        if (currentWarehouseDetail != null)
                        {
                            currentWarehouseDetail.QuanTity += removedItem.SoLuong;
                            db.Entry(currentWarehouseDetail).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                }
                else
                {
                    // Nếu tất cả sản phẩm đều đủ hàng, lưu thay đổi và commit giao dịch
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                // Rollback lại số lượng sản phẩm đã giảm
                foreach (var removedItem in removedItems)
                {
                    var originalQuantity = rollbackQuantities[removedItems.IndexOf(removedItem)];
                    var currentWarehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == removedItem.ProductDetailId);
                    if (currentWarehouseDetail != null)
                    {
                        currentWarehouseDetail.QuanTity += removedItem.SoLuong;
                        db.Entry(currentWarehouseDetail).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }

            return insufficientItems;
        }

        private tb_Order CreateOrder(ShoppingCart cart, tb_Customer customerInfo, int typePayment)
        {
            var order = new tb_Order
            {
                CustomerName = customerInfo.CustomerName,
                Phone = customerInfo.PhoneNumber,
                Address = customerInfo.Loaction,
                Email = customerInfo.Email,
                typeOrder = false,
                TypePayment = typePayment,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Confirm = false,
                Status = null,
                typeReturn = false,
                Success = false,
                Code = GenerateOrderCode(),
                CustomerId = customerInfo.CustomerId,

            };
            decimal totalAmount = 0;
            int totalQuantity = 0;
            foreach (var item in cart.Items)
            {

                if (item.PercentPriceReduction.HasValue && item.PercentPriceReduction.Value > 0 && item.CodeVoucher != null)
                {
                    totalAmount += item.PriceTotal;

                }
                else
                {
                    totalAmount += item.PriceTotal;
                }
                totalQuantity += item.SoLuong;
            }


            order.TotalAmount = totalAmount;
            order.Quantity = totalQuantity;

            // Thêm các chi tiết đơn hàng
            cart.Items.ForEach(row => order.tb_OrderDetail.Add(new tb_OrderDetail
            {
                ProductDetailId = row.ProductDetailId,
                Quantity = row.SoLuong,
                Price = row.PriceTotal
            }));

            return order;
        }

      




        //private void UpdateVoucherDetail(string voucherCode, int voucherId, int orderId, tb_Customer customerInfo)
        //{
        //    try
        //    {
        //        // Truy vấn dữ liệu từ bảng tb_VoucherDetail kèm theo dữ liệu từ bảng tb_Voucherx
        //        var voucherDetail = db.tb_VoucherDetail
        //            .Include(vd => vd.tb_Voucher)
        //            .FirstOrDefault(vd => vd.Code == voucherCode && vd.Status == false && vd.VoucherId == voucherId);

        //        if (voucherDetail != null)
        //        {
        //            // Kiểm tra xem voucherDetail có chứa dữ liệu từ bảng tb_Voucher hay không
        //            if (voucherDetail.tb_Voucher != null)
        //            {
        //                // Cập nhật thông tin cho voucherDetail
        //                voucherDetail.OrderId = orderId;
        //                voucherDetail.Status = true;
        //                voucherDetail.UsedDate = DateTime.Now;
        //                voucherDetail.UsedBy = customerInfo.CustomerName;
        //                voucherDetail.CreatedDate = DateTime.Now;
        //                voucherDetail.VoucherId = voucherId;
        //                // Cập nhật bảng tb_VoucherDetail trong cơ sở dữ liệu
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                Console.WriteLine("Voucher detail not found or already used.");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Voucher detail not found or already used.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        if (ex.InnerException != null)
        //        {
        //            Console.WriteLine(ex.InnerException.Message);
        //        }
        //    }
        //}








        private string GenerateOrderCode()
        {
            Random ran = new Random();
            return "DH" + string.Concat(Enumerable.Range(0, 5).Select(_ => ran.Next(0, 10)));
        }

        private void SendConfirmationEmails(ShoppingCart cart, tb_Order order, tb_Customer customerInfo)
        {
            var itemsTable = GenerateItemsTable(cart);
            var totalAmount = cart.Items.Sum(item => item.Price * item.SoLuong);

            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
            contentCustomer = ReplaceOrderPlaceholders(contentCustomer, order, customerInfo, itemsTable, totalAmount);
            WebSite_CuaHangDienThoai.Common.Common.SendMail("LTDMiniStore", "Đơn hàng #" + order.Code, contentCustomer, customerInfo.Email);

            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
            contentAdmin = ReplaceOrderPlaceholders(contentAdmin, order, customerInfo, itemsTable, totalAmount);
            WebSite_CuaHangDienThoai.Common.Common.SendMail("LTDMiniStore", "Đơn hàng mới #" + order.Code, contentAdmin, ConfigurationManager.AppSettings["EmailAdmin"]);
        }

        private string GenerateItemsTable(ShoppingCart cart)
        {
            var sb = new StringBuilder();
            foreach (var item in cart.Items)
            {
                sb.Append("<tr>")
                    .Append("<td>").Append(item.ProductName).Append("</td>")
                    .Append("<td>").Append(item.SoLuong).Append("</td>")
                    .Append("<td>").Append(WebSite_CuaHangDienThoai.Common.Common.FormatNumber(item.PriceTotal, 0)).Append("</td>")
                    .Append("</tr>");
            }
            return sb.ToString();
        }

        private string ReplaceOrderPlaceholders(string template, tb_Order order, tb_Customer customerInfo, string itemsTable, decimal totalAmount)
        {
            return template
                .Replace("{{MaDon}}", order.Code)
                .Replace("{{SanPham}}", itemsTable)
                .Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"))
                .Replace("{{TenKhachHang}}", order.CustomerName)
                .Replace("{{Phone}}", order.Phone)
                .Replace("{{Email}}", customerInfo.Email)
                .Replace("{{DiaChiNhanHang}}", order.Address)
                .Replace("{{ThanhTien}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(totalAmount, 0))
                .Replace("{{TongTien}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(totalAmount, 0));
        }






        private void DeleteCartSucces(int idKhachHang, int productId)
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var checkCart = db.tb_Cart.FirstOrDefault(x => x.CustomerId == idKhach);
                if (checkCart != null)
                {
                    var checkItemCart = db.tb_CartItem.SingleOrDefault(x => x.CartId == checkCart.CartId && x.ProductDetailId == productId);
                    if (checkItemCart != null)
                    {
                        db.tb_CartItem.Remove(checkItemCart);
                        db.SaveChanges();
                    }
                }
            }
        }
        public ActionResult CheckOutSucces()
        {
            return View();
        }
        public ActionResult CheckOutFailVnpay()
        {
            return View();
        }




        public ActionResult Partial_Voucher() 
        {
            return PartialView();   
        }


        [HttpGet]
        public JsonResult GetVoucher(string Code)
        {
            var voucher = (from chiTiet in db.tb_VoucherDetail
                           join voucherDetail in db.tb_Voucher on chiTiet.VoucherId equals voucherDetail.VoucherId
                           where chiTiet.Code.Trim() == Code.Trim()
                           select new
                           {
                               voucherDetail.PercentPriceReduction, // PhanTramGiam
                               voucherDetail.Title,
                               voucherDetail.CreatedBy,
                               voucherDetail.CreatedDate,
                               voucherDetail.ModifiedDate,
                               voucherDetail.UsedBy,
                               voucherDetail.UsedDate,
                               voucherDetail.Quantity,
                               chiTiet.Status
                           }).ToList();
          


            return Json(voucher, JsonRequestBehavior.AllowGet);
        }




        

        //ShoppingCartList 

        //cap nhạt lai tong tien khi cí voucher


        public ActionResult Partial_TotalPriceCheckOut()
        {

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            decimal totalPrice = 0;
            decimal save = 0;
           

            if (cart != null && cart.Items.Any())
            {
                foreach (var item in cart.Items) {
                    if (item.OriginalPriceTotal.HasValue)
                    {
                        save += (decimal)item.OriginalPriceTotal.Value-(decimal)item.PriceTotal ;
                        ViewBag.Save = save;
                    }
                }
               


                totalPrice = cart.GetPriceTotal();
            }

            ViewBag.TotalPrice = totalPrice;
           
            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateTotalPriceShoppingCartItem(int PercentPriceReduction ,string Code  )
        {
            var code = new { Success = false, Code = -1, Url = "" };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                foreach (var item in cart.Items)
                {
                    // Lưu giá trị gốc nếu chưa lưu
                    if (!item.OriginalPriceTotal.HasValue)
                    {
                        item.OriginalPriceTotal = item.PriceTotal;
                    }

                    // Áp dụng giảm giá
                    item.PercentPriceReduction = PercentPriceReduction;
                    decimal discountAmount = item.PriceTotal * PercentPriceReduction / 100;
                    item.PriceTotal -= discountAmount;
                    item.CodeVoucher = Code;
                }

                Session["Cart"] = cart; // Cập nhật lại session
                code = new { Success = true, Code = 1, Url = "" };
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult RemoveVoucher()
        {
            var code = new { Success = false, Code = -1, Url = "" };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                foreach (var item in cart.Items)
                {
                    if (item.OriginalPriceTotal.HasValue)
                    {
                        item.PriceTotal = item.OriginalPriceTotal.Value;
                        item.OriginalPriceTotal = null;
                        item.PercentPriceReduction = null;
                    }
                }

                Session["Cart"] = cart; // Cập nhật lại session
                code = new { Success = true, Code = 1, Url = "" };
            }
            return Json(code);
        }

        [HttpPost]
            public ActionResult DeleteCartItem(int id)
            {
                var code = new { Success = false, Code = -1, Url = "" };
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
               
                    cart.Remove(id);
                    Session["Cart"] = cart; // Cập nhật lại session
                    code = new { Success = true, Code = 1, Url = "" };
                }
                return Json(code);
            }


        [HttpPost]
        public ActionResult UpdateQuantityCartItem(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpSoLuong(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
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