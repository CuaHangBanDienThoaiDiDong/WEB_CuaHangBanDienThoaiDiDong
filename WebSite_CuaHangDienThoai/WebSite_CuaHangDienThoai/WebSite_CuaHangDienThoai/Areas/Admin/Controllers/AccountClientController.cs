using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class AccountClientController : Controller
    {
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Admin/AccountClient
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

                    var items = db.tb_Customer.ToList();
                    if (items != null)
                    {
                        ViewBag.Count = items.Count();
                        return View(items);
                    }
                    return View();
                }

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

                tb_Staff nvSession = (tb_Staff)Session["user"];
                var role = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (role == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                    if (id > 0)
                    {
                        var customer = db.tb_Customer.Find(id);
                        if (customer == null)
                        {
                            return HttpNotFound();
                        }
                        ViewBag.NAme = customer.CustomerName; return View(customer);
                    }

                    return View();

                }

            }

        }



        public ActionResult Partail_Index(int? page)
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
                    IEnumerable<tb_Customer> items = db.tb_Customer.OrderByDescending(x => x.CustomerId);
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    ViewBag.PageSize = pageSize;
                    ViewBag.Page = page;
                    ViewBag.Count = items.Count();
                    return View(items);
                }

            }
        }




        //[HttpPost]
        //public ActionResult IsLock(int id)
        //{
        //    var item = db.tb_Customer.Find(id);
        //    if (item != null)
        //    {
        //        item.Clock = !item.Clock;
        //        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return Json(new { success = true, isAcive = item.Clock });
        //    }

        //    return Json(new { success = false });
        //}

        public ActionResult Partial_Order(int id)
        {
            if (id > 0)
            {
                var order = db.tb_Order.Where(x => x.CustomerId == id).OrderByDescending(x => x.OrderId).ToList();
                if (order.Any())
                {
                    ViewBag.OrderId = id;
                    ViewBag.Count = order.Count();
                    return PartialView(order);
                }
                return PartialView();
            }
            else
            {
                return RedirectToAction("NonRole", "HomePage");
            }

        }
        public ActionResult Partail_Ordertail(int id)
        {
            try
            {


                var orderDetail = db.tb_OrderDetail.Where(x => x.OrderId == id).OrderByDescending(x => x.OrderId).ToList();
                if (orderDetail != null)
                {
                    ViewBag.Count = orderDetail.Count();
                    return PartialView(orderDetail);
                }
                return PartialView();


            }
            catch (Exception)
            {
                return RedirectToAction("NonRole", "HomePage");

            }

        }

        public ActionResult Partail_TrangThaiDonHang(int id)
        {
            try
            {
                if (id > 0)
                {
                    var cheCheckORderDetail = db.tb_Order.Find(id);
                    if (cheCheckORderDetail != null)
                    {
                        var checkOutOrder = db.tb_ExportWareHouse.FirstOrDefault(x => x.OrderId == cheCheckORderDetail.OrderId);
                        if (checkOutOrder != null)
                        {
                            ViewBag.Out = "XuatKho";
                            return PartialView(cheCheckORderDetail);
                        }
                        else
                        {
                            return PartialView(cheCheckORderDetail);

                        }

                    }
                    return PartialView();
                }
                else
                {
                    return PartialView();
                }



            }
            catch (Exception)
            {
                return RedirectToAction("NonRole", "HomePage");

            }

        }


        [HttpPost]
        public ActionResult IsLock(int id)
        {
            try
            {
                var customer = db.tb_Customer.FirstOrDefault(c => c.CustomerId == id);
                if (customer != null)
                {
                    customer.Clock = !customer.Clock;
                    db.SaveChanges();
                    return Json(new { success = true, isActive = customer.Clock });
                }
                else
                {
                    return Json(new { success = false, error = "Không tìm thấy khách hàng" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult CheckAllClocks()
        {
            try
            {
                var allClocksActive = db.tb_Customer.All(c => c.Clock == true);
                return Json(new { success = true, allClocksActive = allClocksActive }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateAllClocks()
        {
            try
            {

                var customers = db.tb_Customer.ToList();
                foreach (var customer in customers)
                {
                    customer.Clock = true;
                    db.Entry(customer).State = EntityState.Modified;
                }
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult UpdateSelectedClocks(List<CustomerViewModelClock> selectedCustomers)
        {
            try
            {
                foreach (var customer in selectedCustomers)
                {
                    var dbCustomer = db.tb_Customer.Find(customer.CustomerId);
                    if (dbCustomer != null)
                    {
                        // Cập nhật trạng thái Clock của khách hàng
                        dbCustomer.Clock = !dbCustomer.Clock; // Đảo ngược trạng thái
                        db.Entry(dbCustomer).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }





        public ActionResult AddClient() 
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                return PartialView();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddClient(Admin_TokenAddClient req)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Session["user"] == null)
                    {
                        return Json(new { success = false, code = -99, redirectTo = Url.Action("DangNhap", "Account") });
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            tb_Customer customer = new tb_Customer();
                            customer.PhoneNumber = req.PhoneNumber;
                            customer.CustomerName = req.CustomerName;
                            customer.Email = req.Email;
                            customer.NumberofPurchases += 1;

                            db.tb_Customer.Add(customer);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return Json(new { success = true, code = 1 });
                        }
                        else
                        {
                            return Json(new { success = true, code = -1 });//Không đủ dữ liệu
                        }
                    }

                }


                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100 });

                }
            }
        }



        [HttpPost]
        public ActionResult UpdateAllUnClocks()
        {
            try
            {

                var customers = db.tb_Customer.ToList();
                foreach (var customer in customers)
                {
                    customer.Clock = false;
                    db.Entry(customer).State = EntityState.Modified;
                }
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}