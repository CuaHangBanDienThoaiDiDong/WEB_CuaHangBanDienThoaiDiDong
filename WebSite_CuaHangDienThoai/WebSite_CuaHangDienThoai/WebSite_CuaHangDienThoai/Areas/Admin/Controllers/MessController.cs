using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json.Linq;
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
    public class MessController : Controller
    {
        // GET: Admin/Mess
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
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    var items = db.tb_Message.OrderByDescending(x => x.MessageId).ToList();
                    return View(items);
                }


            }

        }

        public ActionResult StaffMess()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }
            else
            {

                tb_Staff nvSession = (tb_Staff)Session["user"];
                var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 4));
                if (item == null)
                {
                    return RedirectToAction("NonRole", "HomePage");
                }
                else
                {
                    var items = db.tb_Message.OrderByDescending(x => x.MessageId).ToList();
                    return View(items);
                }


            }

        }
        public ActionResult Paritial_Left()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            tb_Staff nvSession = (tb_Staff)Session["user"];
            var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2 || row.FunctionId == 4));

            if (item == null)
            {
                return RedirectToAction("NonRole", "HomePage");
            }

            var items = db.tb_Message
                            .Select(m => new
                            {
                                Message = m,
                                LatestCustomerMessage = m.tb_CustomerMessageDetail
                                                        .OrderByDescending(d => d.Timestamp)
                                                        .FirstOrDefault(),
                                IsAnyUnread = m.tb_CustomerMessageDetail
                                                .Any(d => !d.IsRead)
                            })
                            .OrderByDescending(m => m.IsAnyUnread) // Đưa tin nhắn chưa đọc lên đầu
                            .ThenByDescending(m => m.LatestCustomerMessage != null ? m.LatestCustomerMessage.Timestamp : DateTime.MinValue) // Sắp xếp theo thời gian giảm dần
                            .ToList()
                            .Select(m => m.Message); // Chọn tin nhắn gốc

            return PartialView(items);
        }



        public ActionResult Partial_MessIndex()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult IsRead(int messageId)
        {
            try
            {
                if (messageId < 0)
                {
                    return Json(new { Success = false, code = -1 });
                }

                var Mess = db.tb_Message.FirstOrDefault(x => x.MessageId == messageId);
                if (Mess == null)
                {
                    return Json(new { Success = false, code = -2 });
                }

                var customerMessageDetails = db.tb_CustomerMessageDetail.Where(x => x.MessageId == messageId && !x.IsRead).ToList();

                if (customerMessageDetails.Count == 0)
                {
                    return Json(new { Success = false, code = -3, });
                }

                foreach (var customerMessageDetail in customerMessageDetails)
                {
                    customerMessageDetail.IsRead = true;
                    db.Entry(customerMessageDetail).State = EntityState.Modified;
                }

                db.SaveChanges();
                return Json(new { Success = true, code = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, code = -99, message = ex.Message });
            }
        }



        public ActionResult GetChatMessages(int messageId)
        {
            try
            {
              


                var staffMessages = db.tb_StaffMessageDetail
                    .Where(smd => smd.MessageId == messageId)
                    .OrderBy(smd => smd.Timestamp)
                    .Select(smd => new Admin_ChatMessageViewModel
                    {
                        DetailId = smd.DetailId,
                        MessageId = smd.MessageId,
                        StaffId = smd.StaffId, 
                        MessageContent = smd.Content,
                        Timestamp = smd.Timestamp,
                        IsRead= smd.IsRead, 
                        Staff=db.tb_Staff.FirstOrDefault(x=>x.StaffId== smd.StaffId)
                    })
                    .ToList();

                // Lấy tin nhắn từ bảng tb_CustomerMessageDetail
                var customerMessages = db.tb_CustomerMessageDetail
                    .Where(cmd => cmd.MessageId == messageId)
                    .OrderBy(cmd => cmd.Timestamp)
                    .Select(cmd => new Admin_ChatMessageViewModel
                    {
                        DetailId = cmd.DetailId,
                        MessageId = cmd.MessageId,
                        CustomerId = cmd.CustomerId,
                        MessageContent = cmd.Content,
                        Timestamp = cmd.Timestamp,
                        IsRead = cmd.IsRead,
Customer=db.tb_Customer.FirstOrDefault(x=>x.CustomerId == cmd.CustomerId)

                    })
                    .ToList();

                // Kết hợp và sắp xếp tất cả các tin nhắn theo thời gian gửi
                var allMessages = staffMessages.Concat(customerMessages)
                    .OrderBy(m => m.Timestamp);
                if (allMessages != null)
                {

                    ViewBag.Messid = messageId;
                    return PartialView(allMessages.ToList());
                }
                else 
                {
                    return PartialView(null);
                }

                
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return PartialView(null);
            }
        }













        // Action to send a message
        [HttpPost]
        public JsonResult SendMessage(int MessageId, string Content)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction()) 
            {
                try
                {
                    // Get current time for timestamp
                    var timestamp = DateTime.Now;

                    // Get the current logged in staff's ID from session
                    var currentStaffId = (Session["user"] as tb_Staff)?.StaffId;
                    if (currentStaffId == null) 
                    {
                        return Json(new { success = false, Code = -1 });
                    }
                    // Create a new StaffMessageDetail object
                    var newMessageDetail = new tb_StaffMessageDetail
                    {
                        MessageId = MessageId,
                        StaffId = (int)currentStaffId,
                        Content = Content.Trim(),
                        Timestamp = timestamp,
                        IsRead = false
                    };

                    // Add the new message detail to the database
                    db.tb_StaffMessageDetail.Add(newMessageDetail);
                    db.SaveChanges();

                    dbContextTransaction.Commit();  
                    return Json(new { success = true, Code = 1 });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { success = false,Code =-100 });
                }
            }
                
        }

        public ActionResult Partail_AllMess() 
        {

            tb_Staff nvSession = (tb_Staff)Session["user"];
            var item = db.tb_Role.SingleOrDefault(row => row.StaffId == nvSession.StaffId && (row.FunctionId == 1 || row.FunctionId == 2));
            if (item == null)
            {
                return RedirectToAction("NonRole", "HomePage");
            }
            else
            {
                var messages = db.tb_Message.ToList();
                return PartialView(messages); ;
            }
        }
    }
}