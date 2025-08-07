using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLaptop.Models
{
    public class wishlist
    {
        public SanPham sp { get; set; }
        public int Soluong { get; set; }
        public wishlist()
        {

        }
        public wishlist(SanPham spm, int sl)
        {
            this.sp = spm;
            this.Soluong = sl;
        }
    }
}