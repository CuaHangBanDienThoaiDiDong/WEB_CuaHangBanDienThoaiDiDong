using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class WareHouseImportController : Controller
    {
        // GET: Admin/WareHouseImport
        2CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
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
            var item = db.tb_ImportWarehouseDetail.FirstOrDefault(x => x.ProductDetailId == id);
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
                            foreach (var item in cart.Items)
                            {
                                if (item.Quantity > 0)
                                {
                                    var checkQuantityPro = db.tb_ImportWarehouseDetail.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId && x.tb_ImportWarehouse.tb_Warehouse.WarehouseId == checkWareHouse.WarehouseId);
                                    if (checkQuantityPro != null)
                                    {
                                        checkQuantityPro.QuanTity += item.Quantity;


                                        db.Entry(checkQuantityPro).State = System.Data.Entity.EntityState.Modified;

                                    }

                                }
                                else
                                {
                                    code = new { Success = false, Code = -5, Url = "" };//Khoong nha cung cap
                                    return Json(code);
                                }

                            }
                            //var checkWareHouse = db.tb_Store.FirstOrDefault(x => x.StoreId == nvSession.StoreId);


                            tb_ImportWarehouse ImportWarehouse = new tb_ImportWarehouse();
                            ImportWarehouse.SupplierId = req.Supplierid;
                            ImportWarehouse.CreatedBy = nvSession.NameStaff;
                            ImportWarehouse.CreateDate = DateTime.Now;
                            ImportWarehouse.WarehouseId = checkWareHouse.WarehouseId;


                            cart.Items.ForEach(x => ImportWarehouse.tb_ImportWarehouseDetail.Add(new tb_ImportWarehouseDetail
                            {
                                ProductDetailId = x.ProductDetailId,
                                QuanTity = x.Quantity,

                            }));

                            db.tb_ImportWarehouse.Add(ImportWarehouse);
                            db.SaveChanges();


                            cart.ClearImport();
                            code = new { Success = true, Code = 1, Url = "" };//Xuất hóa đơn thành công
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