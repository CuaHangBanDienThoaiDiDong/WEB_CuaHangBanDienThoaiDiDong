using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
        {
            IEnumerable<tb_Products> items = db.tb_Products.OrderByDescending(x => x.ProductsId);
            if (items != null)
            {
                var pageSize = 16;
                if (page == null)
                {
                    page = 1;
                }
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                items = items.ToPagedList(pageIndex, pageSize);
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return View();
            }
        }
     



        public ActionResult Search(string searchString)
        {
            var products = db.tb_Products.Where(p => p.Alias.Contains(searchString)).ToList();
            ViewBag.SearchString = searchString;
            return View(products);
        }


        //Goi y khi tim san phảm

      
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
                    return PartialView( items);
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




        [HttpGet]
        public ActionResult Result(string searchString)
        {
            var products = db.tb_Products.Where(p => p.Title.Contains(searchString)).ToList();
            return View(products);
        }


        public ActionResult Partial_ItemsByCateId()
        {
            var items = db.tb_Products.Where(x => x.IsHome == true && x.IsActive == true).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ItemsByCateIdTest()
        {
            var items = db.tb_Products.Where(x => x.IsHome == true && x.IsActive == true).Take(12).ToList();
            var dungLuong = Session["DungLuong"];
            ViewBag.DungLuong = dungLuong;
            return PartialView(items);
        }



        public ActionResult Details(int? id)
        {
            if (id > 0)
            {
                var item = db.tb_Products.Find(id);
                if (item != null)
                {
                    ViewBag.Title = item.Title;
                    ViewBag.ProductId = id; // Truyền ProductId vào ViewBag

                    ViewBag.ProductCompany = item.tb_ProductCompany.Title;

                    var itemProductDetail = db.tb_ProductDetail.FirstOrDefault(r => r.ProductsId == id);
                    if (itemProductDetail != null)
                    {
                        //ViewBag.DungLuong = itemProductDetail.Capacity;
                        return View(itemProductDetail);
                    }
                }
                return View(item);



            }
            else
            {
                RedirectToAction("NotFound", "Error");
                return View();
            }
        }

        public ActionResult DetailsTest(string Alias, int? id)
        {
            if (id > 0)
            {
                var item = db.tb_Products.Find(id);
                if (item != null)
                {
                    ViewBag.Title = item.Title;
                    ViewBag.ProductId = id; // Truyền ProductId vào ViewBag

                    ViewBag.ProductCompany = item.tb_ProductCompany.Title;


                    var itemProductDetail = db.tb_ProductDetail.FirstOrDefault(r => r.ProductsId == id);
                    if (itemProductDetail != null)
                    {
                        //ViewBag.DungLuong = itemProductDetail.Capacity;
                        return View(itemProductDetail);
                    }
                }
                return View(item);



            }
            else
            {
                RedirectToAction("NotFound", "Error");
                return View();
            }


        }

        public ActionResult DungLuong(int productid, int DungLuong)
        {
            using (var dbContext = new CUAHANGDIENTHOAIEntities())
            {
                var uniqueCapacitiesWithIdsAndImages = dbContext.tb_ProductDetail
                .Where(p => p.ProductsId == productid)
                .GroupBy(p => p.Capacity)
                .Select(g => new
                {
                    Capacity = g.Key,
                    ProductDetailId = g.Min(p => p.ProductDetailId),

                })
                .ToList();

                var viewModels = uniqueCapacitiesWithIdsAndImages.Select(item => new ProductColorViewModel
                {

                    ProductDetailId = item.ProductDetailId,
                    ProductslId = productid,
                    Capacity = (int)item.Capacity,

                }).ToList();

                ViewBag.ProductId = productid;
                ViewBag.Capacity = DungLuong;
                return PartialView(viewModels);
            }
        }


        public ActionResult Partial_DetailImageById(int id)
        {
            using (var dbContext = new CUAHANGDIENTHOAIEntities())
            {
                var uniqueColorsWithIdsAndImages = dbContext.tb_ProductDetail
                    .Where(p => p.ProductsId == id)
                    .GroupBy(p => p.Color)
                    .Select(g => new
                    {
                        Color = g.Key,
                        ProductDetailId = g.Min(p => p.ProductDetailId),
                        Image = dbContext.tb_ProductDetailImage
                                    .Where(x => x.ProductDetailId == g.Min(p => p.ProductDetailId) && x.IsDefault)
                                    .Select(x => x.Image)
                                    .FirstOrDefault()
                    })
                    .ToList();

                var viewModels = uniqueColorsWithIdsAndImages.Select(item => new ProductColorViewModel
                {
                    Color = item.Color,
                    ProductDetailId = item.ProductDetailId,
                    ProductslId = id,
                    Image = item.Image
                }).ToList();

                ViewBag.ProductId = id;
                return PartialView(viewModels);
            }
            
        }








        //public ActionResult Partial_DetailImageById(int id)
        //{


        //    var checkProductDetail =db.tb_ProductDetail.Where(x=>x.ProductsId == id).ToList();
        //    while (checkProductDetail != null)
        //    {
        //        var imgDefault = db.tb_ProductImage.FirstOrDefault(x => x.ProductsId == id &&x.IsDefault);
        //        ViewBag.ProductImage = imgDefault.Image;  
        //        return PartialView(checkProductDetail);
        //    }
        //    return PartialView();
        //}


        public ActionResult Partail_LoadListImgByProDetailId(int id) 
        {
            var checkImg = db.tb_ProductDetailImage.Where(x => x.ProductDetailId == id).ToList();
            while (checkImg != null)
            {
               
                return PartialView(checkImg);
            }
            return PartialView();
        }




    }
}