using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;


namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class FunctionController : Controller
    {
        // GET: Admin/Function
        CUAHANGDIENTHOAIEntities db =new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
        {


            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {


                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }
                else
                {

                    tb_Staff nvSession = (tb_Staff)Session["user"];
                    var check = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                    if (check == null)
                    {
                        return RedirectToAction("NonRole", "HomePage");
                    }
                    else
                    {
                        IEnumerable<tb_Function> items = db.tb_Function.OrderByDescending(x => x.FunctionId);
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
                            return View(items);
                        }
                        else
                        {
                            ViewBag.txt = "Không tồn tại chức năng";
                            return View();
                        }

                    }

                }





            }

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

                    var item = db.tb_Function.Find(id);
                    return View(item);
                }
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Function model)
        {
            if (ModelState.IsValid)
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.TitLe);
                model.ModifiedDate = DateTime.Now;
                model.Modifeby = checkStaff.NameStaff + "-" + checkStaff.Code;
                db.tb_Function.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View();
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
                var CheckRole = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (CheckRole == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                 
                    return View();
                }
            }
        }
        public ActionResult Partail_AddFunction()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Function model, Admin_TokenFunction req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                if (nvSession != null)
                {
                    var checkFunction = db.tb_Function.FirstOrDefault(x => x.TitLe == req.Name.Trim());
                    if (checkFunction == null)
                    {
                        var item = db.tb_Staff.SingleOrDefault(row => row.StaffId == nvSession.StaffId);
                        model.TitLe=req.Name.Trim();    
                        model.CreatedBy = item.NameStaff;
                        model.CreatedDate = DateTime.Now;
                        if (string.IsNullOrEmpty(model.Alias))
                        {
                            model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.TitLe);
                        }
                        db.tb_Function.Add(model);
                        db.SaveChanges();
                        code = new { Success = true, Code = 1, Url = "" };
                    }
                    else
                    {
                        code = new { Success = false, Code = -3, Url = "" };// Da ton tia
                    }
                }
                else
                {
                    code = new { Success = false, Code = -2, Url = "" };// Khoong tim nhan vin
                }
            }
            return Json(code);
        }
        public ActionResult Details(int id)
        {
            var checkIdPQuyen = db.tb_Function.Find(id);
            if (checkIdPQuyen != null) 
            {
                ViewBag.item = checkIdPQuyen.FunctionId;
                return View(checkIdPQuyen);
            }
            return View();
          
        }


        public ActionResult Partial_SatffOfFuntion(int id)
        {
            var checkIdPQuyen = db.tb_Staff.Where(row => row.FunctionId == id).ToList();
            if (checkIdPQuyen != null) 
            {
                ViewBag.Count=checkIdPQuyen.Count; return PartialView(checkIdPQuyen);  
            }
            return PartialView();
        }






        
    }
}