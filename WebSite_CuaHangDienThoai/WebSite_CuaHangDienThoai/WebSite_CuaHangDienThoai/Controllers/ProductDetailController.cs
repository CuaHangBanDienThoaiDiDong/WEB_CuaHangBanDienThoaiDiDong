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
                            Capacity = pd.Capacity
                        };

            // Tạo một danh sách để lưu trữ các màu và dung lượng mà không bị lặp lại
            List<string> colors = new List<string>();
            List<int> Capacitys = new List<int>();

            foreach (var item in query)
            {
                if (!colors.Contains(item.Color))
                {
                    colors.Add(item.Color);
                }

                if (!Capacitys.Contains((int)item.Capacity))
                {
                    Capacitys.Add((int)item.Capacity);
                }
            }
           

            // Chuyển đổi danh sách dung lượng sang một danh sách các đối tượng ProductDetailViewModel
            List<ProductDetailViewModel> result = Capacitys.Select(dl => new ProductDetailViewModel
            {
                Color = string.Join(", ", colors),
                DungLuong = dl
            }).ToList();
            ViewBag.ProductsId = id;
            //ViewBag.DungLuongList = result;
            // Lấy dung lượng của sản phẩm đầu tiên trong kết quả query
            var productDetailViewModel = query.FirstOrDefault();
          
            ViewBag.ProductsId = id;
            var dungLuong = productDetailViewModel?.Capacity;

            Session["DungLuong"] = dungLuong;
            return PartialView(result);
        }





        //[HttpPost]
        public ActionResult PriceById(int id, string DungLuong)
        {
            if (DungLuong != null)
            {
                int Capacity = int.Parse(DungLuong);
                ViewBag.txt = "abc";
                var checkPrice = db.tb_ProductDetail.FirstOrDefault(x => x.ProductsId == id && x.Capacity == Capacity);
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
                int Capacity = int.Parse(DungLuong);
                ViewBag.txt = "abc";
                var checkPrice = db.tb_ProductDetail.FirstOrDefault(x => x.ProductsId == id && x.Capacity == Capacity);
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



        public ActionResult ProductDetailByCapacity(int? Productsid,int Capacity) 
        {
            var item = db.tb_ProductDetail.FirstOrDefault(r=>r.ProductsId== Productsid&& r.Capacity== Capacity);
            if (item != null)
            {
                ViewBag.Title = item.tb_Products.Title;
                ViewBag.Capacity = item.Capacity;
                ViewBag.ProductId = Productsid; // Truyền ProductId vào ViewBag
                return View(item);  
            }
            else
            {
                return View();
            }

           
        }


        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("DangNhap", "Account");
        }




    }
}