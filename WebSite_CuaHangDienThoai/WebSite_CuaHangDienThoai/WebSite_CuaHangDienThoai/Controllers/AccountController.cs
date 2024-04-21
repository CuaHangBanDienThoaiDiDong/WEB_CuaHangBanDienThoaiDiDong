using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Client;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class AccountController : Controller
    {
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_Login() 
        {
            return PartialView();
        }




        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string msnv, string sdt, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = MaHoaPass(password);
                var data = db.tb_KhachHang.Where(s => s.SDT.Equals(sdt) && s.Password.Equals(f_password)).ToList();
                if (data.Count > 0 /*&& dataNhanVien.Count == 0*/)
                {
                    var checkClock = db.tb_KhachHang.FirstOrDefault(s => s.SDT == sdt && s.Clock == false);
                    if (checkClock != null)
                    {
                        Session["Client"] = data;
                        Session["TenKhachHang"] = checkClock.TenKhachHang;
                        Session["IdKhachHang"] = checkClock.IdKhachHang;
                        Session["Email"] = checkClock.Email;
                        Session["SDT"] = checkClock.SDT;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.error = "Tài khoản bị đã khóa";
                    }

                }

                else
                {
                    ViewBag.error = "Không tồn tại tài khoản";
                    //return RedirectToAction("Register", "Account");
                }
            }

            return View();
        }

    
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Forgotpassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Forgot(string Email = "")
        {


            if (!string.IsNullOrEmpty(Email))
            {
                //var FindProduc = db.tb_KhachHang.Where(x => x.Email.ToUpper().Contains(Email.ToUpper()));
                var FindClient = db.tb_KhachHang.SingleOrDefault(x => x.Email == Email);
                if (FindClient != null)
                {
                    ViewBag.Find = Email;



                    Random ran = new Random();
                    FindClient.Code = "KP" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);

                    db.tb_KhachHang.Add(FindClient);

                    db.Entry(FindClient).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();




                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/SendForgotPass.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", FindClient.Code);

                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", FindClient.TenKhachHang);

                    WebSite_CuaHangDienThoai.Common.Common.SendMail("Cửa hàng điện thoại", "Mã khôi phục #" + FindClient.Code, contentCustomer.ToString(), FindClient.Email);


                    return RedirectToAction("UpdatePass", new { id = FindClient.IdKhachHang });

                }

            }
            return View();
        }



        public ActionResult Partial_Register()
        {
            return PartialView();   
        }



        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(tb_KhachHang _khachhang    )
        {
            if (ModelState.IsValid)
            {
                var checkmail = db.tb_KhachHang.FirstOrDefault(s => s.Email == _khachhang.Email);
                var checkPhone = db.tb_KhachHang.FirstOrDefault(s => s.SDT == _khachhang.SDT);
                var checkId = db.tb_KhachHang.FirstOrDefault(s => s.IdKhachHang == _khachhang.IdKhachHang);
                if (checkmail == null)
                {
                    if (checkPhone == null)
                    {
                        _khachhang.Password = MaHoaPass(_khachhang.Password);
                        _khachhang.SoLanMua = 1;
                        _khachhang.Clock=false;
                        //_khachhang.

                        db.Configuration.ValidateOnSaveEnabled = false;

                        db.tb_KhachHang.Add(_khachhang);
                       
                       

                        db.SaveChanges();

                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        checkPhone.Email = _khachhang.Email;
                        checkPhone.Password = MaHoaPass(_khachhang.Password);
                        checkPhone.Birthday = _khachhang.Birthday;
                        checkPhone.DiaChi = _khachhang.DiaChi;


                        db.tb_KhachHang.Add(checkPhone);

                        db.Entry(checkPhone).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    ViewBag.error = "Email đã tồn tại";
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }


        public ActionResult UpdatePass(int id)
        {
            var KhachHang = db.tb_KhachHang.Find(id);

            return View(KhachHang);
        }
        public ActionResult Partail_UpdatePass(int id)
        {
            ViewBag.id = id;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePass(Client_ForgetPassClient req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                var checkClient = db.tb_KhachHang.FirstOrDefault(row => row.IdKhachHang == req.Id && row.Code == req.Code);
                if (checkClient != null)
                {
                    checkClient.Password = MaHoaPass(req.Password);
                    db.tb_KhachHang.Add(checkClient);

                    db.Entry(checkClient).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    code = new { Success = true, Code = 1, Url = "" };//Cập nhập thành công

                }
                else
                {
                    code = new { Success = false, Code = -2, Url = "" };//Lỗi mã code
                }

            }
            return Json(code);
        }


        //MaHoaPassword khi Regsiter
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