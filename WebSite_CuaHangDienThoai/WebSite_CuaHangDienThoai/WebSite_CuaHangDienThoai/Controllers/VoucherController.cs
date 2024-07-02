using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Voucher
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            var currentDate = DateTime.Now;

            var validVouchers = db.tb_Voucher
                .Where(v => v.UsedDate.HasValue && v.ModifiedDate.HasValue &&
                            v.UsedDate <= currentDate &&
                            v.ModifiedDate >= currentDate)
                .ToList();

            return View(validVouchers);
        }
        public ActionResult Partial_VoucherDetail(int id)
        {
            if (id <= 0)
            {
                ViewBag.error = "Không tìm thấy bảng ghi nào !!!";
                return PartialView();
            }
            var voucherDetail = db.tb_VoucherDetail.Where(x => x.VoucherId == id).Take(5);
            if (voucherDetail != null) 
            {
                return PartialView(voucherDetail);  
            } 
            else
            {
                ViewBag.error = "Không tìm thấy bảng ghi nào !!!";
                return PartialView();
            }
        }

        public ActionResult DieuKien(int id)
        {
            if (id <= 0)
            {
                ViewBag.error = "Không tìm thấy bảng ghi nào !!!";
                return View();
            }

            var voucher = db.tb_Voucher.Find(id);
            if (voucher != null)
            {
                // Kiểm tra ModifiedDate và tính toán thời gian còn lại
                if (voucher.ModifiedDate.HasValue)
                {
                    var now = DateTime.Now;
                    var modifiedDate = voucher.ModifiedDate.Value;
                    var timeLeft = modifiedDate - now;

                    // Lưu kết quả tính toán vào ViewBag
                    ViewBag.TimeLeft = timeLeft;
                    ViewBag.ModifiedDate = modifiedDate;
                }
                else
                {
                    ViewBag.TimeLeft = null;
                }

                return View(voucher);
            }
            else
            {
                ViewBag.error = "Không tìm thấy bảng ghi nào !!!";
                return View();
            }
        }


    }


}