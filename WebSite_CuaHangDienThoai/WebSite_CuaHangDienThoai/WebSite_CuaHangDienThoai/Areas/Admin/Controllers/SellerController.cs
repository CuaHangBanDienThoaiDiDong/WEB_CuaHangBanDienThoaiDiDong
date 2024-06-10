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


        //Goi y khi tim san phảm
        //public ActionResult SuggestTop(string searchString)
        //{

        //    if (!string.IsNullOrEmpty(searchString))
        //    {

        //        var products = db.tb_Products.Where(p => p.Title.Contains(searchString)).Take(5).ToList();
        //        ViewBag.products = products;
        //        return PartialView(products);
        //    }
        //    else
        //    {
        //        return PartialView(null);
        //    }
        //}

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

        public ActionResult Partial_LeftIndex() 
        {
            return PartialView();
        }

    }
}