using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
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
        //Start All Order
        public ActionResult Partial_OrderIndex(int? page)
        {
            IEnumerable<tb_Order> items = db.tb_Order.OrderByDescending(x => x.OrderId);
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
                return PartialView(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return PartialView();
            }
        }

        public ActionResult SuggestOrderCustomer(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var customerIds = db.tb_Customer
                    .Where(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search))
                    .Select(c => c.CustomerId)
                    .ToList();

                var orders = db.tb_Order
                    .Where(s => customerIds.Contains((int)s.CustomerId) || s.Code.Contains(search))
                    .ToList();

                if (orders.Any())
                {
                    var count = orders.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(orders);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return PartialView();
            }
        }

        public ActionResult GetOrderAll(DateTime ngayxuat)
        {
            // Lấy ngày từ datetime-local
            DateTime selectedDate = ngayxuat;

            // Lấy ngày bắt đầu và ngày kết thúc của ngày đã chọn
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1); // Tăng ngày lên 1 để lấy đến cuối ngày

            // Truy vấn để lấy danh sách đơn hàng trong khoảng thời gian startDate và endDate
            var orders = db.tb_Order
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

            if (orders != null && orders.Count > 0)
            {
                ViewBag.Count = orders.Count;
                return PartialView(orders);
            }
            else
            {
                // Trả về partial view mặc định nếu không có đơn hàng nào
                return PartialView();
            }
        }

        //End All Order


        //Start Dơn hàng mưới 


        public ActionResult OrderNew()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                //var item = db.tb_Order.ToList();

                return View();
            }

        }



        public ActionResult Partial_OrderNew(int? page)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {

                IEnumerable<tb_Order> items = db.tb_Order.Where(row => row.Confirm == false).OrderByDescending(x => x.OrderId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_Order.ToList();

                    ViewBag.Count = products.Count;
                    ViewBag.PageSize = pageSize;
                    ViewBag.Page = page;
                    return PartialView(items);
                }
                else
                {
                    ViewBag.txt = "Không tồn tại sản phẩm";
                    return PartialView();
                }



            }
        }


        public ActionResult SuggestOrderNewCustomer(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var customerIds = db.tb_Customer
                    .Where(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search))
                    .Select(c => c.CustomerId)
                    .ToList();

                var orders = db.tb_Order
                    .Where(s => customerIds.Contains((int)s.CustomerId) || s.Code.Contains(search) && s.Confirm == false)
                    .ToList();

                if (orders.Any())
                {
                    var count = orders.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(orders);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return PartialView();
            }
        }

        // kiem ngày theo tất cả đơn hàng



        public ActionResult OrderNewToday(DateTime ngayxuat)
        {
            // Lấy ngày từ datetime-local
            DateTime selectedDate = ngayxuat;

            // Lấy ngày bắt đầu và ngày kết thúc của ngày đã chọn
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1); // Tăng ngày lên 1 để lấy đến cuối ngày

            // Truy vấn để lấy danh sách đơn hàng trong khoảng thời gian startDate và endDate
            var orders = db.tb_Order
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate && o.Confirm == false)
                .ToList();

            if (orders != null && orders.Count > 0)
            {
                ViewBag.Count = orders.Count;
                return PartialView(orders);
            }
            else
            {
                // Trả về partial view mặc định nếu không có đơn hàng nào
                return PartialView();
            }
        }
        //end Dơn hàng mưới 

      





        public ActionResult Detail(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Order.Find(id);
                ViewBag.Code = item.Code;
                return View(item);
            }

        }


        public ActionResult Detail_SanPham(int id)
        {
            var item = db.tb_OrderDetail.Where(row => row.OrderId == id).ToList();
            if (item != null)
            {
                ViewBag.Count = item.Count;
                return PartialView(item);

            }
            return PartialView();
        }

        public ActionResult CountUnConfimred()
        {
            var item = db.tb_Order.Count(row => row.Confirm.HasValue);
            return PartialView(item);
        }

        // Start Dơn hang xuất

        public ActionResult OrderExportToday()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2|| row.FunctionId == 3));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    return View();
                }
             }
        }
        public ActionResult Partial_OrderExportToday(int? page)
        {
            
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 3));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    DateTime today = DateTime.Today;
                    DateTime startOfDay = today.Date;
                    DateTime endOfDay = today.Date.AddDays(1).AddTicks(-1);

                    IEnumerable<tb_ExportWareHouse> items = db.tb_ExportWareHouse
                        .Where(row => row.CreatedDate >= startOfDay && row.CreatedDate <= endOfDay)
                        .OrderByDescending(x => x.OrderId);

                    if (items != null)
                    {
                        var pageSize = 10;
                        if (page == null)
                        {
                            page = 1;
                        }
                        var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        items = items.ToPagedList(pageIndex, pageSize);
                        var products = db.tb_Order.ToList();

                        ViewBag.Count = products.Count;
                        ViewBag.PageSize = pageSize;
                        ViewBag.Page = page;
                        return PartialView(items);
                    }
                    else
                    {
                        ViewBag.txt = "Không tồn tại sản phẩm";
                        return PartialView();
                    }
                }
                



            }
        }

        public ActionResult SuggestOrderExport(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var customerIds = db.tb_Customer
                    .Where(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search))
                    .Select(c => c.CustomerId)
                    .ToList();

                var orders = db.tb_Order
                    .Where(s => customerIds.Contains((int)s.CustomerId) || s.Code.Contains(search) && s.Confirm == false)
                    .ToList();

                if (orders.Any())
                {
                    var count = orders.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(orders);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return PartialView();
            }
        }




        // End Dơn hang xuất


        public ActionResult GetOrderNewToDay(DateTime ngayxuat)
        {
            // Lấy ngày từ datetime-local
            DateTime selectedDate = ngayxuat;

            // Lấy ngày bắt đầu và ngày kết thúc của ngày đã chọn
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1); // Tăng ngày lên 1 để lấy đến cuối ngày

            // Truy vấn để lấy danh sách đơn hàng trong khoảng thời gian startDate và endDate
            var orders = db.tb_Order
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

            if (orders != null && orders.Count > 0)
            {
                ViewBag.Count = orders.Count;
                return PartialView(orders);
            }
            else
            {
                // Trả về partial view mặc định nếu không có đơn hàng nào
                return PartialView();
            }
        }




        [HttpPost]
        public ActionResult IsComfrim(int id)
        {
            var item = db.tb_Order.Find(id);
            if (item != null)
            {
                item.Confirm = !item.Confirm;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, Confirm = item.Confirm });
            }

            return Json(new { success = false });
        }


    }
}