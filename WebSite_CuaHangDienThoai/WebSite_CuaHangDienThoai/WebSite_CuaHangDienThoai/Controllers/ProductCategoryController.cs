using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ProductCategoryController : Controller
    {
        // GET: ProductCategory
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Partial_Category() 
        {



            var item = db.tb_ProductCategory.Where(r=>r.IsActive==true).ToList();
            return PartialView(item); 
        }   




        public ActionResult Detail(int id)
        {
            var item = db.tb_Products.ToList();
            if(id>0 ) 
            {
                item=item.Where(r=>r.ProductCategoryId==id && r.IsActive==true).ToList();   
            }
            var cate=db.tb_ProductCategory.Find(id);
            if (cate != null) 
            {
                ViewBag.cate = cate;    
            }
            ViewBag.CateId = id;
            return View(item);
        }









    }
}