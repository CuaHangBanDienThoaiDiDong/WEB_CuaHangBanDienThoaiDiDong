using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ProductCompanyController : Controller
    {
        // GET: ProductCompany
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities  (); 
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Partail_CompanyForCategory(int id)
        {
           

            var companies = (from p in db.tb_Products
                             join pc in db.tb_ProductCompany on p.ProductCompanyId equals pc.ProductCompanyId
                             where p.ProductCategoryId == id
                             select new CompanyModel
                             {
                                 Image=pc.Icon,
                                 ProductCompanyId = pc.ProductCompanyId,
                                 Title = pc.Title
                             }).Distinct().ToList();

            if (companies.Any())
            {
                return PartialView(companies);
            }

            return PartialView();


        }

    }
}