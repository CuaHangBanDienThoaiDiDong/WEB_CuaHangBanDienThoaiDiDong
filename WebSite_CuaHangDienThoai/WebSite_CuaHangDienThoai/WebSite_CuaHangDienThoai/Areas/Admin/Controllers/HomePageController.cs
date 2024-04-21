using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class HomePageController : Controller
    {
        // GET: Admin/HomePage
        public ActionResult Index()
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
        public ActionResult NonRole()
        {
            return View();
        }
    }
}