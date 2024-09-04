using Antlr.Runtime.Misc;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using WebSite_CuaHangDienThoai.Models.Token.Client;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Web.Configuration;
using Microsoft.Extensions.Configuration;


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
            if (Session["CustomerId"] != null)
            {
                var item = db.tb_Customer.Find(id);
                if (item == null)
                {
                    return HttpNotFound();
                }
                ViewBag.NAme = item.CustomerName;
                return View(item);
            }
            else
            {
                return RedirectToAction("Login"); // Redirect tới trang đăng nhập nếu khách hàng chưa đăng nhập
            }
        }





        //public ActionResult Login()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(string msnv, string PhoneNumber, string password)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var f_password = MaHoaPass(password);
        //            var data = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber.Equals(PhoneNumber) && s.Password.Equals(f_password));
        //            if (data != null/*&& dataNhanVien.Count == 0*/)
        //            {
        //                var checkClock = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber == PhoneNumber && s.Clock == false);
        //                if (checkClock != null)
        //                {
        //                    Session["Client"] = data;
        //                    Session["CustomerName"] = checkClock.CustomerName;
        //                    Session["CustomerId"] = checkClock.CustomerId;
        //                    Session["Email"] = checkClock.Email;
        //                    Session["PhoneNumber"] = checkClock.PhoneNumber;
        //                    if (checkClock.Image != null)
        //                    {
        //                        string base64String = Convert.ToBase64String(checkClock.Image);
        //                        Session["img"] = base64String;
        //                    }

        //                    return RedirectToAction("Index", "Home");
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Tài khoản đã bị khóa ");
        //                }

        //            }

        //            else
        //            {
        //                ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không đúng");

        //                return View();
        //                //return RedirectToAction("Register", "Account");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("NotFound", "Error");
        //    }
        //    return View();
        //}



        //App_Start test KEy Bao mât

        public ActionResult Login()
        {
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(string PhoneNumber, string password)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var encryptedPassword = MaHoaPass(password.Trim());
        //            var customer = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber.Equals(PhoneNumber) && s.Password.Equals(encryptedPassword));

        //            if (customer != null)
        //            {
        //                if (!(bool)customer.Clock)
        //                {
        //                    // Tạo token
        //                    var tokenHandler = new JwtSecurityTokenHandler();
        //                    var key = Encoding.ASCII.GetBytes(WebConfigurationManager.AppSettings["JwtSecretKey"]);

        //                    // Tạo key với KeyId
        //                    var signingKey = new CustomSymmetricSecurityKey(key)
        //                    {
        //                        KeyId = "my_unique_key_id_123"
        //                    };

        //                    var tokenDescriptor = new SecurityTokenDescriptor
        //                    {
        //                        Subject = new ClaimsIdentity(new Claim[]
        //                        {
        //                    new Claim(ClaimTypes.Name, customer.CustomerName),
        //                    new Claim(ClaimTypes.Email, customer.Email), 
        //                            // Thêm các thông tin khác của người dùng vào đây
        //                        }),
        //                        Expires = DateTime.UtcNow.AddDays(7), // Thời hạn token là 7 ngày
        //                        SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
        //                    };

        //                    var token = tokenHandler.CreateToken(tokenDescriptor);
        //                    var tokenString = tokenHandler.WriteToken(token);

        //                    // Kiểm tra trình duyệt và đặt cookie tương ứng
        //                    if (Request.UserAgent != null)
        //                    {
        //                        if (Request.UserAgent.Contains("Coc Coc"))
        //                        {
        //                            SetCookie(tokenString, "/coc");
        //                        }
        //                        else if (Request.UserAgent.Contains("Chrome"))
        //                        {
        //                            SetCookie(tokenString, "/chrome");
        //                        }
        //                        else // Mặc định cho trường hợp không phát hiện trình duyệt
        //                        {
        //                            SetCookie(tokenString, "/");
        //                        }
        //                    }

        //                    // Lưu thông tin người dùng vào Session
        //                    Session["Client"] = customer.CustomerId;
        //                    Session["CustomerId"] = customer.CustomerId;
        //                    Session["CustomerName"] = customer.CustomerName;
        //                    Session["Email"] = customer.Email;
        //                    if (customer.Image != null)
        //                    {
        //                        string base64String = Convert.ToBase64String(customer.Image);
        //                        Session["img"] = base64String;
        //                    }
        //                    // Thêm thông tin khác của người dùng vào Session nếu cần

        //                    return RedirectToAction("Index", "Home");
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Tài khoản đã bị khóa");
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không đúng");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý ngoại lệ tại đây
        //        return RedirectToAction("NotFound", "Error");
        //    }

        //    // Trường hợp ModelState không hợp lệ hoặc không tìm thấy tài khoản
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string PhoneNumber, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var encryptedPassword = MaHoaPass(password.Trim());
                    var customer = db.tb_Customer.FirstOrDefault(s => s.PhoneNumber.Equals(PhoneNumber) && s.Password.Equals(encryptedPassword));

                    if (customer != null)
                    {
                        if (!(bool)customer.Clock)
                        {
                            // Tạo token
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var key = Encoding.ASCII.GetBytes(WebConfigurationManager.AppSettings["JwtSecretKey"]);

                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(new Claim[]
                                {
                                    new Claim(ClaimTypes.Name, customer.CustomerName),
                                    new Claim(ClaimTypes.Email, customer.Email), 
                                    // Thêm các thông tin khác của người dùng vào đây
                                }),
                                Expires = DateTime.UtcNow.AddDays(7), // Thời hạn token là 7 ngày
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                            };

                            var token = tokenHandler.CreateToken(tokenDescriptor);
                            var tokenString = tokenHandler.WriteToken(token);

                            // Gửi token về client dưới dạng cookie
                            Response.Cookies["access_token"].Value = tokenString;
                            Response.Cookies["access_token"].Expires = DateTime.Now.AddDays(7); // Thời gian sống của cookie

                            // Lưu thông tin người dùng vào Session
                            Session["Client"] = customer.CustomerId;
                            Session["CustomerId"] = customer.CustomerId;
                            Session["CustomerName"] = customer.CustomerName;
                            Session["Email"] = customer.Email;
                            if (customer.Image != null)
                            {
                                string base64String = Convert.ToBase64String(customer.Image);
                                Session["img"] = base64String;
                            }
                            // Thêm thông tin khác của người dùng vào Session nếu cần

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Tài khoản đã bị khóa");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số điện thoại hoặc mật khẩu không đúng");
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ tại đây
                return RedirectToAction("NotFound", "Error");
            }

            // Trường hợp ModelState không hợp lệ hoặc không tìm thấy tài khoản
            return View();
        }

        private void SetCookie(string tokenString, string path)
        {
            var cookie = new HttpCookie("access_token", tokenString)
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = path
            };
            Response.SetCookie(cookie);
        }

        private void SetUserInfoInSession(tb_Customer customer)
        {
            Session["Client"] = customer.CustomerId;
            Session["CustomerId"] = customer.CustomerId;
            Session["CustomerName"] = customer.CustomerName;
            Session["Email"] = customer.Email;
            if (customer.Image != null)
            {
                string base64String = Convert.ToBase64String(customer.Image);
                Session["img"] = base64String;
            }
            // Thêm thông tin khác của người dùng vào Session nếu cần
        }


        private byte[] GenerateRandomKey()
        {
            // Độ dài chuỗi khóa, đảm bảo đủ dài cho thuật toán HMAC-SHA256
            int keyLength = 32; // 32 byte = 256 bit

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] key = new byte[keyLength];
                rng.GetBytes(key);
                return key;
            }
        }

        // Đảm bảo action chỉ có người dùng đã đăng nhập mới có thể truy cập
        [AuthorizeSession]
        public ActionResult SecurePage()
        {
            return View();
        }







        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult CheckToken()
        {
            var cookie = Request.Cookies["access_token"];
            if (cookie != null)
            {
                var tokenString = cookie.Value;
                // In ra Console hoặc sử dụng tokenString
                System.Diagnostics.Debug.WriteLine("Token from cookie: " + tokenString);
                return Content("Token from cookie: " + tokenString);
            }
            return Content("No token found");
        }





        private ClaimsPrincipal DecodeJWT(string tokenString)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);
            var claims = token.Claims;
            var identity = new ClaimsIdentity(claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }

        private string GenerateToken(List<Claim> claims)
        {
            // Đọc key từ cấu hình
            var secretKey = WebConfigurationManager.AppSettings["JwtSecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            // Tạo các SigningCredentials từ secretKey
            var signingKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Tạo JWT token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // Thời hạn của token, ví dụ 7 ngày
                signingCredentials: signingCredentials
            );

            // Tạo mã chuỗi từ token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        //End test KEy Bao mât




        public ActionResult LogOut()
        {
            // Xóa thông tin trong session
            //Session.Clear();
          
            if (Session["user"] != null || Session["Client"] != null || Session["CustomerId"] != null || Session["CustomerName"] != null || Session["Email"] != null || Session["img"] != null)
            {
                Session["user"] = null;
                Session["Client"] = null;
                Session["CustomerId"] = null;
                Session["CustomerName"] = null;
                Session["Email"] = null;
                Session["img"] = null;
            }

            // Xóa cookie chứa token
            if (Request.Cookies["access_token"] != null)
            {
                HttpCookie myCookie = new HttpCookie("access_token");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }

            // Chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }


        // Hàm xác thực token JWT
        private bool AuthenticateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = WebConfigurationManager.AppSettings["JwtSecretKey"]; // Đọc key từ cấu hình
            var key = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public ActionResult Forgotpassword()
        {
            return View();
        }


        public ActionResult NonAccount() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot(string Email = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(Email))
                {
                    var FindClient = db.tb_Customer.FirstOrDefault(x => x.Email == Email);
                    if (FindClient != null)
                    {
                        // Tạo token với claim CustomerId
                        var claims = new List<Claim>
                {
                    new Claim("CustomerId", FindClient.CustomerId.ToString())
                };
                        var tokenString = GenerateToken(claims);

                        // Tạo mã khôi phục ngẫu nhiên
                        Random ran = new Random();
                        FindClient.Code = "KP" + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9) + ran.Next(0, 9);

                        // Lưu mã khôi phục vào cơ sở dữ liệu
                        db.Entry(FindClient).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Response.Cookies["access_token"].Value = tokenString;
                        Response.Cookies["access_token"].Expires = DateTime.Now.AddDays(7); // Thời gian sống của cookie
                        // Gửi email với mã khôi phục
                        string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/SendForgotPass.html"));
                        contentCustomer = contentCustomer.Replace("{{MaDon}}", FindClient.Code);
                        contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                        contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", FindClient.CustomerName);

                        WebSite_CuaHangDienThoai.Common.Common.SendMail("Cửa hàng điện thoại LTDMiniStore", "Mã khôi phục #" + FindClient.Code, contentCustomer.ToString(), FindClient.Email);

                        return RedirectToAction("UpdatePass", new { id = FindClient.CustomerId, token = tokenString }); // Chuyển hướng đến trang cập nhật mật khẩu
                    }
                }
                // Nếu không tìm thấy người dùng hoặc email trống, trả về view
              
                return RedirectToAction("NonAccount", "Account");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return RedirectToAction("NotFound", "Error");
            }
        }

        public ActionResult UpdatePass(int id, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                // Nếu không có token, xử lý lỗi hoặc chuyển hướng đến trang không hợp lệ
                return RedirectToAction("InvalidToken", "Error");
            }

            ViewBag.Token = token;

            if (id > 0)
            {
                var KhachHang = db.tb_Customer.Find(id);
                if (KhachHang != null)
                {
                    return View(KhachHang);
                }
                else
                {
                    // Xử lý khi không tìm thấy khách hàng
                    return RedirectToAction("NotFound", "Error");
                }
            }
            else
            {
                // Xử lý khi id không hợp lệ
                return RedirectToAction("NotFound", "Error");
            }
        }

        public ActionResult Partail_UpdatePass(int id, string token)
        {
            ViewBag.id = id;
            ViewBag.Token = token;
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePass(Client_ForgetPassClient req)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cookie = Request.Cookies["access_token"];
                    if (cookie != null)
                    {
                        var tokenString = cookie.Value;
                        if (AuthenticateToken(tokenString))
                        {
                            var claimsPrincipal = DecodeJWT(tokenString);
                            var claims = claimsPrincipal.Claims;

                            var customerIdClaim = claims.FirstOrDefault(c => c.Type == "CustomerId");
                            if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int customerId))
                            {
                                var checkCode = db.tb_Customer.FirstOrDefault(r => r.Code == req.Code.Trim());
                                if (checkCode == null)
                                {
                                    return Json(new { success = false, code = -3 });

                                }
                                var checkClient = db.tb_Customer.FirstOrDefault(row => row.CustomerId == customerId && row.Code == req.Code.Trim());
                                if (checkClient != null)
                                {
                                    checkClient.Password = MaHoaPass(req.Password.Trim());
                                    db.Entry(checkClient).State = EntityState.Modified;
                                    db.SaveChanges();

                                    Session.Clear();

                                    // Xóa cookie chứa token
                                    if (Request.Cookies["access_token"] != null)
                                    {
                                        HttpCookie myCookie = new HttpCookie("access_token");
                                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                                        Response.Cookies.Add(myCookie);
                                    }

                                    // Chuyển hướng về trang chủ
                                    return Json(new { success = true, code = 1 });
                                }
                                else
                                {
                                    return Json(new { success = false, code = -2 });
                                }
                            }
                        }
                    }

                    return Json(new { success = false, code = -3 });
                }
                else
                {
                    return Json(new { success = false, code = -1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = -100 });
            }
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
                var checkCustomer = db.tb_Customer.FirstOrDefault(x => x.PhoneNumber == _khachhang.PhoneNumber && x.Email == _khachhang.Email && x.NumberofPurchases > 0);
                if (checkCustomer != null)
                {
                    checkCustomer.Email = _khachhang.Email;
                    checkCustomer.Password = MaHoaPass(_khachhang.Password);
                    checkCustomer.Birthday = _khachhang.Birthday;
                    checkCustomer.Loaction = _khachhang.Loaction;
                    _khachhang.Clock = false;
                    db.Entry(checkPhone).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    if (checkmail == null)
                    {
                        if (checkPhone == null)
                        {
                            _khachhang.Password = MaHoaPass(_khachhang.Password.Trim()).Trim();
                            _khachhang.NumberofPurchases = 1;
                            _khachhang.Clock = false;

                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.tb_Customer.Add(_khachhang);
                            db.SaveChanges();

                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {

                            ModelState.AddModelError("", "Số điện thoại đã tồn tại");
                            return View(_khachhang);
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("", "Email đã tồn tại");
                        return View(_khachhang);
                    }
                }

            }

            // Trả về view với dữ liệu đang nhập và thông báo lỗi nếu dữ liệu không hợp lệ
            return View(_khachhang);
        }




        public ActionResult Partail_EditPassWord(int id)
        {
            if (Session["Client"] != null && id > 0) // Sửa điều kiện id > 0 để tránh lỗi
            {
                var customer = db.tb_Customer.Find(id);
                if (customer != null)
                {

                    var viewModel = new Client_EditPassWord
                    {


                        Id = customer.CustomerId,
                        Password = customer.PhoneNumber,
                       
                    };
                    return PartialView(viewModel);

                    
                }
                else
                {
                    return RedirectToAction("Login", "Account"); // Thêm return trước RedirectToAction
                }
            }
            else
            {
                return RedirectToAction("Login", "Account"); // Thêm return trước RedirectToAction
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Partail_EditPassWord(Client_EditPassWord req)
        {
            using (var dbContext = db.Database.BeginTransaction()) 
            {
                try
                {

                    string passOld = MaHoaPass(req.Password.Trim());
                    string passNew= MaHoaPass(req.PasswordNew.Trim());
                    var customer = db.tb_Customer.FirstOrDefault(x => x.CustomerId == req.Id && x.Password == passOld.Trim());
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






        public ActionResult Partial_Edit(int id)
        {
            if (id > 0)
            {
                var KhachHang = db.tb_Customer.Find(id);
                if (KhachHang != null)
                {
                    var viewModel = new CLient_EditCustomerViewModel
                    {
                        CustomerId = KhachHang.CustomerId,
                        PhoneNumber = KhachHang.PhoneNumber,
                        CustomerName = KhachHang.CustomerName,
                        Email = KhachHang.Email,
                        Code = KhachHang.Code,
                        Password = KhachHang.Password,
                        Birthday = KhachHang.Birthday,
                        Loaction = KhachHang.Loaction,
                        NumberofPurchases = (int)KhachHang.NumberofPurchases,
                        Clock = (bool)KhachHang.Clock,
                        Image = KhachHang.Image,
                    };
                    return View(viewModel);
                }

            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CLient_EditCustomerViewModel item, HttpPostedFileBase newImage) 
        {
            using (var dbContext = db.Database.BeginTransaction()) 
            {
                try 
                {
                    if (ModelState.IsValid)
                    {
                        var customer=db.tb_Customer.Find(item.CustomerId);
                        if (customer != null)
                        {
                            customer.CustomerId = item.CustomerId;
                            customer.PhoneNumber = item.PhoneNumber.Trim();
                            customer.CustomerName = item.CustomerName.Trim();
                            customer.Email = item.Email.Trim();
                            customer.Code = item.Code;
                            customer.Password = item.Password.Trim();
                            customer.Birthday = item.Birthday;
                            customer.Loaction = item.Loaction;
                            customer.NumberofPurchases = (int)item.NumberofPurchases;
                            customer.Clock = (bool)item.Clock;
                            customer.Image = item.Image;
                            db.Entry(customer).State= System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            if (customer.Image != null)
                            {
                                string base64String = Convert.ToBase64String(customer.Image);
                                Session["img"] = base64String;
                            }

                            Session["Client"] = customer.CustomerId;
                            Session["CustomerId"] = customer.CustomerId;
                            Session["CustomerName"] = customer.CustomerName;
                            Session["Email"] = customer.Email;




                            dbContext.Commit();
                            return Json(new { success = true, code = 1, customerId = customer.CustomerId });
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






        public ActionResult EditImage(int id)
        {
            var item = db.tb_Customer.Find(id);
            if (item != null) 
            {
                return PartialView(item);
            }
            return PartialView();   
        }


        [HttpPost]
        public JsonResult EditImage(int customerId, HttpPostedFileBase newImage)
        {
            try
            {
                if (newImage != null && newImage.ContentLength > 0)
                {
                    var checkCustomer = db.tb_Customer.Find(customerId);
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