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

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ExportWareHouseController : Controller
    {
        // GET: Admin/ExportWareHouse
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
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
        public ActionResult StaffIndex()
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


        public ActionResult Partial_ExportWareHouseIndex()
        {
            return PartialView();
            
        }




        public ActionResult Partial_ThongTinDonHang(string code)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {

                var checkOder = db.tb_Order.FirstOrDefault(x => x.Code.Contains(code.Trim()));

                try
                {
                    if (checkOder != null)
                    {
                        var checkOrderDetail = db.tb_OrderDetail.Where(x => x.OrderId == checkOder.OrderId).Count();
                        var ExportWareHouse = db.tb_ExportWareHouse.FirstOrDefault(x => x.OrderId == checkOder.OrderId);
                        ViewBag.OrderId = checkOder.OrderId;
                        ViewBag.Count = checkOrderDetail;
                        if (ExportWareHouse != null)
                        {
                            ViewBag.ExportWareHouse = ExportWareHouse.CreatedDate;
                        }
                        return PartialView(checkOder);
                    }
                    return PartialView();
                }
                catch (Exception ex) { return PartialView(); }

            }


        }



        public ActionResult Partial_ThongTinXuat()
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

        //[HttpPost]

        //public JsonResult Partial_ExportWareHouse(int Orderid)
        //{
        //    using (var dbContextTransaction = db.Database.BeginTransaction())
        //    {

        //        var code = new { Success = false, Code = -1, Url = "" };
        //        try
        //        {
        //            var checkOrder = db.tb_Order.Find(Orderid);
        //            if (checkOrder != null)
        //            {
        //                var checkCancelOrder = db.tb_Order.FirstOrDefault(x => x.OrderId == checkOrder.OrderId && x.typeOrder == false);
        //                if (checkCancelOrder != null)
        //                {
        //                    var checkConfim = db.tb_Order.FirstOrDefault(x => x.OrderId == checkCancelOrder.OrderId && x.Confirm == true);
        //                    if (checkConfim != null)
        //                    {
        //                        var OrderReturn = db.tb_Order.FirstOrDefault(x => x.OrderId == checkConfim.OrderId && x.typeReturn == false);
        //                        if (OrderReturn != null)
        //                        {
        //                            var checkTBOut = db.tb_ExportWareHouse.FirstOrDefault(x => x.OrderId == OrderReturn.OrderId);
        //                            if (checkTBOut == null)
        //                            {
        //                                tb_Staff nvSession = (tb_Staff)Session["user"];
        //                                var staff = db.tb_Staff.SingleOrDefault(row => row.StaffId == nvSession.StaffId);
        //                                var checkWareHouse = db.tb_Warehouse.FirstOrDefault(r => r.StoreId == staff.tb_Store.StoreId);
        //                                tb_ExportWareHouse model = new tb_ExportWareHouse();
        //                                model.CreatedDate = DateTime.Now;
        //                                model.StaffId = staff.StaffId;
        //                                model.OrderId = checkOrder.OrderId;
        //                                model.WarehouseId = checkWareHouse.WarehouseId;
        //                                db.tb_ExportWareHouse.Add(model);
        //                                db.SaveChanges();
        //                                dbContextTransaction.Commit();
        //                                ExportInvoice(checkOrder.OrderId);
        //                                code = new { Success = true, Code = 1, Url = "" };
        //                            }
        //                            else
        //                            {

        //                                code = new { Success = false, Code = -6, Url = "" };
        //                            }
        //                        }
        //                        else
        //                        {

        //                            code = new { Success = false, Code = -5, Url = "" };
        //                        }


        //                    }
        //                    else
        //                    {
        //                        code = new { Success = false, Code = -4, Url = "" };
        //                    }

        //                }
        //                else
        //                {
        //                    //Don Hang da bi huy
        //                    code = new { Success = false, Code = -3, Url = "" };

        //                }
        //            }
        //            else
        //            {
        //                //Khong thay ma Order
        //                code = new { Success = false, Code = -2, Url = "" };
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            dbContextTransaction.Rollback(); code = new { Success = false, Code = -100, Url = "" };

        //        }
        //        return Json(code);
        //    }


        //}

        [HttpPost]
        public JsonResult Partial_ExportWareHouse(int Orderid)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
               
                try
                {
                    var checkOrder = db.tb_Order.Find(Orderid);
                    if (checkOrder != null)
                    {
                        var checkCancelOrder = db.tb_Order.FirstOrDefault(x => x.OrderId == checkOrder.OrderId && x.typeOrder == false);
                        if (checkCancelOrder != null)
                        {
                            var checkConfim = db.tb_Order.FirstOrDefault(x => x.OrderId == checkCancelOrder.OrderId && x.Confirm == true);
                            if (checkConfim != null)
                            {
                                var OrderReturn = db.tb_Order.FirstOrDefault(x => x.OrderId == checkConfim.OrderId && x.typeReturn == false);
                                if (OrderReturn != null)
                                {
                                    var checkTBOut = db.tb_ExportWareHouse.FirstOrDefault(x => x.OrderId == OrderReturn.OrderId);
                                    if (checkTBOut == null)
                                    {
                                        tb_Staff nvSession = (tb_Staff)Session["user"];
                                        var staff = db.tb_Staff.SingleOrDefault(row => row.StaffId == nvSession.StaffId);
                                        var checkWareHouse = db.tb_Warehouse.FirstOrDefault(r => r.StoreId == staff.tb_Store.StoreId);
                                        tb_ExportWareHouse model = new tb_ExportWareHouse();
                                        model.CreatedDate = DateTime.Now;
                                        model.StaffId = staff.StaffId;
                                        model.OrderId = checkOrder.OrderId;
                                        model.WarehouseId = checkWareHouse.WarehouseId;
                                        db.tb_ExportWareHouse.Add(model);
                                        db.SaveChanges();
                                       
                                        string invoicePath = ExportInvoice(Orderid);
                                        if (!string.IsNullOrEmpty(invoicePath))
                                        {
                                            dbContextTransaction.Commit();
                                            return Json(new { Success = true, Code = 1, FilePath = invoicePath }) ;
                                        }
                                        else
                                        {
                                            return Json(new { Success = false, Code = -7, FilePath = invoicePath });
                                         
                                        }
                                        // Trả về mã JavaScript và đường dẫn hóa đơn
        

                                    }
                                    else
                                    {
                         
                                        return Json(new { Success = false, Code = -6, Url = "" });
                                    }
                                }
                                else
                                {
                                    return Json(new { Success = false, Code = -5, Url = "" });
                                   
                                }
                            }
                            else
                            {
                                return Json(new { Success = false, Code = -4, Url = "" });
                                
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, Code = -3, Url = "" });
                        }
                    }
                    else
                    {
                        
                        return Json(new { Success = false, Code = -2, Url = "" });
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                  
                    return Json(new { Success = false, Code = -100, Url = "" });
                }
                
            }
        }
        // Action để tải hóa đơn

        ////Start Xuat Word
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





        public string ExportInvoice(int orderId)
        {
            if (orderId < 0) 
            {
                return null;
            }
            var order = db.tb_Order.Find(orderId);
            if (order != null)
            {
                try
                {
                    string templatePath = Server.MapPath("~/Content/templates/DonHang.html");
                    string htmlContent = System.IO.File.ReadAllText(templatePath);

                    // Thay thế các biến trong template HTML bằng giá trị thực tế
                    htmlContent = htmlContent.Replace("#{{Code}}", order.Code);
                    htmlContent = htmlContent.Replace("#{{CreatedDate}}", order.CreatedDate.ToString("dd/MM/yyyy"));
                    htmlContent = htmlContent.Replace("#{{CustomerName}}", order.CustomerName);
                    htmlContent = htmlContent.Replace("#{{Address}}", order.Address);

                    // Lấy chi tiết đơn hàng
                    var orderDetails = db.tb_OrderDetail
                        .Where(od => od.OrderId == order.OrderId)
                        .ToList();

                    string productRows = "";
                    int index = 1;

                    foreach (var detail in orderDetails)
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
                    decimal total = order.TotalAmount;
                    htmlContent = htmlContent.Replace("#{{totalOrder}}", WebSite_CuaHangDienThoai.Common.Common.FormatNumber(order.TotalAmount) + " đ");

                    string fileName = $"LTD_{order.Code}.docx";
                    string folderPath = Server.MapPath("~/Order");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);

                    // Chuyển đổi HTML sang Word
                    string logoPath = Server.MapPath("~/images/Logo/logoWEnMew.png");

                    ConvertHTMLToWord(htmlContent, filePath, logoPath, order.Code);

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



        //endXuat Word


        public ActionResult Partial_OrderDetail(int id)
        {
            if (id > 0)
            {
                var item = db.tb_OrderDetail.Where(x => x.OrderId == id).ToList();
                if (item != null)
                {
                    ViewBag.OrderId = id;
                    ViewBag.Count = item.Count;
                    return PartialView(item);
                }
            }

            return PartialView();
        }
        
    }
}