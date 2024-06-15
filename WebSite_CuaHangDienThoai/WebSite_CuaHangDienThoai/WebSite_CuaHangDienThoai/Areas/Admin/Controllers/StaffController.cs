using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models.Token.Admin;
using WebSite_CuaHangDienThoai.Models;
using System.IO;
using System.Windows.Forms;
using PagedList;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        // GET: Admin/Staff
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Staff
        public ActionResult Index()
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
                    var items = db.tb_Staff.OrderByDescending(x => x.Code).ToList();
                    return View(items);
                }
            }

        }


        public ActionResult Partial_Index(int? page)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_Staff> items = db.tb_Staff.OrderByDescending(x => x.StaffId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_Staff.ToList();

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
        public ActionResult Partial_AddStaff()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("DangNhap", "Account");
            //}
            //else
            //{

            //tb_NhanVien nvSession = (tb_NhanVien)Session["user"];
            //var item = db.tb_PhanQuyen.SingleOrDefault(row => row.Code == nvSession.Code && (row.IdChucNang == 1 || row.IdChucNang == 2));
            //if (item == null)
            //{
            //    return RedirectToAction("NonRole", "HomePage");
            //}
            //else
            //{
            Random ran = new Random();
            ViewBag.Code = "2" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);


            ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "FunctionId", "TitLe");
            ViewBag.Store = new SelectList(db.tb_Store.ToList(), "StoreId", "Location");
            return PartialView();
            //}
            //}

        }
  

        public ActionResult Add()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);

                var functionTitle = db.tb_Function
                                            .Where(func => func.FunctionId == checkStaff.FunctionId)
                                            .Select(func => func.TitLe)
                                            .FirstOrDefault();
                if (functionTitle != null) 
                {
                    if (checkStaff.FunctionId == 2 || checkStaff.FunctionId==1 || functionTitle.Contains("Quản Trị Viên") || functionTitle.Contains("Quản Lý"))
                    {
                        Session["user"] = checkStaff;
                        return View();

                    }
                    else 
                    {
                        return RedirectToAction("NonRole", "HomePage");
                    }
                }
                return RedirectToAction("DangNhap", "Account");

            }




        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Staff model, tb_Role modelPhanQuyen, Admin_Add_Staff_ToKen req, HttpPostedFileBase newImage)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            try
            {
                if (ModelState.IsValid)
                {
                    if (req.SDT != null && req.Name != null && req.CCCD != null && req.Email != null && req.DiaChi != null && req.GioiTinh != null && req.Luong > 0)
                    {
                        var checkMail = db.tb_Staff.FirstOrDefault(row => row.Email == req.Email);
                        var checkPhone = db.tb_Staff.FirstOrDefault(row => row.PhoneNumber == req.SDT);
                        if (checkMail == null)
                        {
                            if (checkPhone == null)
                            {
                                if (newImage != null && newImage.ContentLength > 0) 
                                {
                                    tb_Staff nvSession = (tb_Staff)Session["user"];
                                    var item = db.tb_Staff.SingleOrDefault(x => x.Code == nvSession.Code);
                                    if (item != null)
                                    {
                                        if (req.FunctionId != null)
                                        {
                                            string pass = "123";
                                            byte[] imageData = null;
                                            using (var binaryReader = new BinaryReader(newImage.InputStream))
                                            {
                                                imageData = binaryReader.ReadBytes(newImage.ContentLength);
                                            }
                                            model.Image = imageData;


                                            Random ran = new Random();
                                            string Code = "2" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);
                                            model.Code = Code.Trim();
                                            model.PhoneNumber = req.SDT;
                                            model.NameStaff = req.Name.Trim();
                                            model.CitizenIdentificationCard = req.CCCD;
                                            model.Email = req.Email;
                                            model.Birthday = req.Birthday;
                                            model.Location = req.DiaChi;
                                            model.DayofWork = DateTime.Now;
                                            model.Sex = req.GioiTinh;
                                            model.Wage = req.Luong;
                                            model.CreatedBy = nvSession.NameStaff.Trim();
                                            model.CreatedDate = DateTime.Now;
                                            model.ModifiedDate = DateTime.Now;
                                            model.Clock = false;
                                            model.FunctionId = req.FunctionId;
                                            model.StoreId =5;
                                            model.Password = MaHoaPass(pass);

                                            db.tb_Staff.Add(model);
                                            db.SaveChanges();

                                            int NhaVienIdNew = model.StaffId;
                                            modelPhanQuyen.StaffId = NhaVienIdNew;
                                            modelPhanQuyen.FunctionId = (int)req.FunctionId;

                                            db.tb_Role.Add(modelPhanQuyen);
                                            db.SaveChanges();

                                            //Đăng ký nhân viên thành công
                                            code = new { Success = true, Code = 1, Url = "" };
                                        }
                                        else
                                        {
                                            //"Không thấy chức năng cho nhân viên mới";
                                            code = new { Success = false, Code = -4, Url = "" };
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.error = "Không thấy Session nhân viên hiện tại";
                                    }
                                }
                                else
                                {
                                    code = new { Success = false, Code = -6, Url = "" };
                                   
                                }
                               
                            }
                            else
                            {
                                // "Số điện thoại đã tồn tại";
                                code = new { Success = false, Code = -3, Url = "" };
                            }
                        }
                        else
                        {
                            //"Email đã tồn tại";
                            code = new { Success = false, Code = -2, Url = "" };
                        }
                    }
                    else
                    {
                        //"Vui lòng nhậ đầy đủ thông tin nhân viên ";
                        code = new { Success = false, Code = -5, Url = "" };
                    }
                }
                else 
                {
                    code = new { Success = false, Code = -7, Url = "" };
                    return Json(code);
                }
             
            }
            catch (Exception ex)
            {
                code = new { Success = false, Code = -100, Url = "" };
            }

            ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "IdChucNang", "TenChucNang");
            ViewBag.Store = new SelectList(db.tb_Store.ToList(), "StoreId", "Location");
            return Json(code);
        }




        public ActionResult Detail(int id) 
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);

                var functionTitle = db.tb_Function
                                            .Where(func => func.FunctionId == checkStaff.FunctionId)
                                            .Select(func => func.TitLe)
                                            .FirstOrDefault();
                if (functionTitle != null)
                {
                    if (checkStaff.FunctionId == 2 || checkStaff.FunctionId == 1 || functionTitle.Contains("Quản Trị Viên") || functionTitle.Contains("Quản Lý"))
                    {
                        if (id > 0)
                        {
                            var item = db.tb_Staff.Find(id);
                            ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "FunctionId", "TitLe");
                            ViewBag.Store = new SelectList(db.tb_Store.ToList(), "StoreId", "Location");
                            return View(item);
                        }

                    }
                    else
                    {
                        return RedirectToAction("NonRole", "HomePage");
                    }
                }
                return RedirectToAction("DangNhap", "Account");

            }




         
         
        }

        public ActionResult Edit(int id)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var checkStaff = db.tb_Staff.SingleOrDefault(row => row.Code == nvSession.Code);

                var functionTitle = db.tb_Function
                                            .Where(func => func.FunctionId == checkStaff.FunctionId)
                                            .Select(func => func.TitLe)
                                            .FirstOrDefault();
                if (functionTitle != null)
                {
                    if (checkStaff.FunctionId == 2 || checkStaff.FunctionId == 1 || functionTitle.Contains("Quản Trị Viên") || functionTitle.Contains("Quản Lý"))
                    {
                        if (id > 0)
                        {
                            if (id > 0)
                            {
                                var item = db.tb_Staff.Find(id);
                                var viewModel = new EditStaffViewModel
                                {
                                    Id = item.StaffId,
                                    Code = item.Code,
                                    PhoneNumber = item.PhoneNumber,
                                    NameStaff = item.NameStaff,
                                    CitizenIdentificationCard = item.CitizenIdentificationCard,
                                    Email = item.Email,
                                    Password = item.Password,
                                    Birthday = item.Birthday,
                                    Location = item.Location,
                                    DayofWork = (DateTime)item.DayofWork,
                                    Wage = item.Wage,
                                    Sex = item.Sex,
                                    CreatedBy = item.CreatedBy,
                                    CreatedDate = item.CreatedDate,
                                    FunctionId = (int)item.FunctionId,
                                    StoreId = (int)item.StoreId,
                                    Clock = (bool)item.Clock,
                                    Image = item.Image,

                              
                                };
                                ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "FunctionId", "TitLe");
                                ViewBag.Store = new SelectList(db.tb_Store.ToList(), "StoreId", "Location");
                                return View(viewModel);
                            }
                            return View();
                        }

                    }
                    else
                    {
                        return RedirectToAction("NonRole", "HomePage");
                    }
                }
                return RedirectToAction("DangNhap", "Account");

            }

          
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditStaffViewModel item, HttpPostedFileBase newImage)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var staff = db.tb_Staff.Find(item.Id);
                        if (staff != null)
                        {
                            tb_Staff nvSession = (tb_Staff)Session["user"];

                            staff.StaffId = item.Id;
                            staff.Password = item.Password;
                            staff.DayofWork = item.DayofWork;
                            staff.Code = item.Code;
                            staff.PhoneNumber = item.PhoneNumber;
                            staff.NameStaff = item.NameStaff;
                            staff.CitizenIdentificationCard = item.CitizenIdentificationCard;
                            if (item.Birthday != null)
                            {
                                staff.Birthday = (DateTime)item.Birthday;
                            }
                            else
                            {
                                staff.Birthday = DateTime.Now; // Gán mặc định nếu không có giá trị
                            }
                            staff.Location = item.Location;
                            staff.DayofWork = item.DayofWork;
                            staff.Wage = item.Wage;
                            staff.Sex = item.Sex;
                            if (item.CreatedBy == null) 
                            {
                                staff.CreatedBy = nvSession.NameStaff;

                            }
                            else
                            {
                                staff.CreatedBy = item.CreatedBy;
                            }
                          
                            staff.CreatedDate=item.CreatedDate;
                            staff.Clock = item.Clock;
                      

                            
                          
                       
                            staff.Email = item.Email;
                           
                           
                          
                           
                            staff.ModifiedDate = DateTime.Now;
                            staff.ModifiedBy = nvSession.NameStaff;
                            staff.FunctionId = (int)item.FunctionId;
                            staff.StoreId = (int)item.StoreId;

                            // Kiểm tra và gán giá trị cho Birthday
                            

                            // Kiểm tra và gán giá trị cho Image
                            if (newImage != null)
                            {
                                byte[] imageData = null;
                                using (var binaryReader = new BinaryReader(newImage.InputStream))
                                {
                                    imageData = binaryReader.ReadBytes(newImage.ContentLength);
                                }
                                staff.Image = imageData;
                            }
                            else
                            {
                                staff.Image = item.Image;
                            }

                            db.Entry(staff).State = EntityState.Modified;
                            db.SaveChanges();

                            var role = db.tb_Role.FirstOrDefault(x => x.StaffId == item.Id);
                            if (role != null)
                            {
                               
                                db.tb_Role.Remove(role);
                                db.SaveChanges();
                            }

                            
                            db.tb_Role.Add(new tb_Role
                            {
                                StaffId = item.Id,
                                FunctionId = (int)item.FunctionId,
                                Note = "Updated role" 
                            });
                            db.SaveChanges();

                            dbContextTransaction.Commit();
                            return Json(new { success = true, code = 1 });
                        }
                        else
                        {
                            return Json(new { success = false, code = -2, message = "Staff not found" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, code = -3, message = "Invalid model state" });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    dbContextTransaction.Rollback();
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    return Json(new { success = false, code = -100, error = exceptionMessage });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100, error = ex.Message });
                }
            }
        }


        public ActionResult IsLock(int? id)
        {

            var item = db.tb_Staff.SingleOrDefault(x => x.StaffId == id);
            if (item != null)
            {
                item.Clock = !item.Clock;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isAcive = item.Clock });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult KhoiPhucMatKhau(int? id) 
        {
            var item = db.tb_Staff.SingleOrDefault(x => x.StaffId == id);
            if (item != null)
            {
                string pass = "123";
                item.Password = MaHoaPass(pass);
               
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, code=1});
            }

            return Json(new { success = false });
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