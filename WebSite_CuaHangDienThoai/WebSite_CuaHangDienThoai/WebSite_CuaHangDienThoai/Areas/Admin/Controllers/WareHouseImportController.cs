﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.PerformanceData;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;



using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using CKFinder.Settings;

using ZXing;
using ZXing.QrCode;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;




using ImageProcessor.Processors;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Runtime.Remoting.Channels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class WareHouseImportController : Controller
    {
        // GET: Admin/WareHouseImport
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 3));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    ViewBag.Count = db.tb_ImportWarehouse.Count();
                    var items = db.tb_Staff.OrderByDescending(x => x.Code).ToList();
                    return View(items);
                }

            }
        }
        public ActionResult StaffIndex(int? page)
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
                    var items = db.tb_Staff.OrderByDescending(x => x.Code).ToList();
                    ViewBag.Count = db.tb_ImportWarehouse.Count();

                    return View(items);
                }

            }
        }

        public ActionResult Partial_WareHouseImportIndex(int? page)
        {
            IEnumerable<tb_ImportWarehouse> items = db.tb_ImportWarehouse.OrderByDescending(x => x.ImportWarehouseId);
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
                return View(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return View();
            }
        }

        public ActionResult SuggestImportWareHouse(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var SupplierId = db.tb_Supplier
          .Where(c => c.Name.Contains(search))
          .Select(c => c.SupplierId)
          .ToList();

                var StaffId = db.tb_Staff
                    .Where(c => c.NameStaff.Contains(search))
                    .Select(c => c.StaffId)
                    .ToList();

                var ImportWarehouse = db.tb_ImportWarehouse
                    .Where(p => p.CreatedBy.Contains(search) ||
                                SupplierId.Contains((int)p.SupplierId) ||
                                StaffId.Contains((int)p.StaffId))
                    .ToList();



                if (ImportWarehouse.Any())
                {
                    var count = ImportWarehouse.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ImportWarehouse);
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
        public ActionResult ImportWareHouseday(DateTime ngaynhap)
        {
            DateTime selectedDate = ngaynhap;
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1);

            tb_Staff nvSession = (tb_Staff)Session["user"];
            if (nvSession == null)
            {
                return RedirectToAction("NonRole", "HomePage");
            }

            var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 3));
            if (item == null)
            {
                return RedirectToAction("NonRole", "HomePage");
            }


            var importWarehousers = db.tb_ImportWarehouse
                .Where(o => o.CreateDate >= startDate && o.CreateDate < endDate)
                .ToList();

            if (importWarehousers != null && importWarehousers.Count > 0)
            {

                if (item.FunctionId == 2 || item.FunctionId == 1)
                {
                    ViewBag.QuanLy = item.FunctionId;
                }

                ViewBag.Date = ngaynhap;
                ViewBag.Count = importWarehousers.Count;
                return PartialView(importWarehousers);
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult SuggestWareHouseImportCustomer(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {



                var customerids = db.tb_Products
 .Where(c => c.Title.Contains(search))
 .Select(c => c.ProductsId)
 .ToList();


                var productdetail = db.tb_ProductDetail
.Where(s => customerids.Contains((int)s.ProductsId) || s.Color.Contains(search))
.Select(s => s.ProductDetailId)
.ToList();



                var SupplierIds = db.tb_Supplier
       .Where(c => c.Name.Contains(search))
       .Select(c => c.SupplierId)
       .ToList();

                var import = db.tb_ImportWarehouse
                    .FirstOrDefault(s => SupplierIds.Contains((int)s.SupplierId) /*|| s.Code.Contains(search)*/);


                if (import != null)
                {
                    var count = db.tb_ImportWarehouse.Where(s => s.ImportWarehouseId == import.ImportWarehouseId).Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(new List<tb_ImportWarehouse> { import });
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

        public ActionResult WareHouseImportExportToday(DateTime ngayxuat)
        {
            DateTime selectedDate = ngayxuat;
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1);


            var ImportWarehouse = db.tb_ImportWarehouse
                .Where(o => o.CreateDate >= startDate && o.CreateDate < endDate)
                .ToList();

            if (ImportWarehouse != null && ImportWarehouse.Count > 0)
            {
                ViewBag.Date = ngayxuat;
                ViewBag.Count = ImportWarehouse.Count;
                return PartialView(ImportWarehouse);
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult Inventory()//Kho toonf 
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

        public ActionResult SuggestProductInventory(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var ProductCategoryId = db.tb_ProductCategory
          .Where(c => c.Title.Contains(search))
          .Select(c => c.ProductCategoryId)
          .ToList();

                var ProductCompanyId = db.tb_ProductCompany
                    .Where(c => c.Title.Contains(search))
                    .Select(c => c.ProductCompanyId)
                    .ToList();


                var Products = db.tb_Products
                             .Where(p =>
                                 p.Title.Contains(search) ||
                                 p.Alias.Contains(search.Trim()) ||
                                 ProductCategoryId.Contains((int)p.ProductCategoryId) ||
                                 ProductCompanyId.Contains((int)p.ProductCompanyId))
                             .ToList();

                var productIds = Products.Select(p => p.ProductsId).ToList();


                var ProductDetail = db.tb_ProductDetail
                    .Where(s => productIds.Contains((int)s.ProductsId))
                    .ToList();

                if (ProductDetail.Any())
                {
                    var count = ProductDetail.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ProductDetail);
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
        public ActionResult Partial_ListProductInventory(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_ProductDetail> items = db.tb_ProductDetail.OrderByDescending(x => x.ProductDetailId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_ProductDetail.ToList();

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
        public ActionResult ListProduct(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {


                var items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId).ToList();

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(items.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult ListProductWareHouse(int? page)
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

        public ActionResult Details(int? id)
        {
            var item = db.tb_ImportWarehouse.Find(id);
            if (item != null)
            {
                ViewBag.Supplier = item.tb_Supplier.Name;
                return PartialView(item);
            }
            else
            {
                return RedirectToAction("index", "WareHouseImport");
            }
        }


        public ActionResult Edit(int? id) 
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            tb_Staff nvSession = (tb_Staff)Session["user"];
            var role = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 3));
            if (role == null)
            {
                
                
                return RedirectToAction("NonRole", "HomePage");
            }



            var wareHouse = db.tb_ImportWarehouse.Find(id);
            Admin_TokenEditImportWareHouse viewModel = new Admin_TokenEditImportWareHouse
            {
                ImportWareHosue = wareHouse.ImportWarehouseId,
                WarehouseId = (int)wareHouse.WarehouseId,
                SupplierId = (int)wareHouse.SupplierId,
                CreatedBy = wareHouse.CreatedBy,
                CreatedDate = (DateTime)wareHouse.CreateDate,
                StaffId = (int)wareHouse.StaffId,
                Staff = db.tb_Staff.FirstOrDefault(x => x.StaffId == wareHouse.StaffId)
            };

            var checkItem = db.tb_ImportWarehouseDetail.Where(x => x.ImportWarehouseId == id)
                          .Select(detail => new Admin_TokenEditImportWareHouseItem
                          {
                              ImportWarehouseDetailId = detail.ImportWarehouseDetailId,
                              ImportWareHosueId = (int)detail.ImportWarehouseId,
                              Quantity = (int)detail.QuanTity,
                              ProductDetailId = (int)detail.ProductDetailId,
                              Product = db.tb_ProductDetail.FirstOrDefault(p => p.ProductDetailId == detail.ProductDetailId)
                          }).ToList();

            if (viewModel == null)
            {
                return HttpNotFound();
            }
            viewModel.Items = checkItem;
            Session["Admin_TokenEditImportWareHouse_" + id] = viewModel;
            ViewBag.SupplierList = new SelectList(db.tb_Supplier.ToList(), "SupplierId", "Name", wareHouse.SupplierId);
            return PartialView(viewModel);
        }
        public ActionResult Partail_ListProductForEdit(int id)
        {
            Admin_TokenEditImportWareHouse checkItem = (Admin_TokenEditImportWareHouse)Session["Admin_TokenEditImportWareHouse_" + id];

            if (checkItem != null && checkItem.Items.Any())
            {
                int count = checkItem.Items.Count;
                ViewBag.Count = count;
                return PartialView("_ListProductForEdit", checkItem); 
            }

            return PartialView("_ListProductForEdit");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin_TokenEditImportWareHouse model)
        {
            // Kiểm tra xác thực
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, code = -1, message = "Dữ liệu không hợp lệ." });
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra đăng nhập
                    if (Session["user"] == null)
                    {
                        return RedirectToAction("DangNhap", "Account");
                    }

                    // Lấy thông tin người dùng từ session
                    tb_Staff nvSession = (tb_Staff)Session["user"];

                    // Lấy thông tin nhập kho cần chỉnh sửa từ database
                    var importWarehouse = db.tb_ImportWarehouse.Find(model.ImportWareHosue);
                    if (importWarehouse == null)
                    {
                        return Json(new { success = false, code = -2, message = "Không tìm thấy hóa đơn nhập kho." });
                    }

                    // Cập nhật thông tin hóa đơn nhập kho
                    importWarehouse.WarehouseId = model.WarehouseId;
                    importWarehouse.SupplierId = model.SupplierId;
                    importWarehouse.CreatedBy = model.CreatedBy;
                    importWarehouse.CreateDate = model.CreatedDate;
                    importWarehouse.StaffId = model.StaffId;
                    importWarehouse.ModifiedDate = DateTime.Now;
                    importWarehouse.Modifeby = nvSession.NameStaff;

                    db.Entry(importWarehouse).State = EntityState.Modified;

                    // Lưu thông tin hóa đơn nhập kho vào database
                    db.SaveChanges();

                    // Xử lý chi tiết nhập kho
                    var sessionKey = "Admin_TokenEditImportWareHouse_" + model.ImportWareHosue;
                    var viewModel = Session[sessionKey] as Admin_TokenEditImportWareHouse;
                    if (viewModel == null || viewModel.Items.Count < 1)
                    {
                        return Json(new { success = false, code = -4, message = "Hóa đơn phải có ít nhất 1 sản phẩm." });
                    }

                    // Lấy các chi tiết nhập kho hiện tại từ database
                    var existingDetails = db.tb_ImportWarehouseDetail.Where(d => d.ImportWarehouseId == importWarehouse.ImportWarehouseId).ToList();

                    // Duyệt qua các chi tiết nhập kho hiện tại trong database
                    foreach (var detail in existingDetails.ToList())
                    {
                        // Tìm chi tiết nhập kho tương ứng từ viewModel
                        var viewModelDetail = viewModel.Items.FirstOrDefault(item => item.ImportWarehouseDetailId == detail.ImportWarehouseDetailId);
                        if (viewModelDetail != null)
                        {
                            // Nếu số lượng có sự thay đổi
                            if (detail.QuanTity != viewModelDetail.Quantity)
                            {
                                // Tính toán sự thay đổi số lượng
                                int oldQuantity = (int)detail.QuanTity; // Số lượng hiện tại
                                int newQuantity = viewModelDetail.Quantity; // Số lượng mới
                                int quantityChange = newQuantity - oldQuantity; // Thay đổi số lượng

                                // Cập nhật số lượng trong chi tiết nhập kho
                                detail.QuanTity = newQuantity;
                                db.Entry(detail).State = EntityState.Modified;

                                // Cập nhật số lượng trong tb_WarehouseDetail
                                if (detail.ProductDetailId != null)
                                {
                                    var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == detail.ProductDetailId);
                                    if (warehouseDetail != null)
                                    {
                                        warehouseDetail.QuanTity += quantityChange; // Cập nhật số lượng tồn kho
                                        db.Entry(warehouseDetail).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        // Trường hợp không tìm thấy tb_WarehouseDetail
                                        return Json(new { success = false, code = -5, message = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                                    }
                                }
                                else
                                {
                                    // Trường hợp ProductDetailId không có giá trị
                                    return Json(new { success = false, code = -6, message = "Chi tiết nhập kho không có ProductDetailId." });
                                }
                            }
                        }
                        else
                        {
                            // Trường hợp không tìm thấy chi tiết nhập kho trong viewModel
                          
                            // Xóa chi tiết nhập kho cũ trong tb_WarehouseDetail
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    warehouseDetail.QuanTity -= detail.QuanTity; // Giảm số lượng tồn kho
                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                    db.tb_ImportWarehouseDetail.Remove(detail);
                                }
                                else
                                {
                                    // Trường hợp không tìm thấy tb_WarehouseDetail
                                    return Json(new { success = false, code = -5, message = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                // Trường hợp ProductDetailId không có giá trị
                                return Json(new { success = false, code = -6, message = "Chi tiết nhập kho không có ProductDetailId." });
                            }
                        }
                    }

                    // Lưu các thay đổi vào database
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    // Xuất hóa đơn và trả về đường dẫn file
                    string invoicePath = ExportInvoice(model.ImportWareHosue);
                    if (!string.IsNullOrEmpty(invoicePath))
                    {
                        return Json(new { success = true, code = 1, filePath = invoicePath });
                    }
                    else
                    {
                        return Json(new { success = false, code = -7, message = "Không thể xuất hóa đơn." });
                    }
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100, message = $"Lỗi ngoại lệ: {ex.Message}" });
                }
            }
        }



  
        [HttpPost]
        public ActionResult UpdateQuantityForEditNew(int productDetailId, int importWarehouseDetailId, int importWarehouseId, int quantity)
     {
            
            var sessionKey = "Admin_TokenEditImportWareHouse_" + importWarehouseId;
            var viewModel = Session[sessionKey] as Admin_TokenEditImportWareHouse;

            if (viewModel == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin hóa đơn trong session." });
            }

          
            var itemToUpdate = viewModel.Items.FirstOrDefault(item => item.ImportWarehouseDetailId == importWarehouseDetailId && item.ProductDetailId == productDetailId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;

              
                Session[sessionKey] = viewModel;

                return Json(new { success = true, message = "Cập nhật số lượng thành công." });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm cần cập nhật." });
        }

        [HttpPost]
        public ActionResult DeleteItemImportWareHouse(int id, int ImportWarehouseDetailId)
        {
            try
            {
                if (Session["Admin_TokenEditImportWareHouse_" + id] != null)
                {
                    Admin_TokenEditImportWareHouse viewModel = (Admin_TokenEditImportWareHouse)Session["Admin_TokenEditImportWareHouse_" + id];
                    if (viewModel != null)
                    {
                        if (viewModel.Items.Count > 1)
                        {
                            viewModel.Items.RemoveAll(x => x.ImportWarehouseDetailId == ImportWarehouseDetailId);

                            Session["Admin_TokenEditImportWareHouse_" + id] = viewModel;

                            return Json(new { success = true, code = 1, message = "Xóa sản phẩm thành công" });
                        }
                        else { return Json(new { success = false, message = "Bắt buộc 1 sản phẩm" }); }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }




        [HttpPost]
        public ActionResult UpdateQuanTityForEdit(int id, int ImportWareHouseDetailId, int ImportWareHouseId, int IdWareHouse, int quantity)
        {
            var code = new { Success = false, Code = -1, Url = "" };

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var CheckWareHouse = db.tb_ImportWarehouseDetail.Find(ImportWareHouseDetailId);
                    if (CheckWareHouse != null)
                    {
                        // Cập nhật tb_ImportWarehouseDetail
                        var CheckItem = db.tb_ImportWarehouseDetail.FirstOrDefault(x => x.ImportWarehouseDetailId == ImportWareHouseDetailId && x.ProductDetailId == id && x.ImportWarehouseId == ImportWareHouseId);
                        if (CheckItem != null)
                        {
                            CheckItem.QuanTity = quantity;
                            db.Entry(CheckItem).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            code = new { Success = false, Code = -2, Url = "" }; // Sản phẩm không ở kho
                        }

                        // Cập nhật tb_WarehouseDetail
                        var CheckItemWareHouse = db.tb_WarehouseDetail.FirstOrDefault(x => x.WarehouseId == IdWareHouse && x.ProductDetailId == id);
                        if (CheckItemWareHouse != null)
                        {
                            CheckItemWareHouse.QuanTity = quantity;
                            db.Entry(CheckItemWareHouse).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            code = new { Success = false, Code = -2, Url = "" }; // Sản phẩm không ở kho
                        }

                        transaction.Commit(); // Giao dịch thành công, lưu các thay đổi
                        code = new { Success = true, Code = 1, Url = "" };
                    }
                    else
                    {
                        code = new { Success = false, Code = -1, Url = "" }; // Không tìm thấy phiếu nhập
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Nếu có lỗi, rollback giao dịch
                    code = new { Success = false, Code = -3, Url = "" }; // Cập nhập phiếu thất bại
                }
            }

            return Json(code);
        }

        public ActionResult Partail_ProductDetail()
        {
            var item = db.tb_ProductDetail.ToList();
            return View(item);
        }

        public ActionResult Partail_PhieuNhapKho()
        {

            ViewBag.Supplier = new SelectList(db.tb_Supplier.ToList(), "SupplierId", "name");
            return PartialView();
        }



        public ActionResult Partail_ListProductForDetail(int id)
        {
            var items = db.tb_ImportWarehouseDetail.Where(x => x.ImportWarehouseId == id).ToList();
            ViewBag.Count = items.Count();
            return PartialView(items);
        }


      



        public ActionResult Partail_ListProduct() 
        {
            ImportWareHouse cart = (ImportWareHouse)Session["ImportWareHouse"];
            if (cart != null && cart.Items.Any())
            {
                int count = cart.Items.Count;
                ViewBag.Count = count;
                return PartialView(cart.Items);
            }
            return PartialView();
        }

      
        public ActionResult Partial_Supplier(int id)
        {
            var item = db.tb_Supplier.Find(id);
            if (item != null)
            {
                return PartialView("_SupplierDetails", item);
            }
            return PartialView("_SupplierDetails");
        }


     
       

        [HttpPost]
        public ActionResult AddListProduct(int id, int soluong)
        {
            var code = new { Success = false, Code = -1, Count = 0 };
            var checkSanPham = db.tb_ProductDetail.FirstOrDefault(row => row.ProductDetailId  == id);
            if (checkSanPham != null)
            {

                if (checkSanPham !=null)
                {
                    ImportWareHouse cart = (ImportWareHouse)Session["ImportWareHouse"];
                    if (cart == null)
                    {
                        cart = new ImportWareHouse();
                    }
                    ImportWareHouseItem item = new ImportWareHouseItem
                    {
                        ProductDetailId = checkSanPham.ProductDetailId,
                        ProductName = checkSanPham.tb_Products.Title,
                        Category = checkSanPham.tb_Products.tb_ProductCategory.Title,
                        Capacity=(int)checkSanPham.Capacity,
                        Color=checkSanPham.Color,
                        Company=checkSanPham.tb_Products.tb_ProductCompany.Title,
                        
                        Quantity = 0,
                    };
                    if (checkSanPham.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                    {
                        item.ProductDetailImg = checkSanPham.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                    }
                

                    //checkSanPham.Quantity = -soluong;
                    cart.AddToCart(item, soluong);
                    Session["ImportWareHouse"] = cart;
                    code = new { Success = true, Code = 1, Count = cart.Items.Count };

                }
                else
                {
                    code = new { Success = false, Code = -1, Count = 0 };//Số Lượng Không Đủ
                }

            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult UpdateQuanTity(int id, int quantity)
        {
            ImportWareHouse cart = (ImportWareHouse)Session["ImportWareHouse"];
            if (cart != null && cart.Items.Any())
            {
                cart.UpQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

      

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, Code = -1, Count = 0 };

            ImportWareHouse cart = (ImportWareHouse)Session["ImportWareHouse"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductDetailId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, Code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }

        public ActionResult Partial_QuantityProductDtail(int id) {
            var item = db.tb_WarehouseDetail.FirstOrDefault(x => x.ProductDetailId == id);
            if (item != null) 
            {
                ViewBag.Quantity = item.QuanTity;
                return PartialView(item);
            }
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
        public ActionResult SuggestProduct(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var ProductCategoryId = db.tb_ProductCategory
          .Where(c => c.Title.Contains(search))
          .Select(c => c.ProductCategoryId)
          .ToList();

                var ProductCompanyId = db.tb_ProductCompany
                    .Where(c => c.Title.Contains(search))
                    .Select(c => c.ProductCompanyId)
                    .ToList();


                var Products = db.tb_Products
                             .Where(p =>
                                 p.Title.Contains(search) ||
                                 p.Alias.Contains(search.Trim()) ||
                                 ProductCategoryId.Contains((int)p.ProductCategoryId) ||
                                 ProductCompanyId.Contains((int)p.ProductCompanyId))
                             .ToList();


                var productIds = Products.Select(p => p.ProductsId).ToList();


                var ProductDetail = db.tb_ProductDetail
                    .Where(s => productIds.Contains((int)s.ProductsId))
                    .ToList();

                if (ProductDetail.Any())
                {
                    var count = ProductDetail.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ProductDetail);
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


        public ActionResult SuggestProductListProduct(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var ProductCategoryId = db.tb_ProductCategory
          .Where(c => c.Title.Contains(search))
          .Select(c => c.ProductCategoryId)
          .ToList();

                var ProductCompanyId = db.tb_ProductCompany
                    .Where(c => c.Title.Contains(search))
                    .Select(c => c.ProductCompanyId)
                    .ToList();


                var Products = db.tb_Products
                             .Where(p =>
                                 p.Title.Contains(search) ||
                                 p.Alias.Contains(search.Trim()) ||
                                 ProductCategoryId.Contains((int)p.ProductCategoryId) ||
                                 ProductCompanyId.Contains((int)p.ProductCompanyId))
                             .ToList();


                var productIds = Products.Select(p => p.ProductsId).ToList();


                var ProductDetail = db.tb_ProductDetail
                    .Where(s => productIds.Contains((int)s.ProductsId))
                    .ToList();

                if (ProductDetail.Any())
                {
                    var count = ProductDetail.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ProductDetail);
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
        public ActionResult AddWareHouse()
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
        public ActionResult Add(Admin_TokenImportWareHouse req)
        {
           
                try
            {
                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }
                else
                {
                    tb_Staff nvSession = (tb_Staff)Session["user"];

                    if (nvSession.NameStaff != null)
                    {
                        if (req.Supplierid > 0)
                        {
                            ImportWareHouse cart = (ImportWareHouse)Session["ImportWareHouse"];
                            if (cart != null)
                            {
                                var checkWareHouse = db.tb_Warehouse.SingleOrDefault(x => x.StoreId == nvSession.StoreId);
                                if (checkWareHouse != null)
                                {

                                    //var checkWareHouse = db.tb_Store.FirstOrDefault(x => x.StoreId == nvSession.StoreId);
                                    using (var transaction = db.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            int ImportWarehouseId = 0;
                                            try
                                            {
                                                tb_ImportWarehouse ImportWarehouse = new tb_ImportWarehouse();
                                                ImportWarehouse.SupplierId = req.Supplierid;
                                                ImportWarehouse.CreatedBy = nvSession.NameStaff;
                                                ImportWarehouse.CreateDate = DateTime.Now;
                                                ImportWarehouse.WarehouseId = checkWareHouse.WarehouseId;
                                                ImportWarehouse.StaffId = nvSession.StaffId;

                                                cart.Items.ForEach(x => ImportWarehouse.tb_ImportWarehouseDetail.Add(new tb_ImportWarehouseDetail
                                                {
                                                    ProductDetailId = x.ProductDetailId,
                                                    QuanTity = x.Quantity,

                                                }));

                                                db.tb_ImportWarehouse.Add(ImportWarehouse);
                                                db.SaveChanges();
                                                ImportWarehouseId = ImportWarehouse.ImportWarehouseId;
                                            }
                                            catch (Exception ex)
                                            {
                                                return Json(new { Success = false, Code = -7, Url = "" }); // Phiếu nhậ bị lỗi

                                            }

                                            try
                                            {
                                                foreach (var item in cart.Items)
                                                {
                                                    if (item.Quantity > 0)
                                                    {

                                                        var warehouseDetail = db.tb_WarehouseDetail
                                                            .FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId && x.WarehouseId == checkWareHouse.WarehouseId);

                                                        if (warehouseDetail == null)
                                                        {
                                                            warehouseDetail = new tb_WarehouseDetail
                                                            {
                                                                ProductDetailId = item.ProductDetailId,
                                                                QuanTity = item.Quantity,
                                                                WarehouseId = checkWareHouse.WarehouseId
                                                            };
                                                            db.tb_WarehouseDetail.Add(warehouseDetail);
                                                            db.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            warehouseDetail.QuanTity += item.Quantity;
                                                            db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;
                                                            db.SaveChanges();
                                                        }

                                                    }
                                                    else
                                                    {

                                                        return Json(new { Success = false, Code = -5, Url = "" }); // Số lượng không hợp lệ

                                                    }

                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                return Json(new { Success = false, Code = -7, Url = "" }); // Lưu kho bị lỗi

                                            }


                                            if (ImportWarehouseId < 0)
                                            {
                                                return Json(new { Success = false, Code = -7, Url = "" });//Lỗi xuất phiéue nhập
                                            }
                                            string invoicePath = ExportInvoice(ImportWarehouseId);
                                            if (!string.IsNullOrEmpty(invoicePath))
                                            {
                                                cart.ClearImport();
                                                transaction.Commit();
                                                return Json(new { Success = true, Code = 1, FilePath = invoicePath });
                                            }
                                            else
                                            {
                                                return Json(new { Success = false, Code = -7, FilePath = invoicePath });

                                            }



                                        }
                                        catch (Exception ex)
                                        {
                                            transaction.Rollback();
                                            return Json(new { Success = false, Code = -10, Url = "" }); // Cập nhập phiếu thất bại
                                        }


                                    }

                                }
                                else
                                {
                                    return Json(new { Success = false, Code = -6, Url = "" }); // Không tồn tại kho

                                }


                            }
                            else
                            {
                                return Json(new { Success = false, Code = -3, Url = "" }); // Không có sản phẩm
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, Code = -4, Url = "" });//Khoong nha cung cap
                        }

                    }
                    else
                    {
                        return Json(new { Success = false, Code = -2, Url = "" }); // Lỗi không tìm thấy nhân viên
                    }

                }
            }
               
                catch (Exception ex)
                {
                    
                        return Json(new { Success = false, Code = -100, Url = "" });
                } 
            

         

        }






        public ActionResult Partial_AddQuantityForProduct(int id ) 
        {
            return PartialView();
        }
        public ActionResult AddQuantityForProduct(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_ProductDetail.Find(id);

                if (item == null)
                {

                    return RedirectToAction("Index", "Store");
                }
                return View(item);
            }
                
        }




        //Start Xuat Word

        [HttpGet]
        public ActionResult InBillLaiNew(int id)
        {
            try
            {
                if (Session["user"] == null)
                {
                    return Json(new { success = false, Code = -1, Message = "Phiên đăng nhập không tồn tại" }, JsonRequestBehavior.AllowGet);
                }

                if (id < 0)
                {
                    return Json(new { success = false, Code = -2, Message = "ID không hợp lệ" }, JsonRequestBehavior.AllowGet);
                }

                string invoicePath = ExportInvoice(id);

                if (!string.IsNullOrEmpty(invoicePath))
                {
                    return Json(new { success = true, Code = 1, FilePath = invoicePath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Code = -10, Message = "Lỗi khi in hóa đơn" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Code = -100, Message = "Lỗi ngoại lệ: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DownloadInvoiceEdit(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }

        public ActionResult DownloadInvoice(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }

        private string ExportInvoice(int orderId)
        {
            var order = db.tb_ImportWarehouse.Find(orderId);
            if (order != null)
            {
                var Supplier = db.tb_Supplier.Find(order.SupplierId);
                if (Supplier != null)
                {
                    try
                    {
                        string templatePath = Server.MapPath("~/Content/templates/PhieuNhap.html");
                        if (templatePath == null) 
                        {
                            return null;
                        }
                        string htmlContent = System.IO.File.ReadAllText(templatePath);

                        string ImportWarehouseId = order.ImportWarehouseId.ToString();
                        string PhoneSupplier = Supplier.Phone.ToString();
                        string CreateDate = order.CreateDate.ToString();
                       
                        htmlContent = htmlContent.Replace("#{{Day}}", DateTime.Now.ToString("dd/MM/yyyy"));
                        htmlContent = htmlContent.Replace("#{{ImportWarehouseId}}", ImportWarehouseId);

                        htmlContent = htmlContent.Replace("#{{SupplierName}}", Supplier.Name);
                        htmlContent = htmlContent.Replace("#{{Location}}", Supplier.Location);
                        htmlContent = htmlContent.Replace("#{{Phone}}", PhoneSupplier);
                  
                            




                        htmlContent = htmlContent.Replace("#{{CreatedBy}}", order.CreatedBy);
                        htmlContent = htmlContent.Replace("#{{CreateDate}}", CreateDate);

                        // Lấy chi tiết đơn hàng
                        var orderDetails = db.tb_ImportWarehouseDetail
                            .Where(od => od.ImportWarehouseId == order.ImportWarehouseId)
                            .ToList();



                   
                        string TotalQuantity = orderDetails.Count().ToString();
                        htmlContent = htmlContent.Replace("#{{TotalQuantity}}", TotalQuantity);
                        string productRows = "";
                        int index = 1;

                        foreach (var detail in orderDetails)
                        {

                            var productDetail = db.tb_ProductDetail.FirstOrDefault(pd => pd.ProductDetailId == detail.ProductDetailId);
                            string productTitle =null;
                            if (productDetail != null )
                            {
                                productTitle = detail.tb_ProductDetail.tb_Products.Title;
                            }
                            else 
                            {
                                return null;
                            }

                            var test = detail.tb_ProductDetail.tb_Products.Title;
                            string productRow = $@"
                <tr>
                    <td>{index}</td>
                    <td>{productDetail.tb_Products.Title}</td>
                    <td>{GetCapacity((int)productDetail.Capacity)} / {productDetail.Color}</td>
                    <td>{detail.QuanTity}</td>
                   </tr>";
                            productRows += productRow;
                            index++;
                        }

                        htmlContent = htmlContent.Replace("#{{ProductRows}}", productRows);

                        // Tính tổng tiền thanh toán



                        //string fileName = $"PhieuNhap_{order.ImportWarehouseId}_{order.tb_Supplier.Name}.docx";
                        string fileName = $"PhieuNhap_{order.ImportWarehouseId}_{order.tb_Supplier.Name}_{DateTime.Now.Date.ToString("dd-MM-yyyy")}.docx";

                        string folderPath = Server.MapPath("~/Order");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string filePath = Path.Combine(folderPath, fileName);

                        // Chuyển đổi HTML sang Word
                        string logoPath = Server.MapPath("~/images/Logo/LogoWebpng.png");
                        ConvertHTMLToWord(htmlContent, filePath, logoPath);

                        return filePath;
                    }
                    catch (Exception ex)
                    {
                        // Xử lý nếu có lỗi
                        // Có thể ghi log hoặc thông báo lỗi ở đây
                        return null;
                    }
                }
                return null;

            }
            return null;
        }

        private string GetImageBase64(string imagePath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            string mimeType = "image/" + Path.GetExtension(imagePath).TrimStart('.');
            return $"data:{mimeType};base64,{base64String}";
        }
        private void ConvertHTMLToWord(string htmlContent, string wordFilePath, string logoPath)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                // Tạo một tài liệu Word mới
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    // Tạo phần main của tài liệu
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Thêm hình ảnh logo vào tài liệu Word
                    if (System.IO.File.Exists(logoPath))
                    {
                        AddImageToBody(mainPart, logoPath);
                    }

                    // Chuyển đổi HTML thành Open XML và thêm vào tài liệu Word
                    string altChunkId = "AltChunkId1";
                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, altChunkId);
                    using (StreamWriter streamWriter = new StreamWriter(chunk.GetStream()))
                    {
                        streamWriter.Write(htmlContent);
                    }

                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;
                    body.Append(altChunk);

                    // Lưu tài liệu Word xuống tệp
                    mainPart.Document.Save();
                }

                // Lưu tài liệu Word xuống tệp
                using (FileStream fileStream = new FileStream(wordFilePath, FileMode.Create))
                {
                    memStream.Position = 0;
                    memStream.CopyTo(fileStream);
                }
            }
        }

        // Hàm thêm hình ảnh vào tài liệu Word
        private void AddImageToBody(MainDocumentPart mainPart, string imagePath)
        {
            string imagePartId = "imageId";
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png, imagePartId);

            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            var element = new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = 990000L, Cy = 792000L },
                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture 1"
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "New Bitmap Image.png"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()
                                ),
                                new PIC.BlipFill(
                                    new A.Blip()
                                    {
                                        Embed = imagePartId,
                                        CompressionState = A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle()
                                    )
                                ),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = 990000L, Cy = 792000L }
                                    ),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                                )
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                    )
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = "50D07946"
                });

            mainPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        private string GetCapacity(int capacity)
        {
            if (capacity > 1999)
            {
                return "2 Tb";
            }
            else if (capacity > 999)
            {
                return "1 Tb";
            }
            else
            {
                return capacity + " Gb";
            }
        }


        //endXuat Word



    }
}