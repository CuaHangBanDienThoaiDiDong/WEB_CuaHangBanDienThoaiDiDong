using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

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
        }


        public ActionResult ListProduct(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_ProductDetail> items = db.tb_ProductDetail.OrderByDescending(x => x.ProductsId);
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
        }

        public ActionResult Details(int? id)
        {
            var item = db.tb_ImportWarehouse.Find(id);
            if (item != null)
            {
                ViewBag.Supplier = item.tb_Supplier.Name;
                return View(item);
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

            var item = db.tb_ImportWarehouse.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.SupplierList = new SelectList(db.tb_Supplier.ToList(), "SupplierId", "Name", item.SupplierId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_ImportWarehouse model)
        {
            var result = new { Success = false, Message = "Error" };
            if (ModelState.IsValid)
            {
                try
                {
                    tb_Staff nvSession = (tb_Staff)Session["user"];
                    var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                    model.Modifeby = checkStaff.NameStaff + "-" + checkStaff.Code;
                    model.ModifiedDate = DateTime.Now;

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    result = new { Success = true, Message = "Edit successful" };
                }
                catch (Exception ex)
                {
                    result = new { Success = false, Message = ex.Message };
                }
            }
            return Json(result);
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


        public ActionResult Partail_ListProductForEdit(int id) 
        {
            var checkItem= db.tb_ImportWarehouseDetail.Where(x => x.ImportWarehouseId == id).ToList();
            ViewBag.Count = checkItem.Count();
            if (checkItem == null)
            {
                return HttpNotFound();
            }

       
            return PartialView(checkItem);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Admin_TokenImportWareHouse req) 
        {
           tb_Staff nvSession = (tb_Staff)Session["user"];
            var code = new { Success = false, Code = -1, Url = "" };
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

                                    }
                                    catch (Exception ex)
                                    {
                                        code = new { Success = false, Code = -7, Url = "" }; // Phiếu nhậ bị lỗi

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

                                                code = new { Success = false, Code = -5, Url = "" }; // Số lượng không hợp lệ
                                                return Json(code);
                                            }

                                        }
                                       
                                    }
                                    catch (Exception ex)
                                    {
                                        code = new { Success = false, Code = -8, Url = "" }; // Lưu kho bị lỗi

                                    }
                                    //foreach (var item in cart.Items)
                                    //{
                                    //    if (item.Quantity > 0)
                                    //    {
                                    //        var checkQuantityPro = db.tb_ImportWarehouseDetail.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId && x.tb_ImportWarehouse.tb_Warehouse.WarehouseId == checkWareHouse.WarehouseId);
                                    //        if (checkQuantityPro != null)
                                    //        {
                                    //            tb_ImportWarehouseDetail ImportWarehouseDetail = new tb_ImportWarehouseDetail();
                                    //            ImportWarehouseDetail.

                                    //        }

                                    //    }
                                    //    else
                                    //    {
                                           
                                    //    }

                                    //}

                                    cart.ClearImport();
                                    transaction.Commit(); // Giao dịch thành công, lưu các thay đổi
                                    code = new { Success = true, Code = 1, Url = "" };
                                   
                                    return Json(code);
                                   
                                    

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback(); 
                                    code = new { Success = false, Code = -10, Url = "" }; // Cập nhập phiếu thất bại
                                }


                            }
                               
                        } 
                        else
                        {
                            code = new { Success = false, Code = -6, Url = "" }; // Không tồn tại kho

                        }
                       

                    }
                    else
                    {
                        code = new { Success = false, Code = -3, Url = "" }; // Không có sản phẩm
                    }
                } 
                else
                {
                    code = new { Success = false, Code = -4, Url = "" };//Khoong nha cung cap
                }
                
            }
            else
            {
                code = new { Success = false, Code = -2, Url = "" }; // Lỗi không tìm thấy nhân viên
            }
            return Json(code);

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







    }
}