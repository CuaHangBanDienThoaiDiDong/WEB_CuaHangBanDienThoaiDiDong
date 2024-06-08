using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order

        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var customer=db.tb_Customer.Find(idKhach);  
                ViewBag.id = idKhach;
                ViewBag.Name = customer.CustomerName;
                return View();


            }
            else
            {
                return RedirectToAction("Login", "Account");
            }


        }

        public ActionResult Partial_Order(int id)
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


        public ActionResult Partail_Ordertail(int id)
        {
            try
            {
                if (Session["CustomerId"] != null)

                {
                    int idKhach = (int)Session["CustomerId"];

                    var orderDetail = db.tb_OrderDetail.Where(x => x.OrderId == id).OrderByDescending(x => x.OrderId).ToList();
                    if (orderDetail != null)
                    {
                        ViewBag.Count = orderDetail.Count();
                        return PartialView(orderDetail);
                    }
                    return PartialView();

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");

            }
           
        }

        public ActionResult Partail_TrangThaiDonHang(int id)
        {
            try 
            {
                if (Session["CustomerId"] != null)

                {
                    int idKhach = (int)Session["CustomerId"];

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
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");

            }    
           
        }



        public ActionResult Partial_WaitPayOrder()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var cheCheckORder = db.tb_Order.Where(x => x.CustomerId == idKhach && x.TypePayment == 1).OrderByDescending(x => x.OrderId).ToList();
                if (cheCheckORder != null)
                {
                    ViewBag.Count = cheCheckORder.Count;
                    return PartialView(cheCheckORder);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public ActionResult Partail_ListOrderCancel(int id)
        {
            if (id > 0)
            {
                var order = db.tb_Order.Find(id);
                if (order != null)
                {
                    return PartialView(order);
                }
            }
            return PartialView();
        }
        public ActionResult Partial_OrderCanceled()
        {

            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var checkORder = db.tb_Order.Where(x => x.CustomerId == idKhach && x.typeOrder == true).OrderByDescending(x => x.OrderId).ToList();
                if (checkORder != null)
                {
                    ViewBag.Count = checkORder.Count;
                    return PartialView(checkORder);
                }
                return PartialView();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

          
        }




        public ActionResult Partial_CancelOrder(int id)
        {
            if (id > 0)
            {

                var order = db.tb_Order.Find(id);
                if (order != null)
                {
                    ViewBag.OrderId = id;
                    ViewBag.Code=order.Code;
                    return PartialView();    
                }
                else
                {
                    return PartialView();
                }
              
            }
            return PartialView();
        }


        public ActionResult SuccessCancelOrder(string code, int id)
        {
            if (id > 0)
            {
                 var order=db.tb_Order.Find(id);
                if (order != null) 
                {
                    ViewBag.Code = code;
                    return View(order);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
            
        }



        public ActionResult Partial_OrderSuccess()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var cheCheckORder = db.tb_Order.Where(x => x.CustomerId == idKhach && x.Success == true).OrderByDescending(x => x.OrderId).ToList();
                if (cheCheckORder != null)
                {
                    ViewBag.Count = cheCheckORder.Count();
                   
                    return View(cheCheckORder);
                }
                else
                {

                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpPost]
        public JsonResult OrderSuccess(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (id > 0)
                    {
                        var order = db.tb_Order.Find(id);
                        if (order != null)
                        {
                            order.Success = true;
                            order.SuccessDate = DateTime.Now;
                            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return Json(new { success = true, code = 1 });
                        }
                        else
                        {
                            return Json(new { success = false, code = -1 });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, code = -2});

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
        public ActionResult CancelOrder(ReturnAndCancelOrder req , int orderId) 
        {
            using (var dbContextTransaction = db.Database.BeginTransaction()) 
            {
                try 
                {
                    if (orderId > 0)
                    {
                        var order = db.tb_Order.Include(o => o.tb_OrderDetail).FirstOrDefault(o => o.OrderId == orderId);
                        if (order != null)
                        {
                            
                            foreach (var productDetailId in order.tb_OrderDetail) 
                            {
                                var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(x => x.ProductDetailId == productDetailId.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    warehouseDetail.QuanTity += productDetailId.Quantity;
                                    if (!string.IsNullOrEmpty(req.CustomStatusHidden))
                                    {
                                        order.Status = req.CustomStatusHidden.Trim();
                                    }
                                    else
                                    {
                                        order.Status = req.Status.Trim();
                                    }


                                    db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                } 
                                else { 
                                    return Json(new { success = false, code = -3 });//Không tìm thấy mã sản phẩm này
                                                                                    }
                            }
                            order.typeOrder = true;//capp nhap trang thai cho bang order
                            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return Json(new { success = true, code = 1 });
                        }
                        else
                        {
                            return Json(new { success = false, code = -2 });
                        }

                    }
                    else 
                    {
                        return Json(new { success = false, code = -1 });
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100 });

                } 
            }
        }

    }
}