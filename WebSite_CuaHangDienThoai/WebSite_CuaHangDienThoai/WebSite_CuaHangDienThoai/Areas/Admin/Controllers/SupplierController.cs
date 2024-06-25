using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Admin/Supplier
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    IEnumerable<tb_Supplier> items = db.tb_Supplier.OrderByDescending(x => x.SupplierId);
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
        public ActionResult Partial_AddSupplier()
        {

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Supplier model,  Admin_TokenSupplier req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            tb_Staff nvSession = (tb_Staff)Session["user"];
            if (nvSession.StaffId > 0)
            {
                if (req.Location != null && req.Phone != 0 && req.Name != null )
                {
                   

                 
                    var checkTitle = db.tb_Supplier.SingleOrDefault(r => r.Location == req.Location  && r.Name == req.Name && r.Phone == req.Phone );
                    if (checkTitle == null)
                    {

                        if (ModelState.IsValid)
                        {


                            var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                        
                            model.Location = req.Location;
model.Name = req.Name;
                            model.Phone = req.Phone;

                          
                            db.tb_Supplier.Add(model);
                            db.SaveChanges();
                            code = new { Success = true, Code = 1, Url = "" };
                        }

                    }
                    else
                    {
                        //Cửa hàng đã tồn tại 
                        code = new { Success = false, Code = -3, Url = "" };
                    }
                }
                else
                {
                    // Điền không đầy đủ thông tin
                    code = new { Success = false, Code = -2, Url = "" };
                }
            }
            else
            {

                code = new { Success = false, Code = -4, Url = "" };
            }

            return Json(code);
        }

        public ActionResult Details(int id)
        {
            var items = db.tb_Supplier.Find(id);
            return View(items);

        }
        public ActionResult Partail_DanhSachPhieu(int id) 
        {
            var item =db.tb_ImportWarehouse.Where(x=>x.SupplierId==id).ToList();
            if(item != null) 
            {
                return PartialView(item);   
            }
            return PartialView();   
        }



        public ActionResult Edit(int? id)
        {

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Supplier.Find(id);
        
                if (item == null)
                {

                    return RedirectToAction("Index", "Store");
                }
                return View(item);
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Supplier model)
        {
            if (ModelState.IsValid)
            {

                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                db.tb_Supplier.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm cửa hàng thành công."; // Thông báo thành công

                return RedirectToAction("Index");
            }
            return View(model);

        }







    }




}