using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        // GET: Admin/Statistical
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1));
                if (item == null)
                {
                    ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                    return View();
                }

            }
        }







        public ActionResult StatisticalByDay()
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

        public ActionResult StatisticalByMon()
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
                    ViewBag.Date = DateTime.Now.Month.ToString("D2");

                    return View();
                }

            }
        }
        public ActionResult StatisticalByYear()
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
                    return View();
                }
            }
        }

        public ActionResult Partial_StatisticalByYear(int year) 
        {
           ViewBag.Year=year;
            return PartialView();
        }

        //thong ke theo ngay

        [HttpGet]
        public ActionResult GetStatisticalByDay(string fromDate, string toDate)
        {


            var loinhuan = from a in db.tb_Order
                           join b in db.tb_OrderDetail
                           on a.OrderId equals b.OrderId
                           join c in db.tb_ProductDetail
                           on b.ProductDetailId equals c.ProductDetailId
                         
                           where (a.typeOrder == false)
                           select new
                           {
                               CreatedDate = a.CreatedDate,
                               Quantity = b.Quantity,
                               Price = a.TotalAmount,
                               OriginalPrice = c.OrigianlPrice

                           };




            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                loinhuan = loinhuan.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                loinhuan = loinhuan.Where(x => x.CreatedDate < endDate);
            }

            var result = loinhuan.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy,
                TienGoc=x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }



        //thong ke theo tháng

        [HttpGet]
        public ActionResult GetStatisticalByMon(string fromDate, string toDate)
        {
            var loinhuan = from a in db.tb_Order
                           join b in db.tb_OrderDetail on a.OrderId equals b.OrderId
                           join c in db.tb_ProductDetail on b.ProductDetailId equals c.ProductDetailId

                           where (a.typeOrder == false)
                           select new
                           {
                               CreatedDate = a.CreatedDate,
                               Quantity = b.Quantity,
                               Price = a.TotalAmount,
                               OriginalPrice = c.OrigianlPrice
                           };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                loinhuan = loinhuan.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                loinhuan = loinhuan.Where(x => x.CreatedDate < endDate.AddDays(1));
            }

            var result = loinhuan.GroupBy(x => new { x.CreatedDate.Year, x.CreatedDate.Month })
                .Select(x => new
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                    TotalSell = x.Sum(y=>y.Price),
                })
                .Select(x => new
                {
                    Year = x.Year,
                    Month = x.Month,
                    DoanhThu = x.TotalSell,
                    LoiNhuan = x.TotalSell - x.TotalBuy,
                    TienGoc=x.TotalBuy
                });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetYearlyStatistical(int selectedYear)
        {
            // Lấy tất cả các đơn hàng
            var allOrders = db.tb_Order.ToList();
            // Lấy tất cả các chi tiết đơn hàng
            var allOrderDetails = db.tb_OrderDetail.ToList();
            // Lấy tất cả các chi tiết sản phẩm
            var allProductDetails = db.tb_ProductDetail.ToList();

            // Log số lượng bản ghi để kiểm tra
            System.Diagnostics.Debug.WriteLine("Total Orders: " + allOrders.Count);
            System.Diagnostics.Debug.WriteLine("Total Order Details: " + allOrderDetails.Count);
            System.Diagnostics.Debug.WriteLine("Total Product Details: " + allProductDetails.Count);

            var salesData = from a in db.tb_Order
                            join b in db.tb_OrderDetail on a.OrderId equals b.OrderId
                            join c in db.tb_ProductDetail on b.ProductDetailId equals c.ProductDetailId
                            where a.typeOrder == false && a.CreatedDate.Year == selectedYear
                            select new
                            {
                                a.CreatedDate.Year,
                                a.Quantity,
                                a.TotalAmount,
                                c.OrigianlPrice
                            };

    
            System.Diagnostics.Debug.WriteLine("Filtered Sales Data: " + salesData.Count());

            var result = salesData.AsEnumerable()
                                  .GroupBy(x => x.Year)
                                  .Select(x => new
                                  {
                                      Year = x.Key,

                                      TotalRevenue = x.Sum(a => a.TotalAmount),
                                      TotalCost = x.Sum(y => y.Quantity * y.OrigianlPrice)
                           
                                  })
                                  .Select(x => new
                                  {
                                      x.Year,
                                      DoanhThu = x.TotalRevenue,
                                      LoiNhuan = x.TotalRevenue - x.TotalCost,
                                      TienGoc= x.TotalCost
                                  }).ToList();

        
            System.Diagnostics.Debug.WriteLine("Result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }





    }
}