using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebLaptop.Models;

namespace WebLaptop.Models
{
    public class SanPhamComparer : IEqualityComparer<SanPham>
    {
        public bool Equals(SanPham x, SanPham y)
        {
            if (x == null || y == null) return false;

            return x.Ma == y.Ma &&
                   x.Ten == y.Ten &&
                   x.ThuongHieu == y.ThuongHieu &&
                   x.NamSX == y.NamSX;
        }

        public int GetHashCode(SanPham obj)
        {
            unchecked 
            {
                int hash = 17;
                hash = hash * 23 + obj.Ma.GetHashCode();
                hash = hash * 23 + (obj.Ten?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.ThuongHieu?.GetHashCode() ?? 0);
                hash = hash * 23 + obj.NamSX.GetHashCode();
                return hash;
            }
        }
    }

}