using Antlr.Runtime.Tree;
using DocumentFormat.OpenXml.EMMA;
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






using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using CKFinder.Settings;

using ZXing;
using ZXing.QrCode;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;




using ImageProcessor.Processors;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using ZXing.QrCode.Internal;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Drawing.Charts;


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
                    ViewBag.Count = db.tb_Seller.Count();
                    return View();
                }
            }

        }
        public ActionResult IndexAdmin()
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
                return RedirectToAction("DangNhap", "Account"); // Redirect to login if user is not authenticated
            }
            else
            {


                tb_Staff nvSession = (tb_Staff)Session["user"];
                if (nvSession == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }

                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 4));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {


                    IEnumerable<tb_Seller> items = db.tb_Seller.OrderByDescending(x => x.SellerId);

                    if (items.Any()) // Check if there are any items
                    {
                        var pageSize = 10;
                        if (page == null)
                        {
                            page = 1;
                        }
                        var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        items = items.ToPagedList(pageIndex, pageSize);
                        if (item.FunctionId == 2 || item.FunctionId == 1) 
                        {
                            ViewBag.QuanLy = item.FunctionId;
                        }
                        ViewBag.PageSize = pageSize;
                        ViewBag.Page = page;
                        return PartialView(items);

                        // Return partial view with paginated data

                    }
                    else
                    {
                        // No items found
                        ViewBag.txt = "Không tồn tại sản phẩm";
                        return PartialView(); // Return partial view without data
                    }
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



                tb_Staff nvSession = (tb_Staff)Session["user"];
                if (nvSession == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }

                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 4));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                if (sellers.Any())
                {
                    if (item.FunctionId == 2 || item.FunctionId == 1)
                    {
                        ViewBag.QuanLy = item.FunctionId;
                    }
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

            tb_Staff nvSession = (tb_Staff)Session["user"];
            if (nvSession == null)
            {
                return RedirectToAction("NonRole", "HomePage");
            }

            var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 4));
            if (item == null)
            {
                return RedirectToAction("NonRole", "HomePage");
            }


            var orders = db.tb_Seller
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                .ToList();

            if (orders != null && orders.Count > 0)
            {

                if (item.FunctionId == 2 || item.FunctionId == 1)
                {
                    ViewBag.QuanLy = item.FunctionId;
                }

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
        public ActionResult UpdateQuantityForEditNew(int productId, int sellerId, int sellerDetailId, int newQuantity)
        {
            try
            {
               
                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }

             
                var sessionKey = "Admin_TokenEditBill_" + sellerId;
                Admin_TokenEditBill viewModel = (Admin_TokenEditBill)Session[sessionKey];

               
                if (viewModel == null)
                {
                    return HttpNotFound();
                }

                var itemToUpdate = viewModel.Items.FirstOrDefault(item =>
                    item.ProductDetailId == productId &&
                    item.SellerId == sellerId && item.Id == sellerDetailId);

                if (itemToUpdate != null)
                {
              
                    int oldQuantity = itemToUpdate.Quantity;

                    // Cập nhật số lượng mới
                    itemToUpdate.Quantity = newQuantity;

                    // Cập nhật lại session
                    Session[sessionKey] = viewModel;

                    // Trả về kết quả thành công
                    return Json(new { success = true });

                
                   
                }

                // Nếu không tìm thấy sản phẩm để cập nhật
                return Json(new { success = false, errorMessage = "Không tìm thấy sản phẩm để cập nhật số lượng." });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false, errorMessage = ex.Message });
            }
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

                    // Kiểm tra dữ liệu model
                    if (!ModelState.IsValid)
                    {
                        return Json(new { success = false, code = -1 });
                    }

                    var sessionKey = "Admin_TokenEditBill_" + item.SellerId;
                    var viewModel = Session[sessionKey] as Admin_TokenEditBill;

                    // Kiểm tra viewModel
                    if (viewModel == null || viewModel.Items.Count < 1)
                    {
                        return Json(new { success = false, code = viewModel == null ? -3 : -4 });
                    }

                    var nvSession = (tb_Staff)Session["user"];
                    var seller = db.tb_Seller.Find(item.SellerId);

                    // Kiểm tra seller
                    if (seller == null)
                    {
                        return Json(new { success = false, code = -2 });
                    }

                    // Tính lại tổng tiền
                    decimal totalAmount = viewModel.Items.Sum(detail => detail.Price * detail.Quantity);
                    seller.TotalAmount = totalAmount;

                    // Cập nhật các thông tin khác của seller
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

                    // Tìm và cập nhật thông tin khách hàng mới (nếu có)
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

                    var existingDetails = db.tb_SellerDetail.Where(d => d.SellerId == seller.SellerId).ToList();
                    var newDetails = viewModel.Items;

                    foreach (var detail in existingDetails.ToList())
                    {
                        var viewModelDetail = viewModel.Items.FirstOrDefault(x => x.Id == detail.Id);
                        if (viewModelDetail != null)
                        {
                            int oldQuantity = detail.Quantity;
                            int newQuantity = viewModelDetail.Quantity;
                            int quantityChange = newQuantity - oldQuantity;

                            // Cập nhật số lượng trong chi tiết hóa đơn bán hàng
                            detail.Quantity = newQuantity;
                            db.Entry(detail).State = EntityState.Modified;

                            // Kiểm tra và cập nhật số lượng trong kho
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    if (quantityChange > 0) // Nếu tăng số lượng
                                    {
                                        // Kiểm tra kho đủ số lượng không
                                        if (warehouseDetail.QuanTity < quantityChange)
                                        {
                                            dbContextTransaction.Rollback();
                                            return Json(new { success = false, code = -8, message = $"Số lượng sản phẩm '{detail.ProductDetailId}' trong kho không đủ." });
                                        }

                                       
                                        warehouseDetail.QuanTity -= quantityChange;
                                    }
                                    else if (quantityChange < 0) // Nếu giảm số lượng
                                    {
                                        // Kiểm tra số lượng trong SellerDetail đủ để giảm không
                                        //if (detail.Quantity < -quantityChange)
                                        //{
                                        //    dbContextTransaction.Rollback();
                                        //    return Json(new { success = false, code = -9, message = $"Số lượng sản phẩm '{detail.ProductDetailId}' trong chi tiết đơn hàng không đủ để giảm." });
                                        //}

                                        warehouseDetail.QuanTity += -quantityChange;
                                    }

                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                }
                                else
                                {
                                    return Json(new { success = false, code = -5, message = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -6, message = "Chi tiết hóa đơn không có ProductDetailId." });
                            }
                        }
                        else
                        {
                            // Xóa chi tiết hóa đơn không có trong viewModel
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    warehouseDetail.QuanTity += detail.Quantity; // Cộng lại số lượng tồn kho
                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                    db.tb_SellerDetail.Remove(detail);
                                }
                                else
                                {
                                    return Json(new { success = false, code = -5, message = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -6, message = "Chi tiết hóa đơn không có ProductDetailId." });
                            }
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    string invoicePath = ExportInvoice(item.SellerId);

                    if (!string.IsNullOrEmpty(invoicePath))
                    {
                        return Json(new { success = true, Code = 1, FilePath = invoicePath });
                    }
                    else
                    {
                        return Json(new { success = false, Code = -10, message = "Không thể xuất hóa đơn." });
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -5, message = $"Lỗi ngoại lệ: {ex.Message}" });
                }
            }
        }



        //Strart In Bill

        [HttpGet]
        public ActionResult InBillLaiNew(int id)
        {
            try
            {
                if (Session["user"] == null)
                {
                    return Json(new { success = false, Code = -1, Message = "Phiên đăng nhập không tồn tại" }, JsonRequestBehavior.AllowGet);
                }

                if (id < 0)
                {
                    return Json(new { success = false, Code = -2, Message = "ID không hợp lệ" }, JsonRequestBehavior.AllowGet);
                }

                string invoicePath = ExportInvoice(id);

                if (!string.IsNullOrEmpty(invoicePath))
                {
                    return Json(new { success = true, Code = 1, FilePath = invoicePath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Code = -10, Message = "Lỗi khi in hóa đơn" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Code = -100, Message = "Lỗi ngoại lệ: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public string ExportInvoice(int sellerId)
        {
            try 
            {
                if (sellerId < 0)
                {
                    return null;
                }

                var seller = db.tb_Seller.Find(sellerId);
                if (seller != null)
                {
                    var customer = db.tb_Customer.Find(seller.CustomerId);
                    if (customer == null)
                    {
                        return null;
                    }
                    var staff=db.tb_Staff.Find(seller.StaffId);
                    if (staff == null)
                    {
                        return null;
                    }
                    try
                    {
                        string templatePath = Server.MapPath("~/Content/templates/HoaDon.html");
                        string htmlContent = System.IO.File.ReadAllText(templatePath);

                        // Thay thế các biến trong template HTML bằng giá trị thực tế
                        htmlContent = htmlContent.Replace("#{{Code}}", seller.Code);
                        htmlContent = htmlContent.Replace("#{{CreatedDate}}", seller.CreatedDate.ToString("dd/MM/yyyy"));
                        htmlContent = htmlContent.Replace("#{{CustomerName}}", customer.CustomerName);
                        htmlContent = htmlContent.Replace("#{{Phone}}", seller.Phone);
                        htmlContent = htmlContent.Replace("#{{CreatedBy}}", staff.NameStaff);

                        // Lấy chi tiết đơn hàng
                        var sellerDetail = db.tb_SellerDetail
                            .Where(od => od.SellerId == seller.SellerId)
                            .ToList();

                        string productRows = "";
                        int index = 1;

                        foreach (var detail in sellerDetail)
                        {
                            string productRow = $@"
                    <tr>
                        <td>{index}</td>
                        <td>{detail.tb_ProductDetail.tb_Products.Title}</td>
                        <td>{GetCapacity((int)detail.tb_ProductDetail.Capacity)} / {detail.tb_ProductDetail.Color}</td>
                        <td>{detail.Quantity}</td>
                        <td>{WebSite_CuaHangDienThoai.Common.Common.FormatNumber(detail.tb_ProductDetail.PriceSale > 0 ? detail.tb_ProductDetail.PriceSale : detail.tb_ProductDetail.Price)}</td>
                        <td>{WebSite_CuaHangDienThoai.Common.Common.FormatNumber(detail.Quantity * (detail.tb_ProductDetail.PriceSale > 0 ? detail.tb_ProductDetail.PriceSale : detail.tb_ProductDetail.Price))} đ</td>
                    </tr>";
                            productRows += productRow;
                            index++;
                        }

                        htmlContent = htmlContent.Replace("#{{ProductRows}}", productRows);

                        // Tính tổng tiền thanh toán
                        decimal total = seller.TotalAmount;
                        htmlContent = htmlContent.Replace("#{{TotalAmountSeller}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(seller.TotalAmount) + " đ");

                        string fileName = $"LTD_HD_{seller.Code}.docx";
                        string folderPath = Server.MapPath("~/Order");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        string filePath = Path.Combine(folderPath, fileName);

                        // Chuyển đổi HTML sang Word
                        string logoPath = Server.MapPath("~/images/Logo/LogoWebpng.png");

                        ConvertHTMLToWord(htmlContent, filePath, logoPath, seller.Code);

                        return filePath;
                    }
                    catch (Exception ex)
                    {
                        // Xử lý nếu có lỗi
                        // Có thể ghi log hoặc thông báo lỗi ở đây
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }
        public ActionResult DownBill(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }

        public ActionResult DownloadInvoice123(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }





        private string GetCapacity(int capacity)
        {
            if (capacity > 1999)
            {
                return "2 Tb";
            }
            else if (capacity > 999)
            {
                return "1 Tb";
            }
            else
            {
                return capacity + " Gb";
            }
        }




        public void ConvertHTMLToWord(string htmlContent, string wordFilePath, string logoPath, string qrCodeContent)
        {
            // Thay thế placeholder với hình ảnh logo và mã QR code
            string logoPlaceholder = "#{{Image}}";
            string qrCodePlaceholder = "#{{QRCode}}";

            if (System.IO.File.Exists(logoPath))
            {
                string logoBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(logoPath));
                string imgTag = $"<img src='data:image/png;base64,{logoBase64}' style='width: 100px; height: 100px;' />";
                htmlContent = htmlContent.Replace(logoPlaceholder, imgTag);
            }

            if (!string.IsNullOrEmpty(qrCodeContent))
            {
                using (MemoryStream qrCodeStream = GenerateQRCode(qrCodeContent))
                {
                    string qrCodeBase64 = Convert.ToBase64String(qrCodeStream.ToArray());
                    string imgTag = $"<img src='data:image/png;base64,{qrCodeBase64}' style='width: 50px; height: 50px;' />";
                    htmlContent = htmlContent.Replace(qrCodePlaceholder, imgTag);
                }
            }

            using (MemoryStream memStream = new MemoryStream())
            {
                // Tạo một tài liệu Word mới
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memStream, WordprocessingDocumentType.Document))
                {
                    // Tạo phần main của tài liệu
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Chuyển đổi HTML thành Open XML và thêm vào tài liệu Word
                    string altChunkId = "AltChunkId1";
                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, altChunkId);
                    using (StreamWriter streamWriter = new StreamWriter(chunk.GetStream()))
                    {
                        streamWriter.Write(htmlContent);
                    }

                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;
                    body.Append(altChunk);

                    // Lưu tài liệu Word xuống tệp
                    mainPart.Document.Save();
                }

                // Lưu tài liệu Word xuống tệp
                using (FileStream fileStream = new FileStream(wordFilePath, FileMode.Create))
                {
                    memStream.Position = 0;
                    memStream.CopyTo(fileStream);
                }
            }

            // Chỉnh sửa tài liệu Word để đảm bảo hình ảnh có kích thước đúng
            FixImageSizes(wordFilePath);
        }

        private void FixImageSizes(string wordFilePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(wordFilePath, true))
            {
                foreach (var image in wordDocument.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Drawing.Blip>())
                {
                    if (image.Embed != null)
                    {
                        var imagePart = wordDocument.MainDocumentPart.GetPartById(image.Embed.Value) as ImagePart;
                        if (imagePart != null)
                        {
                            using (var imageStream = imagePart.GetStream())
                            {
                                using (var bitmap = new System.Drawing.Bitmap(imageStream))
                                {
                                    var width = bitmap.Width * 9525; // 1 pixel = 9525 EMUs
                                    var height = bitmap.Height * 9525;

                                    var drawing = image.Ancestors<DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline>().FirstOrDefault();
                                    if (drawing != null)
                                    {
                                        drawing.Extent.Cx = width;
                                        drawing.Extent.Cy = height;

                                        var extents = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Extents>().FirstOrDefault();
                                        if (extents != null)
                                        {
                                            extents.Cx = width;
                                            extents.Cy = height;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                wordDocument.MainDocumentPart.Document.Save();
            }
        }

        private void AddQRCodeToBody(MainDocumentPart mainPart, string qrCodeContent)
        {
            // Tạo mã QR Code và lưu vào MemoryStream
            using (MemoryStream qrCodeStream = GenerateQRCode(qrCodeContent))
            {
                // Thêm hình ảnh QR Code vào tài liệu Word
                AddImageToBody(mainPart, qrCodeStream, "rIdQrCode");
            }
        }
        private MemoryStream GenerateQRCode(string qrCodeContent)
        {
            // Khởi tạo đối tượng BarcodeWriter để tạo mã QR
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new QrCodeEncodingOptions
            {
                Height = 50,
                Width = 50,
                Margin = 0,
                ErrorCorrection = ErrorCorrectionLevel.H
            };

            // Tạo mã QR từ qrCodeContent
            var qrCodeBitmap = barcodeWriter.Write(qrCodeContent);

            // Chuyển đổi mã QR thành MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return new MemoryStream(ms.ToArray());
            }
        }

        private void AddImageToBody(MainDocumentPart mainPart, Stream imageStream, string relationshipId)
        {
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png, relationshipId);
            imagePart.FeedData(imageStream);

            var element = new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = 990000L, Cy = 792000L },
                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture 1"
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "QR Code"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()
                                ),
                                new PIC.BlipFill(
                                    new A.Blip()
                                    {
                                        Embed = relationshipId,
                                        CompressionState = A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle()
                                    )
                                ),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = 990000L, Cy = 792000L }
                                    ),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                                )
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                    )
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = "50D07946"
                });

            mainPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }





        //End In Bill


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