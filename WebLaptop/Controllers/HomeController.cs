using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Controllers
{
    public class HomeController : Controller
    {

        WebLaptopEntities db = new WebLaptopEntities();

        public ActionResult Index()
        {
            ViewBag.Banner4 = db.CauHinhs.Where(x => x.Ma == 1).FirstOrDefault().Banner4;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public PartialViewResult Header()
        {

            var theloai = db.LoaiSPs.ToList();

            return PartialView(theloai);
        }
        public PartialViewResult Footer()
        {

            return PartialView();
        }
        public PartialViewResult Banner()
        {

            ViewBag.Banner1 = db.CauHinhs.Where(x => x.Ma == 1).FirstOrDefault().Banner1;
            ViewBag.Banner2 = db.CauHinhs.Where(x => x.Ma == 1).FirstOrDefault().Banner2;
            ViewBag.Banner3 = db.CauHinhs.Where(x => x.Ma == 1).FirstOrDefault().Banner3;
            ViewBag.Banner4 = db.CauHinhs.Where(x => x.Ma == 1).FirstOrDefault().Banner4;
            return PartialView();
        }
        public PartialViewResult NewProduct()
        {
            var newProduct = db.SanPhams.OrderByDescending(x => x.NamSX).Take(10).ToList();

            return PartialView(newProduct);
        }
        
        public ActionResult IdProduct(int ma)
        {
            ViewBag.Ma = ma;
            return RedirectToAction("ShowDetail" , new { ma = ma });
        }
        public PartialViewResult ShowDetail(int ? ma)
        {
            if (ma > 0)
            {
                ViewBag.Anh = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().Anh;
                ViewBag.Gia = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().Gia;
                ViewBag.Ten = "mone";
                ViewBag.MoTaNgan = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().Mota;
                ViewBag.ThuongHieu = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().ThuongHieu;
                ViewBag.NamSX = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().NamSX;
                ViewBag.NhaXB = db.SanPhams.Where(x => x.Ma == ma).FirstOrDefault().NhaSanXuat;
                
            }

            return PartialView();
           
        }
        public ActionResult Contact()
        {
            ViewBag.email = db.CauHinhs.FirstOrDefault().Email;
            ViewBag.Sdt = db.CauHinhs.FirstOrDefault().SoDienThoai;
            ViewBag.dc = db.CauHinhs.FirstOrDefault().DiaChi;
            ViewBag.ct = db.CauHinhs.FirstOrDefault().CongTy;

            return View();
        }
        public ActionResult TaoTaiKhoan()
        {
           
                return View();
           
        }
        [HttpPost]
        public ActionResult TaoTaiKhoan(string tenkh, string diachi, string sodienthoai, string password, string email)
        {
           
            KhachHang kh = new KhachHang();

            kh.Ten = tenkh;
            kh.DiaChi = diachi;
            kh.DienThoai = sodienthoai;
            kh.Email = email;
            kh.Password = password;
            db.KhachHangs.Add(kh);
            db.SaveChanges();
            ViewBag.mes = "Tạo tài khoản thành công";
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string SoDienThoai, string MatKhau)
        {
            var isKH = db.KhachHangs.Where(x => x.DienThoai == SoDienThoai && x.Password == MatKhau).FirstOrDefault();
            if (isKH != null)
            {
                Session["KhachHang"] = isKH;
                return Redirect("Index");
            }
            else
            {
                ViewBag.mess = "Đăng nhập không thành công";
            }
            return View();
        
        }
        public ActionResult DanhSachDonHang()
        {
            KhachHang kh = (KhachHang)Session["KhachHang"];
            if(kh == null)
            {
                return Redirect("Index");
            }
            var dssp = db.DonHangs.Where(x => x.MaKhachHang == kh.Ma).OrderByDescending(x => x.Ma).ToList();
            return View(dssp);
        }
        public ActionResult XemChiTietDonHang(int madh)
        {
            var cdth = db.ChiTietDonHangs.Where(x => x.MaDH == madh).ToList();
            return View(cdth);
        }
        public ActionResult XoaDonHang(int madh)
        {
            var dh = db.DonHangs.Where(x => x.Ma == madh).FirstOrDefault();
            db.DonHangs.Remove(dh);
            db.SaveChanges();

            return RedirectToAction("DanhSachDonHang");
        }
        public ActionResult Logout()
        {
            Session["KhachHang"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult myAccount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult myAccount( int makh ,  string tenkh, string diachi, string sodienthoai, string password, string email)
        {
            var kh = db.KhachHangs.Where(x => x.Ma == makh).FirstOrDefault();
            kh.Ten = tenkh;
            kh.DiaChi = diachi;
            kh.DienThoai = sodienthoai;
            kh.Email = email;
            kh.Password = password;
            db.SaveChanges();
            Session["KhachHang"] = kh;
            ViewBag.mess123 = "Cập nhật tài khoản thành công";
            return View();
        }

    }
}