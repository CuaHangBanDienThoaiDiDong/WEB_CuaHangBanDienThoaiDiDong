﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                
                ViewBag.id = idKhach;
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
                ViewBag.Count=order.Count();    
                return PartialView(order);
            }
            return PartialView();   
        }


        public ActionResult Partail_Ordertail(int id) 
        {
            var orderDetail = db.tb_OrderDetail.Where(x => x.OrderId == id).OrderByDescending(x => x.OrderId).ToList();
            if (orderDetail != null)
            {
                ViewBag.Count = orderDetail.Count();
                return PartialView(orderDetail);
            }
            return PartialView();
        }









        public ActionResult Partail_TrangThaiDonHang(int id)
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


    }
}