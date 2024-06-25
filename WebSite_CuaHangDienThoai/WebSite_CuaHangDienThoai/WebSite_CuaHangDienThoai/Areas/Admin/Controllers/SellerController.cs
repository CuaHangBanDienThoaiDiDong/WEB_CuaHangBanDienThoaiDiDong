using System;
using System.Collections.Generic;
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
    public class SellerController : Controller
    {
        // GET: /Admin/Seller/IndexAdmin
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId ==4));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    DateTime today = DateTime.Today;
                    DateTime startOfDay = today.Date;
                    DateTime endOfDay = today.Date.AddDays(1).AddTicks(-1);
                    ViewBag.Today = today;
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

                    DateTime today = DateTime.Today;
                    DateTime startOfDay = today.Date;
                    DateTime endOfDay = today.Date.AddDays(1).AddTicks(-1);
                    ViewBag.Today = today;
                    return View();
                }
            }

        }
        public ActionResult Partail_SellerIndex()
        {
            return PartialView();
        }

        public ActionResult SuggestProduct(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var ProductCategoryId = db.tb_ProductCategory
          .Where(c => c.Title.Contains(search))
          .Select(c => c.ProductCategoryId)
          .ToList();

                var ProductCompanyId = db.tb_ProductCompany
                    .Where(c => c.Title.Contains(search))
                    .Select(c => c.ProductCompanyId)
                    .ToList();

                var Products = db.tb_Products
                    .Where(p => p.Title.Contains(search) ||
                                ProductCategoryId.Contains((int)p.ProductCategoryId) ||
                                ProductCompanyId.Contains((int)p.ProductCompanyId))
                    .ToList();


                var productIds = Products.Select(p => p.ProductsId).ToList();


                var ProductDetail = db.tb_ProductDetail
                    .Where(s => productIds.Contains((int)s.ProductsId))
                    .ToList();

                if (ProductDetail.Any())
                {
                    var count = ProductDetail.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ProductDetail);
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

        public ActionResult SuggestProductLeft(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var ProductCategoryId = db.tb_ProductCategory
          .Where(c => c.Title.Contains(search))
          .Select(c => c.ProductCategoryId)
          .ToList();

                var ProductCompanyId = db.tb_ProductCompany
                    .Where(c => c.Title.Contains(search))
                    .Select(c => c.ProductCompanyId)
                    .ToList();

                var Products = db.tb_Products
                    .Where(p => p.Title.Contains(search) ||
                                ProductCategoryId.Contains((int)p.ProductCategoryId) ||
                                ProductCompanyId.Contains((int)p.ProductCompanyId))
                    .ToList();


                var productIds = Products.Select(p => p.ProductsId).ToList();


                var ProductDetail = db.tb_ProductDetail
                    .Where(s => productIds.Contains((int)s.ProductsId))
                    .ToList();

                if (ProductDetail.Any())
                {
                    var count = ProductDetail.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(ProductDetail);
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
   


        public ActionResult Partail_ProductDetail()
        {
            var item = db.tb_ProductDetail.ToList();
            return PartialView(item);
        }




        [HttpGet]
        public ActionResult Suggest(string searchString)
        {

            if (!string.IsNullOrEmpty(searchString))
            {

                var suggestedProducts = db.tb_Products
                                     .Where(p => p.Title.Contains(searchString))
                                     .Select(x => x.ProductsId)  // Chỉ lấy ID sản phẩm
                                     .ToList();
                if (suggestedProducts != null)
                {
                    var items = db.tb_ProductDetail
                                     .Where(pd => pd.ProductsId.HasValue && suggestedProducts.Contains(pd.ProductsId.Value))
                                     .OrderByDescending(pd => pd.ProductsId)
                                     .Take(5)
                                     .ToList();
                    HttpContext.Items["SuggestedProducts"] = items;
                    ViewBag.NoiDung = searchString;
                    return PartialView(items);
                }
                else
                {
                    return PartialView("_SuggestedProducts", null);
                }
            }
            else
            {
                return PartialView("_SuggestedProducts", null);
            }
        }



        public ActionResult SuggestCustomer(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var customer = db.tb_Customer
              .FirstOrDefault(c => c.PhoneNumber.Contains(search) || c.CustomerName.Contains(search));

                SellerCart cart = (SellerCart)Session["SellerCart"];
                if (cart.Items.Count > 0)
                {
                    ViewBag.Data=cart.Items;
                }
                if (customer != null)
                {
                    return PartialView(new List<tb_Customer> { customer });
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

        [HttpPost]
        public JsonResult CheckOutSeller(int CustomerId)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Session["user"] == null)
                    {
                        return Json(new { success = false, code = -99 });
                    }
                    else
                    {
                        tb_Staff nvSession = (tb_Staff)Session["user"];
                        if (CustomerId > 0)
                        {
                            var customer = db.tb_Customer.SingleOrDefault(x => x.CustomerId == CustomerId);
                            if (customer != null)
                            {
                                SellerCart cart = (SellerCart)Session["SellerCart"];
                                if (cart.Items.Count < 0)
                                {
                                    return Json(new { success = false, code = -3 }); // Không có sản phẩm trong phiếu
                                }
                                else 
                                {
                                    if (cart != null && cart.Items.Count > 0)
                                    {

                                        foreach (var item in cart.Items)
                                        {
                                            var checkQuantityPro = db.tb_ProductDetail.Find(item.ProductDetailId);
                                            var warehouseDetail = db.tb_WarehouseDetail.FirstOrDefault(w => w.ProductDetailId == item.ProductDetailId);
                                            if (warehouseDetail != null)
                                            {
                                                if (warehouseDetail.QuanTity >= item.SoLuong)
                                                {
                                                    warehouseDetail.QuanTity -= item.SoLuong;
                                                    db.Entry(checkQuantityPro).State = System.Data.Entity.EntityState.Modified;
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    var productName = db.tb_ProductDetail.FirstOrDefault(p => p.ProductDetailId == checkQuantityPro.ProductDetailId)?.tb_Products.Title;
                                                    return Json(new { success = false, code = -7, productName = productName }); // Kho không đủ số lượng
                                                }
                                            }
                                            else
                                            {
                                                var productName = db.tb_ProductDetail.FirstOrDefault(p => p.ProductDetailId == checkQuantityPro.ProductDetailId)?.tb_Products.Title;
                                                return Json(new { success = false, code = -7, productName = productName }); // Kho không đủ số lượng
                                            }
                                        }

                                        tb_Seller seller = new tb_Seller
                                        {
                                            CustomerId = customer.CustomerId,
                                            Phone = customer.PhoneNumber,
                                            StaffId = nvSession.StaffId,
                                            TypePayment = 0,
                                            CreatedDate = DateTime.Now,
                                            CreatedBy = nvSession.NameStaff,
                                            Code = "HD" + new Random().Next(0, 9999).ToString("D4")
                                        };

                                        cart.Items.ForEach(x => seller.tb_SellerDetail.Add(new tb_SellerDetail
                                        {
                                            ProductDetailId = x.ProductDetailId,
                                            Quantity = x.SoLuong,
                                            Price = x.Price
                                        }));

                                        seller.TotalAmount = cart.GetPriceTotal();

                                        db.tb_Seller.Add(seller);
                                        db.SaveChanges();

                                        customer.NumberofPurchases += 1;
                                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        cart.ClearCart();
                                        Session["SellerCart"] = cart; // Cập nhật lại session

                                        string invoicePath = ExportInvoice(seller.SellerId);
                                        if (!string.IsNullOrEmpty(invoicePath))
                                        {
                                            dbContextTransaction.Commit();
                                            return Json(new { success = true, code = 1, filePath = invoicePath });
                                        }
                                        else
                                        {
                                            return Json(new { success = false, code = -7, filePath = invoicePath });
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { success = false, code = -3 }); // Không có sản phẩm trong phiếu
                                    }
                                }
                              
                            }
                            else
                            {
                                return Json(new { success = false, code = -1 }); // Không tìm thấy khách hàng
                            }
                        }
                        else
                        {
                            return Json(new { success = false, code = -1 }); // Không tìm thấy khách hàng
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -100 });
                }
            }
        }



        //Strart In Bill

        public string ExportInvoice(int sellerId)
        {

            if (sellerId < 0) 
            {
                return null;
            }

            var seller = db.tb_Seller.Find(sellerId);
            if (seller != null)
            {
                var customer =db.tb_Customer.Find(seller.CustomerId);
                if (customer == null) {
                    return null;
                }
                var staff = db.tb_Staff.Find(seller.StaffId);
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
                    string logoPath = Server.MapPath("~/images/Logo/logoWEnMew.png");

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


        public ActionResult DownloadInvoice(string filePath)
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
        public ActionResult AddListProduct(int id, int soluong)
        {
            var code = new { Success = false, Code = -1, Count = 0 };
            try
            {
                var checkSanPham = db.tb_ProductDetail.FirstOrDefault(row => row.ProductDetailId == id);
                if (checkSanPham != null)
                {
                    SellerCart cart = (SellerCart)Session["SellerCart"];
                    if (cart == null)
                    {
                        cart = new SellerCart();
                    }

                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == id);
                    if (existingItem != null)
                    {
                        // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng lên
                        existingItem.SoLuong += soluong;
                        existingItem.PriceTotal = existingItem.SoLuong * existingItem.Price;
                    }
                    else
                    {
                        SellerCartItem item = new SellerCartItem
                        {
                            ProductDetailId = checkSanPham.ProductDetailId,
                            ProductName = checkSanPham.tb_Products.Title,
                            CategoryName = checkSanPham.tb_Products.tb_ProductCategory.Title,
                            Capcity = (int)checkSanPham.Capacity,
                            Color = checkSanPham.Color,
                            SoLuong = soluong, // Thay đổi ở đây
                            ProductsId = checkSanPham.tb_Products.ProductsId,
                            Alias = checkSanPham.tb_Products.Alias,
                        };

                        if (checkSanPham.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                        {
                            item.ProductImg = checkSanPham.tb_Products.tb_ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                        }

                        item.Price = (decimal)checkSanPham.Price;
                        item.PriceSale = (decimal)checkSanPham.PriceSale;
                        if (checkSanPham.PriceSale > 0)
                        {
                            item.PriceSale = (decimal)checkSanPham.PriceSale;
                            item.PriceTotal = item.SoLuong * item.PriceSale;
                        }
                        else
                        {
                            item.PriceTotal = item.SoLuong * item.Price;
                        }

                        cart.AddToCart(item, soluong);
                    }

                    Session["SellerCart"] = cart;
                    code = new { Success = true, Code = 1, Count = cart.Items.Count };
                }
                else
                {
                    code = new { Success = false, Code = -2, Count = 0 }; // Sản phẩm không tồn tại
                }
            }
            catch (Exception ex)
            {
                code = new { Success = false, Code = -99, Count = 0 }; // Lỗi xảy ra
            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult UpdateQuantityCartItem(int id, int quantity)
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null)
            {
                cart.UpSoLuong(id, quantity);
                Session["SellerCart"] = cart; // Cập nhật lại session
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        public ActionResult DeleteCartItem(int id)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null)
            {
                cart.Remove(id);
                Session["SellerCart"] = cart; // Cập nhật lại session
                code = new { Success = true, Code = 1, Url = "" };
            }
            return Json(code);
        }
        public ActionResult Partial_TotalPriceCheckOut()
        {

            SellerCart cart = (SellerCart)Session["SellerCart"];
            decimal totalPrice = 0;
            decimal save = 0;

            if (cart != null && cart.Items.Any())
            {
                foreach (var item in cart.Items)
                {
                    if (item.OriginalPriceTotal.HasValue)
                    {
                        save += (decimal)item.OriginalPriceTotal.Value - (decimal)item.PriceTotal;
                        ViewBag.Save = save;
                    }
                }



                totalPrice = cart.GetPriceTotal();
            }

            ViewBag.TotalPrice = totalPrice;

            return PartialView();
        }





        public ActionResult Partail_ListProduct()
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null && cart.Items.Any())
            {




                int count = cart.Items.Count;
                ViewBag.Count = count;
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        public ActionResult Partial_LeftIndex()
        {
            return PartialView();
        }










        //Cho dữ liệu ch có kahsc hhjafng
        public ActionResult AddClient()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {
                return PartialView();
            }
        }
        //public JsonResult AddClient() 
        //{
        //    using (var dbContextTransaction = db.Database.BeginTransaction()) 
        //    {
        //        try 
        //        {
        //            if (Session["user"] == null)
        //            {
        //                return Json(new { success = false, code = -99, redirectTo = Url.Action("DangNhap", "Account") });
        //            }
        //            else
        //            {
        //                return Json(new { success = true, code = 1 });
        //            }
        //        catch (Exception ex) 
        //        {
        //            dbContextTransaction.Rollback();
        //            return Json(new { success = false, code = -100 });

        //        }
        //    }
        //}

    }
}