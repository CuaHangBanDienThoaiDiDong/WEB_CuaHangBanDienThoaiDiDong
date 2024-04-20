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