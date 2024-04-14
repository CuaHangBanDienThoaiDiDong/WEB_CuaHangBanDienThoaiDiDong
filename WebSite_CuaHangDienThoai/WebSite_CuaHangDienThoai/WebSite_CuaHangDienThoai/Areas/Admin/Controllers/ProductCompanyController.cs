using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ProductCompanyController : Controller
    {
        // GET: Admin/ProductCompany
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        public ActionResult Index(int? page)
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






        public ActionResult Add()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_ProductCompany model)
        {
            if (ModelState.IsValid)
            {
                if (model.Title != null)
                {
                    model.CreatedDate = DateTime.Now;
                    model.ModifiedDate = DateTime.Now;
                    model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                    db.tb_ProductCompany.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.txt = "Vui lòng nhập thông tin";
                    return View();
                }

            }

            return View();
        }

        public ActionResult Edit(int? id)
        {
            var item = db.tb_ProductCompany.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ProductCompany model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Title);
                db.tb_ProductCompany.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View();
        }
    }
}