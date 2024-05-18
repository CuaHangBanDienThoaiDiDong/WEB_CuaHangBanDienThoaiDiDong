using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class WareHouseImportController : Controller
    {
        // GET: Admin/WareHouseImport
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_ProductDetail> items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId);
                if (items != null)
                {
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
                else
                {
                    ViewBag.txt = "Không tồn tại sản phẩm";
                    return View();
                }
            }
        }

        public ActionResult Partail_ProductDetail()
        {
            var item=db.tb_ProductDetail.ToList();
            return View(item);  
        }











        public ActionResult Partial_QuantityProductDtail(int id) {
            var item = db.tb_ImportWarehouse.FirstOrDefault(x => x.ProductDetailId == id);
            if (item != null) 
            {
                ViewBag.Quantity = item.QuanTity;
                return PartialView(item);
            }
            return PartialView();
           
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






        public ActionResult Partial_AddQuantityForProduct(int id ) 
        {
            return PartialView();
        }
        public ActionResult AddQuantityForProduct(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_ProductDetail.Find(id);

                if (item == null)
                {

                    return RedirectToAction("Index", "Store");
                }
                return View(item);
            }
                
        }







    }
}