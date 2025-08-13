using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Areas.Admin.Controllers
{
    public class PhieuNhapController : Controller
    {
        private WebLaptopEntities db = new WebLaptopEntities();

        // GET: Admin/PhieuNhap
        public ActionResult Index()
        {
            var phieuNhapList = db.PhieuNhaps.ToList();
            return View(phieuNhapList);
        }


        // GET: Admin/PhieuNhap/Create
        public ActionResult Create()
        {
            ViewBag.SanPhamList = db.SanPhams.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhieuNhap phieuNhap, List<PhieuNhapChiTiet> chiTietPhieuNhap, string TenKhachHang)
        {
            if (ModelState.IsValid)
            {
                phieuNhap.NhaCungCap = TenKhachHang;
                phieuNhap.NgayNhap = DateTime.Now;

                decimal tongTien = 0;

                foreach (var item in chiTietPhieuNhap)
                {
                    var sanPham = db.SanPhams.Find(item.SanPhamId);
                    if (sanPham != null)
                    {
                        tongTien += item.GiaNhap * item.SoLuong;

                        sanPham.SoLuongTon += item.SoLuong;
                    }

                    item.PhieuNhapId = phieuNhap.Id;

                    db.PhieuNhapChiTiets.Add(item);
                }

                phieuNhap.TongTien = tongTien;

                db.PhieuNhaps.Add(phieuNhap);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.SanPhamList = db.SanPhams.ToList();
            return View(phieuNhap);
        }



        // GET: Admin/PhieuNhap/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuNhap phieuNhap = db.PhieuNhaps.Include(p => p.PhieuNhapChiTiets.Select(c => c.SanPham)).FirstOrDefault(p => p.Id == id);
            if (phieuNhap == null)
            {
                return HttpNotFound();
            }
            return View(phieuNhap);
        }

        // GET: Admin/PhieuNhap/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuNhap phieuNhap = db.PhieuNhaps.Find(id);
            if (phieuNhap == null)
            {
                return HttpNotFound();
            }
            return View(phieuNhap);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            PhieuNhap phieuNhap = db.PhieuNhaps.Find(id);

            var chiTietPhieuNhaps = db.PhieuNhapChiTiets.Where(c => c.PhieuNhapId == id).ToList();
            foreach (var chiTiet in chiTietPhieuNhaps)
            {
                db.PhieuNhapChiTiets.Remove(chiTiet);
            }

            foreach (var chiTiet in chiTietPhieuNhaps)
            {
                var sanPham = db.SanPhams.Find(chiTiet.SanPhamId);
                if (sanPham != null)
                {
                    sanPham.SoLuongTon -= chiTiet.SoLuong;
                }
            }

            db.PhieuNhaps.Remove(phieuNhap);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
