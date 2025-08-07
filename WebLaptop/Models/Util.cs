using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebLaptop.Models
{
    public class Util
    {
        public static decimal GiaGiam(decimal gia, string pt)
        {
            decimal ptDecimal = 0;

            if (!decimal.TryParse(pt, out ptDecimal))
            {
                return gia;
            }
            decimal giamoi = gia * ptDecimal / 100;
            return giamoi;
        }
    }
}   