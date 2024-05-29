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
                    ViewBag.item = item;
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
                                //code = new { Success = false, Code = -2, Url = "" }; // Mã lỗi cho số lượng không đủ
                                return Json(new { Success = false, Code = -2, InsufficientItems = insufficientItems });
                            }

                            var order = CreateOrder(cart, inforKhachHang, req.TypePayment);

                            if (order == null)
                            {
                                code = new { Success = false, Code = -5, Url = "" }; // Mã lỗi chung
                                return Json(code);
                            }

                            db.tb_Order.Add(order);
                            db.SaveChanges();
                            int Oder = order.OrderId;

                            foreach (var item in cart.Items)
                            {

                               
                                //UpdateVoucherDetail(item.Code, Oder, inforKhachHang);
                            }



                            SendConfirmationEmails(cart, order, inforKhachHang);
                            cart.ClearCart();

                            code = new { Success = true, Code = 1, Url = "" };
                            if (req.TypePayment == 2)
                            {
                                var url = UrlPayment(req.TypePaymentVN, order.Code);
                                code = new { Success = true, Code = req.TypePayment, Url = url };
                            }
                        }
                        else
                        {
                            code = new { Success = false, Code = -6, Url = "" }; // Mã lỗi cho giỏ hàng null
                        }
                    }
                    else
                    {
                        code = new { Success = false, Code = -3, Url = "" }; // Mã lỗi cho session khách hàng null
                    }
                }
                else
                {
                    code = new { Success = false, Code = -4, Url = "" }; // Mã lỗi cho model state không hợp lệ
                }

                return Json(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message); // Lấy chi tiết từ ngoại lệ bên trong
                code = new { Success = false, Code = -10, Url = "" };
                return Json(code);

            }
        }
     

        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        public ActionResult Cartnull()
        {
            return View();
        }
        #region/* Thanh toán vnpay*/
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.tb_Order.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
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
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion

        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }


                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.tb_Order.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.TypePayment = 2;//đã thanh toán

                            db.tb_Order.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    }
                    else
                    {
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                    }
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                }

            }
            //var a = UrlPayment(0, "DH64050");
            return View();
        }


       




        private List<ShoppingCartItem> ProcessCartItems(ShoppingCart cart, int customerId)
        {
            List<ShoppingCartItem> insufficientItems = new List<ShoppingCartItem>();

            foreach (var item in cart.Items)
            {
                var warehouseDetail = db.tb_WarehouseDetail.Find(item.ProductDetailId);
                if (warehouseDetail != null && warehouseDetail.QuanTity >= item.SoLuong)
                {
                    warehouseDetail.QuanTity -= item.SoLuong;
                    DeleteCartSucces(customerId, item.ProductDetailId);
                    db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    // Nếu số lượng không đủ, thêm sản phẩm vào danh sách không đủ số lượng
                    insufficientItems.Add(item);
                }
            }

            return insufficientItems; // Trả về danh sách sản phẩm không đủ số lượng
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
               
                if (item.PercentPriceReduction.HasValue && item.PercentPriceReduction.Value > 0)
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



        //Hàm kiểm tra và áp dụng mã giảm giá từ voucher
        //  private void UpdateVoucherDetail(string voucherCode, int orderId, tb_Customer customerInfo)
        //  {
        //      try
        //      {
        //          // Truy vấn dữ liệu từ bảng tb_VoucherDetail kèm theo dữ liệu từ bảng tb_Voucher
        //          var voucherDetail = db.tb_VoucherDetail
        //.Include(vd => vd.tb_Voucher)
        //.FirstOrDefault(vd => vd.Code == voucherCode && vd.Status == false);

        //          if (voucherDetail != null)
        //          {
        //              // Kiểm tra xem voucherDetail có chứa dữ liệu từ bảng tb_Voucher hay không

        //                  // Cập nhật thông tin cho voucherDetail
        //                  voucherDetail.OrderId = orderId;
        //                  voucherDetail.Status = true;
        //                  voucherDetail.UsedDate = DateTime.Now;
        //                  voucherDetail.UsedBy = customerInfo.CustomerName;
        //                  voucherDetail.CreatedDate = DateTime.Now;

        //                  // Cập nhật bảng tb_VoucherDetail trong cơ sở dữ liệu
        //                  db.Entry(voucherDetail).State = System.Data.Entity.EntityState.Modified;
        //                  db.SaveChanges();

        //          }
        //          else
        //          {
        //              Console.WriteLine("Voucher detail not found or already used.");
        //          }
        //      }
        //      catch (Exception ex)
        //      {
        //          Console.WriteLine(ex.Message);
        //          if (ex.InnerException != null)
        //          {
        //              Console.WriteLine(ex.InnerException.Message);
        //          }
        //      }
        //  }


 




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
                               voucherDetail.Quantity
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
                    item.Code = Code;
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