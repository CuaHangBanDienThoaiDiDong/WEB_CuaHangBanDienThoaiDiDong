using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class ImportWareHouse
    {
        public List<ImportWareHouseItem> Items { get; set; }



        public ImportWareHouse()
        {
            Items = new List<ImportWareHouseItem>();
        }

        public void AddToCart(ImportWareHouseItem item, int SoLuong)
        {
            var checkSanPham = Items.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId);

            if (checkSanPham != null)
            {
                checkSanPham.Quantity = 0;
              
            }
            else
            {
                Items.Add(item);
            }
        }
        public void Remove(int id)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkSanPham != null)
            {
                Items.Remove(checkSanPham);
            }
        }

        public void UpQuantity(int id, int SoLuong)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkSanPham != null)
            {
                checkSanPham.Quantity = SoLuong;
                //checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.SoLuong;
            }
        }
        public decimal GetPriceTotal()
        {
            return Items.Sum(x => x.PriceTotal);
        }
        public int GetTongSoLuong()
        {
            return Items.Sum(x => x.Quantity);
        }
        public void ClearImport()
        {
            Items.Clear();
        }


    }

    public class ImportWareHouseItem
    {
        public int ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public byte[] ProductDetailImg { get; set; }
        public string Alias { get; set; }

        public string Category { get; set; }
        public string Company { get; set; }
        public string Color { get; set; }
        public int  Capacity { get; set; }
      
        public int SupplierId { get; set; }

        public decimal Price { get; set; }
        public decimal PriceTotal { get; set; }

        public int Quantity { get; set; }
     

    }
}