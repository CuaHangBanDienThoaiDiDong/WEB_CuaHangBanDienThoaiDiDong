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


        public ActionResult ProFile(int id) 
        {
         
            var item = db.tb_Customer.Find(id); 
            return View(item);
        }    

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string msnv, string PhoneNumber, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = MaHoaPass(password);
                var data = db.tb_Customer.Where(s => s.PhoneNumber.Equals(PhoneNumber) && s.Password.Equals(f_password)).ToList();
                if (data.Count > 0 /*&& dataNhanVien.Count == 0*/)
                {
                    var checkClock = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber == PhoneNumber && s.Clock == false);
                    if (checkClock != null)
                    {
                        Session["Client"] = data;
                        Session["CustomerName"] = checkClock.CustomerName;
                        Session["CustomerId"] = checkClock.CustomerId;
                        Session["Email"] = checkClock.Email;
                        Session["PhoneNumber"] = checkClock.PhoneNumber;
                        Session["img"] = checkClock.Image;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.error = "Tài khoản bị đã khóa";
                    }

                }

                else
                {
                    ViewBag.nonAccount = "Không tồn tại tài khoản";
                    //return RedirectToAction("Register", "Account");
                }
            }

            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(string msnv, string PhoneNumber, string password)
        //{
        //    var code = new { Success = false, Code = -1, Url = "" };
        //    if (ModelState.IsValid)
        //    {
        //        var f_password = MaHoaPass(password);
        //        var data = db.tb_Customer.Where(s => s.PhoneNumber.Equals(PhoneNumber) && s.Password.Equals(f_password)).ToList();
        //        if (data.Count > 0 /*&& dataNhanVien.Count == 0*/)
        //        {
        //            var checkClock = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber == PhoneNumber && s.Clock == false);
        //            if (checkClock != null)
        //            {
        //                Session["Client"] = data;
        //                Session["CustomerName"] = checkClock.CustomerName;
        //                Session["CustomerId"] = checkClock.CustomerId;
        //                Session["Email"] = checkClock.Email;
        //                Session["PhoneNumber"] = checkClock.PhoneNumber;
        //                Session["img"] = checkClock.Image;
        //                code = new { Success = true, Code = 1, Url = "" };
        //            }
        //            else
        //            {
        //                ViewBag.error = "Tài khoản bị đã khóa";
        //                code = new { Success = false, Code = -3, Url = "" };
        //            }

        //        }

        //        else
        //        {
        //            //ViewBag.error = "Không tồn tại tài khoản";
        //            code = new { Success = false, Code = -4, Url = "" };// Không tồn tại tài khoản
        //        }
        //    }

        //    return Json(code);
        //}


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
                //var FindProduc = db.tb_Customer.Where(x => x.Email.ToUpper().Contains(Email.ToUpper()));
                var FindClient = db.tb_Customer.SingleOrDefault(x => x.Email == Email);
                if (FindClient != null)
                {
                    ViewBag.Find = Email;



                    Random ran = new Random();
                    FindClient.Code = "KP" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);

                    db.tb_Customer.Add(FindClient);

                    db.Entry(FindClient).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();




                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/SendForgotPass.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", FindClient.Code);

                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{CustomerName}}", FindClient.CustomerName);

                    WebSite_CuaHangDienThoai.Common.Common.SendMail("Cửa hàng điện thoại", "Mã khôi phục #" + FindClient.Code, contentCustomer.ToString(), FindClient.Email);


                    return RedirectToAction("UpdatePass", new { id = FindClient.CustomerId });

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
        public ActionResult Register(tb_Customer _khachhang)
        {
            if (ModelState.IsValid)
            {
                var checkmail = db.tb_Customer.FirstOrDefault(s => s.Email == _khachhang.Email);
                var checkPhone = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber == _khachhang.PhoneNumber);
                var checkId = db.tb_Customer.FirstOrDefault(s => s.CustomerId == _khachhang.CustomerId);
                if (checkmail == null)
                {
                    if (checkPhone == null)
                    {
                        _khachhang.Password = MaHoaPass(_khachhang.Password);
                        _khachhang.NumberofPurchases = 1;
                        _khachhang.Clock = false;

                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.tb_Customer.Add(_khachhang);
                        db.SaveChanges();

                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        checkPhone.Email = _khachhang.Email;
                        checkPhone.Password = MaHoaPass(_khachhang.Password);
                        checkPhone.Birthday = _khachhang.Birthday;
                        checkPhone.Loaction = _khachhang.Loaction;

                        db.tb_Customer.Add(checkPhone);
                        db.Entry(checkPhone).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    // Trả về thông báo lỗi khi email đã tồn tại
                    ModelState.AddModelError("", "Email đã tồn tại");
                    return View(_khachhang);
                }
            }

            // Trả về view với dữ liệu đang nhập và thông báo lỗi nếu dữ liệu không hợp lệ
            return View(_khachhang);
        }


        public ActionResult UpdatePass(int id)
        {
            var KhachHang = db.tb_Customer.Find(id);

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
                var checkClient = db.tb_Customer.FirstOrDefault(row => row.CustomerId == req.Id && row.Code == req.Code);
                if (checkClient != null)
                {
                    checkClient.Password = MaHoaPass(req.Password);
                    db.tb_Customer.Add(checkClient);

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