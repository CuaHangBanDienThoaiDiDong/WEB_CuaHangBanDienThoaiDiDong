using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Admin;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Admin/Voucher
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_Voucher> items = db.tb_Voucher.OrderByDescending(x => x.VoucherId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_Voucher.ToList();

                    ViewBag.Count = products.Count;
                    ViewBag.PageSize = pageSize;
                    ViewBag.Page = page;
                    return View(items);
                }
                else
                {
                    ViewBag.txt = "Không tồn tại sản phẩm";
                    return View();
                }
            }
        }
        // Hàm tạo chuỗi ngẫu nhiên
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult Partial_AddVoucher()
        {

            return PartialView();
        }

        public ActionResult Add()
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Voucher model, Admin_TokenVoucher req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.Title != null)
            {
                var title = db.tb_ProductCompany.FirstOrDefault(r => r.Title == req.Title);
                if (title == null)
                {


                    if (model.Title != null)
                    {
                        string randomString = GenerateRandomString(10);
                        tb_Staff nvSession = (tb_Staff)Session["user"];
                        var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                        model.CreatedBy = checkStaff.NameStaff + "-" + checkStaff.Code;
                        //model.Code = randomString;
                        model.Code = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Title);


                        model.Title = req.Title;
                        model.CreatedDate = req.UsedDate;
                        model.ModifiedDate = Convert.ToDateTime(req.ModifiedDate); //Ngay keyt thuc
                        model.UsedDate = Convert.ToDateTime(req.UsedDate);//Ngay bát dau
                        db.tb_Voucher.Add(model);
                        db.SaveChanges();
                        code = new { Success = true, Code = 1, Url = "" };
                    }
                    else
                    {
                        ViewBag.txt = "Vui lòng nhập thông tin";
                        return View();
                    }



                }
                else
                {
                    // Tên đã tồn tại
                    code = new { Success = false, Code = -3, Url = "" };
                }
            }
            else
            {
                // Vui lòng điền tên tiêu đề
                code = new { Success = false, Code = -2, Url = "" };
            }
            return Json(code);
        }


        public ActionResult Details(int id) 
        {
            var item =db.tb_Voucher.Find(id);
            if (item != null) 
            {
                return View(item);
            }   
            return View();
        }

        public class EditVoucherViewModel
        {
            public int VoucherId { get; set; }
            public string Title { get; set; }
            public DateTime? UsedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            // Thêm các thuộc tính khác của model tại đây
        }

        public ActionResult Edit(int id)
        {
            var item = db.tb_Voucher.Find(id);
            if (item != null)
            {
                var viewModel = new EditVoucherViewModel
                {
                    VoucherId = item.VoucherId,
                    Title = item.Title,
                    UsedDate = item.UsedDate,
                    ModifiedDate = item.ModifiedDate
                };
                return View(viewModel);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditVoucherViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var item = db.tb_Voucher.Find(viewModel.VoucherId);
                if (item != null)
                {
                    item.Title = viewModel.Title;
                    item.UsedDate = viewModel.UsedDate;
                    item.ModifiedDate = viewModel.ModifiedDate;

                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(viewModel);
        }



    }
}