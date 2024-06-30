using CKFinder.Settings;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);

                var functionTitle = db.tb_Function
                                       .Where(func => func.FunctionId == checkStaff.FunctionId)
                                       .Select(func => func.TitLe)
                                       .FirstOrDefault();

                var items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId).ToList();

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(items.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Partial_DetailColorById(int id)
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
        public ActionResult SuggestProductDetail(string search)
      {
            if (!string.IsNullOrEmpty(search))
            {
                var ProductCategoryId = db.tb_ProductCategory
          .Where(c => c.Title.Contains(search))
          .Select(c => c.ProductCategoryId)
          .ToList();

                var ProductCompanyId = db.tb_ProductCompany
                    .Where(c => c.Title.Contains(search))
                    .Select(c => c.ProductCompanyId)
                    .ToList();

                var Products = db.tb_Products
                    .Where(p => p.Title.Contains(search) ||
                                ProductCategoryId.Contains((int)p.ProductCategoryId) ||
                                ProductCompanyId.Contains((int)p.ProductCompanyId))
                    .ToList();


                var productIds = Products.Select(p => p.ProductsId).ToList();


                var ProductDetail = db.tb_ProductDetail
                    .Where(s => productIds.Contains((int)s.ProductsId))
                    .ToList();

                if (ProductDetail.Any())
                {
                    var count = ProductDetail.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ProductDetail);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }






        public ActionResult Partial_ListProductDetail(int? page) 
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
                return PartialView(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return PartialView();
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

   
        public ActionResult Partial_DetailProduct(int id)
        {
            var item = db.tb_ProductDetail.Where(x => x.ProductsId == id).ToList();
            return PartialView("_DetailProduct", item);
        }




        public ActionResult Partial_CapacityByProductsId(int id)
        {
            var query = from pd in db.tb_ProductDetail
                        join p in db.tb_Products on pd.ProductsId equals p.ProductsId
                        where pd.ProductsId == id
                        select new
                        {
                            
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
                else 
                {
                    
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

        public ActionResult Partail_ColorByProductsId(int id)
        {
            if (id != null)
            {

                var query = from pd in db.tb_ProductDetail
                            join p in db.tb_Products on pd.ProductsId equals p.ProductsId
                            where pd.ProductsId == id
                            select new
                            {

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
                    else
                    {

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
            else
            {
                return PartialView(null);
            }


        }

        public ActionResult Partial_AddProductDetail(int id)
        {

            if (id > 0)
            {
                ViewBag.id = id;
                var product = db.tb_Products.Find(id);
                if (product != null)
                {
                    ViewBag.Title=product.Title;
                    ViewBag.Company = product.tb_ProductCompany.Title;
                    ViewBag.Category=product.tb_ProductCategory.Title;  
                }
                return PartialView();
            }
            else 
            {
                return PartialView();
            }
            
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


      
            var checkProductDetail = db.tb_ProductDetail.SingleOrDefault(r => r.Color == req.Color && r.Ram == req.Ram && r.ProductsId == req.ProductsId && r.Capacity == req.DungLuong);
            if (checkProductDetail == null)
            {
                if (req.ProductsId != null && req.Ram != 1 && req.DungLuong != 1)
                {
                    var itemProducts = db.tb_Products.SingleOrDefault(p => p.ProductsId == req.ProductsId);

                    string alias = itemProducts.Title.Trim() + "" + req.Color.Trim();
                    string products = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(alias);
                    var checkProduct = db.tb_ProductDetail.FirstOrDefault(x => x.Title == products);
                    if (checkProduct != null)
                    {
                        code = new { Success = false, Code = -6, Url = "" };
                        return Json(code);
                    }
                    if (itemProducts != null) 
                    {
                        if (Images != null && Images.Count > 0)
                        {
                            try
                            {


                                model.Color = req.Color.Trim();
                                model.Title = itemProducts.Title.Trim()+""+ req.Color.Trim() + "" + req.Color.Trim();
                                model.ProductsId = req.ProductsId;
                                model.Price = req.Price;
                                model.OrigianlPrice = req.OrigianlPrice;
                                model.PriceSale = req.PriceSale;
                                model.TypeProduct = req.TypeProduct;
                                model.Capacity = req.DungLuong;
                                model.Ram = req.Ram;

                                db.tb_ProductDetail.Add(model);
                                db.SaveChanges();

                                int index = 0;
                                foreach (var image in Images)
                                {

                                    if (string.IsNullOrEmpty(image))
                                    {
                                        code = new { Success = false, Code = -5, Url = "" };
                                        continue;
                                    }

                                    string physicalPath = Server.MapPath(image);

                                    if (!System.IO.File.Exists(physicalPath))
                                    {
                                        code = new { Success = false, Code = -5, Url = "" };
                                        continue;
                                    }

                                    try
                                    {
                                        // Đọc tệp và chuyển đổi thành mảng byte
                                        byte[] imageBytes = System.IO.File.ReadAllBytes(physicalPath);

                                        tb_ProductDetailImage productDetailImage = new tb_ProductDetailImage
                                        {
                                            ProductDetailId = model.ProductDetailId,
                                            Image = imageBytes,
                                            IsDefault = index == (rDefault[0] - 1) // Kiểm tra xem hình ảnh này có phải là mặc định không
                                        };

                                        db.tb_ProductDetailImage.Add(productDetailImage);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error reading file: {ex.Message}");
                                    }

                                    index++;
                                }

                                db.SaveChanges();
                                code = new { Success = true, Code = 1, Url = "" };
                            }
                            catch (Exception ex)
                            {
                                // Xử lý ngoại lệ nếu có
                                ViewBag.Error = "Đã xảy ra lỗi khi thêm mới sản phẩm: " + ex.Message;
                            }
                        }
                        else
                        {
                            // Hãy tải ảnh lên 
                            code = new { Success = false, Code = -5, Url = "" };
                        }
                    }
                    else 
                    {//Khoong tim thaasyt san pham
                        code = new { Success = false, Code = -7, Url = "" };
                    }
                    
                }
                else
                {
                    // Vui lòng nhập đủ thông tin
                    code = new { Success = false, Code = -4, Url = "" };
                }
            }
            else
            {
                // Cấu hình đã tồn tại
                code = new { Success = false, Code = -6, Url = "" };
            }

            return Json(code);
        }








        //public ActionResult Add(tb_ProductDetail model, Admin_TokenProductDetail req, List<string> Images, List<int> rDefault)
        //{
        //    var code = new { Success = false, Code = -1, Url = "" };



        //    //var checkProductDetail = db.tb_ProductDetail.FirstOrDefault(r => r.Title == req.Title && r.ProductsId == req.ProductsId);
        //    var checkProductDetail = db.tb_ProductDetail.SingleOrDefault(r => r.Color == req.Color && r.Ram == req.Ram && r.ProductsId == req.ProductsId && r.Capacity == req.DungLuong);
        //    if (checkProductDetail == null)
        //    {
        //        if (req.ProductsId != null)
        //        {
        //            if (req.Ram != 1 && req.DungLuong != 1)
        //            {
        //                if (Images != null && Images.Count > 0)
        //                {
        //                    for (int i = 0; i < Images.Count; i++)
        //                    {
        //                        if (i + 1 == rDefault[0])
        //                        {
        //                            model.Image = Images[i];
        //                            db.tb_ProductDetailImage.Add(new tb_ProductDetailImage
        //                            {
        //                                ProductDetailId = model.ProductDetailId,
        //                                Image = Images[i],
        //                                IsDefault = true
        //                            });
        //                        }
        //                        else
        //                        {
        //                            db.tb_ProductDetailImage.Add(new tb_ProductDetailImage
        //                            {
        //                                ProductDetailId = model.ProductDetailId,
        //                                Image = Images[i],
        //                                IsDefault = true
        //                            });
        //                        }
        //                    }
        //                    model.Color = req.Color;
        //                    model.Title = req.Title.Trim();
        //                    model.ProductsId = req.ProductsId;
        //                    model.Price = req.Price;
        //                    model.OrigianlPrice = req.OrigianlPrice;
        //                    model.PriceSale = req.PriceSale;
        //                    model.TypeProduct = req.TypeProduct;

        //                    model.Capacity = req.DungLuong;
        //                    model.Ram = req.Ram;
        //                    db.tb_ProductDetail.Add(model);
        //                    db.SaveChanges();
        //                    code = new { Success = true, Code = 1, Url = "" };
        //                }
        //                else
        //                {
        //                    // Hãy tải ảnh lên 
        //                    code = new { Success = false, Code = -5, Url = "" };
        //                }

        //            }
        //            else
        //            {//Vui lòng nhập đủ thông tin
        //                code = new { Success = false, Code = -4, Url = "" };
        //            }
        //        }
        //        else
        //        {//Không tồn tại mã sản phẩm
        //            code = new { Success = false, Code = -3, Url = "" };
        //        }
        //    }
        //    else
        //    {
        //        //Cấu hình đã tồn tại
        //        code = new { Success = false, Code = -6, Url = "" };
        //    }
        //    return Json(code);
        //}


        public ActionResult Edit(int id) 
        {
            var item = db.tb_ProductDetail.Find(id);
            ViewBag.Title= item.Title;  
                ViewBag.Products = new SelectList(db.tb_Products.ToList(), "ProductsId", "Title");
            //ViewBag.DungLuongSelected = item.Capacity;
            //ViewBag.RamSelected = item.Ram;
            ViewBag.DungLuongOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "64", Text = "64GB" },
                new SelectListItem { Value = "128", Text = "128GB" },
                new SelectListItem { Value = "256", Text = "256GB" },
                new SelectListItem { Value = "512", Text = "512GB" },
                new SelectListItem { Value = "10000", Text = "1TB" },
                new SelectListItem { Value = "20000", Text = "2TB" }
            }, "Value", "Text", item.Capacity);


            ViewBag.RamOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Ram sản phẩm" },
                new SelectListItem { Value = "4", Text = "4GB" },
                new SelectListItem { Value = "6", Text = "6GB" },
                new SelectListItem { Value = "8", Text = "8GB" },
                new SelectListItem { Value = "16", Text = "16GB" },
                new SelectListItem { Value = "24", Text = "24GB" },
                new SelectListItem { Value = "32", Text = "32GB" },
                new SelectListItem { Value = "64", Text = "64GB" },
                new SelectListItem { Value = "128", Text = "128GB" }
            }, "Value", "Text", item.Ram); 
            return PartialView(item);
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ProductDetail model, IEnumerable<HttpPostedFileBase> newImages, int[] productImageIds)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction()) 
            {
                try 
                {
                    if (ModelState.IsValid)
                    {
                        // Cập nhật thông tin sản phẩm
                        var existingProduct = db.tb_ProductDetail.Find(model.ProductDetailId);
                        if (existingProduct == null)
                        {
                            return HttpNotFound();
                        }

                        existingProduct.Title = model.Title;
                        existingProduct.Color = model.Color;
                        existingProduct.Price = model.Price;
                        existingProduct.PriceSale = model.PriceSale;
                        existingProduct.OrigianlPrice = model.OrigianlPrice;
                        existingProduct.Ram = model.Ram;
                        existingProduct.Capacity = model.Capacity;
                        existingProduct.ProductsId = model.ProductsId;

                        // Cập nhật hình ảnh
                        for (int i = 0; i < newImages.Count(); i++)
                        {
                            var file = newImages.ElementAt(i);
                            int productImageId = productImageIds.ElementAt(i);

                            if (file != null && file.ContentLength > 0)
                            {
                                byte[] imageData = null;
                                using (var binaryReader = new BinaryReader(file.InputStream))
                                {
                                    imageData = binaryReader.ReadBytes(file.ContentLength);
                                }

                                // Cập nhật hình ảnh trong cơ sở dữ liệu
                                var imageDetail = db.tb_ProductDetailImage.FirstOrDefault(x => x.ProductImageId == productImageId);
                                if (imageDetail != null)
                                {
                                    if (imageDetail.IsDefault == true)
                                    {
                                        imageDetail.Image = imageData;
                                        imageDetail.IsDefault = true; // Cập nhật hình ảnh mới là mặc định

                                    }
                                    else
                                    {
                                        imageDetail.Image = imageData;
                                        imageDetail.IsDefault = false;
                                    }
                                    db.Entry(imageDetail).State = EntityState.Modified;
                                }
                                else
                                {
                                    return Json(new { success = false, Code = -1 });//Lỗi Update hình ảnh
                                }
                            }
                        }

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.Entry(existingProduct).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, Code = 1 });
                       
                    }
                    return View(model);
                }
                catch (Exception ex) 
                {
                    return Json(new { success = false, Code = -99 });
                }

            }
            
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(tb_ProductDetail model, HttpPostedFileBase[] newImage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var image in newImage)
        //        {
        //            if (image != null && image.ContentLength > 0)
        //            {
        //                byte[] imageData = null;
        //                using (var binaryReader = new BinaryReader(image.InputStream))
        //                {
        //                    imageData = binaryReader.ReadBytes(image.ContentLength);
        //                }

        //                // Lấy productId từ tên file
        //                var productIdStr = image.FileName.Split('_').LastOrDefault();
        //                int productId;
        //                if (int.TryParse(productIdStr, out productId))
        //                {
        //                    // Tìm sản phẩm theo productId và cập nhật hình ảnh mới
        //                    var checkImageDetail = db.tb_ProductDetailImage.FirstOrDefault(x => x.ProductImageId == productId);
        //                    if (checkImageDetail != null)
        //                    {
        //                        checkImageDetail.Image = imageData;
        //                        checkImageDetail.IsDefault = true;
        //                        db.Entry(checkImageDetail).State = EntityState.Modified;
        //                    }
        //                    else
        //                    {
        //                        // Nếu không tìm thấy sản phẩm, bạn có thể xử lý theo ý định của mình
        //                        // Ví dụ: Tạo một sản phẩm mới với hình ảnh này
        //                    }
        //                }
        //                else
        //                {
        //                    // Xử lý lỗi không lấy được ProductImageId từ tên file
        //                }
        //            }
        //        }

        //        // Lưu các thay đổi vào database
        //        db.Entry(model).State = EntityState.Modified;
        //        db.SaveChanges();

        //        return RedirectToAction("index");
        //    }

        //    return View(model);
        //}


        public ActionResult ShowDetails(int id)
        {
            // Kết nối đến cơ sở dữ liệu và thực hiện truy vấn LINQ
            using (var context = new CUAHANGDIENTHOAIEntities())
            {
                var query = (from pd in context.tb_ProductDetail
                             where pd.ProductsId == id
                             group pd by new { pd.Ram, pd.Capacity } into g
                             where g.Count() == 1 // Lọc bỏ các bản ghi trùng lặp
                             select new
                             {
                                 Ram = g.Key.Ram,
                                 DungLuong = g.Key.Capacity
                             }).ToList();

                // Xóa các bản ghi trùng lặp
                foreach (var item in query)
                {
                    var duplicates = context.tb_ProductDetail
                        .Where(pd => pd.ProductsId == id && pd.Ram == item.Ram && pd.Capacity == item.DungLuong)
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
                                 DungLuong = (int)pd.Capacity,
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

      






        //public ActionResult test(int id)
        //{
        //    var query = from pd in db.tb_ProductDetail
        //                join p in db.tb_Products on pd.ProductsId equals p.ProductsId
        //                where pd.ProductsId == id
        //                group new { pd, p } by new { pd.ProductsId, p.Title, p.Image, pd.Ram, pd.DungLuong } into grouped
        //                select new
        //                {
        //                    ProductsId = grouped.Key.ProductsId,
        //                    ProductTitle = grouped.Key.Title,
        //                    ProductImage = grouped.Key.Image,
        //                    Ram = grouped.Key.Ram,
        //                    DungLuong = grouped.Key.DungLuong,
        //                    Colors = grouped.Select(x => x.pd.Color).ToList(),
        //                    Prices = grouped.Select(x => new { Price = x.pd.Price, PriceSale = x.pd.PriceSale, OrigianlPrice = x.pd.OrigianlPrice }).ToList()
        //                };

        //    List<ProductDetailViewModel> products = new List<ProductDetailViewModel>();

        //    foreach (var item in query)
        //    {
        //        List<ProductDetailViewModel> prices = item.Prices != null
        //                                        ? item.Prices.Select(x => new ProductDetailViewModel { Price = (decimal)x.Price, PriceSale = (decimal)x.PriceSale, OrigianlPrice = (decimal)x.OrigianlPrice }).ToList()
        //                                        : new List<ProductDetailViewModel>();

        //        products.Add(new ProductDetailViewModel
        //        {
        //            //ProductsId = item.ProductsId,
        //            //ProductTitle = item.ProductTitle,
        //            //ProductImage = item.ProductImage,
        //            Ram = (int)item.Ram,
        //            DungLuong = (int)item.DungLuong,
        //            Color = string.Join(", ", item.Colors),
        //            Price = prices.FirstOrDefault()?.Price ?? 0m
        //        });
        //    }

        //    return PartialView(products);
        //}


    }
}