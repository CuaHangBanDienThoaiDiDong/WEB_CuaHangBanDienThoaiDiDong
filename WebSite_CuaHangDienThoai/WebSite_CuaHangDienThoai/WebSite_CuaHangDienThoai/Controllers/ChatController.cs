using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int staffId)
        {
            // Lấy tất cả tin nhắn chưa được đọc bởi nhân viên này
            var messages = db.tb_Message
                             .Where(m => m.ReceiverId == 0 || m.ReceiverId == staffId)
                             .OrderBy(m => m.Timestamp)
                             .ToList();
            ViewBag.StaffId = staffId;
            return View(messages);
        }
    }
}