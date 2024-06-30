using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections.Generic;
using System.IO;
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

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap","Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                    var items = db.tb_ProductCategory.ToList().OrderByDescending(x => x.ProductCategoryId);
                    if (items == null)
                    {
                        ViewBag.txt = "Không Có Loại Sản Phẩm ";
                    }

                    return View(items);
                }
            }


        }




        public ActionResult Partial_AddProductCategory() 
        {
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
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                    return View();
                }
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_ProductCategory model, Admin_TokenProductCategory req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.Title != null)
            {
                var title = db.tb_ProductCategory.FirstOrDefault(r => r.Title == req.Title);
                if (title == null)
                {
                    if (req.Image != null)
                    {
                        
                            if (model.Title != null)
                            {
                                tb_Staff nvSession = (tb_Staff)Session["user"];
                                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                                model.CreatedBy = checkStaff.NameStaff + "-" + checkStaff.Code;
                                string physicalPath = Server.MapPath(req.Image);
                                byte[] imageBytes = System.IO.File.ReadAllBytes(physicalPath);

                                // Lưu trữ mảng byte vào thuộc tính Icon
                                model.Icon = imageBytes;
                                model.CreatedDate = DateTime.Now;
                                model.ModifiedDate = DateTime.Now;
                                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                                db.tb_ProductCategory.Add(model);
                                db.SaveChanges();
                                code = new { Success = true, Code = 1, Url = "" };
                            }
                            else
                            {
                                ViewBag.txt = "Vui lòng nhập thông tin";
                                return View();
                            }
                        }
                      
                            //code = new { Success = false, Code = -4, Url = "" };
                            //ViewBag.Error = "Đã xảy ra lỗi khi thêm mới loại sản phẩm: " + ex.Message;
                        
                    
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
            else
            {
                // Vui lòng điền tên tiêu đề
                code = new { Success = false, Code = -2, Url = "" };
            }
            return Json(code);
        }







        public ActionResult Edit(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var CheckRole = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (CheckRole == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                    var item = db.tb_ProductCategory.Find(id);
                    return View(item);
                }
            }

           
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ProductCategory model, HttpPostedFileBase newImage)
        {
            if (ModelState.IsValid)
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                model.Modifiedby = checkStaff.NameStaff + "-" + checkStaff.Code;
                model.IsActive = false;
                db.tb_ProductCategory.Attach(model);
                if (newImage != null && newImage.ContentLength > 0)
                {
                    // Xử lý hình ảnh mới
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(newImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(newImage.ContentLength);
                    }
                    model.Icon = imageData;
                }
                else 
                {
                    model.Icon = model.Icon;
                }
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