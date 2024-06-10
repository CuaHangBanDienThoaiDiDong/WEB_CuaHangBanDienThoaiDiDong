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
            return View();
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
            if (search != null) 
            {
                var customers = db.tb_Customer
                   .Where(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search))
                   .Select(c => new {
                       c.CustomerId,
                       c.PhoneNumber,
                       c.CustomerName
                   })
                   .ToList();
                ViewBag.Count=customers.Count;  
                return PartialView(customers);
            }
            else 
            {
                return PartialView();
            }

          
        }





        [HttpPost]
        public ActionResult AddListProduct(int id, int soluong)
        {
            var code = new { Success = false, Code = -1, Count = 0 };
            var checkSanPham = db.tb_ProductDetail.FirstOrDefault(row => row.ProductDetailId == id);
            if (checkSanPham != null)
            {

                if (checkSanPham != null)
                {
                    SellerCart cart = (SellerCart)Session["SellerCart"];
                    if (cart == null)
                    {
                        cart = new SellerCart();
                    }
                    SellerCartItem item = new SellerCartItem
                    {
                        ProductDetailId = checkSanPham.ProductDetailId,
                        ProductName = checkSanPham.tb_Products.Title,
                        CategoryName = checkSanPham.tb_Products.tb_ProductCategory.Title,
                        Capcity = (int)checkSanPham.Capacity,
                        Color = checkSanPham.Color,
                     

                        SoLuong = 0,
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
                    Session["SellerCart"] = cart;
                    code = new { Success = true, Code = 1, Count = cart.Items.Count };

                }
                else
                {
                    code = new { Success = false, Code = -1, Count = 0 };//Số Lượng Không Đủ
                }

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

    }
}