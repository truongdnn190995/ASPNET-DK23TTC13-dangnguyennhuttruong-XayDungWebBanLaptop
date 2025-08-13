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
    public class PhieuXuatController : Controller
    {
        private WebLaptopEntities db = new WebLaptopEntities();

        // GET: Admin/PhieuXuat
        public ActionResult Index()
        {
            var phieuXuatList = db.PhieuXuats.ToList();
            return View(phieuXuatList);
        }

        // GET: Admin/PhieuXuat/Create
        public ActionResult Create()
        {
            ViewBag.SanPhamList = db.SanPhams.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhieuXuat phieuXuat, List<PhieuXuatChiTiet> chiTietPhieuXuat, string TenKhachHang)
        {
            if (ModelState.IsValid)
            {
                phieuXuat.KhachHang = TenKhachHang;
                phieuXuat.NgayXuat = DateTime.Now;

                decimal tongTien = 0;

                foreach (var item in chiTietPhieuXuat)
                {
                    var sanPham = db.SanPhams.Find(item.SanPhamId);
                    if (sanPham != null)
                    {
                        // Subtract stock when a product is sold
                        if (sanPham.SoLuongTon >= item.SoLuong)
                        {
                            tongTien += item.GiaXuat * item.SoLuong;
                            sanPham.SoLuongTon -= item.SoLuong;  // Decrease stock
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Không đủ số lượng sản phẩm {sanPham.Ten}");
                            return View(phieuXuat);  // Error if not enough stock
                        }
                    }

                    item.PhieuXuatId = phieuXuat.Id;
                    db.PhieuXuatChiTiets.Add(item);
                }

                phieuXuat.TongTien = tongTien;
                db.PhieuXuats.Add(phieuXuat);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.SanPhamList = db.SanPhams.ToList();
            return View(phieuXuat);
        }

        // GET: Admin/PhieuXuat/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuXuat phieuXuat = db.PhieuXuats.Include(p => p.PhieuXuatChiTiets.Select(c => c.SanPham)).FirstOrDefault(p => p.Id == id);
            if (phieuXuat == null)
            {
                return HttpNotFound();
            }
            return View(phieuXuat);
        }

        // GET: Admin/PhieuXuat/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuXuat phieuXuat = db.PhieuXuats.Find(id);
            if (phieuXuat == null)
            {
                return HttpNotFound();
            }
            return View(phieuXuat);
        }

        // POST: Admin/PhieuXuat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhieuXuat phieuXuat = db.PhieuXuats.Find(id);

            var chiTietPhieuXuats = db.PhieuXuatChiTiets.Where(c => c.PhieuXuatId == id).ToList();
            foreach (var chiTiet in chiTietPhieuXuats)
            {
                db.PhieuXuatChiTiets.Remove(chiTiet);

                var sanPham = db.SanPhams.Find(chiTiet.SanPhamId);
                if (sanPham != null)
                {
                    sanPham.SoLuongTon += chiTiet.SoLuong;  
                }
            }

            db.PhieuXuats.Remove(phieuXuat);
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
