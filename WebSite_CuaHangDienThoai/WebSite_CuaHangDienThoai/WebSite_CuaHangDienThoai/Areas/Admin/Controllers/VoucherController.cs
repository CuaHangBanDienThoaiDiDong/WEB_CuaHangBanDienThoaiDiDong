using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
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
                tb_Staff nvSession = (tb_Staff)Session["user"];

                if (nvSession == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {




                    var items = db.tb_Voucher.OrderByDescending(x => x.VoucherId).ToList();
                   
                    if (items != null)
                    {
                        int pageSize = 10;
                        int pageNumber = (page ?? 1);

                        var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        
                        var products = db.tb_Voucher.ToList();

                        ViewBag.Count = products.Count;
                        ViewBag.PageSize = pageSize;
                        ViewBag.Page = page;
                        return View(items.ToPagedList(pageNumber, pageSize));

                    }
                    else
                    {
                        ViewBag.txt = "Không tồn tại sản phẩm";
                        return View();
                    }
                }
            }
        }

        // Hàm tạo chuỗi ngẫu nhiên
        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var randomString = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }

            return randomString.ToString();
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

            using (var dbContextTransaction = db.Database.BeginTransaction()) 
            {
                try 
                {
                  
                  
                    if (req.Title != null)
                    {
                        var title = db.tb_ProductCompany.FirstOrDefault(r => r.Title == req.Title);
                        if (title == null)
                        {
                            if (model.Title != null)
                            {
                                string randomString = GenerateRandomString(3);
                                tb_Staff nvSession = (tb_Staff)Session["user"];
                                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                                model.CreatedBy = checkStaff.NameStaff + "-" + checkStaff.Code;
                                model.Code = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim());
                                model.Title = req.Title.Trim();
                                model.CreatedDate = req.UsedDate;
                                model.ModifiedDate = Convert.ToDateTime(req.ModifiedDate);
                                model.UsedDate = Convert.ToDateTime(req.UsedDate);
                                db.tb_Voucher.Add(model);
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                var result = AddVoucherDetails(model, req);
                                if (result)
                                {
                                    return Json(new { Success = true, Code = 1});
                                }
                                else
                                {
                                    // Rollback the voucher creation
                                    db.tb_Voucher.Remove(model);
                                    db.SaveChanges();
                                    ViewBag.txt = "Có lỗi xảy ra khi lưu voucher details. Vui lòng thử lại sau.";
                                    return View();
                                }
                            }
                            else
                            {
                                ViewBag.txt = "Vui lòng nhập thông tin";
                                return View();
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, Code = -3});
                           
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, Code = -2});
                       
                    }
                    
                  
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { Success = false , Code=-99});
                }
            }

           
        }

        private bool AddVoucherDetails(tb_Voucher model, Admin_TokenVoucher req)
        {
            try
            {
                for (int i = 0; i < model.Quantity; i++)
                {


                    while (true)
                    {
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        var random = new Random();
                        var randomString = new StringBuilder(3);

                        for (int x = 0; x < 3; x++)
                        {
                            randomString.Append(chars[random.Next(chars.Length)]);
                        }

                        string randomString123 = randomString.ToString(); ;
                        string combinedCode = req.Title.Trim() + randomString123;
                        string filteredCode = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(combinedCode);

                        var checkCode = db.tb_VoucherDetail.FirstOrDefault(x => x.Code == filteredCode);

                        if (checkCode == null)
                        {
                            var voucherDetail = new tb_VoucherDetail();
                            voucherDetail.Code = filteredCode;
                            voucherDetail.CreatedBy = model.CreatedBy;
                            voucherDetail.CreatedDate = model.CreatedDate;
                           
                            voucherDetail.UsedDate = model.UsedDate;
                            voucherDetail.VoucherId = model.VoucherId;
                            voucherDetail.Status = false;

                            db.tb_VoucherDetail.Add(voucherDetail);
                            db.SaveChanges(); 
                            break; 
                        }
                    }



                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is set up)
                return false;
            }
        }

        //public ActionResult Add(tb_Voucher model, Admin_TokenVoucher req)
        //{
        //    var code = new { Success = false, Code = -1, Url = "" };
        //    if (req.Title != null)
        //    {
        //        var title = db.tb_ProductCompany.FirstOrDefault(r => r.Title == req.Title);
        //        if (title == null)
        //        {


        //            if (model.Title != null)
        //            {
        //                string randomString = GenerateRandomString(3);
        //                tb_Staff nvSession = (tb_Staff)Session["user"];
        //                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
        //                model.CreatedBy = checkStaff.NameStaff + "-" + checkStaff.Code;
        //                //model.Code = randomString;
        //                model.Code = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim());


        //                model.Title = req.Title.Trim();
        //                model.CreatedDate = req.UsedDate;
        //                model.ModifiedDate = Convert.ToDateTime(req.ModifiedDate); //Ngay keyt thuc
        //                model.UsedDate = Convert.ToDateTime(req.UsedDate);//Ngay bát dau
        //                db.tb_Voucher.Add(model);
        //                db.SaveChanges();




        //                for (int i = 0; i < model.Quantity; i++)
        //                {
        //                    var voucherDetail = new tb_VoucherDetail();
        //                    string randomString123 = GenerateRandomString(4);
        //                    string codedetail = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim() + randomString123);
        //                    voucherDetail.Code = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(codedetail);
        //                    voucherDetail.CreatedBy = model.CreatedBy;
        //                    voucherDetail.CreatedDate = model.CreatedDate;
        //                    voucherDetail.UsedBy = model.UsedBy;
        //                    voucherDetail.UsedDate = model.UsedDate;
        //                    voucherDetail.VoucherId = model.VoucherId;
        //                    voucherDetail.Status = false;


        //                    db.tb_VoucherDetail.Add(voucherDetail);
        //                }


        //                db.SaveChanges();

        //                code = new { Success = true, Code = 1, Url = "" };
        //            }
        //            else
        //            {
        //                ViewBag.txt = "Vui lòng nhập thông tin";
        //                return View();
        //            }



        //        }
        //        else
        //        {
        //            // Tên đã tồn tại
        //            code = new { Success = false, Code = -3, Url = "" };
        //        }
        //    }
        //    else
        //    {
        //        // Vui lòng điền tên tiêu đề
        //        code = new { Success = false, Code = -2, Url = "" };
        //    }
        //    return Json(code);
        //}


        public ActionResult Partail_VoucherDetail(int id )
        {
            var item = db.tb_VoucherDetail.Where(x => x.VoucherId == id);
            if (item != null) 
            {
               ViewBag.Count = item.Count();
                return PartialView(item);
            }
            return PartialView();   
        }

        public ActionResult Details(int id) 
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Voucher.Find(id);
                if (item != null)
                {
                    return View(item);
                }
                return View();
            }
               
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
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    PhanTramGiaGiam = (int)item.PercentPriceReduction,
                    Quantity = (int)item.Quantity,
                    OriginalQuantity = (int)item.Quantity,
                };

              
               
                return View(viewModel);
            }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditVoucherViewModel viewModel)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction()) 
            {
                try 
                {
                    if (ModelState.IsValid)
                    {
                        var item = db.tb_Voucher.Find(viewModel.VoucherId);
                        if (item != null)
                        {
                            item.Code = viewModel.Code;
                            item.PercentPriceReduction = viewModel.PhanTramGiaGiam;

                            item.Title = viewModel.Title;
                            item.CreatedBy = viewModel.CreatedBy;
                            item.CreatedDate = viewModel.CreatedDate;
                            item.UsedDate = viewModel.UsedDate;
                            item.ModifiedDate = viewModel.ModifiedDate;
                            item.Quantity = viewModel.Quantity;


                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            if (viewModel.Quantity != viewModel.OriginalQuantity)
                            {
                                // Tính toán số lượng mới được thêm vào
                                int quantityAdded = viewModel.Quantity - viewModel.OriginalQuantity;

                                // Tạo mới các bản ghi trong tb_VoucherDetail và thêm vào cơ sở dữ liệu
                                for (int i = 0; i < quantityAdded; i++)
                                {
                                    while (true)
                                    {
                                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                                        var random = new Random();
                                        var randomString = new StringBuilder(3);

                                        for (int x = 0; x < 3; x++)
                                        {
                                            randomString.Append(chars[random.Next(chars.Length)]);
                                        }

                                        string randomString123 = randomString.ToString(); ;
                                        string combinedCode = viewModel.Title.Trim() + randomString123;
                                        string filteredCode = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(combinedCode);

                                        var checkCode = db.tb_VoucherDetail.FirstOrDefault(x => x.Code == filteredCode);

                                        if (checkCode == null)
                                        {
                                            var voucherDetail = new tb_VoucherDetail();
                                            voucherDetail.Code = filteredCode;
                                            voucherDetail.CreatedBy = viewModel.CreatedBy;
                                            voucherDetail.CreatedDate = viewModel.CreatedDate;
                                            voucherDetail.VoucherId = viewModel.VoucherId;
                                            voucherDetail.Status = false;

                                            db.tb_VoucherDetail.Add(voucherDetail);
                                            db.SaveChanges();
                                            break;
                                        }
                                    }
                                }

                                var voucherDetailsToDelete = db.tb_VoucherDetail.Where(x => x.Status == false && x.OrderId == null && x.VoucherId == viewModel.VoucherId).ToList();
                                if (voucherDetailsToDelete.Any())
                                {
                                    db.tb_VoucherDetail.RemoveRange(voucherDetailsToDelete);
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "Tất cả voucher đã được dùng.";
                                    return RedirectToAction("Edit", new { id = viewModel.VoucherId });
                                }
                            }
                            dbContextTransaction.Commit();  
                            return RedirectToAction("Index");
                        }
                    }
                    return View(viewModel);
                }
                catch (Exception e) 
                {
                    dbContextTransaction.Rollback();
                   
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi thực hiện thao tác. Vui lòng thử lại sau.");
                    return View(viewModel);
                }  


            }
             
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(EditVoucherViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var item = db.tb_Voucher.Find(viewModel.VoucherId);
        //        if (item != null)
        //        {
        //            item.Code = viewModel.Code;
        //            item.PercentPriceReduction = viewModel.PhanTramGiaGiam;

        //            item.Title = viewModel.Title;
        //           item.CreatedBy = viewModel.CreatedBy;    
        //            item.CreatedDate = viewModel.CreatedDate;   
        //            item.UsedDate = viewModel.UsedDate;
        //            item.ModifiedDate = viewModel.ModifiedDate;
        //            item.Quantity=viewModel.Quantity;


        //            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();

        //            if (viewModel.Quantity != viewModel.OriginalQuantity)
        //            {
        //                // Tính toán số lượng mới được thêm vào
        //                int quantityAdded = viewModel.Quantity -viewModel.OriginalQuantity   ;

        //                // Tạo mới các bản ghi trong tb_VoucherDetail và thêm vào cơ sở dữ liệu
        //                for (int i = 0; i < quantityAdded; i++)
        //                {
        //                    while (true)
        //                    {
        //                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //                        var random = new Random();
        //                        var randomString = new StringBuilder(3);

        //                        for (int x = 0; x < 3; x++)
        //                        {
        //                            randomString.Append(chars[random.Next(chars.Length)]);
        //                        }

        //                        string randomString123 = randomString.ToString(); ;
        //                        string combinedCode = viewModel.Title.Trim() + randomString123;
        //                        string filteredCode = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(combinedCode);

        //                        var checkCode = db.tb_VoucherDetail.FirstOrDefault(x => x.Code == filteredCode);

        //                        if (checkCode == null)
        //                        {
        //                            var voucherDetail = new tb_VoucherDetail();
        //                            voucherDetail.Code = filteredCode;
        //                            voucherDetail.CreatedBy = viewModel.CreatedBy;
        //                            voucherDetail.CreatedDate = viewModel.CreatedDate;
        //                            voucherDetail.VoucherId = viewModel.VoucherId;
        //                            voucherDetail.Status = false;

        //                            db.tb_VoucherDetail.Add(voucherDetail);
        //                            db.SaveChanges();
        //                            break;
        //                        }
        //                    }
        //                }

        //                var voucherDetailsToDelete = db.tb_VoucherDetail.Where(x => x.Status == false && x.OrderId == null && x.VoucherId == viewModel.VoucherId).ToList();
        //                db.tb_VoucherDetail.RemoveRange(voucherDetailsToDelete);
        //                db.SaveChanges();
        //            }




        //            return RedirectToAction("Index");
        //        }
        //    }
        //    return View(viewModel);
        //}



    }
}