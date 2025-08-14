using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;
using PagedList;

namespace WebLaptop.Controllers
{
    public class LoaiSPController : Controller
    {
        // GET: LoaiSP
        WebLaptopEntities db = new WebLaptopEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult XemLoaiSP(int? ma, int? page)
        {
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            var LoaiSP = db.SanPhams.Where(x => x.MaLoai == ma).OrderByDescending(s => s.Ma).ToPagedList(pageNumber, pageSize);
            if (LoaiSP.Count() >  0)
            {

                LoaiSP = db.SanPhams.Where(x => x.MaLoai == ma).OrderByDescending(s => s.Ma).ToPagedList(pageNumber, pageSize);
                ViewBag.Ma = ma;
                ViewBag.TheLoai = db.SanPhams.Where(x => x.MaLoai == ma).FirstOrDefault().LoaiSP.Ten;
                ViewBag.count = db.SanPhams.Where(x => x.MaLoai == ma).ToList().Count();
            }
            else
            {
                LoaiSP = db.SanPhams.OrderByDescending(s => s.Ma).ToPagedList(pageNumber, pageSize);
            }
            ViewBag.LoaiSP = db.LoaiSPs.ToList();
            return View(LoaiSP);
        }
    }
}