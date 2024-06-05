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

                return View();
            }
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(tb_Staff model, tb_Role modelPhanQuyen, Admin_Add_Staff_ToKen req, HttpPostedFileBase newImage)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (req.SDT != null && req.Name != null && req.CCCD != null && req.Email != null && req.DiaChi != null && req.GioiTinh != null && req.Luong > 0)

            {
                var checkMail = db.tb_Staff.FirstOrDefault(row => row.Email == req.Email);
                var checkPhone = db.tb_Staff.FirstOrDefault(row => row.PhoneNumber == req.SDT);
                if (checkMail == null)
                {
                    if (checkPhone == null)
                    {
                        tb_Staff nvSession = (tb_Staff)Session["user"];
                        var item = db.tb_Staff.SingleOrDefault(x => x.Code == nvSession.Code);
                        if (item != null)
                        {
                            if (req.FunctionId != null)
                        {
                            string pass = "123";
                                if (newImage != null && newImage.ContentLength > 0)
                                {
                                    try 
                                    {

                                        byte[] imageData = null;
                                        using (var binaryReader = new BinaryReader(newImage.InputStream))
                                        {
                                            imageData = binaryReader.ReadBytes(newImage.ContentLength);
                                        }

                                        model.Image = imageData;
                                    }
                                    catch (Exception ex) 
                                    {
                                        code = new { Success = false, Code = -5, Url = "" };
                                        return Json(code);
                                    }

                                    
                                }

                                Random ran = new Random();

                            string Code = "2" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);
                            model.Code = Code.Trim();
                            model.PhoneNumber = req.SDT;
                            model.NameStaff = req.Name.Trim();
                            model.Password = MaHoaPass(pass);

                            model.CitizenIdentificationCard = req.CCCD;
                            model.Email = req.Email;
                            model.Birthday = req.Birthday;
                            model.Location = req.DiaChi;
                            model.Wage = req.Luong;
                            model.FunctionId = req.FunctionId;
model.StoreId = req.StoreId;        
                            model.Clock = false;

                            model.CreatedDate = DateTime.Now;
                            model.DayofWork = DateTime.Now;
                            model.ModifiedDate = DateTime.Now;
                            model.Sex = req.GioiTinh;

                            //if (req.GioiTinh == "Nam")
                            //{
                            //    model.Sex = "Nam";
                            //}
                            //else
                            //{
                            //    model.Sex = "Nữ";
                            //}

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
                        { ViewBag.error = "Không thấy Session nhân viên hiện tại"; }
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

          

            ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "IdChucNang", "TenChucNang");
            ViewBag.Store = new SelectList(db.tb_Store.ToList(), "StoreId", "Location");
            return Json(code);
        }




        [HttpPost]
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



        //[HttpPost]
        //public ActionResult IsLock(int id)
        //{

        //    var item = db.tb_Staff.SingleOrDefault(x => x.StaffId==id);
        //    if (item != null)
        //    {
        //        item.Clock = !item.Clock;
        //        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return Json(new { success = true, isAcive = item.Clock });
        //    }

        //    return Json(new { success = false });
        //}


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