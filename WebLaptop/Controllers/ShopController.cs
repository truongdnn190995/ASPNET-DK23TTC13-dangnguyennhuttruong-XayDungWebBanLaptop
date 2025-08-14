using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;
using PagedList;
namespace WebLaptop.Controllers
{
    public class ShopController : Controller
    {
        WebLaptopEntities db = new WebLaptopEntities();
        // GET: Shop
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Shop( int? page , string tensp , decimal? gia)
        {
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            var  LoaiSP = db.SanPhams.OrderByDescending(s => s.Ma).ToPagedList(pageNumber, pageSize);

            if(tensp != null)
            {
                LoaiSP = db.SanPhams.Where(x=>x.Ten.Contains(tensp)).OrderByDescending(s => s.Ma).ToPagedList(pageNumber, pageSize);
            }


            if (gia > 0)
            {
                LoaiSP = db.SanPhams.Where(x => x.Gia <= gia).OrderByDescending(s => s.Ma).ToPagedList(pageNumber, pageSize);
            }

            ViewBag.LoaiSP = db.LoaiSPs.ToList();
            return View(LoaiSP);
        }
    }
}