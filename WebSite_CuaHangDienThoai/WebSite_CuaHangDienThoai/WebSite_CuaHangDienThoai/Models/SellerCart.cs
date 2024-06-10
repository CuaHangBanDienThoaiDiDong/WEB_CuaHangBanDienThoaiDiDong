using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite_CuaHangDienThoai.Models
{
    public class SellerCart
    {

        public List<SellerCartItem> Items { get; set; }
        public SellerCart()
        {

            Items = new List<SellerCartItem>();
        }

        public void AddToCart(SellerCartItem item, int SoLuong)
        {
            var checkSanPham = Items.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId);

            if (checkSanPham != null)
            {
                checkSanPham.SoLuong = SoLuong;
                if (checkSanPham.PriceSale > 0)
                {
                    checkSanPham.PriceTotal = checkSanPham.PriceSale * checkSanPham.SoLuong;
                }
                else
                {
                    checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.SoLuong;
                }

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
            var checkSanPham = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkSanPham != null)
            {
                Items.Remove(checkSanPham);
            }
        }

        //Cap Nhap San Pham Ra khoi gio hang
        public void UpSoLuong(int id, int SoLuong)
        {
            var checkSanPham = Items.SingleOrDefault(x => x.ProductDetailId == id);
            if (checkSanPham != null)
            {
                checkSanPham.SoLuong = SoLuong;
                if (checkSanPham.PriceSale > 0)
                {

                    checkSanPham.PriceTotal = checkSanPham.PriceSale * checkSanPham.SoLuong;
                }
                else
                {
                    checkSanPham.PriceTotal = checkSanPham.Price * checkSanPham.SoLuong;
                }
            }
        }


    }

    public class SellerCartItem
    {
        public int ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string CategoryName { get; set; }
        public byte[] ProductImg { get; set; }
        public int SoLuong { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public decimal PriceTotal { get; set; }
        public decimal? OriginalPriceTotal { get; set; }

        public int? PercentPriceReduction { get; set; }
        public string Code { get; set; }

        public string Color { get; set; }
        public int Capcity { get; set; }

    }
}