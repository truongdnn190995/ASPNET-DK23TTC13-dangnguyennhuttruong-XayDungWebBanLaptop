using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLaptop.Models
{
    public class GioHang
    {
        public SanPham sp { get; set; }
        public int Soluong { get; set; }
        public GioHang()
        {

        }
        public GioHang(SanPham spm, int sl)
        {
            this.sp = spm;
            this.Soluong = sl;
        }
    }
}