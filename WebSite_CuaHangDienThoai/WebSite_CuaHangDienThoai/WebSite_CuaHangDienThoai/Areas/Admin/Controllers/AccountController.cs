﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

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

            if (ModelState.IsValid)
            {
                var f_password = MaHoaPass(password);

                //var data = db.tb_NhanVien.Count(s => s.Code.Equals(Code) && s.Password.Equals(f_password));
                var data = db.tb_Staff.SingleOrDefault(s => s.Code.Equals(Code) && s.Password.Equals(f_password));

                if (data != null)
                {
                    var checklock = db.tb_Staff.FirstOrDefault(s => s.Code.Equals(Code) && s.Clock == false);
                    if (checklock != null)
                    {
                        var checkRole = db.tb_Staff.SingleOrDefault(s => s.Code == Code);
                        if (checkRole != null)
                        {
                            if (checkRole.FunctionId == 1 || checkRole.FunctionId == 2)
                            {
                                Session["user"] = data;
                                return RedirectToAction("Index", "HomePage");
                            }
                            else if (checkRole.FunctionId == 3)
                            {
                                Session["user"] = data;
                                return RedirectToAction("Index", "Warehouse");
                            }
                            else if (checkRole.FunctionId == 4)
                            {
                                Session["user"] = data;
                                return RedirectToAction("Index", "Seller");
                            }
                        }
                        else
                        {
                            ViewBag.error = "Không tồn tại tài khoản";
                        }
                    }
                    else
                    {
                        ViewBag.error = "Tài khoản đã khóa";
                    }

                }

                else
                {
                    ViewBag.error = "Không tồn tại tài khoản";
                }
            }

            return View();
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