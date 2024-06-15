
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using System.Web.Security;
using System.Security.Claims;
using System.IO;



namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DangNhap()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string Code, string password, string user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f_password = MaHoaPass(password);

                    var data = db.tb_Staff.SingleOrDefault(s => s.Code.Equals(Code) && s.Password.Equals(f_password));

                    if (data != null)
                    {
                        var checklock = db.tb_Staff.FirstOrDefault(s => s.Code.Equals(Code) && s.Clock == false);

                        if (checklock != null)
                        {
                            var functionTitle = db.tb_Function
                                                .Where(func => func.FunctionId == data.FunctionId)
                                                .Select(func => func.TitLe)
                                                .FirstOrDefault();

                            if (functionTitle != null)
                            {
                                if (data.Image != null)
                                {
                                    string base64String = Convert.ToBase64String(data.Image);
                                    Session["userimg"] = base64String;
                                }

                                string redirectUrl = ""; // Đường dẫn cần redirect

                                if (data.FunctionId== 3 || functionTitle.Contains("Nhân viên kho hàng"))
                                {
                                    Session["user"] = data;
                                    redirectUrl = Url.Action("StaffIndex", "WareHouseImport");
                                }
                                else if (data.FunctionId == 4|| functionTitle.Contains("Nhân viên bán hàng"))
                                {
                                    Session["user"] = data;
                                    redirectUrl = Url.Action("Index", "Seller");
                                }
                                else if (data.FunctionId == 1 || data.FunctionId == 2 || functionTitle.Contains("admin") || functionTitle.Contains("Quản Lý"))
                                {
                                    Session["user"] = data;
                                    redirectUrl = Url.Action("Index", "HomePage");
                                }
                                else
                                {
                                    ViewBag.error = "Chức năng không hợp lệ";
                                    return Json(new { success = false, code = -1 });
                                }

                                // Trả về JSON với thông tin redirectUrl
                                return Json(new { success = true, redirectUrl = redirectUrl });
                            }
                            else
                            {
                                ViewBag.error = "Không tìm thấy chức năng của tài khoản";
                                return Json(new { success = false, code = -2 });
                            }
                        }
                        else
                        {
                            ViewBag.error = "Tài khoản đã bị khóa";
                            return Json(new { success = false, code = -3 });
                        }
                    }
                    else
                    {
                        ViewBag.error = "Tài khoản hoặc mật khẩu không đúng";
                        return Json(new { success = false, code = -4 });
                    }
                }
                else
                {
                    return Json(new { success = false, code = -5 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = -100 });
            }
        }






        public ActionResult Staff(string code ,int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Staff.FirstOrDefault(x=>x.StaffId==id&&x.Code== code);
                if (item == null)
                {
                    return HttpNotFound();
                }
                ViewBag.NAme = item.NameStaff;
                return View(item);
            }

        }
        public ActionResult Partial_Edit(int id)
        {
            if (id > 0)
            {
                var KhachHang = db.tb_Staff.Find(id);
                if (KhachHang != null)
                {
                    var viewModel = new CLient_EditStaffViewModel
                    {
                        StaffId = KhachHang.StaffId,
                        Code = KhachHang.Code,
                        PhoneNumber = KhachHang.PhoneNumber,
                        NameStaff = KhachHang.NameStaff,
                        CitizenIdentificationCard = KhachHang.CitizenIdentificationCard,
                        Email = KhachHang.Email,
                        Password = KhachHang.Password,
                        Birthday =(DateTime) KhachHang.Birthday,
                        DayofWork = (DateTime) KhachHang.DayofWork,
                        Location = KhachHang.Location,
                        CreatedBy = KhachHang.CreatedBy,
                        Sex = KhachHang.Sex,
                        Clock = (bool)KhachHang.Clock,
                        FunctionId = (int)KhachHang.FunctionId,
                        StoreId = (int)KhachHang.StoreId,
                        Wage=KhachHang.Wage,
                        Image = KhachHang.Image,
                    };
                    return View(viewModel);
                }

            }
            return View();
        }

        public ActionResult EditImageStaff(int id)
        {
            if (id < 0)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                var item = db.tb_Staff.Find(id);
                if (item != null)
                {
                    return PartialView(item);
                }
                return PartialView();
            }
            }


        [HttpPost]
        public JsonResult EditImage(int customerId, HttpPostedFileBase newImage)
        {
            try
            {
                if (newImage != null && newImage.ContentLength > 0)
                {
                    var checkCustomer = db.tb_Staff.Find(customerId);
                    if (checkCustomer != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(newImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(newImage.ContentLength);
                        }

                        checkCustomer.Image = imageData;

                        db.Entry(checkCustomer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return Json(new { success = true, Code = 1, message = "Cập nhật ảnh thành công" });
                    }
                    else
                    {
                        return Json(new { success = false, Code = -2, message = "Không tìm thấy khách hàng để cập nhật" });
                    }
                }
                else
                {
                    return Json(new { success = false, Code = -3, message = "Không tìm thấy ảnh để cập nhật" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(CLient_EditStaffViewModel item, HttpPostedFileBase newImage)
        {
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var customer = db.tb_Staff.Find(item.StaffId);
                        if (customer != null)
                        {
                            customer.StaffId = item.StaffId;
                            customer.Code = item.Code;
                            customer.PhoneNumber = item.PhoneNumber.Trim();
                            customer.NameStaff = item.NameStaff.Trim();
                            customer.CitizenIdentificationCard = item.CitizenIdentificationCard.Trim();
                            customer.Password = item.Password.Trim();
                            customer.Email = item.Email.Trim();

                            customer.Birthday = item.Birthday;
                   customer.StoreId=item.StoreId;   


                            customer.Location = item.Location;
                            customer.DayofWork = item. DayofWork;
                            customer.CreatedBy = item.CreatedBy;
                            customer.Wage = item.Wage;
                            customer.Sex = item.Sex;
                            customer.Clock = (bool)item.Clock;
                            customer.FunctionId = item.FunctionId;
                            customer.Image = item.Image;
                            customer.ModifiedBy = item.NameStaff;
                            customer.ModifiedDate=DateTime.Now;
                            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            if (customer.Image != null)
                            {
                                string base64String = Convert.ToBase64String(customer.Image);
                                Session["img"] = base64String;
                            }





                            dbContext.Commit();
                            return Json(new { success = true, code = 1, customerId = customer.StaffId });
                        }
                        else
                        {
                            return Json(new { success = false, code = -1 });//Lỗi dữ liệu khách hàng
                        }
                    }
                    else
                    {
                        return Json(new { success = false, code = -99 });//Thông tin rỗng
                    }
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { success = false, code = -100 });

                }
            }

        }

        public ActionResult Partail_EditPassWordStaff(int id)
        {
            if (Session["user"] != null && id > 0) // Sửa điều kiện id > 0 để tránh lỗi
            {
                var customer = db.tb_Staff.Find(id);
                if (customer != null)
                {

                    var viewModel = new Staff_EditPassWord
                    {


                        Id = customer.StaffId,
                        Password = customer.PhoneNumber,

                    };
                    return PartialView(viewModel);


                }
                else
                {
                    return RedirectToAction("DangNhap", "Account");
                }
            }
            else
            {
                return RedirectToAction("DangNhap", "Account");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Partail_EditPassWordStaff(Staff_EditPassWord req)
        {
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {

                    string passOld = MaHoaPass(req.Password.Trim());
                    string passNew = MaHoaPass(req.PasswordNew.Trim());
                    var customer = db.tb_Staff.FirstOrDefault(x => x.StaffId == req.Id && x.Password == passOld.Trim());
                    if (customer != null)
                    {

                        if (passOld.Equals(passNew))
                        {
                            return Json(new { success = false, code = -2 });
                        }
                        else
                        {
                            customer.Password = MaHoaPass(req.PasswordNew.Trim());

                            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            dbContext.Commit();
                            Session.Clear();
                            return Json(new { success = true, code = 1 });
                       

                            
                        }
                    }
                    else
                    {
                        return Json(new { success = false, code = -1 });
                    }
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { success = false, code = -100 });

                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("DangNhap", "Account");
        }

        public static string MaHoaPass(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;

        }


    }
}