using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        // GET: Admin/Role
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
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
                    var item = db.tb_Function.ToList();
                    return View(item);
                }

            }

        }
        public ActionResult Details(int id)
        {
            var checkIdPQuyen = db.tb_Function.Find(id);
            return View(checkIdPQuyen);
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
        public ActionResult Partial_AddRole() 
        {
            return PartialView();   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Function model , Admin_TokenFunction req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                if (nvSession != null)
                {
                    var checkFunction = db.tb_Function.FirstOrDefault(x => x.TitLe == req.Name);
                      if(checkFunction==null) 
                    {
                        var item = db.tb_Staff.SingleOrDefault(row => row.StaffId == nvSession.StaffId);
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
            return View();
        }


        public ActionResult Edit(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Function.Find(id);
                return View(item);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Function model)
        {
            if (ModelState.IsValid)
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Staff.SingleOrDefault(row => row.StaffId == nvSession.StaffId);
                model.ModifiedDate = DateTime.Now;
                model.Modifeby = item.NameStaff;

                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Alias);

                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }



    }
}