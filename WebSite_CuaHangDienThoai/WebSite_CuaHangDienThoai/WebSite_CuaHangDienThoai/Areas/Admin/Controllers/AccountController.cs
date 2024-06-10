using System;
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
                            if (functionTitle.Contains("Quản lý kho hàng"))
                            {
                                Session["user"] = data;
                                return RedirectToAction("Index", "Warehouse");
                            }
                            else if (functionTitle.Contains("Nhân viên bán hàng"))
                            {
                                Session["user"] = data;
                                return RedirectToAction("Index", "Seller");
                            }
                            else if (functionTitle.Contains("admin") || functionTitle.Contains("Quản Lý"))
                            {
                                Session["user"] = data;
                                return RedirectToAction("Index", "HomePage");
                            }
                            else
                            {
                                ViewBag.error = "Chức năng không hợp lệ";
                            }
                        }
                        else
                        {
                            ViewBag.error = "Không tìm thấy chức năng của tài khoản";
                        }
                    }
                    else
                    {
                        ViewBag.error = "Tài khoản đã bị khóa";
                    }
                }
                else
                {
                    ViewBag.error = "Tài khoản hoặc mật khẩu không đúng";
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