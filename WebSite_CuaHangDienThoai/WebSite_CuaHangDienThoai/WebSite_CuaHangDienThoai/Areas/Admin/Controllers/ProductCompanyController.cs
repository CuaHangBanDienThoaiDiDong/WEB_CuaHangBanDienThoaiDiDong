using PagedList;
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
    public class ProductCompanyController : Controller
    {
        // GET: Admin/ProductCompany
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
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    IEnumerable<tb_ProductCompany> items = db.tb_ProductCompany.OrderByDescending(x => x.ProductCompanyId);
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
             }


             

        }



        public ActionResult Partial_AddProductComapny()
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
        public ActionResult Add(tb_ProductCompany model, Admin_TokenProductComany req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.Title != null)
            {
                var title = db.tb_ProductCompany.FirstOrDefault(r => r.Title == req.Title);
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
                            db.tb_ProductCompany.Add(model);
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

        public ActionResult Edit(int? id)
        {



            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkRole = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (checkRole == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    var item = db.tb_ProductCompany.Find(id);
                    return View(item);
                }
            }

          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ProductCompany model, HttpPostedFileBase newImage)
        {
            if (ModelState.IsValid)
            {
                tb_ProductCompany existingProductCompany = db.tb_ProductCompany.Find(model.ProductCompanyId);
                if (existingProductCompany == null)
                {
                    return HttpNotFound();
                }

                existingProductCompany.Title = model.Title;
                existingProductCompany.ModifiedDate = DateTime.Now;
                existingProductCompany.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);

                // Kiểm tra xem có hình ảnh mới được tải lên không
                if (newImage != null && newImage.ContentLength > 0)
                {
                    // Xử lý hình ảnh mới
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(newImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(newImage.ContentLength);
                    }
                    existingProductCompany.Icon = imageData;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(tb_ProductCompany model)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        model.ModifiedDate = DateTime.Now;
        //        model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
        //        db.tb_ProductCompany.Attach(model);
        //        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("index");
        //    }
        //    return View();
        //}
    }
}