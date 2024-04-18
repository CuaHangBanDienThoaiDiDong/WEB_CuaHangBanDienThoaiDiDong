using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ProductDetailController : Controller
    {
        // GET: ProductDetail
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }


     




        public ActionResult Partail_ProductDetailById(int id)
        {
            var query = from pd in db.tb_ProductDetail
                        join p in db.tb_Products on pd.ProductsId equals p.ProductsId
                        where pd.ProductsId == id
                        select new
                        {
                            Color = pd.Color,
                            DungLuong = pd.DungLuong
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
            ViewBag.ProductsId = id;
            return PartialView(result);
        }

        //[HttpPost]
        public ActionResult PriceById(int id, string DungLuong)
        {
            if (DungLuong != null)
            {
                int dungluong = int.Parse(DungLuong);
                ViewBag.txt = "abc";
                var checkPrice = db.tb_ProductDetail.FirstOrDefault(x => x.ProductsId == id && x.DungLuong == dungluong);
                if (checkPrice != null)
                {
                    ViewBag.txt = "abc";
                    return PartialView(checkPrice);
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


        public ActionResult Partial_PhanTramGiaGiam(int id, string DungLuong)
        {
            if (DungLuong != null)
            {
                int dungluong = int.Parse(DungLuong);
                ViewBag.txt = "abc";
                var checkPrice = db.tb_ProductDetail.FirstOrDefault(x => x.ProductsId == id && x.DungLuong == dungluong);
                if (checkPrice != null)
                {
                    ViewBag.txt = "abc";
                    return PartialView(checkPrice);
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



        public ActionResult ProductDetailByCapacity(int? Productsid,int DungLuong) 
        {
            var item = db.tb_ProductDetail.FirstOrDefault(r=>r.ProductsId== Productsid&& r.DungLuong== DungLuong);
            if (item != null)
            {
                ViewBag.Title = item.tb_Products.Title;
                ViewBag.Capacity = item.DungLuong;

                return View(item);  
            }
            else
            {
                return View();
            }

           
        }





    }
}