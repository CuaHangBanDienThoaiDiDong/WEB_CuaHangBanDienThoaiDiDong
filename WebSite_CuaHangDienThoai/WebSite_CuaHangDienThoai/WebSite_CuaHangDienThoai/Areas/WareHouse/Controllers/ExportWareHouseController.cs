using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.WareHouse.Controllers
{
    public class ExportWareHouseController : Controller
    {
        // GET: WareHouse/ExportWareHouse
 CUAHANGDIENTHOAIEntities db  = new CUAHANGDIENTHOAIEntities(); 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_ThongTinDonHang(string code)
       {
            var checkOder = db.tb_Order.FirstOrDefault(x => x.Code.Contains(code)&& x.Confirm==true);

            if (checkOder != null)
            {
            var checkOrderDetail=db.tb_OrderDetail.Where(x => x.OrderId==checkOder.OrderId).Count();
                ViewBag.OrderId = checkOder.OrderId;
                ViewBag.Count = checkOrderDetail;
                return PartialView(checkOder);
            }
            return PartialView();
        }



        public ActionResult Partial_ThongTinXuat()
        {
            return PartialView();
        }

        [HttpPost]
   
        public JsonResult Partial_ExportWareHouse(int Orderid)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            try {
                var checkOrder = db.tb_Order.Find(Orderid);
                if (checkOrder != null)
                {
                    var checkCancelOrder = db.tb_Order.FirstOrDefault(x => x.OrderId == checkOrder.OrderId && x.typeOrder == false);
                    if (checkCancelOrder != null)
                    {
                        var checkConfim = db.tb_Order.FirstOrDefault(x => x.OrderId == checkCancelOrder.OrderId && x.Confirm == true);
                        if (checkConfim != null)
                        {
                            var OrderReturn = db.tb_Order.FirstOrDefault(x => x.OrderId == checkConfim.OrderId && x.typeReturn == false);
                            if (OrderReturn != null)
                            {
                                var checkTBOut = db.tb_ExportWareHouse.FirstOrDefault(x => x.OrderId == OrderReturn.OrderId);
                                if (checkTBOut == null)
                                {
                                    tb_Staff nvSession = (tb_Staff)Session["user"];
                                    var staff = db.tb_Staff.SingleOrDefault(row => row.StaffId == nvSession.StaffId);
                                    var checkWareHouse = db.tb_Warehouse.FirstOrDefault(r => r.StoreId == staff.tb_Store.StoreId);
                                    tb_ExportWareHouse model = new tb_ExportWareHouse();
                                    model.CreatedDate = DateTime.Now;
                                    model.StaffId = staff.StaffId;
                                    model.OrderId = checkOrder.OrderId;
                                    model.WarehouseId = checkWareHouse.WarehouseId;
                                    db.tb_ExportWareHouse.Add(model);
                                    db.SaveChanges();


                                    code = new { Success = true, Code = 1, Url = "" };
                                }
                                else
                                {
                                    code = new { Success = false, Code = -6, Url = "" };
                                }
                            }
                            else
                            {
                                code = new { Success = false, Code = -5, Url = "" };
                            }


                        }
                        else
                        {
                            code = new { Success = false, Code = -4, Url = "" };
                        }

                    }
                    else
                    {
                        //Don Hang da bi huy
                        code = new { Success = false, Code = -3, Url = "" };
                    }
                }
                else
                {
                    //Khong thay ma Order
                    code = new { Success = false, Code = -2, Url = "" };
                }
            }
            catch (Exception ex) 
            {
                code = new { Success = false, Code = -100, Url = "" };
                return Json(code);

            }
           
            return Json(code);

        }




        public ActionResult Partial_OrderDetail(int id) 
        {
            if (id > 0) 
            {
                var item = db.tb_OrderDetail.Where(x => x.OrderId == id).ToList();
                if (item != null)
                {
                    ViewBag.OrderId = id;
                    ViewBag.Count = item.Count;
                    return PartialView(item);
                }
            }
           
            return PartialView();
        }
    }
}