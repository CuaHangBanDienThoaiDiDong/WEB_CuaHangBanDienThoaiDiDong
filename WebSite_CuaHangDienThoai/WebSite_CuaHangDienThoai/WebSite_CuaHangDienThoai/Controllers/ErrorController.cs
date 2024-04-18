using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            // Xử lý lỗi 404 - Not Found
            //return Content("404 - Không tìm thấy trang");
            return View();  
        }

        // Action xử lý lỗi 500 hoặc các lỗi khác
        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}