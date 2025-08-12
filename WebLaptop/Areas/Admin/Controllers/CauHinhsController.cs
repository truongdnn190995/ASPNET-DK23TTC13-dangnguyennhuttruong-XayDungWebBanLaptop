using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Areas.Admin.Controllers
{
    public class CauHinhsController : Controller
    {
        private WebLaptopEntities db = new WebLaptopEntities();

        // GET: Admin/CauHinhs
        public ActionResult Index()
        {
            return View(db.CauHinhs.ToList());
        }

        // GET: Admin/CauHinhs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CauHinh cauHinh = db.CauHinhs.Find(id);
            if (cauHinh == null)
            {
                return HttpNotFound();
            }
            return View(cauHinh);
        }
        public ActionResult CauHinhWeb()
        {
            ViewBag.CauHinh = db.CauHinhs.Where(x => x.Ma == 1).FirstOrDefault();
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CauHinhWeb(string Logo, string CongTy, string DiaChi, string SoDienThoai, string Banner1, string Banner2, string Banner3 , string Banner4 , string Email)
        {
            var cauhinh = db.CauHinhs.Where(s => s.Ma == 1).FirstOrDefault();
            cauhinh.Logo = Logo;
            cauhinh.CongTy = CongTy;
            cauhinh.DiaChi = DiaChi;
            cauhinh.SoDienThoai = SoDienThoai;
            cauhinh.Banner1 = Banner1;
            cauhinh.Banner2 = Banner2;
            cauhinh.Banner3 = Banner3;
            cauhinh.Banner4 = Banner4;
            cauhinh.Email = Email;
            db.Entry(cauhinh).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("CauHinhWeb");
        }
        // GET: Admin/CauHinhs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/CauHinhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ma,Logo,CongTy,DiaChi,SoDienThoai,Banner1,Banner2,Banner3,Banner4,Email")] CauHinh cauHinh)
        {
            if (ModelState.IsValid)
            {
                db.CauHinhs.Add(cauHinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cauHinh);
        }

        // GET: Admin/CauHinhs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CauHinh cauHinh = db.CauHinhs.Find(id);
            if (cauHinh == null)
            {
                return HttpNotFound();
            }
            return View(cauHinh);
        }

        // POST: Admin/CauHinhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ma,Logo,CongTy,DiaChi,SoDienThoai,Banner1,Banner2,Banner3,Banner4,Email")] CauHinh cauHinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cauHinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cauHinh);
        }

        // GET: Admin/CauHinhs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CauHinh cauHinh = db.CauHinhs.Find(id);
            if (cauHinh == null)
            {
                return HttpNotFound();
            }
            return View(cauHinh);
        }

        // POST: Admin/CauHinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CauHinh cauHinh = db.CauHinhs.Find(id);
            db.CauHinhs.Remove(cauHinh);
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
