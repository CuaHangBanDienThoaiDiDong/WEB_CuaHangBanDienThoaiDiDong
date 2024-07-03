using DocumentFormat.OpenXml.Bibliography;
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

        //THống kê theo năm
        public ActionResult StatisticalByYear()
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
                    return View();
                }
            }
        }
        public ActionResult Partial_StatisticalByYear(int year)
        {
            ViewBag.Year = year;
            return PartialView();
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

                                      TotalRevenue = x.Average(a => a.TotalAmount),
                                      TotalCost = x.Average(y => y.Quantity * y.OrigianlPrice)

                                  })
                                  .Select(x => new
                                  {
                                      x.Year,
                                      DoanhThu = x.TotalRevenue,
                                      LoiNhuan = x.TotalRevenue - x.TotalCost,
                                      TienGoc = x.TotalCost
                                  }).ToList();


            System.Diagnostics.Debug.WriteLine("Result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        // End THống kê theo năm



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
                TotalBuy = x.Average(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Average(y => y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy,
                TienGoc = x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }



        //thong ke theo tháng
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

                    return PartialView();
                }

            }
        }



        public ActionResult StatisticalByMonNew()
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
                    return View();
                }
            }
        }

        public ActionResult Partial_StatisticalByMon(int month)
        {
            ViewBag.Month = month;
            return PartialView();
        }




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
                    TotalBuy = x.Average(y => y.Quantity * y.OriginalPrice),
                    TotalSell = x.Average(y => y.Price),
                })
                .Select(x => new
                {
                    Year = x.Year,
                    Month = x.Month,
                    DoanhThu = x.TotalSell,
                    LoiNhuan = x.TotalSell - x.TotalBuy,
                    TienGoc = x.TotalBuy
                });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetStatisticalByMonSelect(int month)
        {

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
                            where a.typeOrder == false && a.CreatedDate.Month == month
                            select new
                            {
                                a.CreatedDate.Month,
                                a.Quantity,
                                a.TotalAmount,
                                c.OrigianlPrice
                            };


            System.Diagnostics.Debug.WriteLine("Filtered Sales Data: " + salesData.Count());

            var result = salesData.AsEnumerable()
                                  .GroupBy(x => x.Month)
                                  .Select(x => new
                                  {
                                      Month = x.Key,

                                      TotalRevenue = x.Sum(a => a.TotalAmount),
                                      TotalCost = x.Sum(y => y.Quantity * y.OrigianlPrice)

                                  })
                                  .Select(x => new
                                  {
                                      x.Month,
                                      DoanhThu = x.TotalRevenue,
                                      LoiNhuan = x.TotalRevenue - x.TotalCost,
                                      TienGoc = x.TotalCost
                                  }).ToList();


            System.Diagnostics.Debug.WriteLine("Result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        //End thống kê  theo tháng



        //Start thống kê sản phẩm bán chạy


        public ActionResult StatisticalProductsByOrrder()
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
                    return View();
                }
            }
        }
        public ActionResult StatisticalProductsAll()
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

                    return View("NonRole");
                }
                else
                {


                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult GetTopProductsSold()
        {
            var topProducts = db.tb_OrderDetail
                .Join(db.tb_ProductDetail,
                      od => od.ProductDetailId,
                      pd => pd.ProductDetailId,
                      (od, pd) => new { od, pd })
                .Join(db.tb_Products,
                      combined => combined.pd.ProductsId,
                      p => p.ProductsId,
                      (combined, p) => new
                      {
                          ProductDetail = combined.pd,
                          OrderDetail = combined.od,
                          Product = p
                      })
                .GroupBy(x => new
                {
                    x.ProductDetail.ProductDetailId,
                    x.Product.Title,
                    x.ProductDetail.Price,
                    x.ProductDetail.PriceSale,
                    x.ProductDetail.OrigianlPrice,
                    x.ProductDetail.Ram,
                    x.ProductDetail.Capacity,
                    x.ProductDetail.Color
                })
                .Select(g => new
                {
                    ProductDetailId = g.Key.ProductDetailId,
                    ProductTitle = g.Key.Title,
                    Color = g.Key.Color,
                    Ram = g.Key.Ram,
                    Capacity = g.Key.Capacity,
                    TotalQuantitySold = g.Sum(od => od.OrderDetail.Quantity),

                    // Tính TotalRevenue với điều kiện PriceSale > 0
                    TotalRevenue = g.Average(od => od.OrderDetail.Quantity *
                                            (od.ProductDetail.PriceSale > 0 ? od.ProductDetail.PriceSale : od.ProductDetail.Price)),

                    TienGoc = g.Average(od => od.OrderDetail.Quantity *
                                         (od.ProductDetail.OrigianlPrice)),
                    TotalProfit = g.Average(od => od.OrderDetail.Quantity *
                                           (od.ProductDetail.PriceSale > 0 ?
                                            (od.ProductDetail.PriceSale - od.ProductDetail.OrigianlPrice) :
                                            (od.ProductDetail.Price - od.ProductDetail.OrigianlPrice)))
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .ToList();

            return Json(new { Data = topProducts }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Partial_GetTopProductsSoldByMonth(int month) 
        {
            ViewBag.Month = month;
            return PartialView();
        }
        [HttpGet]
        public ActionResult GetTopProductsSoldByMonth(int month = 0)
        {
            // Nếu tháng hoặc năm không được cung cấp, sử dụng giá trị mặc định (hiện tại)
            var currentDate = DateTime.Now;
            month = month > 0 ? month : currentDate.Month;
      

            // Lọc dữ liệu theo tháng và năm
            var topProducts = db.tb_OrderDetail
                .Join(db.tb_Order, // Thực hiện join thêm bảng tb_Order
                      od => od.OrderId, // Khóa chính để join
                      o => o.OrderId, // Khóa chính để join
                      (od, o) => new { od, o })
                .Join(db.tb_ProductDetail,
                      combined => combined.od.ProductDetailId,
                      pd => pd.ProductDetailId,
                      (combined, pd) => new { combined, pd })
                .Join(db.tb_Products,
                      combined => combined.pd.ProductsId,
                      p => p.ProductsId,
                      (combined, p) => new
                      {
                          ProductDetail = combined.pd,
                          OrderDetail = combined.combined.od,
                          Order = combined.combined.o,
                          Product = p
                      })
                .Where(x => x.Order.CreatedDate.Month == month ) // Lọc theo OrderDate
                .GroupBy(x => new
                {
                    x.ProductDetail.ProductDetailId,
                    x.Product.Title,
                    x.ProductDetail.Price,
                    x.ProductDetail.PriceSale,
                    x.ProductDetail.OrigianlPrice,
                    x.ProductDetail.Ram,
                    x.ProductDetail.Capacity,
                    x.ProductDetail.Color
                })
                .Select(g => new
                {
                    ProductDetailId = g.Key.ProductDetailId,
                    ProductTitle = g.Key.Title,
                    Color = g.Key.Color,
                    Ram = g.Key.Ram,
                    Capacity = g.Key.Capacity,
                    TotalQuantitySold = g.Average(od => od.OrderDetail.Quantity),

                    // Tính TotalRevenue với điều kiện PriceSale > 0
                    TotalRevenue = g.Average(od => od.OrderDetail.Quantity *
                                            (od.ProductDetail.PriceSale > 0 ? od.ProductDetail.PriceSale : od.ProductDetail.Price)),

                    TienGoc = g.Average(od => od.OrderDetail.Quantity *
                                         (od.ProductDetail.OrigianlPrice)),
                    TotalProfit = g.Average(od => od.OrderDetail.Quantity *
                                           (od.ProductDetail.PriceSale > 0 ?
                                            (od.ProductDetail.PriceSale - od.ProductDetail.OrigianlPrice) :
                                            (od.ProductDetail.Price - od.ProductDetail.OrigianlPrice)))
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .ToList();

            return Json(new { Data = topProducts }, JsonRequestBehavior.AllowGet);
        }



        //End Thống kê sản phẩm bán chyaj 


    }
}