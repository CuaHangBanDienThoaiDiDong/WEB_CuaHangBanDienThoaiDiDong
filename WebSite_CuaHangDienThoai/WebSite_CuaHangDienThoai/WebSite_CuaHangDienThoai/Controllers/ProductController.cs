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
        //public ActionResult Details(int? id)
        //{
        //    var item = db.tb_Products.Find(id);
        //    if (item != null)
        //    {
        //        ViewBag.Title = item.Title;

        //        ViewBag.ProductCompany = item.tb_ProductCompany.Title;
        //    }
        //    return View(item);
        //}


        [HttpPost]
        public ActionResult Find(string Search = "")
        {
            if (!string.IsNullOrEmpty(Search))
            {
                var FindProduc = db.tb_Products.Where(x => x.Title.ToUpper().Contains(Search.ToUpper()));
                ViewBag.Find = Search;
                return View(FindProduc.ToList());
            }
            return View();
        }




        public ActionResult Partial_ItemsByCateId() 
        {
            var items = db.tb_Products.Where(x => x.IsHome==true && x.IsActive == true).Take(12).ToList();
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
                var item = db.tb_Products.Find(id);
                if (item != null)
                {
                    return View(item);
                }
                return View();
            }
        }

        public ActionResult DetailsTest(int? id){
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
                var item = db.tb_Products.Find(id);
                if (item != null)
                {
                    return View(item);
                }
                return View();
            }


        }










        public ActionResult DungLuong(int id , int DungLuong  )
        {
         
            if (id > 0)
            {
                var query = from pd in db.tb_ProductDetail
                            join p in db.tb_Products on pd.ProductsId equals p.ProductsId
                            where pd.ProductsId == id
                            select new
                            {
                                ProductDetailId = pd.ProductDetailId,
                                Color = pd.Color,
                                DungLuong = pd.Capacity
                            };

                // Tạo một danh sách để lưu trữ các màu và dung lượng mà không bị lặp lại
                List<string> colors = new List<string>();
                List<int> dungLuongs = new List<int>();

                foreach (var item in query)
                {
                    if (!colors.Contains(item.Color))
                    {
                        colors.Add(item.Color);
                    }

                    if (!dungLuongs.Contains((int)item.DungLuong))
                    {
                        dungLuongs.Add((int)item.DungLuong);
                    }
                }

                // Chuyển đổi danh sách dung lượng sang một danh sách các đối tượng ProductDetailViewModel
                List<ProductDetailViewModel> result = dungLuongs.Select(dl => new ProductDetailViewModel
                {
                    Color = string.Join(", ", colors),
                    DungLuong = dl
                }).ToList();

                if (DungLuong > 0)
                {
                    ViewBag.DungLuong=DungLuong;
                }
                ViewBag.ProductsId = id;
                return PartialView(result);
            }
            else 
            {
                
                return PartialView();
            }



          
        }


     

       



        public ActionResult Partial_DetailImageById(int id)
        {
            var checkProductDetail=db.tb_ProductDetail.Where(x=>x.ProductsId == id).ToList();
            while (checkProductDetail != null)
            {
                var imgDefault = db.tb_ProductImage.FirstOrDefault(x => x.ProductsId == id &&x.IsDefault);
                ViewBag.ProductImage = imgDefault.Image;  
                return PartialView(checkProductDetail);
            }
            return PartialView();
        }

      
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