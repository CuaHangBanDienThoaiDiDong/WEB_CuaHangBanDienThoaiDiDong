using Antlr.Runtime.Tree;
using ImageProcessor.Processors;
using Microsoft.Win32;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class BillController : Controller
    {
        // GET: Admin/Bill
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {

                    return View();
                }
            }

        }


        public ActionResult Partial_IndexBill(int? page)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                IEnumerable<tb_Seller> items = db.tb_Seller.OrderByDescending(x => x.SellerId);
                if (items != null)
                {
                    var pageSize = 10;
                    if (page == null)
                    {
                        page = 1;
                    }
                    var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    items = items.ToPagedList(pageIndex, pageSize);
                    var products = db.tb_Seller.ToList();

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

        public ActionResult SuggestBillCustomer(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var customerIds = db.tb_Customer
                    .Where(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search))
                    .Select(c => c.CustomerId)
                    .ToList();

                var sellers = db.tb_Seller
                    .Where(s => customerIds.Contains(s.CustomerId) || s.Code.Contains(search))
                    .ToList();

                if (sellers.Any())
                {
                    ViewBag.Count = sellers.Count; // Số lượng hóa đơn
                    ViewBag.Content = search;
                    return PartialView(sellers); // Trả về danh sách hóa đơn
                }
                else
                {
                    return PartialView(); // Trả về PartialView rỗng
                }
            }
            else
            {
                return PartialView(); // Trả về PartialView rỗng
            }
        }





        public ActionResult BillExportToday(DateTime ngayxuat)
        {
            DateTime selectedDate = ngayxuat;
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1); 

          
            var orders = db.tb_Seller
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

            if (orders != null && orders.Count > 0)
            {
                ViewBag.Date = ngayxuat;
                ViewBag.Count = orders.Count;
                return PartialView(orders);
            }
            else
            {
                return PartialView();
            }
        }



        public ActionResult Detail(int id)
        {
            if (id > 0)
            {
                var seller = db.tb_Seller.Find(id);
                if (seller != null)
                {
                    ViewBag.Code = seller.Code;
                    return View(seller);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }


        public ActionResult Detail_SanPham(int id)
        {
            if (id > 0)
            {
                var SellerDetail = db.tb_SellerDetail.Where(x => x.SellerId == id).ToList();
                if (SellerDetail != null)
                {
                    ViewBag.Count = SellerDetail.Count;
                    return PartialView(SellerDetail);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return PartialView();
            }
        }







        public ActionResult Edit(int id)
        {
            try
            {
                // Kiểm tra đăng nhập
                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }

                tb_Staff nvSession = (tb_Staff)Session["user"];

                // Kiểm tra quyền của nhân viên
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 4));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }

                // Lấy thông tin seller
                var seller = db.tb_Seller.FirstOrDefault(x => x.SellerId == id);
                if (seller == null)
                {
                    return PartialView();
                }

                // Tạo đối tượng Admin_TokenEditBill mới
                Admin_TokenEditBill viewModel = new Admin_TokenEditBill
                {
                    SellerId = seller.SellerId,
                    Code = seller.Code,
                    CustomerId = seller.CustomerId,
                    Phone = seller.Phone,
                    TotalAmount = seller.TotalAmount,
                    Quantity = seller.Quantity,
                    CreatedBy = seller.CreatedBy,
                    CreatedDate = seller.CreatedDate,
                    TypePayment = seller.TypePayment,
                    StaffId = seller.StaffId,
                    Customer = db.tb_Customer.FirstOrDefault(x => x.CustomerId == seller.CustomerId),
                };

                // Lấy chi tiết sản phẩm của seller
                var sellerDetails = db.tb_SellerDetail
                                        .Where(x => x.SellerId == id)
                                        .Select(detail => new Admin_TokenEditBillItem
                                        {
                                            Id = detail.Id,
                                            SellerId = detail.SellerId,
                                            Price = detail.Price,
                                            Quantity = detail.Quantity,
                                            ProductDetailId = detail.ProductDetailId,
                                            Product = db.tb_ProductDetail.FirstOrDefault(p => p.ProductDetailId == detail.ProductDetailId)
                                        })
                                        .ToList();


                viewModel.Items = sellerDetails;


                Session["Admin_TokenEditBill_" + id] = viewModel;

                // Trả về partial view với viewModel
                ViewBag.Count = viewModel.Items.Count;
                return PartialView(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView();
            }
        }
        public ActionResult Partail_ListProductEdit(int sellerId)
        {
            Admin_TokenEditBill bill = (Admin_TokenEditBill)Session["Admin_TokenEditBill_" + sellerId];
            if (bill != null && bill.Items.Any())
            {
                int count = bill.Items.Count;
                ViewBag.Count = count;
                return PartialView(bill);
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Admin_TokenEditBill item)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra đăng nhập
                    if (Session["user"] == null)
                    {
                        return RedirectToAction("DangNhap", "Account");
                    }

                    if (!ModelState.IsValid)
                    {
                        return Json(new { success = false, code = -1 });//Không đủ dữ liệu
                    }

                    // Lấy thông tin từ session
                    var sessionKey = "Admin_TokenEditBill_" + item.SellerId;
                    
                    var viewModel = Session[sessionKey] as Admin_TokenEditBill;
                    if (viewModel.Items.Count < 1) 
                    {
                        return Json(new { success = false, code = -4 });//Hóa phải có 1 sản phẩm
                    }
                    if (viewModel == null)
                    {
                        return Json(new { success = false, code = -3 });//Không tìm thấy hóa đơn
                    }

                    tb_Staff nvSession = (tb_Staff)Session["user"];
                    var seller = db.tb_Seller.Find(item.SellerId);

                    if (seller == null)
                    {
                        return Json(new { success = false, code = -2 });//Không tìm thấy hóa đơn
                    }

                    var newDetailsPrice = viewModel.Items;

                    decimal totalAmount = 0;

    
                    foreach (var detail in newDetailsPrice)
                    {
                        
                        totalAmount += detail.Price * detail.Quantity;
                    }

                    // Cập nhật tổng tiền của hóa đơn
                    seller.TotalAmount = totalAmount;
                    seller.Code = item.Code;
                    seller.CustomerId = item.CustomerId;
                    seller.Phone = item.Phone;
                  
                    seller.Quantity = item.Quantity;
                    seller.CreatedBy = item.CreatedBy;
                    seller.CreatedDate = item.CreatedDate;
                    seller.TypePayment = item.TypePayment;
                    seller.StaffId = item.StaffId;
                    seller.Modifiedby = nvSession.NameStaff;
                    seller.ModifiedDate = DateTime.Now;
                    seller.CustomerId = item.CustomerId;
                    var newCustomer = db.tb_Customer.FirstOrDefault(c => c.PhoneNumber == item.Phone);
                    if (newCustomer != null)
                    {

                        seller.CustomerId = newCustomer.CustomerId;
                    }
                    else 
                    {
                        return Json(new { success = false, code = -6 });
                    }
                    db.Entry(seller).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy danh sách chi tiết hiện tại từ cơ sở dữ liệu
                    var existingDetails = db.tb_SellerDetail.Where(d => d.SellerId == seller.SellerId).ToList();
                    var newDetails = viewModel.Items;
                    
                    // Xóa các chi tiết không còn trong danh sách mới
                    foreach (var existingDetail in existingDetails)
                    {
                        if (!newDetails.Any(d => d.ProductDetailId == existingDetail.ProductDetailId))
                        {
                            // Cập nhật lại số lượng trong tb_ImportWarehouseDetail
                            var importDetail = db.tb_ImportWarehouseDetail
                                .FirstOrDefault(x => x.ProductDetailId == existingDetail.ProductDetailId);

                            if (importDetail != null)
                            {
                                importDetail.QuanTity += existingDetail.Quantity;
                                db.Entry(importDetail).State = EntityState.Modified;
                            }

                            db.tb_SellerDetail.Remove(existingDetail);
                        }
                    }

                    // Cập nhật hoặc thêm các chi tiết mới
                    foreach (var detail in newDetails)
                    {
                        var existingDetail = existingDetails.FirstOrDefault(d => d.ProductDetailId == detail.ProductDetailId);

                        if (existingDetail != null)
                        {
                            // Điều chỉnh số lượng trong tb_ImportWarehouseDetail
                            var importDetail = db.tb_ImportWarehouseDetail
                                .FirstOrDefault(x => x.ProductDetailId == existingDetail.ProductDetailId);

                            if (importDetail != null)
                            {
                                importDetail.QuanTity += existingDetail.Quantity - detail.Quantity;
                                db.Entry(importDetail).State = EntityState.Modified;
                            }

                            existingDetail.Price = detail.Price;
                            existingDetail.Quantity = detail.Quantity;
                            db.Entry(existingDetail).State = EntityState.Modified;
                        }
                        else
                        {
                            var newDetail = new tb_SellerDetail
                            {
                                SellerId = seller.SellerId,
                                Price = detail.Price,
                                Quantity = detail.Quantity,
                                ProductDetailId = detail.ProductDetailId
                            };

                            db.tb_SellerDetail.Add(newDetail);

                            // Điều chỉnh số lượng trong tb_ImportWarehouseDetail
                            var importDetail = db.tb_ImportWarehouseDetail
                                .FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId);

                            if (importDetail != null)
                            {
                                importDetail.QuanTity -= detail.Quantity;
                                db.Entry(importDetail).State = EntityState.Modified;
                            }
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    return Json(new { success = true, code = 1 });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100, message = ex.Message });
                }
            }
        }




        [HttpPost]
        public ActionResult DeleteItem(int sellerId, int productDetailId)
        {
            try
            {
                if (Session["Admin_TokenEditBill_" + sellerId] != null)
                {
                    Admin_TokenEditBill viewModel = (Admin_TokenEditBill)Session["Admin_TokenEditBill_" + sellerId];
                    if (viewModel != null)
                    {
                        if (viewModel.Items.Count > 1)
                        {
                            viewModel.Items.RemoveAll(x => x.Id == productDetailId);

                            Session["Admin_TokenEditBill_" + sellerId] = viewModel;

                            return Json(new { success = true, code = 1, message = "Xóa sản phẩm thành công" });
                        }
                        else { return Json(new { success = false, message = "Bắt buộc 1 sản phẩm" }); }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }


        public ActionResult GetTotalPrice(int sellerId)
        {
            try
            {
                if (Session["Admin_TokenEditBill_" + sellerId] != null)
                {
                    var viewModel = (Admin_TokenEditBill)Session["Admin_TokenEditBill_" + sellerId];
                    if (viewModel != null)
                    {
                        return Json(WebSite_CuaHangDienThoai.Common.Common.FormatNumber(viewModel.GetPriceTotal(), 0) + " đ", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("0 đ", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0 đ", JsonRequestBehavior.AllowGet);
            }
        }





    }
}