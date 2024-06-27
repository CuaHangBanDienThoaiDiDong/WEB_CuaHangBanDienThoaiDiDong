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
                        DetailId = smd.DetailId,
                        MessageId = smd.MessageId,
                        CustomerId = null,
                        StaffId = smd.StaffId,
                        MessageContent = smd.Content,
                        Timestamp = smd.Timestamp, // Lấy thời gian từ bảng tb_StaffMessageDetail
                        IsRead = smd.IsRead,
                        StaffName = s.NameStaff,
                        StaffImage = s.Image
                    }
                ).ToList();

                // Kết hợp và sắp xếp tất cả các tin nhắn theo thời gian gửi
                var allMessages = customerMessages.Concat(staffMessages).OrderBy(m => m.Timestamp);

                // Đánh dấu các tin nhắn chưa đọc là đã đọc
                foreach (var message in allMessages.Where(m => m.IsRead == false && m.CustomerId == id))
                {
                    message.IsRead = true;
                    var messageDetail = db.tb_CustomerMessageDetail.Find(message.DetailId);
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
                if (Session["CustomerId"] == null || id < 0)
                {
                    // Nếu không có session hoặc id nhỏ hơn 0, trả về mã lỗi
                    return Json(new { success = false, code = -99 });
                }

                var messdetail = db.tb_CustomerMessageDetail.FirstOrDefault(x => x.CustomerId == id);
                if (messdetail == null)
                {
                    var newMessage = new tb_Message
                    {
                        Content = content,
                        Timestamp = DateTime.Now
                    };
                    db.tb_Message.Add(newMessage);
                    db.SaveChanges();

                    var newMessageDetail = new tb_CustomerMessageDetail
                    {
                        CustomerId = id,
                        MessageId = newMessage.MessageId,
                        Content = content,
                        Timestamp = DateTime.Now,
                        IsRead = false
                    };
                    db.tb_CustomerMessageDetail.Add(newMessageDetail);
                    db.SaveChanges();

                    // Gọi phương thức SendMessage của SignalR Hub để thông báo cho client về tin nhắn mới
                    _hubContext.Clients.All.broadcastMessage(id, content);

                    return Json(new { success = true, code = 1 });
                }
                else
                {
                    tb_CustomerMessageDetail messDetailNew = new tb_CustomerMessageDetail
                    {
                        CustomerId = id,
                        MessageId = messdetail.MessageId,
                        Content = content,
                        Timestamp = DateTime.Now,
                        IsRead = false
                    };
                    db.tb_CustomerMessageDetail.Add(messDetailNew);
                    db.SaveChanges();


                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.All.broadcastMessage(id, content);

                    return Json(new { success = true, code = 2 }); // Code 2 để phân biệt tin nhắn đã được cập nhật
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, code = -99 });
            }
        }

     

        public ActionResult ShowCountMess()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];

                var unreadMessages = db.tb_CustomerMessageDetail.Count(x => x.CustomerId == idKhach && x.IsRead == false);

                return Json(new { Count = unreadMessages }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }


    }
}