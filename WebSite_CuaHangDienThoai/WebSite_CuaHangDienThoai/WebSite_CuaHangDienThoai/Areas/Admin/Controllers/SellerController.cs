using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class SellerController : Controller
    {
        // GET: Admin/Seller
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {


            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId ==4));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    DateTime today = DateTime.Today;
                    DateTime startOfDay = today.Date;
                    DateTime endOfDay = today.Date.AddDays(1).AddTicks(-1);
                    ViewBag.Today = today;
                    return View();
                }
               
            }



        }

        public ActionResult Partail_SellerIndex()
        {
            return PartialView();
        }




        public ActionResult Partail_ProductDetail()
        {
            var item = db.tb_ProductDetail.ToList();
            return PartialView(item);
        }




        [HttpGet]
        public ActionResult Suggest(string searchString)
        {

            if (!string.IsNullOrEmpty(searchString))
            {

                var suggestedProducts = db.tb_Products
                                     .Where(p => p.Title.Contains(searchString))
                                     .Select(x => x.ProductsId)  // Chỉ lấy ID sản phẩm
                                     .ToList();
                if (suggestedProducts != null)
                {
                    var items = db.tb_ProductDetail
                                     .Where(pd => pd.ProductsId.HasValue && suggestedProducts.Contains(pd.ProductsId.Value))
                                     .OrderByDescending(pd => pd.ProductsId)
                                     .Take(5)
                                     .ToList();
                    HttpContext.Items["SuggestedProducts"] = items;
                    ViewBag.NoiDung = searchString;
                    return PartialView(items);
                }
                else
                {
                    return PartialView("_SuggestedProducts", null);
                }
            }
            else
            {
                return PartialView("_SuggestedProducts", null);
            }
        }



        public ActionResult SuggestCustomer(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var customer = db.tb_Customer
              .FirstOrDefault(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search));

                if (customer != null)
                {
                    return PartialView(new List<tb_Customer> { customer });
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {

                return PartialView();
            }
        }

        [HttpPost]
        public JsonResult CheckOutSeller(int CustomerId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Session["user"] == null)
                    {
                        return Json(new { success = false, code = -99, redirectTo = Url.Action("DangNhap", "Account") });
                    }
                    else
                    {
                        tb_Staff nvSession = (tb_Staff)Session["user"];
                        if (CustomerId > 0)
                        {
                            var customer = db.tb_Customer.SingleOrDefault(x => x.CustomerId == CustomerId);
                            if (customer != null)
                            {
                                SellerCart cart = (SellerCart)Session["SellerCart"];
                                if (cart != null)
                                {
                                    foreach (var item in cart.Items)
                                    {
                                        var checkQuantityPro = db.tb_ProductDetail.Find(item.ProductDetailId);
                                        var warehouseDetail = db.tb_WarehouseDetail.SingleOrDefault(w => w.ProductDetailId == item.ProductDetailId);
                                        if (warehouseDetail != null)
                                        {
                                            if (warehouseDetail.QuanTity >= item.SoLuong)
                                            {
                                                warehouseDetail.QuanTity -= item.SoLuong;
                                                db.Entry(checkQuantityPro).State = System.Data.Entity.EntityState.Modified;
                                                db.SaveChanges();



                                            }
                                            else
                                            {
                                                return Json(new { success = false, code = -7 });//Kho không đủ số lượng
                                            }
                                        }
                                        else
                                        {
                                            return Json(new { success = false, code = -7 });//Kho không đủ số lượng
                                        }
                                    }

                                    tb_Seller seller = new tb_Seller();
                                    seller.CustomerId = customer.CustomerId;
                                    seller.Phone = customer.PhoneNumber;
                                    seller.StaffId = nvSession.StaffId;

                                    cart.Items.ForEach(x => seller.tb_SellerDetail.Add(new tb_SellerDetail
                                    {
                                        ProductDetailId = x.ProductDetailId,
                                        Quantity = x.SoLuong,
                                        Price = x.Price,

                                    }));




                                    seller.TotalAmount = cart.GetPriceTotal();  
                                  




                                    //seller.TypePayment = req.TypePayment;
                                    seller.TypePayment = 0;
                                    seller.CreatedDate = DateTime.Now;
                                    seller.ModifiedDate = null;
                                    seller.CreatedBy = nvSession.NameStaff;
                                    Random rd = new Random();
                                    seller.Code = "HD" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);

                                    db.tb_Seller.Add(seller);
                                    db.SaveChanges();


                                    customer.NumberofPurchases += 1;
                                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    cart.ClearCart();
                                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    dbContextTransaction.Commit();


                                    return Json(new { success = true, code = 1 });
                                }
                                else
                                {
                                    return Json(new { success = false, code = -3 });// Không có sản phẩm trong phiếu
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -1 });//Không tìm thấy khách hàng
                            }
                        }
                        else
                        {
                            return Json(new { success = false, code = -1 });//Không tìm thấy khách hàng
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100 });
                }
            }
        }




        [HttpPost]
        public ActionResult AddListProduct(int id, int soluong)
        {
            var code = new { Success = false, Code = -1, Count = 0 };
            try
            {
                var checkSanPham = db.tb_ProductDetail.FirstOrDefault(row => row.ProductDetailId == id);
                if (checkSanPham != null)
                {
                    SellerCart cart = (SellerCart)Session["SellerCart"];
                    if (cart == null)
                    {
                        cart = new SellerCart();
                    }

                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == id);
                    if (existingItem != null)
                    {
                        // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng lên
                        existingItem.SoLuong += soluong;
                        existingItem.PriceTotal = existingItem.SoLuong * existingItem.Price;
                    }
                    else
                    {
                        SellerCartItem item = new SellerCartItem
                        {
                            ProductDetailId = checkSanPham.ProductDetailId,
                            ProductName = checkSanPham.tb_Products.Title,
                            CategoryName = checkSanPham.tb_Products.tb_ProductCategory.Title,
                            Capcity = (int)checkSanPham.Capacity,
                            Color = checkSanPham.Color,
                            SoLuong = 1,
                        };

                        if (checkSanPham.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                        {
                            item.ProductImg = checkSanPham.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault).Image;
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

                        cart.AddToCart(item, soluong);
                    }

                    Session["SellerCart"] = cart;
                    code = new { Success = true, Code = 1, Count = cart.Items.Count };
                }
                else
                {
                    code = new { Success = false, Code = -2, Count = 0 }; // Sản phẩm không tồn tại
                }
            }
            catch (Exception ex)
            {
                code = new { Success = false, Code = -99, Count = 0 }; // Lỗi xảy ra
            }
            return Json(code);
        }














        [HttpPost]
        public ActionResult UpdateQuantityCartItem(int id, int quantity)
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null)
            {
                cart.UpSoLuong(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        [HttpPost]
        public ActionResult DeleteCartItem(int id)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null)
            {

                cart.Remove(id);
                Session["SellerCart"] = cart; // Cập nhật lại session
                code = new { Success = true, Code = 1, Url = "" };
            }
            return Json(code);
        }



        public ActionResult Partial_TotalPriceCheckOut()
        {

            SellerCart cart = (SellerCart)Session["SellerCart"];
            decimal totalPrice = 0;
            decimal save = 0;

            if (cart != null && cart.Items.Any())
            {
                foreach (var item in cart.Items)
                {
                    if (item.OriginalPriceTotal.HasValue)
                    {
                        save += (decimal)item.OriginalPriceTotal.Value - (decimal)item.PriceTotal;
                        ViewBag.Save = save;
                    }
                }



                totalPrice = cart.GetPriceTotal();
            }

            ViewBag.TotalPrice = totalPrice;

            return PartialView();
        }





        public ActionResult Partail_ListProduct()
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null && cart.Items.Any())
            {




                int count = cart.Items.Count;
                ViewBag.Count = count;
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_LeftIndex()
        {
            return PartialView();
        }











        public ActionResult AddClient()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                return PartialView();
            }
        }
        //public JsonResult AddClient() 
        //{
        //    using (var dbContextTransaction = db.Database.BeginTransaction()) 
        //    {
        //        try 
        //        {
        //            if (Session["user"] == null)
        //            {
        //                return Json(new { success = false, code = -99, redirectTo = Url.Action("DangNhap", "Account") });
        //            }
        //            else
        //            {
        //                return Json(new { success = true, code = 1 });
        //            }
        //        catch (Exception ex) 
        //        {
        //            dbContextTransaction.Rollback();
        //            return Json(new { success = false, code = -100 });

        //        }
        //    }
        //}

    }
}