using System.Web.Mvc;

namespace WebSite_CuaHangDienThoai.Areas.WareHouse
{
    public class WareHouseAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WareHouse";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WareHouse_default",
                "WareHouse/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}