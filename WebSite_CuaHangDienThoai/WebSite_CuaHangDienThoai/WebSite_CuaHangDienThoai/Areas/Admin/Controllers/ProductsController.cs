
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

        public ActionResult Partial_AddProduct()
        {
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
            ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
            return PartialView();
        }


        public ActionResult Add()
        {
            //ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
            //ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Products model, List<string> Images, List<int> rDefault, Admin_TokenProducts req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.TocDoCPU != null && req.MangDiDong != null && req.Sim != null && req.Wifi != null)
            {
               
               

                var checkTitle = db.tb_Products.FirstOrDefault(r => r.Title == req.Title &&r.ProductCategoryId==req.ProductCategoryId&&r.ProductCompanyId==req.ProductCompanyId);
                if (checkTitle == null)
                {
                    if (ModelState.IsValid)
                    {
                        if (Images != null && Images.Count > 0)
                        {
                            for (int i = 0; i < Images.Count; i++)
                            {
                                if (i + 1 == rDefault[0])
                                {
                                    model.Image = Images[i];
                                    db.tb_ProductImage.Add(new tb_ProductImage
                                    {
                                        ProductsId = model.ProductsId,
                                        Image = Images[i],
                                        IsDefault = true
                                    });
                                }
                                else
                                {
                                    db.tb_ProductImage.Add(new tb_ProductImage
                                    {
                                        ProductsId = model.ProductsId,
                                        Image = Images[i],
                                        IsDefault = true
                                    });
                                }
                            }
                        }
                        //tb_NhanVien nvSession = (tb_NhanVien)Session["user"];
                        model.CreatedDate = DateTime.Now;
                        model.ModifiedDate = DateTime.Now;
                        model.IsActive = req.IsActive;
                        model.IsHot = req.IsHot;
                        model.IsFeature = req.IsFeature;
                        model.IsSale = req.IsSale;
                        model.IsHome = req.IsHome;

                        model.CPU=req.CPU;
                        model.GPU=req.GPU;
                        model.TocDoCPU = req.TocDoCPU;

                        model.HeDieuHanh = req.HeDieuHanh;
                        model.MangDiDong = req.MangDiDong;
                        model.Sim = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Sim);
                        model.Wifi = req.Wifi;
                        model.GPS = req.GPS;
                        model.Bluetooth = req.Bluetooth;
                        model.Bluetooth = req.Bluetooth;
                        model.CongKetNoi = req.CongKetNoi;
                        model.JackTaiNghe = req.JackTaiNghe;
                        model.LoaiPin = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.LoaiPin);
                        model.HoTroSac = req.HoTroSac;
                        model.CongNghePin = req.CongNghePin;
                        if (string.IsNullOrEmpty(model.Title))
                        {
                            model.SeoTitle = model.Title;
                        }
                        if (string.IsNullOrEmpty(model.Alias))
                        {
                            model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                        }
                        db.tb_Products.Add(model);
                        db.SaveChanges();
                        code = new { Success = true, Code = 1, Url = "" };


                    }
                }
                else
                {//san pham da ton tai
                    code = new { Success = false, Code = -3, Url = "" };

                }
            }
            else //Dien ko day du thong tin
            {
                code = new { Success = false, Code = -2, Url = "" };
            }
            //ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
            //ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
            return Json(code);
            //return RedirectToAction("AddProductDetail", "ProductDetailController", new { productId = newProductId });
        }


        public ActionResult Detail(int id) 
        {
            var items = db.tb_Products.Find(id);
            return View(items);
        }



        public ActionResult Edit(int? id)
        {
            ViewBag.ProductCategory = new SelectList(db.tb_ProductCategory.ToList(), "ProductCategoryId", "Title");
            ViewBag.ProductCompany = new SelectList(db.tb_ProductCompany.ToList(), "ProductCompanyId", "Title");
            var item = db.tb_Products.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Products model)
        {
            if (ModelState.IsValid)
            {
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