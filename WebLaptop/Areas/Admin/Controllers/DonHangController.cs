using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {

        WebLaptopEntities db = new WebLaptopEntities();
        // GET: Admin/DonHang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachDonHang() {

            var donhang = db.DonHangs.OrderByDescending(x=>x.NgayDatHang).ToList();
            return View(donhang);
        }
        public ActionResult XemChiTiet(int ? ma)
        {

            var ctd = db.ChiTietDonHangs.Where(x => x.MaDH == ma).OrderByDescending(x => x.DonGia).ToList();
            ViewBag.tenkh = db.DonHangs.Where(x => x.Ma == ma).FirstOrDefault().KhachHang.Ten;
            ViewBag.diachi = db.DonHangs.Where(x => x.Ma == ma).FirstOrDefault().KhachHang.DiaChi;
            ViewBag.sdt = db.DonHangs.Where(x => x.Ma == ma).FirstOrDefault().KhachHang.DienThoai;
            ViewBag.email = db.DonHangs.Where(x => x.Ma == ma).FirstOrDefault().KhachHang.Email;
            ViewBag.ngaymua = db.DonHangs.Where(x => x.Ma == ma).FirstOrDefault().NgayDatHang;
            ViewBag.TrangThai = db.DonHangs.Where(x => x.Ma == ma).FirstOrDefault().TrangThai;
            return View(ctd);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            DonHang sanPham = db.DonHangs.Find(id);
            db.DonHangs.Remove(sanPham);
            db.SaveChanges();
            return RedirectToAction("DanhSachDonHang");
        }
        public ActionResult xoaChtdh(int ? ma , int ? masp)
        {
            var ctd = db.ChiTietDonHangs.Where(x => x.MaDH == ma && x.MaSP == masp).FirstOrDefault();
            db.ChiTietDonHangs.Remove(ctd);
            db.SaveChanges();
            return RedirectToAction("XemChiTiet" , new { ma = ma });
        }
    }
}