using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                if (nvSession == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    ViewBag.Date = DateTime.Today.Date;
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
            var function = db.tb_Function.Count();

            ViewBag.Count = function;
            return PartialView();




        }


        public ActionResult ShowCountOrderNew()
        {

            var ordernew = db.tb_Order.Count(x => x.Confirm == false);


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

            // Lấy danh sách Seller của ngày hôm nay
            var SellerToDay = db.tb_Seller
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

           
            int sellerCount = SellerToDay.Count;

         
            return Json(new { Count = sellerCount }, JsonRequestBehavior.AllowGet);
        }


        //Thong Ke 
        public ActionResult Partial_ThongKe()
        {

            return PartialView();
        }


        //start tabs thong ke theo ngay
        public ActionResult Partial_TabStatisticalByDay()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {

                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1));
                if (item == null)
                {

                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    return View();
                }

            }
        }

        //End tabs thong ke theo ngay


        //start tabs thong ke theo tháng


        public ActionResult Partial_TabStatisticalByMon()
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
                    var uniqueMonth = (from order in db.tb_Order
                                       select order.CreatedDate.Month).Distinct();
                    SelectList yearList = new SelectList(uniqueMonth);

                    // Đặt SelectList vào ViewBag để truyền sang view
                    ViewBag.UniqueYears = yearList;
                    return View("NonRole");
                }
                else
                {
                    var uniqueMonth = (from order in db.tb_Order
                                       select order.CreatedDate.Month).Distinct();
                    SelectList MonthList = new SelectList(uniqueMonth);

                    // Đặt SelectList vào ViewBag để truyền sang view
                    ViewBag.UniqueMonth = MonthList;

                    ViewBag.SelectedMonth = ""; //
                    ViewBag.Month = DateTime.Now.Month.ToString();
                    return PartialView();
                }
            }
        }


        public ActionResult Partial_StatisticalByMonAll()
        {

            return PartialView();
        }
        public ActionResult Partial_StatisticalByMon(int month)
        {
            ViewBag.Month = month;
            return PartialView();
        }



        //End tabs thong ke theo tháng
        //Start tabs thong ke theo Năm

        public ActionResult Partial_TabStatisticalByYear()
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
                    var uniqueYears = (from order in db.tb_Order
                                       select order.CreatedDate.Year).Distinct();
                    SelectList yearList = new SelectList(uniqueYears);

                    // Đặt SelectList vào ViewBag để truyền sang view
                    ViewBag.UniqueYears = yearList;
                    return View("NonRole");
                }
                else
                {
                    var uniqueYears = (from order in db.tb_Order
                                       select order.CreatedDate.Year).Distinct();
                    SelectList yearList = new SelectList(uniqueYears);

                    // Đặt SelectList vào ViewBag để truyền sang view
                    ViewBag.UniqueYears = yearList;

                    // Đặt SelectedYear mặc định nếu cần thiết
                    ViewBag.SelectedYear = ""; //
                    ViewBag.Year = DateTime.Now.Year.ToString();
                    return PartialView();
                }
            }
        }

        public ActionResult Partial_StatisticalByYear(int year)
        {
            ViewBag.Year = year;
            return PartialView();
        }


        //End tabs thong ke theo Năm



        //EndThong Ke 



    }
}