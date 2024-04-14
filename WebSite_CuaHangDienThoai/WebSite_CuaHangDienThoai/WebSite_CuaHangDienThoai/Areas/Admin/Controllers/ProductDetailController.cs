using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ProductDetailController : Controller
    {
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Admin/ProductDetail
        public ActionResult Index(int? page)
        {

            IEnumerable<tb_ProductDetail> items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId);
            if (items != null)
            {
                var pageSize = 10;
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



        public ActionResult Detail(int? id)
        {
            var item = db.tb_ProductDetail.Find(id);
            if (item != null) 
            {
                ViewBag.Title = item.Title;
                return View(item);
            }
            else
            {
                return View();
            }
        }









        public ActionResult Partial_InforProucts(int id)
        {
            var item = db.tb_Products.Find((int)id);
            return PartialView(item);
        }

        public ActionResult Partial_AddProductDetail(int id)
        {
            ViewBag.id = id;
            return PartialView();
        }

        public ActionResult Partial_DetailProduct(int id)
        {
            var item = db.tb_ProductDetail.Where(x => x.ProductsId == id).ToList();
            return PartialView("_DetailProduct", item);
        }










        public ActionResult Add(string id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_ProductDetail model, Admin_TokenProductDetail req, List<string> Images, List<int> rDefault)
        {
            var code = new { Success = false, Code = -1, Url = "" };



            var checkProductDetail = db.tb_ProductDetail.FirstOrDefault(r => r.Title == req.Title && r.ProductsId == req.ProductsId);
            if (checkProductDetail == null)
            {
                if (req.ProductsId != null)
                {
                    if (req.Ram != 1 && req.DungLuong != 1 && req.DungLuongPin >= 100)
                    {
                        if (Images != null && Images.Count > 0)
                        {
                            for (int i = 0; i < Images.Count; i++)
                            {
                                if (i + 1 == rDefault[0])
                                {
                                    model.Image = Images[i];
                                    db.tb_ProductDetailImage.Add(new tb_ProductDetailImage
                                    {
                                        ProductDetailId = model.ProductDetailId,
                                        Image = Images[i],
                                        IsDefault = true
                                    });
                                }
                                else
                                {
                                    db.tb_ProductDetailImage.Add(new tb_ProductDetailImage
                                    {
                                        ProductDetailId = model.ProductDetailId,
                                        Image = Images[i],
                                        IsDefault = true
                                    });
                                }
                            }
                            model.Color = req.Color;
                            model.Title = req.Title;
                            model.ProductsId = req.ProductsId;
                            model.Price = req.Price;
                            model.OrigianlPrice = req.OrigianlPrice;
                            model.PriceSale = req.PriceSale;
                            model.TypeProduct = req.TypeProduct;
                            model.DungLuongPin = req.DungLuongPin;
                            model.DungLuong = req.DungLuong;
                            model.Ram = req.Ram;
                            db.tb_ProductDetail.Add(model);
                            db.SaveChanges();
                            code = new { Success = true, Code = 1, Url = "" };
                        }
                        else
                        {
                            // Hãy tải ảnh lên 
                            code = new { Success = false, Code = -5, Url = "" };
                        }

                    }
                    else
                    {//Vui lòng nhập đủ thông tin
                        code = new { Success = false, Code = -4, Url = "" };
                    }
                }
                else
                {//Không tồn tại mã sản phẩm
                    code = new { Success = false, Code = -3, Url = "" };
                }
            }
            else
            {
                //Cấu hình đã tồn tại
                code = new { Success = false, Code = -6, Url = "" };
            }
            return Json(code);
        }


        public ActionResult Edit(int id) 
        {
            var item = db.tb_ProductDetail.Find(id);
                ViewBag.Products = new SelectList(db.tb_Products.ToList(), "ProductsId", "Title");
                return PartialView(item);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ProductDetail model)
        {
            if (ModelState.IsValid) 
            {
                //model.ModifiedDate = DateTime.Now;
                //model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                db.tb_ProductDetail.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(model);

        }



        public ActionResult ShowDetails(int id)
        {
            // Kết nối đến cơ sở dữ liệu và thực hiện truy vấn LINQ
            using (var context = new CUAHANGDIENTHOAIEntities())
            {
                var query = (from pd in context.tb_ProductDetail
                             where pd.ProductsId == id
                             group pd by new { pd.Ram, pd.DungLuong } into g
                             where g.Count() == 1 // Lọc bỏ các bản ghi trùng lặp
                             select new
                             {
                                 Ram = g.Key.Ram,
                                 DungLuong = g.Key.DungLuong
                             }).ToList();

                // Xóa các bản ghi trùng lặp
                foreach (var item in query)
                {
                    var duplicates = context.tb_ProductDetail
                        .Where(pd => pd.ProductsId == id && pd.Ram == item.Ram && pd.DungLuong == item.DungLuong)
                        .OrderBy(pd => pd.ProductDetailId) // Sắp xếp dữ liệu theo một trường nào đó, ví dụ: Id
                        .Skip(1) // Bỏ qua bản ghi đầu tiên để chỉ lấy các bản ghi trùng lặp
                        .ToList();


                    context.tb_ProductDetail.RemoveRange(duplicates);
                }

                context.SaveChanges();
            }


            List<ProductDetailViewModel> productDetails = new List<ProductDetailViewModel>();

            // Kết nối đến cơ sở dữ liệu và thực hiện truy vấn LINQ
            using (var context = new CUAHANGDIENTHOAIEntities())
            {
                var query = (from pd in context.tb_ProductDetail
                             where pd.ProductsId == id
                             select new ProductDetailViewModel
                             {
                                 Ram = (int)pd.Ram,
                                 DungLuong = (int)pd.DungLuong,
                                 Color = pd.Color
                             }).ToList();

                productDetails = query.ToList();
            }

            return View(productDetails);



            ////// Chuyển hướng đến action xem chi tiết sản phẩm
            ////return RedirectToAction("Partail_ShowDetails", new { id = id });
        }

        public ActionResult Partail_ShowDetails(int id)
        {
            var item = db.tb_ProductDetail.Where(x => x.ProductsId == id).ToList(); 

            return View(item);
        }

      






        public ActionResult test(int id)
        {
            var query = from pd in db.tb_ProductDetail
                        join p in db.tb_Products on pd.ProductsId equals p.ProductsId
                        where pd.ProductsId == id
                        group new { pd, p } by new { pd.ProductsId, p.Title, p.Image, pd.Ram, pd.DungLuong } into grouped
                        select new
                        {
                            ProductsId = grouped.Key.ProductsId,
                            ProductTitle = grouped.Key.Title,
                            ProductImage = grouped.Key.Image,
                            Ram = grouped.Key.Ram,
                            DungLuong = grouped.Key.DungLuong,
                            Colors = grouped.Select(x => x.pd.Color).ToList(),
                            Prices = grouped.Select(x => new { Price = x.pd.Price, PriceSale = x.pd.PriceSale, OrigianlPrice = x.pd.OrigianlPrice }).ToList()
                        };

            List<ProductDetailViewModel> products = new List<ProductDetailViewModel>();

            foreach (var item in query)
            {
                List<ProductDetailViewModel> prices = item.Prices != null
                                                ? item.Prices.Select(x => new ProductDetailViewModel { Price = (decimal)x.Price, PriceSale = (decimal)x.PriceSale, OrigianlPrice = (decimal)x.OrigianlPrice }).ToList()
                                                : new List<ProductDetailViewModel>();

                products.Add(new ProductDetailViewModel
                {
                    //ProductsId = item.ProductsId,
                    //ProductTitle = item.ProductTitle,
                    //ProductImage = item.ProductImage,
                    Ram = (int)item.Ram,
                    DungLuong = (int)item.DungLuong,
                    Color = string.Join(", ", item.Colors),
                    Price = prices.FirstOrDefault()?.Price ?? 0m
                });
            }

            return PartialView(products);
        }


    }
}