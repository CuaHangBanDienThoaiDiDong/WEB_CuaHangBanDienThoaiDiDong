using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class StoreController : Controller
    {
        // GET: Admin/Store
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_Store> items = db.tb_Store.OrderByDescending(x => x.StoreId);
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







        public ActionResult Details(int id)
        {
            var items = db.tb_Products.Find(id);
            return View(items);

        }



     

        public ActionResult Partial_AddStore()
        {
            ViewBag.Provinces = new SelectList(db.Provinces.ToList(), "idProvinces", "name");
            ViewBag.Districts = new SelectList(db.Districts.ToList(), "idDistricts", "name");
            ViewBag.Wards = new SelectList(db.Wards.ToList(), "idWards", "name");
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
        public ActionResult Add(tb_Store model, List<string> Images, List<int> rDefault, Admin_TokenStore req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            tb_Staff nvSession = (tb_Staff)Session["user"];
            if (nvSession.StaffId >0) 
            {
                if (req.Location != null && req.idProvinces !=0 && req.idDistricts != 0 && req.idWards != 0)
                {
                    var Provinces = db.Provinces.Find(req.idProvinces);
                    var Districts = db.Districts.Find(req.idDistricts);
                    var Wards = db.Wards.Find(req.idWards);

                    string alias = model.Location.Trim() + "" + Provinces.name.Trim() + "" + Districts.name.Trim() + "" + Wards.name.Trim();

                    var checkTitle = db.tb_Store.SingleOrDefault(r => r.Location == req.Location && r.Alias == alias && r.idProvinces == req.idProvinces && r.idDistricts == req.idDistricts && r.idWards == req.idWards);
                    if (checkTitle == null)
                    {
                       
                            if (ModelState.IsValid)
                            {
                                

                                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
                                model.CreatedBy = checkStaff.NameStaff + "-" + checkStaff.Code;
                                model.CreatedDate = DateTime.Now;
                                model.ModifiedDate = DateTime.Now;
                                model.IsStatus = req.isStatus;
                                model.Location = req.Location;
                                
                             
                                if (string.IsNullOrEmpty(model.Alias))
                                {
                                    model.Alias = alias;
                                }
                                db.tb_Store.Add(model);
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




        public ActionResult Edit(int? id)
        {

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Store.Find(id);
                ViewBag.SelectedProvince = item.idProvinces;
                ViewBag.SelectedDistrict = item.idDistricts;
                ViewBag.SelectedWard = item.idWards;

                // Load danh sách tỉnh/thành phố
                ViewBag.Provinces = new SelectList(db.Provinces.ToList(), "idProvinces", "name");

                // Load danh sách quận/huyện dựa trên tỉnh/thành phố đã chọn
                ViewBag.Districts = new SelectList(db.Districts.Where(d => d.idProvinces == item.idProvinces).ToList(), "idDistricts", "name");

                // Load danh sách phường/xã dựa trên quận/huyện đã chọn
                ViewBag.Wards = new SelectList(db.Wards.Where(w => w.idDistricts == item.idDistricts).ToList(), "idWards", "name");




                if (item == null)
                {
                  
                    return RedirectToAction("Index", "Store");
                }
                return View(item);
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_Store model)
        {
            if (ModelState.IsValid)
            {

                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);
               
                model.IsStatus = false;
               
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebSite_CuaHangDienThoai.Models.Common.Filter.FilterChar(model.Location);
                db.tb_Store.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm cửa hàng thành công."; // Thông báo thành công

                return RedirectToAction("Index");
            }
            return View(model);

        }









        [HttpPost]
        public ActionResult IsStatus(int id)
        {
            var item = db.tb_Store.Find(id);
            if (item != null)
            {
                item.IsStatus = !item.IsStatus;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isStatus = item.IsStatus });
            }

            return Json(new { success = false });
        }


    }
}