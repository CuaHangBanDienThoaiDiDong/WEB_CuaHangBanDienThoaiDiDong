using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ProductDetailImageController : Controller
    {
        // GET: Admin/ProductDetailImage
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult DeleteImg(int id)
        {
            var item = db.tb_ProductDetailImage.Find(id);
            db.tb_ProductDetailImage.Remove(item);
            db.SaveChanges();
            return Json(new { success = true });
        }



    }
}