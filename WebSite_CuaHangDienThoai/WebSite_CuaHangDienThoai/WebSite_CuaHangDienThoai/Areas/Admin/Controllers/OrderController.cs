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
        public ActionResult Index(int? page)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                //var item = db.tb_Order.ToList();
                var item = db.tb_Order.OrderByDescending(x => x.OrderId).ToList();
                if (page == null)
                {
                    page = 1;
                }
                if (page == null)
                {
                    page = 1;
                }
                var pageNumber = page ?? 1;
                var pageSize = 10;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = pageNumber;
                return View(item);
            }
        }











        public ActionResult OrderNew(int? page)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                //var item = db.tb_Order.ToList();
                var item = db.tb_Order.Where(row => row.Confirm == false).OrderByDescending(x => x.OrderId).ToList();
                if (page == null)
                {
                    page = 1;
                }
                if (page == null)
                {
                    page = 1;
                }
                var pageNumber = page ?? 1;
                var pageSize = 10;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = pageNumber;
                return View(item);
            }

        }








        public ActionResult Detail(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Order.Find(id);
                ViewBag.Code=item.Code;
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


        public ActionResult OrderExportToday()
        {
            DateTime today = DateTime.Today;
            DateTime startOfDay = today.Date;
            DateTime endOfDay = today.Date.AddDays(1).AddTicks(-1);

            var exportToDay = db.tb_ExportWareHouse.Where(row => row.CreatedDate >= startOfDay && row.CreatedDate <= endOfDay).OrderByDescending(x => x.WarehouseId).ToList();
            if (exportToDay != null)
            {
                ViewBag.Today = today;
                return View(exportToDay);
            }
            return View();

        }

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