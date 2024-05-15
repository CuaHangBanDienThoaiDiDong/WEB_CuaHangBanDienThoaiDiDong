using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; }
        public ShoppingCart()
        {

            Items = new List<ShoppingCartItem>();
        }

        public void AddToCart(ShoppingCartItem item, int SoLuong)
        {
            var checkSanPham = Items.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (checkSanPham != null)
            {
                checkSanPham.SoLuong = SoLuong;
                checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.SoLuong;
            }
            else
            {
                Items.Add(item);
            }
        }
        public decimal GetPriceTotal()
        {
            return Items.Sum(x => x.PriceTotal);
        }
        public int GetTongSoLuong()
        {
            return Items.Sum(x => x.SoLuong);
        }

        public void ClearCart()
        {
            Items.Clear();
        }


        ///Xoa San Pham Ra khoi gio hang
        ///
        public void Remove(int id)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkSanPham != null)
            {
                Items.Remove(checkSanPham);
            }
        }

        //Cap Nhap San Pham Ra khoi gio hang
        public void UpSoLuong(int id, int SoLuong)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductId == id);
            if (checkSanPham != null)
            {
                checkSanPham.SoLuong = SoLuong;
                checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.SoLuong;
            }
        }
    }


    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string CategoryName { get; set; }
        public string ProductImg { get; set; }
        public int SoLuong { get; set; }
        public decimal Price { get; set; }
        public decimal PriceTotal { get; set; }

        public string Color { get; set; }
        public int Capcity { get; set; }

    }
}