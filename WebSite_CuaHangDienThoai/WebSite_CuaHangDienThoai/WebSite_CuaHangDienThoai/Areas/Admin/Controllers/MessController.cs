using System;
using System.Collections.Generic;
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

        public ActionResult Partial_MessIndex()
        {
            return PartialView();
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