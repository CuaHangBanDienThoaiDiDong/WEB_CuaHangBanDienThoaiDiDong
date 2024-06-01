using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class AccountClientController : Controller
    {
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   
        // GET: Admin/AccountClient
        public ActionResult Index()
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

                    var items = db.tb_Customer.ToList ();
                    if (items!=null)
                    {
                        ViewBag.Count = items.Count();
                        return View(items);
                    }
                    return View();
                }

            }
        }

        public ActionResult Partail_Index(int? page) 
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
                    IEnumerable<tb_Customer> items = db.tb_Customer.OrderByDescending(x => x.CustomerId);
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    ViewBag.PageSize = pageSize;
                    ViewBag.Page = page;
                    ViewBag.Count = items.Count();
                    return View(items);
                }

            }
        }




        [HttpPost]
        public ActionResult IsLock(int id)
        {
            var item = db.tb_Customer.Find(id);
            if (item != null)
            {
                item.Clock = !item.Clock;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.Clock });
            }

            return Json(new { success = false });
        }
    }
}