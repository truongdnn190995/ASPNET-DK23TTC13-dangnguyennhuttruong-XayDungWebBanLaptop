using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Areas.Admin.Controllers
{
    public class DefaultController : Controller
    {
        WebLaptopEntities db = new WebLaptopEntities();
        public ActionResult Index()
        {
            var sanpham = db.SanPhams.ToList();

            var soldBooks = db.ChiTietDonHangs
                .GroupBy(c => c.MaSP)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(c => c.SoLuong)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

            var mostSoldBooks = sanpham
                .Where(s => soldBooks.Any(x => x.ProductId == s.Ma))
                .Select(s => new MostSoldBookViewModel
                {
                    Ma = s.Ma,
                    Ten = s.Ten,
                    Anh = s.Anh,
                    SoldQuantity = soldBooks.FirstOrDefault(x => x.ProductId == s.Ma)?.TotalSold ?? 0,
                    Gia = s.Gia
                }).ToList();

            var customersPerMonth = db.DonHangs
                .GroupBy(d => d.NgayDatHang.Value.Month)
                .Select(g => new CustomersPerMonthViewModel
                {
                    Month = g.Key,
                    CustomerCount = g.Select(d => d.MaKhachHang).Distinct().Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.MostSoldBooks = mostSoldBooks;
            ViewBag.CustomersPerMonth = customersPerMonth;

            return View(sanpham);
        }



        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(string TenDangNhap, string MatKhau)
        {
            MaHoa mh = new MaHoa();
            string mk = mh.GetMD5_low(MatKhau);
            NguoiDung nd = db.NguoiDungs.Where(x => x.TenDangNhap == TenDangNhap && x.MatKhau == mk).FirstOrDefault();
            if (nd != null)
            {
                Session["NguoiDung"] = nd;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ms = "Tài khoản mật khẩu không chính xác";
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["nguoidung"] = null;
            return RedirectToAction("Login");
        }
    }
}