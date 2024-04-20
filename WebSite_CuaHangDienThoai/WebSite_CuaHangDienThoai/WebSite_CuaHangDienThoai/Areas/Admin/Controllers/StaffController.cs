using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models.Token.Admin;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        // GET: Admin/Staff
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Staff
        public ActionResult Index()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("DangNhap", "Account");
            //}
            //else
            //{

                //tb_NhanVien nvSession = (tb_NhanVien)Session["user"];
                //var item = db.tb_PhanQuyen.SingleOrDefault(row => row.MSNV == nvSession.MSNV && (row.IdChucNang == 1 || row.IdChucNang == 2));
                //if (item == null)
                //{
                //    return RedirectToAction("NonRole", "HomePage");
                //}
                //else
                //{
                    var items = db.tb_Staff.OrderByDescending(x => x.MSNV).ToList();
                    return View(items);
                //}
            //}

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
            //var item = db.tb_PhanQuyen.SingleOrDefault(row => row.MSNV == nvSession.MSNV && (row.IdChucNang == 1 || row.IdChucNang == 2));
            //if (item == null)
            //{
            //    return RedirectToAction("NonRole", "HomePage");
            //}
            //else
            //{
            Random ran = new Random();
            ViewBag.MSNV = "2" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);


            ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "IdChucNang", "TenChucNang");
            return PartialView();
            //}
            //}

        }
        public ActionResult AddTEst()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("DangNhap", "Account");
            //}
            //else
            //{

            return View();
            //}
        }

        public ActionResult Add()
        {
            //if (Session["user"] == null)
            //{
            //    return RedirectToAction("DangNhap", "Account");
            //}
            //else
            //{

            return View();
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Admin_Add_Staff_ToKen req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            var checkMail = db.tb_Staff.FirstOrDefault(row => row.Email == req.Email);
            var checkPhone = db.tb_Staff.FirstOrDefault(row => row.SDT == req.SDT);

            if (checkMail == null)
            {
                if (checkPhone == null)
                {
                    string pass = "123";
                    Random ran = new Random();
                    string msnv = "2" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);

                    tb_Staff newStaff = new tb_Staff()
                    {
                        MSNV = msnv.Trim(),
                        TenNhanVien = req.Name.Trim(),
                        SDT = req.SDT,
                        CCCD = req.CCCD,
                        Email = req.Email,
                        Birthday = req.Birthday,
                        DiaChi = req.DiaChi,
                        Luong = req.Luong,
                        CreatedDate = DateTime.Now,
                        NgayVaoLam = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        Password = MaHoaPass(pass),
                        Clock = false,
                        IdChucNang = req.FunctionId
                    };

                    db.tb_Staff.Add(newStaff);
                    db.SaveChanges();

                    tb_Role newRole = new tb_Role()
                    {
                        NhanVienId = newStaff.NhanVienId,
                        IdChucNang = req.FunctionId
                    };
                    db.tb_Role.Add(newRole);
                    db.SaveChanges();

                    code = new { Success = true, Code = 1, Url = "" };
                }
                else
                {
                    code = new { Success = false, Code = -3, Url = "" }; // Số điện thoại đã tồn tại
                }
            }
            else
            {
                code = new { Success = false, Code = -2, Url = "" }; // Email đã tồn tại
            }

            return Json(code);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Add(tb_Staff model, tb_Role modelPhanQuyen, Admin_Add_Staff_ToKen req)
        //{
        //    var code = new { Success = false, Code = -1, Url = "" };
        //    var checkMail = db.tb_Staff.FirstOrDefault(row => row.Email == model.Email);
        //    var checkPhone = db.tb_Staff.FirstOrDefault(row => row.SDT == model.SDT);
        //    if (checkMail == null)
        //    {
        //        if (checkPhone == null)
        //        {
        //            //tb_NhanVien nvSession = (tb_NhanVien)Session["user"];
        //            //var item = db.tb_NhanVien.SingleOrDefault(x => x.MSNV == nvSession.MSNV);
        //            //if (item != null)
        //            //{
        //            if (req.FunctionId != null)
        //            {
        //                string pass = "123";


        //                Random ran = new Random();

        //                string msnv = "2" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);
        //                model.MSNV = msnv.Trim();
        //                model.SDT = req.SDT;
        //                model.TenNhanVien = req.Name.Trim();
        //                model.Password = MaHoaPass(pass);

        //                model.CCCD = req.CCCD;
        //                model.Email = req.Email;
        //                model.Birthday = req.Birthday;
        //                model.DiaChi = req.DiaChi;
        //                model.Luong = req.Luong;
        //                model.IdChucNang = req.FunctionId;

        //                model.Clock = false;

        //                model.CreatedDate = DateTime.Now;
        //                model.NgayVaoLam = DateTime.Now;
        //                model.ModifiedDate = DateTime.Now;



        //                db.tb_Staff.Add(model);
        //                db.SaveChanges();


        //                int NhaVienIdNew = model.NhanVienId;
        //                modelPhanQuyen.NhanVienId = NhaVienIdNew;
        //                modelPhanQuyen.IdChucNang = (int)req.FunctionId;

        //                db.tb_Role.Add(modelPhanQuyen);
        //                db.SaveChanges();




        //                code = new { Success = true, Code = 1, Url = "" };


        //                //using (var transaction = db.Database.BeginTransaction())
        //                //{
        //                //    try
        //                //    {
        //                //        db.tb_Staff.Add(model);
        //                //        db.SaveChanges();

        //                //        // Lấy ngay NhanVienId sau khi thêm nhân viên vào cơ sở dữ liệu
        //                //        int NhaVienIdNew = model.NhanVienId;

        //                //        modelPhanQuyen.NhanVienId = NhaVienIdNew;
        //                //        modelPhanQuyen.IdChucNang = (int)req.FunctionId;

        //                //        db.tb_Role.Add(modelPhanQuyen);
        //                //        db.SaveChanges();

        //                //        transaction.Commit();

        //                //        code = new { Success = true, Code = 1, Url = "" };
        //                //    }
        //                //    catch (Exception ex)
        //                //    {
        //                //        transaction.Rollback();
        //                //        code = new { Success = false, Code = -1, Url = "Lỗi:"+ ex.Message }; // Trả về lỗi nếu có
        //                //    }
        //                //}



        //            }
        //            else
        //            {
        //                //"Không thấy chức năng cho nhân viên mới";
        //                code = new { Success = false, Code = -4, Url = "" };
        //            }
        //            //}
        //            //else
        //            //{ ViewBag.error = "Không thấy Session nhân viên hiện tại"; }
        //        }
        //        else
        //        {
        //            // "Số điện thoại đã tồn tại";
        //            code = new { Success = false, Code = -3, Url = "" };
        //        }

        //    }
        //    else
        //    {
        //        //"Email đã tồn tại";
        //        code = new { Success = false, Code = -2, Url = "" };
        //    }

        //    ViewBag.ChucNang = new SelectList(db.tb_Function.ToList(), "IdChucNang", "TenChucNang");
        //    return Json(code);
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