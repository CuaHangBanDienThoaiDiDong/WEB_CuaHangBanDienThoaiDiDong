using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ProvincesVNController : Controller
    {
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        // GET: ProvincesVN
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchProvinces(string key)
        {
            
        }



        public ActionResult Partial_Provinces()
        {
            var item = db.Provinces.ToList();
            ViewBag.Provinces = item;
            return PartialView(item);
        }


        public ActionResult Partial_Districts(int id) 
        {
            var checkDistricts = db.Districts.Where(r => r.idProvinces == id).ToList();
            if (checkDistricts != null)
            {
                ViewBag.Districts = checkDistricts; 
                return PartialView(checkDistricts);
            }
            else 
            {
                return PartialView();   
            }
           
        }

        public ActionResult Partial_Wards(int id)
        {
            var checkWards = db.Wards.Where(r => r.idDistricts == id).ToList();
            if (checkWards != null)
            {
                ViewBag.Districts = checkWards;
                return PartialView(checkWards);
            }
            else
            {
                return PartialView();
            }

        }

    }
}