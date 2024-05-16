
using PagedList;
using System;
using System.Collections.Generic;
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
               
        }

        public ActionResult Partial_AddProduct()
        {
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
            ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
            return PartialView();
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
            tb_Staff nvSession = (tb_Staff)Session["user"];
            if (nvSession.Code != null)
            {
                if (req.TocDoCPU != null && req.MangDiDong != null && req.Sim != null && req.Wifi != null)
                {
                    var ProductCategory = db.tb_ProductCategory.Find(req.ProductCategoryId);
                    var ProductCompany = db.tb_ProductCompany.Find(req.ProductCompanyId);

                    string alias = model.Title.Trim() + "" + ProductCategory.Title.Trim() + "" + ProductCompany.Title.Trim();

                    var checkTitle = db.tb_Products.SingleOrDefault(r => r.Title == req.Title && r.Alias == alias && r.ProductCategoryId == req.ProductCategoryId && r.ProductCompanyId == req.ProductCompanyId);
                    if (checkTitle == null)
                    {
                        if (Images != null)
                        {
                            if (ModelState.IsValid)
                            {
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
                                                ProductsId = model.ProductsId,
                                                Image = imageBytes,
                                                IsDefault = isDefault
                                            };

                                            db.tb_ProductImage.Add(productImage);
                                        }
                                        catch (System.IO.IOException ex)
                                        {
                                            code = new { Success = false, Code = -5, Url = "" };
                                        }
                                        catch (System.UnauthorizedAccessException ex)
                                        {
                                            code = new { Success = false, Code = -5, Url = "" };
                                        }
                                        catch (System.Exception ex)
                                        {
                                            code = new { Success = false, Code = -5, Url = "" };
                                        }
                                    }

                                }

                                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                                model.CreatedBy = checkStaff.NameStaff + "-" + checkStaff.Code;
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
                                model.Sim = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Sim.Trim());
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
                                code = new { Success = true, Code = 1, Url = "" };
                            }
                        }
                        else
                        {
                            // Không tìm thấy ảnh
                            code = new { Success = false, Code = -5, Url = "" };
                        }
                    }
                    else
                    {
                        // Sản phẩm đã tồn tại
                        code = new { Success = false, Code = -3, Url = "" };
                    }
                }
                else
                {
                    // Điền không đầy đủ thông tin
                    code = new { Success = false, Code = -2, Url = "" };
                }
            }
            else
            {
                // Không tìm thấy session
                code = new { Success = false, Code = -4, Url = "" };
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
        public ActionResult Edit(tb_Products model)
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
                db.tb_Products.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(model);

        }

        //[HttpPost]
        //public ActionResult UpdateImage(int productsId, string newImageUrl)
        //{
        //    try
        //    {
        //        // Tìm tb_ProductImage tương ứng với ProductsId
        //        var productImage = db.tb_ProductImage.FirstOrDefault(pi => pi.ProductsId == productsId);

        //        // Nếu tồn tại tb_ProductImage, cập nhật đường dẫn ảnh mới
        //        if (productImage != null)
        //        {
        //            productImage.Image = newImageUrl;
        //            db.SaveChanges();
        //            return Json(new { success = true, message = "Cập nhật ảnh thành công" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Không tìm thấy ảnh để cập nhật" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
        //    }
        //}
        //public ActionResult Edit(tb_Products model)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        tb_Staff nvSession = (tb_Staff)Session["user"];
        //        var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
        //        model.Modifeby = checkStaff.NameStaff + "-" + checkStaff.Code;
        //        model.IsActive = false;
        //        model.IsHome = false;
        //        model.IsFeature = false;
        //        model.IsSale = false;
        //        model.IsHot = false;
        //        model.ModifiedDate = DateTime.Now;
        //        model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
        //        db.tb_Products.Add(model);
        //        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("index");
        //    }
        //    return View(model);

        //}







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




    }
}