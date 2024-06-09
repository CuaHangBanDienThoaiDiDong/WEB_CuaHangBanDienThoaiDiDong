using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace WebSite_CuaHangDienThoai.Models
{
    public class CustomSymmetricSecurityKey : SymmetricSecurityKey
    {
        public CustomSymmetricSecurityKey(byte[] key) : base(key)
        {
        }

        public override string KeyId { get; set; }
    }
}