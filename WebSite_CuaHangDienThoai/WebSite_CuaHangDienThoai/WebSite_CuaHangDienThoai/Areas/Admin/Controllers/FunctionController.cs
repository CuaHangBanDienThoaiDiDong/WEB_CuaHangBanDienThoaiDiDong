using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Admin;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class FunctionController : Controller
    {
        // GET: Admin/Function
        CUAHANGDIENTHOAIEntities db =new CUAHANGDIENTHOAIEntities();    
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.NhanVienId == nvSession.NhanVienId && (row.IdChucNang == 1 || row.IdChucNang == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    var items = db.tb_Function.ToList().OrderByDescending(x => x.IdChucNang);
                    if (items == null)
                    {
                        ViewBag.txt = "Không tồn tại chức năng";
                    }

                    return View(items);
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
                var CheckRole = db.tb_Role.SingleOrDefault(row => row.NhanVienId == nvSession.NhanVienId && (row.IdChucNang == 1 || row.IdChucNang == 2));
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
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.MSNV == nvSession.MSNV);
                model.MaChucNang = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.TenChucNang);
                model.ModifiedDate = DateTime.Now;
                model.Modifeby = checkStaff.TenNhanVien + "-" + checkStaff.MSNV;
                db.tb_Function.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View();
        }


        public ActionResult Add() 
        {
            return View();
        }
        public ActionResult Partail_AddFunction()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Admin_TokenFunction req, tb_Function model)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.Title != null)
            {
                if (ModelState.Any())
                {
                    var checkFunction = db.tb_Function.Where(x => x.TenChucNang == req.Title).ToList();
                    if (checkFunction.Count < 0)
                    {
                        model.TenChucNang = req.Title;
                        model.MaChucNang = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Title);
                        model.CreatedDate = DateTime.Now;
                        db.tb_Function.Add(model);
                        db.SaveChanges();
                        code = new { Success = true, Code = 1, Url = "" };

                    }
                    else
                    { /*Tên sản chức năng đã tồn tại*/
                        code = new { Success = false, Code = -3, Url = "" };
                    }
                }
            }
            else 
            {
                /*Tên sản chức năng đã tồn tại*/
                code = new { Success = false, Code = -2, Url = "" };
            }
        

            return Json(code);
        }

    }
}