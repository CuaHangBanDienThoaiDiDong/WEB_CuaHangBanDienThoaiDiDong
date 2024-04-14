using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class MenuController : Controller
    {
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuLeft(int? id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var item = db.tb_ProductCategory.ToList();
            return PartialView("_MenuLeft", item);
        }
        public ActionResult MenuProductDetail()
        {

            var items = db.tb_Products.OrderByDescending(x => x.ProductsId).ToList();

            return PartialView("_MenuProductDetail", items);

        }
        public ActionResult MenuProductCompany()
        {

            var items = db.tb_Products.OrderByDescending(x => x.ProductsId).ToList();

            return PartialView("_MenuProductCompany", items);

        }


        public ActionResult MenuArrivals()
        {
            var items = db.tb_Products.OrderByDescending(x => x.ProductsId).ToList();
            return PartialView("_MenuArrivals", items);
        }





        public ActionResult MenuDanhMucNoiBat(int? id)
        {

            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var items = db.tb_ProductCategory.OrderByDescending(x => x.ProductCategoryId).ToList();
            return PartialView("_MenuDanhMucNoiBat", items);

        }

        public ActionResult MenuFlashSaleAll()
        {
            List<tb_ProductDetail> productDetails = new List<tb_ProductDetail>();
            var checkSale = db.tb_Products.Where(x => x.IsSale == true && x.ProductCategoryId == 2 && x.IsActive == true).ToList();
            if (checkSale.Any())
            {

                foreach (var item in checkSale)
                {

                    ViewBag.ProductCatgory = item.ProductCategoryId;
                    var CheckProductDetail = db.tb_ProductDetail.Where(x => x.ProductsId == item.ProductsId && x.PriceSale > 0).ToList();

                    var items = CheckProductDetail.OrderByDescending(x => x.ProductDetailId).ToList();
                    productDetails.AddRange(items);

                }
                ViewBag.txt = "abc";

                return PartialView("_MenuFlashSaleAll", productDetails.ToList());
            }
            else
            {
                return PartialView("_MenuFlashSaleAll");
            }

        }



        public ActionResult MenuDeal()
        {
            List<tb_ProductDetail> productDetails = new List<tb_ProductDetail>();
            var checkSale = db.tb_Products.Where(x => x.IsSale == true && x.ProductCategoryId == 2 && x.IsActive == true).ToList();
            if (checkSale.Any())
            {

                foreach (var item in checkSale)
                {

                    ViewBag.ProductCatgory = item.ProductCategoryId;
                    var CheckProductDetail = db.tb_ProductDetail.Where(x => x.ProductsId == item.ProductsId && x.PriceSale > 0).ToList();

                    var items = CheckProductDetail.OrderByDescending(x => x.ProductDetailId).ToList();
                    productDetails.AddRange(items);

                }

                ViewBag.txt = "abc";
                return PartialView("_MenuDeal", productDetails.ToList());
            }
            else
            {
                return PartialView("_MenuDeal");
            }
        }



        public ActionResult MenuFlashSalePhone()
        {
            List<tb_ProductDetail> productDetails = new List<tb_ProductDetail>();
            var checkSale = db.tb_Products.Where(x => x.IsSale == true && x.ProductCategoryId == 2 && x.IsActive == true).ToList();
            if (checkSale.Any())
            {

                foreach (var item in checkSale)
                {

                    ViewBag.ProductCatgory = item.ProductCategoryId;
                    var CheckProductDetail = db.tb_ProductDetail.Where(x => x.ProductsId == item.ProductsId && x.PriceSale > 0).ToList();

                    var items = CheckProductDetail.OrderByDescending(x => x.ProductDetailId).ToList();
                    productDetails.AddRange(items);

                }

                ViewBag.txt = "abc";
                return PartialView("_MenuFlashSalePhone", productDetails.ToList());
            }
            else
            {
                return PartialView("_MenuFlashSalePhone");
            }

        }



        public ActionResult MenuGoiYHomNay() 
        {
            var item = db.tb_Products.Where(x => x.IsActive == true ).Take(40).ToList();
            if (item != null)
            {

                return PartialView(item);
            }
            else 
            {
                return PartialView();
            }
        }








    }
}