using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Admin;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ProductCategoryController : Controller
    {

        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Admin/ProductCategory
        public ActionResult Index()
        {

            var items = db.tb_ProductCategory.ToList().OrderByDescending(x => x.ProductCategoryId);
            if (items == null)
            {
                ViewBag.txt = "Không Có Loại Sản Phẩm ";
            }

            return View(items);
        }




        public ActionResult Partial_AddProductCategory() 
        {
            return PartialView();
        }





        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_ProductCategory model, Admin_TokenProductCategory req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.Title != null) {
                var title = db.tb_ProductCategory.FirstOrDefault(r => r.Title == req.Title);
                if (title == null) 
                {
                    if (req.Image != null) 
                    {
                        try
                        {
                            if (model.Title != null)
                            {
                                model.Icon = req.Image;
                                model.CreatedDate = DateTime.Now;
                                model.ModifiedDate = DateTime.Now;
                                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                                db.tb_ProductCategory.Add(model);
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ViewBag.txt = "Vui lòng nhập thông tin";
                                return View();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Xử lý ngoại lệ nếu có
                            ViewBag.Error = "Đã xảy ra lỗi khi thêm mới loại sản phẩm: " + ex.Message;
                        }
                    }
                    else
                    {
                        // Chọn ảnh đại diện
                        code = new { Success = false, Code = -4, Url = "" };
                    }   
                }
                else
                {
                    // Tên đã tồn tại
                    code = new { Success = false, Code = -3, Url = "" };
                }
            }
            else {
                // Vui lòng điền tên tiêu đề
                code = new { Success = false, Code = -2, Url = "" };
            }
            return Json(code);
        }

        public ActionResult Edit(int id)
        {
            var item = db.tb_ProductCategory.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                db.tb_ProductCategory.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = db.tb_ProductCategory.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActive });
            }

            return Json(new { success = false });
        }
    }
}