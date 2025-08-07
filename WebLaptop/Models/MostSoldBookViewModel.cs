using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLaptop.Models
{
    public class MostSoldBookViewModel
    {
        public int Ma { get; set; }
        public string Ten { get; set; }
        public string Anh { get; set; }
        public int SoldQuantity { get; set; }
        public decimal? Gia { get; set; }
    }

}
