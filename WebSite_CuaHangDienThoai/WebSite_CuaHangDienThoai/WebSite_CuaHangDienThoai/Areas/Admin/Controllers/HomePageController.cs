using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class HomePageController : Controller
    {
        // GET: Admin/HomePage
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    ViewBag.Date = DateTime.Today;
                    var items = db.tb_Staff.OrderByDescending(x => x.Code).ToList();
                    return View(items);
                }
            }
        }
        public ActionResult NonRole()
        {
            return View();
        }

        public ActionResult CountClient()
        {
            var customer = db.tb_Customer.ToList();
            if (customer != null)
            {
                ViewBag.Count = customer.Count;
                return PartialView();
            }
            return PartialView();
        }
        public ActionResult CountStaff()
        {
            var customer = db.tb_Staff.ToList();
            if (customer != null)
            {
                ViewBag.Count = customer.Count;
                return PartialView();
            }
            return PartialView();
        }
        public ActionResult WareHouseImportExportToday()
        {
            // Lấy ngày hôm nay
            DateTime selectedDate = DateTime.Today;
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1);

            var ImportWarehouse = db.tb_ImportWarehouse
                .Where(o => o.CreateDate >= startDate && o.CreateDate < endDate)
                .ToList();

            if (ImportWarehouse != null && ImportWarehouse.Count > 0)
            {
                ViewBag.Date = selectedDate;
                ViewBag.Count = ImportWarehouse.Count;
                return PartialView();
            }
            else
            {
                return PartialView();
            }
        }

        public ActionResult CountFunction() 
        {
                var function=db.tb_Function.Count();
           
                ViewBag.Count=function;
                return PartialView( );

            
            

        }


        public ActionResult ShowCountOrderNew()
        {

            var ordernew = db.tb_Order.Count(x => x.Confirm ==  false);
           

                return Json(new { Count = ordernew }, JsonRequestBehavior.AllowGet);
           

           
        }
        public ActionResult GetOrderExportDay()
        {
            // Lấy ngày hôm nay
            DateTime selectedDate = DateTime.Today;
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1);

            var orders = db.tb_ExportWareHouse
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

           
                ViewBag.Date = selectedDate;
                ViewBag.Count = orders.Count;
                return Json(new { Count = orders }, JsonRequestBehavior.AllowGet);
           
        }
        public ActionResult CountSellerToday()
        {
            // Lấy ngày hôm nay
            DateTime selectedDate = DateTime.Today;
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1);

            var SellerToDay = db.tb_Seller
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

            if (SellerToDay != null)
            {
                return Json(new { Count = SellerToDay }, JsonRequestBehavior.AllowGet);

            }
            else 
            {
                return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}