using Microsoft.Win32;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class BillController : Controller
    {
        // GET: Admin/Bill
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 2));
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


        public ActionResult Partial_IndexBill(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_Seller> items = db.tb_Seller.OrderByDescending(x => x.SellerId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_Seller.ToList();

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




        public ActionResult Detail(int id) 
        {
            if (id > 0)
            {
                var seller=db.tb_Seller.Find(id);
                if (seller != null)
                {
                    ViewBag.Code = seller.Code;
                    return View(seller);
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


        public ActionResult Detail_SanPham(int id)
        {
            if (id > 0)
            {
                var SellerDetail = db.tb_SellerDetail.Where(x => x.SellerId == id).ToList();
                if (SellerDetail != null) 
                {
                    ViewBag.Count=SellerDetail.Count;
                    return PartialView(SellerDetail);
                }
                else
                {
                    return PartialView();
                }
            }
            else 
            {
            return PartialView() ;  
            }
        }




        public ActionResult Edit (int id) 
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    if (id > 0)
                    {
                        var seller = db.tb_Seller.Find(id);
                        if (seller != null)
                        {
                            var viewModel = new Admin_TokenEditBill
                            {
                                SellerId = seller.SellerId,
                                Code = seller.Code,
                                CustomerId = seller.CustomerId,
                                Phone = seller.Phone,
                                TotalAmount = seller.TotalAmount,
                                Quantity = seller.Quantity,
                                CreatedBy = seller.CreatedBy,
                                CreatedDate = seller.CreatedDate,
                                TypePayment = seller.TypePayment,
                                StaffId = seller.StaffId,
                            };
                            return PartialView(seller);
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
            }
        }









    }
}