using Antlr.Runtime.Tree;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;
using Microsoft.AspNet.SignalR;
namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat

        private IHubContext _hubContext;

        public ChatController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        }

        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int staffId)
        {
            // Lấy tất cả tin nhắn chưa được đọc bởi nhân viên này
            return View();
        }
        public ActionResult Mess(int id)
        {

            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id < 0)
            {
                return RedirectToAction("Login", "Account");
            }
            var customer =db.tb_Customer.Find(id);
            return View(customer);
           
        }


       


        public ActionResult Partial_ContentMess(int id)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (Session["CustomerId"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra xem id khách hàng có hợp lệ không
                if (id < 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Lấy tin nhắn của khách hàng
                var customerMessages = (
                    from cmd in db.tb_CustomerMessageDetail
                    join m in db.tb_Message on cmd.MessageId equals m.MessageId
                    join c in db.tb_Customer on cmd.CustomerId equals c.CustomerId
                    where cmd.CustomerId == id
                    select new ChatMessageViewModel
                    {
                        DetailId = cmd.DetailId,
                        MessageId = cmd.MessageId,
                        CustomerId = cmd.CustomerId,
                        StaffId = null,
                        MessageContent = cmd.Content,
                        Timestamp = cmd.Timestamp,
                        IsRead = cmd.IsRead,
                        CustomerName = c.CustomerName,
                        CustomerImage = c.Image
                    }
                ).ToList();

                // Lấy tin nhắn của nhân viên gửi cho khách hàng tương ứng
                var staffMessages = (
                    from smd in db.tb_StaffMessageDetail
                    join m in db.tb_Message on smd.MessageId equals m.MessageId
                    join s in db.tb_Staff on smd.StaffId equals s.StaffId
                    where db.tb_CustomerMessageDetail.Any(cmd => cmd.CustomerId == id && cmd.MessageId == smd.MessageId)
                    select new ChatMessageViewModel
                    {
                        DetailIdMessStaff = smd.DetailId,
                        MessageId = smd.MessageId,
                        CustomerId = null,
                        StaffId = smd.StaffId,
                        MessageContent = smd.Content,
                        Timestamp = smd.Timestamp, 
                        IsRead = smd.IsRead,
                        StaffName = s.NameStaff,
                        StaffImage = s.Image
                    }
                ).ToList();

                // Kết hợp và sắp xếp tất cả các tin nhắn theo thời gian gửi
                var allMessages = customerMessages.Concat(staffMessages).OrderBy(m => m.Timestamp);

                // Đánh dấu các tin nhắn chưa đọc là đã đọc
                foreach (var message in staffMessages.Where(m => m.IsRead == false ))
                {
                    message.IsRead = true;
                    var messageDetail = db.tb_StaffMessageDetail.Find(message.DetailIdMessStaff);
                    if (messageDetail != null)
                    {
                        messageDetail.IsRead = true;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return PartialView(allMessages.ToList());
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public JsonResult SentMess(int id, string content)
        {
            try
            {
                // Kiểm tra session và id hợp lệ
                if (Session["CustomerId"] == null || id < 0)
                {
                    return Json(new { success = false, code = -99 }); // Trả về lỗi
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Tìm kiếm tin nhắn chi tiết của khách hàng
                        var messdetail = db.tb_CustomerMessageDetail.FirstOrDefault(x => x.CustomerId == id);

                        // Nếu chưa có tin nhắn chi tiết, tạo mới
                        if (messdetail == null)
                        {
                            // Tạo mới tb_Message
                            var newMessage = new tb_Message
                            {
                                Content = content,
                                Timestamp = DateTime.Now
                            };
                            db.tb_Message.Add(newMessage);
                            db.SaveChanges();

                            // Tạo mới tb_CustomerMessageDetail và lưu vào cơ sở dữ liệu
                            var newMessageDetail = new tb_CustomerMessageDetail
                            {
                                CustomerId = id,
                                MessageId = newMessage.MessageId,
                                Content = content,
                                Timestamp = DateTime.Now,
                                
                            };

                            System.Diagnostics.Debug.WriteLine($"Before SaveChanges: IsRead = {newMessageDetail.IsRead}");

                            db.tb_CustomerMessageDetail.Add(newMessageDetail);
                            db.SaveChanges();

                            System.Diagnostics.Debug.WriteLine($"After SaveChanges: IsRead = {newMessageDetail.IsRead}");

                            // Thông báo cho client về tin nhắn mới sử dụng SignalR
                            _hubContext.Clients.All.broadcastMessage(id, content);

                            transaction.Commit();

                            return Json(new { success = true, code = 1 }); // Trả về thành công
                        }
                        else
                        {
                            // Nếu đã có tin nhắn chi tiết, tạo mới chi tiết tin nhắn và lưu vào cơ sở dữ liệu
                            var newMessageDetail = new tb_CustomerMessageDetail
                            {
                                CustomerId = id,
                                MessageId = messdetail.MessageId,
                                Content = content,
                                Timestamp = DateTime.Now,
                             
                            };

                            System.Diagnostics.Debug.WriteLine($"Before SaveChanges: IsRead = {newMessageDetail.IsRead}");

                            db.tb_CustomerMessageDetail.Add(newMessageDetail);
                            db.SaveChanges();

                            System.Diagnostics.Debug.WriteLine($"After SaveChanges: IsRead = {newMessageDetail.IsRead}");

                            transaction.Commit();

                            return Json(new { success = true, code = 2 }); // Trả về thành công với mã code 2
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine($"Transaction rolled back due to: {e.Message}");
                        return Json(new { success = false, code = -99 });
                    }
                }
            }
            catch (Exception e)
            {
                // Xử lý ngoại lệ và trả về mã lỗi
                return Json(new { success = false, code = -99 });
            }
        }




        public ActionResult ShowCountMess()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];

                // Lấy danh sách các MessageId của tin nhắn chưa đọc cho khách hàng
                var unreadMessageIds = db.tb_CustomerMessageDetail
                    .Where(x => x.CustomerId == idKhach)
                    .Select(x => x.MessageId)
                    .Distinct()
                    .ToList();

                // Nếu không có tin nhắn nào chưa đọc, trả về 0
                if (!unreadMessageIds.Any())
                {
                    return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
                }

                // Sử dụng HashSet để kiểm tra hiệu suất tốt hơn
                var unreadMessageIdSet = new HashSet<int>(unreadMessageIds);

                // Đếm số lượng tin nhắn chưa đọc từ bảng tb_StaffMessageDetail
                var unreadCount = db.tb_StaffMessageDetail
                    .Count(x => unreadMessageIdSet.Contains(x.MessageId) && x.IsRead == false);

                // Trả về số lượng tin nhắn chưa đọc dưới dạng JSON
                return Json(new { Count = unreadCount }, JsonRequestBehavior.AllowGet);
            }

            // Trả về số lượng tin nhắn là 0 nếu không có CustomerId trong Session
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }



    }
}