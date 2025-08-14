using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;



namespace WebLaptop.Controllers
{
    public class SinglePageController : Controller
    {
        WebLaptopEntities db = new WebLaptopEntities();
        // GET: SinglePage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SingleProduct(int ? ma)
        {

            ViewBag.sanpham = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault();
            ViewBag.LoaiSP = db.LoaiSPs.ToList();
            var mal = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().MaLoai;
            ViewBag.SPCL = db.SanPhams
        .Where(x => x.MaLoai == mal)
        .GroupBy(x => x.Ten)
        .Select(g => g.FirstOrDefault())
        .ToList();

            ViewBag.BESTSELL = db.SanPhams.OrderByDescending(x => x.Ma).Take(5).ToList();
            return View();
        }
    }
}