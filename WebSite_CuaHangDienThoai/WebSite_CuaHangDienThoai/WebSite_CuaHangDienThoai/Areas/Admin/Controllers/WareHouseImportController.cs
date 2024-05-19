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
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
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
                        
                        SoLuong = 0,
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