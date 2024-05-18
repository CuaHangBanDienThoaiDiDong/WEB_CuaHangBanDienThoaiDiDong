using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Areas.Admin.Controllers
{
    public class ProvincesVNController : Controller
    {
        // GET: Admin/ProvincesVN
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Partial_Provinces(int id) 
        {
            var item =db.Provinces.Find(id);
            return View(item);  
        }

        [HttpGet]
        public JsonResult GetDistrictsByProvince(int idProvinces)
        {
            var districts = db.Districts
                .Where(d => d.idProvinces == idProvinces)
                .Select(d => new
                {
                    d.idDistricts,
                    d.name
                })
                .ToList();
            return Json(districts, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetWardsByDistrict(int idDistricts)
        {
            var wards = db.Wards
                .Where(w => w.idDistricts == idDistricts)
                .Select(w => new
                {
                    w.idWards,
                    w.name
                })
                .ToList();
            return Json(wards, JsonRequestBehavior.AllowGet);
        }
    }
}