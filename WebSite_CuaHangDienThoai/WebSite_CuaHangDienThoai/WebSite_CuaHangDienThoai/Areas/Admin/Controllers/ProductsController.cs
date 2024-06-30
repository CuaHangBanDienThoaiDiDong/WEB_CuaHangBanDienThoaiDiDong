
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Admin/Products
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    var products = db.tb_Products.ToList();
                    if (products != null) 
                    {
                        ViewBag.Count = products.Count;
                    }
                 
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    //var items = db.tb_Staff.OrderByDescending(x => x.Code).ToList();
                    return View();
                }

            }

        }

        
        public ActionResult SuggestProduct(string search)
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



                if (Products.Any())
                {
                    var count = Products.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(Products);
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
        public ActionResult Partial_Index(int? page) 
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_Products> items = db.tb_Products.OrderByDescending(x => x.ProductsId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_Products.ToList();

                    ViewBag.Count = products.Count;
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
        }

      



        public ActionResult Partial_AddProduct()
        {
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
            ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
            return PartialView();
        }

        public string AbbreviateCompanyName(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return companyName; // Return the original name if it's null, empty, or whitespace
            }

            if (companyName.Length <= 2)
            {
                return companyName; // Return the original name if it's 2 characters or less
            }

            return $"{companyName[0]}{companyName[companyName.Length - 1]}";
        }
        public ActionResult Add()
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                return View();
            }
               
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Products model, List<string> Images, List<int> rDefault, Admin_TokenProducts req)
        {
            var code = new { Success = false, Code = -1, Url = "" };


            if (Images == null || Images.Count == 0)
            {
                code = new { Success = false, Code = -5, Url = "" };
                return Json(code);
            }
            if (req.TocDoCPU == null || req.MangDiDong == null || req.Sim == null || req.Wifi == null)
            {
                code = new { Success = false, Code = -2, Url = "" };
                return Json(code);
            }
            if (!ModelState.IsValid)
            {
                code = new { Success = false, Code = -6, Url = "" };
                return Json(code);
            }

            tb_Staff nvSession = (tb_Staff)Session["user"];
            if (nvSession == null || nvSession.Code == null)
            {
                code = new { Success = false, Code = -4, Url = "" };
                return Json(code);
            }

           
           
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var ProductCategory = db.tb_ProductCategory.Find(req.ProductCategoryId);
                    var ProductCompany = db.tb_ProductCompany.Find(req.ProductCompanyId);
                    if (ProductCategory == null || ProductCompany == null)
                    {
                        code = new { Success = false, Code = -7, Url = "" }; // Mã lỗi mới cho trường hợp không tìm thấy
                        return Json(code);
                    }

                    string abbreviatedCompanyName = AbbreviateCompanyName(ProductCompany.Title);
                    string alias = model.Title.Trim() + "" + ProductCategory.Title.Trim() + "" + ProductCompany.Title.Trim();
                    string products = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(alias);
                    var checkProduct = db.tb_Products.FirstOrDefault(x => x.Alias == products);
                    if (checkProduct != null)
                    {
                        code = new { Success = false, Code = -3, Url = "" };
                        return Json(code);
                    }

                    var checkTitle = db.tb_Products.FirstOrDefault(r => r.Title == req.Title.Trim() && r.ProductCategoryId == req.ProductCategoryId && r.ProductCompanyId == req.ProductCompanyId);
                    if (checkTitle != null)
                    {
                        code = new { Success = false, Code = -3, Url = "" };
                        return Json(code);
                    }

                    if (Images == null || Images.Count == 0)
                    {
                        code = new { Success = false, Code = -5, Url = "" };
                        return Json(code);
                    }

                    var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                    if (checkStaff == null)
                    {
                        code = new { Success = false, Code = -4, Url = "" };
                        return Json(code);
                    }

                    model.CreatedBy = checkStaff.NameStaff.Trim();
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.IsActive = req.IsActive;
                    model.IsHot = req.IsHot;
                    model.IsFeature = req.IsFeature;
                    model.IsSale = req.IsSale;
                    model.IsHome = req.IsHome;

                    model.CPU = req.CPU.Trim();
                    model.GPU = req.GPU.Trim();
                    model.CPUspeed = req.TocDoCPU.Trim();
                    model.BatteryCapacity = req.DungLuongPin;
                    model.OperatingSystem = req.HeDieuHanh.Trim();
                    model.MobileNetwork = req.MangDiDong.Trim();
                    model.Sim = req.Sim.Trim();
                    model.Wifi = req.Wifi.Trim();
                    model.GPS = req.GPS.Trim();
                    model.Bluetooth = req.Bluetooth.Trim();
                    model.Connector = req.CongKetNoi.Trim();
                    model.Headphonejack = req.JackTaiNghe.Trim();
                    model.BatteryType = req.LoaiPin.Trim();
                    model.ChargingSupport = req.HoTroSac.Trim();
                    model.BatteryTechnology = req.CongNghePin.Trim();

                    if (string.IsNullOrEmpty(model.Title))
                    {
                        model.SeoTitle = model.Title.Trim();
                    }
                    if (string.IsNullOrEmpty(model.Alias))
                    {
                        model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(alias);
                    }
                    db.tb_Products.Add(model);
                    db.SaveChanges();

                    for (int i = 0; i < Images.Count; i++)
                    {
                        try
                        {
                            byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(Images[i]));
                            bool isDefault = (i + 1 == rDefault[0]);

                            tb_ProductImage productImage = new tb_ProductImage
                            {
                                ProductsId = model.ProductsId,
                                Image = imageBytes,
                                IsDefault = isDefault
                            };

                            db.tb_ProductImage.Add(productImage);
                            db.SaveChanges();
                        }
                        catch (System.IO.IOException ex)
                        {
                            dbContextTransaction.Rollback();
                            code = new { Success = false, Code = -5, Url = "" };
                            return Json(code);
                        }
                        catch (System.UnauthorizedAccessException ex)
                        {
                            dbContextTransaction.Rollback();
                            code = new { Success = false, Code = -5, Url = "" };
                            return Json(code);
                        }
                        catch (System.Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            code = new { Success = false, Code = -5, Url = "" };
                            return Json(code);
                        }
                    }

                    dbContextTransaction.Commit();
                   return Json ( new { Success = true, Code = 1, Url = "", ProductsId = model.ProductsId });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    // Log lỗi ra nhật ký để kiểm tra sau này
                    // logger.LogError(ex, "An error occurred while adding the product.");
                    code = new { Success = false, Code = -100, Url = "" };
                }
            }

            return Json(code);
        }

        public ActionResult Detail(int id) 
        {
            var items = db.tb_Products.Find(id);
            return View(items);
        }



       
        public ActionResult Edit(int? id)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
                ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
                var item = db.tb_Products.Find(id);
                return View(item);
            }

          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Products model, HttpPostedFileBase newImage) 
        {
            if (ModelState.IsValid)
            {

                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                model.Modifeby = checkStaff.NameStaff + "-" + checkStaff.Code;
                model.IsActive = false;
                model.IsHome = false;
                model.IsFeature = false;
                model.IsSale = false;
                model.IsHot = false;
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                //db.tb_Products.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // Handling image upload
                if (newImage != null && newImage.ContentLength > 0)
                {
                    // Xử lý hình ảnh mới
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(newImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(newImage.ContentLength);
                    }

                   
                    var productImage = db.tb_ProductImage.FirstOrDefault(x => x.ProductsId == model.ProductsId);

                    if (productImage != null)
                    {
                       
                        productImage.Image = imageData;
                        productImage.IsDefault = true;
                        db.Entry(productImage).State = EntityState.Modified;
                    }
                    else
                    {
                        
                        var newProductImage = new tb_ProductImage
                        {
                            ProductsId = model.ProductsId,
                            Image = imageData,
                            IsDefault = true 
                        };
                        db.tb_ProductImage.Add(newProductImage);
                    }
                    db.SaveChanges();
                }



              


                    return RedirectToAction("index");
            }
            return View(model);

        }
        //[HttpPost]
        //public ActionResult AddImage(int productsId, string imageUrl, bool isDefault)
        //{vvvvvvvvvv
        //{vvvvvvvvvv
        //    try
        //    {
        //        // Tạo một tb_ProductImage mới
        //        var newImage = new tb_ProductImage
        //        {
        //            ProductsId = productsId,
        //            Image = imageUrl,
        //            IsDefault = isDefault
        //        };

        //        // Lưu vào cơ sở dữ liệu
        //        db.tb_ProductImage.Add(newImage);
        //        db.SaveChanges();

        //        return Json(new { success = true, message = "Thêm ảnh thành công" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public ActionResult UpdateImage(int productsId,int productImgId , string imageUrl, bool isDefault)
        {
            try
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(imageUrl));
                var productImage = db.tb_ProductImage.FirstOrDefault(pi => pi.ProductsId == productsId && pi.ProductImageId== productImgId);

                // Nếu không tìm thấy ảnh
                if (productImage == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy ảnh để cập nhật" });
                }

                productImage.Image = imageBytes;    
                productImage.IsDefault = isDefault;
                db.Entry(productImage).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật ảnh thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }



  
        [HttpPost]
  
        public ActionResult AddImg(tb_Products model, List<string> Images, List<int> rDefault, int productsId)
        {
            var code = new { Success = true, Code = 1, Url = "" };

            if (Images != null && Images.Count > 0)
            {
                for (int i = 0; i < Images.Count; i++)
                {
                    try
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(Images[i]));
                        bool isDefault = (i + 1 == rDefault[0]);

                        tb_ProductImage productImage = new tb_ProductImage
                        {
                            ProductsId = productsId,
                            Image = imageBytes,
                            IsDefault = isDefault
                        };

                        db.tb_ProductImage.Add(productImage);
                        db.SaveChanges();
                    }
                    catch (System.IO.IOException)
                    {
                        code = new { Success = false, Code = -5, Url = "" };
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        code = new { Success = false, Code = -5, Url = "" };
                    }
                    catch (System.Exception)
                    {
                        code = new { Success = false, Code = -5, Url = "" };
                    }
                }
            }

            return Json(code);
        }


    






        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.tb_Products.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActive });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var items = db.tb_Products.Find(id);
            if (items != null)
            {
                items.IsHome = !items.IsHome;
                db.Entry(items).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isHome = items.IsHome });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsHot(int id)
        {
            var item = db.tb_Products.Find(id);
            if (item != null)
            {
                item.IsHot = !item.IsHot;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isHot = item.IsHot });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsFeature(int id)
        {
            var item = db.tb_Products.Find(id);
            if (item != null)
            {
                item.IsFeature = !item.IsFeature;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isFeature = item.IsFeature });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsSale(int id)
        {
            var item = db.tb_Products.Find(id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isSale = item.IsSale });
            }

            return Json(new { success = false });
        }


        //start IsActive
        [HttpPost]
        public ActionResult UpdateAllIsActive()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        product.IsActive = true;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateAllUnIsActive()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        product.IsActive = false;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult CheckAllIsActive()
        {
            try
            {
                var allClocksActive = true;

                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    // Kiểm tra trạng thái IsActive của sản phẩm
                    if (product.IsActive == true)
                    {
                        allClocksActive = true;
                        break; // Nếu có sản phẩm nào không có IsActive là true, thoát vòng lặp
                    }

                    // Kiểm tra xem có chi tiết sản phẩm nào liên kết với sản phẩm không
                    var hasDetails = db.tb_ProductDetail.Any(detail => detail.ProductsId == product.ProductsId);

                    // Nếu không có chi tiết sản phẩm nào liên kết với sản phẩm
                    if (hasDetails)
                    {
                        allClocksActive = false;
                        break; // Nếu có sản phẩm không có chi tiết sản phẩm liên kết, thoát vòng lặp
                    }
                }

                return Json(new { success = true, allClocksActive = allClocksActive }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // End IsActive

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




        //start IsHome
        [HttpPost]
        public ActionResult UpdateAllIsHome()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        product.IsHome = true;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateAllUnIsHome()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        product.IsHome = false;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult CheckAllIsHome()
        {
            try
            {
                var allIsHome = true;

                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                   
                    if (product.IsHome == true)
                    {
                        allIsHome = true;
                        break; 
                    }
                    var hasDetails = db.tb_ProductDetail.Any(detail => detail.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        allIsHome = false;
                        break; 
                    }
                }

                return Json(new { success = true, allIsHome = allIsHome }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // End IsActive






        //start IsHot
        [HttpPost]
        public ActionResult UpdateAllIsHot()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        product.IsHot = true;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateAllUnIsHot()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        product.IsHot = false;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult CheckAllIsHot()
        {
            try
            {
                var allIsHot = true;

                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {

                    if (product.IsHot == true)
                    {
                        allIsHot = true;
                        break;
                    }
                    var hasDetails = db.tb_ProductDetail.Any(detail => detail.ProductsId == product.ProductsId);
                    if (hasDetails)
                    {
                        allIsHot = false;
                        break;
                    }
                }

                return Json(new { success = true, allIsHot = allIsHot }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // End IsActive
        //start IsSale
        [HttpPost]
        public ActionResult UpdateAllIsSale()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId && pd.PriceSale>0);
                    if (hasDetails)
                    {
                        product.IsSale = true;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateAllUnIsSale()
        {
            try
            {
                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId && pd.PriceSale > 0);
                    if (hasDetails)
                    {
                        product.IsSale = false;
                        db.Entry(product).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult CheckAllIsSale()
        {
            try
            {
                var allIsSale = true;

                var products = db.tb_Products.ToList();

                foreach (var product in products)
                {

                    if (product.IsSale == true)
                    {
                        allIsSale = true;
                        break;
                    }
                    var hasDetails = db.tb_ProductDetail.Any(pd => pd.ProductsId == product.ProductsId && pd.PriceSale > 0);
                    if (hasDetails)
                    {
                        allIsSale = false;
                        break;
                    }
                }

                return Json(new { success = true, allIsSale = allIsSale }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // End IsActive
    }
}