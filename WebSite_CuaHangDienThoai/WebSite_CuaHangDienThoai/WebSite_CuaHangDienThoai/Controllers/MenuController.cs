﻿using System;
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

        //public ActionResult MenuLeft(int? id)
        //{
        //    if (id != null)
        //    {
        //        ViewBag.CateId = id;
        //    }
        //    var item = db.tb_ProductCategory.ToList();
        //    return PartialView("_MenuLeft", item);
        //}

        public ActionResult MenuLeft(int? id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }

            // Lấy tất cả các danh mục
            var allCategories = db.tb_ProductCategory.Where(x=>x.IsActive==true).ToList();

            // Lấy các danh mục "Tai nghe", "Cáp sạc", "Ốp lưng"
            var accessoryCategories = db.tb_ProductCategory
                                        .Where(pc => pc.Title == "Tai nghe" ||
                                                     pc.Title == "Cáp sạc" ||
                                                     pc.Title == "Ốp lưng")
                                        .Select(pc => pc.ProductCategoryId)
                                        .ToList();

            // Lấy các sản phẩm từ các danh mục trên
            var accessories = db.tb_Products
                                .Where(p => accessoryCategories.Contains((int)p.ProductCategoryId))
                                .ToList();

            // Tạo ViewModel để truyền dữ liệu
            var viewModel = new MenuLeftViewModel
            {
                AllCategories = allCategories,
                Accessories = accessories
            };

            return PartialView("_MenuLeft", viewModel);
        }

        public ActionResult MenuBrand(int? id)
        {
            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var item = db.tb_ProductCompany.ToList();
            return PartialView("_MenuBrand", item);
        
        }
        public ActionResult MenuProductDetail()
        {
            var checkProducts = db.tb_Products.FirstOrDefault(x => x.IsActive == true && x.IsHome == true);
          
            if (checkProducts != null)
            {
                var items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId == checkProducts.ProductsId).Take(15).ToList();
                if (items != null)
                {
                    ViewBag.txt = "abc";
                    return PartialView("_MenuProductDetail", items);

                }
                else 
                {
                    return PartialView("_MenuProductDetail");
                }
               
            }
            return PartialView("_MenuProductDetail");

        }




        public ActionResult MenuIphone()
        {
            //var checkPhone = db.tb_Products.SingleOrDefault(x => x.IsActive == true && x.IsHome == true&&x.ProductCompanyId == 1 && x.ProductCategoryId == 1);

            var checkPhone = db.tb_Products
                     .Where(x => x.IsActive == true && x.IsHome == true && x.ProductCompanyId == 1 && x.ProductCategoryId == 1)
                     .Select(x => x.ProductsId)  // Chỉ lấy ID sản phẩm
                     .ToList();

            if (checkPhone != null)
            {

                //var productIds = checkPhone.Select(p => p.ProductsId);
                //var items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId == checkPhone.ProductsId).Take(15).ToList();
                var items = db.tb_ProductDetail
                .Where(pd => pd.ProductsId.HasValue && checkPhone.Contains(pd.ProductsId.Value))
                .OrderByDescending(pd => pd.ProductsId)
                .Take(15)
                .ToList();

                ViewBag.txt = "abc";
                return PartialView(items);
            }
            return PartialView();
        }




        public ActionResult MenuProductCompany()
        {

            var items = db.tb_Products.OrderByDescending(x => x.ProductsId).ToList();

            return PartialView("_MenuProductCompany", items);

        }


        public ActionResult MenuArrivals()
        {
            var CheckAcive = db.tb_ProductCategory.FirstOrDefault(x => x.IsActive == true);
            if (CheckAcive != null) 
            {
                var items = db.tb_ProductCategory.OrderByDescending(x => x.ProductCategoryId == CheckAcive.ProductCategoryId).Take(4).ToList();
                return PartialView("_MenuArrivals", items);
            }
            ViewBag.Txt = "Không tìm thấy bảng ghi !!!";
            return PartialView("_MenuArrivals");




        }





        public ActionResult MenuDanhMucNoiBat(int? id)
        {

            if (id != null)
            {
                ViewBag.CateId = id;
            }
            var items = db.tb_ProductCategory.Where(x=>x.IsActive==true).ToList();
            return PartialView("_MenuDanhMucNoiBat", items);

        }

        //public ActionResult MenuFlashSaleAll()
        //{
        //    List<tb_ProductDetail> productDetails = new List<tb_ProductDetail>();
        //    var checkSale = db.tb_Products.Where(x => x.IsSale == true && x.ProductCategoryId == 2 && x.IsActive == true).ToList();
        //    if (checkSale.Any())
        //    {

        //        foreach (var item in checkSale)
        //        {

        //            ViewBag.ProductCatgory = item.ProductCategoryId;
        //            var CheckProductDetail = db.tb_ProductDetail.Where(x => x.ProductsId == item.ProductsId && x.PriceSale > 0).ToList();

        //            var items = CheckProductDetail.OrderByDescending(x => x.ProductDetailId).ToList();
        //            productDetails.AddRange(items);

        //        }
        //        ViewBag.txt = "abc";

        //        return PartialView("_MenuFlashSaleAll", productDetails.ToList());
        //    }
        //    else
        //    {
        //        return PartialView("_MenuFlashSaleAll");
        //    }

        //}

        public ActionResult MenuFlashSaleAll()
        {
            List<tb_ProductDetail> productDetails = new List<tb_ProductDetail>();
            var checkSale = db.tb_Products.Where(x => x.IsSale == true /*&& x.IsActive == true*/).ToList();
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
            var checkSale = db.tb_Products.Where(x => x.IsSale == true && x.IsActive == true).ToList();
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
                ViewBag.txt = "abc";
                return PartialView(item);
            }
            else 
            {
                return PartialView();
            }
        }



      






    }
}