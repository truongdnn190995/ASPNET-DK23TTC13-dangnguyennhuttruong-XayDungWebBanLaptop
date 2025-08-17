using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Controllers
{
    public class GioHangsController : Controller
    {
        WebLaptopEntities db = new WebLaptopEntities();
        private static decimal amount = 0;
        private static int id = 0;
        string giohang = "GioHang";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThemGioHang(int masp, int sl)
        {

            var gh = (List<GioHang>)Session[giohang];
            var sp = db.SanPhams.Find(masp);

            if (gh == null)
            {
                var gio_hang_mois = new List<GioHang>();

                GioHang gh_moi = new GioHang();
                gh_moi.sp = sp;
                gh_moi.Soluong = sl;
                gio_hang_mois.Add(gh_moi);
                Session[giohang] = gio_hang_mois;
            }
            else
            {
                if (gh.Exists(x => x.sp.Ma == masp))
                {
                    var sanpham = gh.Where(s => s.sp.Ma == masp).FirstOrDefault();
                    sanpham.Soluong = sanpham.Soluong + sl;


                }
                else
                {
                    GioHang gh_moi = new GioHang();
                    gh_moi.sp = sp;
                    gh_moi.Soluong = sl;
                    gh.Add(gh_moi);

                }
                Session[giohang] = gh;
            }

            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult XemGioHang()
        {
            if (Session[giohang] == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                List<GioHang> gh = (List<GioHang>)Session[giohang];
                return View(gh);
            }
        }
        public ActionResult XoaSP(int masp)
        {

            var gh = (List<GioHang>)Session[giohang];
            var sp = db.SanPhams.Find(masp);
            if (gh.Exists(x => x.sp.Ma == masp))
            {
                var sanpham = gh.Where(s => s.sp.Ma == masp).FirstOrDefault();
                gh.Remove(sanpham);
                Session[giohang] = gh;
            }
            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult TangSl(int masp)
        {
            var gh = (List<GioHang>)Session[giohang];
            var sp = db.SanPhams.Find(masp);
            if (gh.Exists(x => x.sp.Ma == masp))
            {
                var sanpham = gh.Where(s => s.sp.Ma == masp).FirstOrDefault();
                sanpham.Soluong = sanpham.Soluong + 1;
                Session[giohang] = gh;

            }
            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult GiamSl(int masp)
        {
            var gh = (List<GioHang>)Session[giohang];
            var sp = db.SanPhams.Find(masp);
            if (gh.Exists(x => x.sp.Ma == masp))
            {
                var sanpham = gh.Where(s => s.sp.Ma == masp).FirstOrDefault();
                if (sanpham.Soluong > 0)
                {
                    sanpham.Soluong = sanpham.Soluong - 1;
                    Session[giohang] = gh;
                }

            }
            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult CheckOut()
        {
            List<GioHang> gh = (List<GioHang>)Session[giohang];
            if (gh == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return View(gh);
            }
        }
        public ActionResult CheckOutProduct(int makh, string paymentMethod, string tenkh, string diachi, string SoDienThoai, string email, string diachigiaohang, string checkGiaoHang)
        {
            List<GioHang> gh = (List<GioHang>)Session["giohang"];

            KhachHang kh = db.KhachHangs.Find(makh);

            kh.Ten = tenkh;
            kh.DiaChi = diachi;
            kh.DienThoai = SoDienThoai;
            kh.Email = email;
            db.Entry(kh).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            DonHang dh = new DonHang();
            dh.MaKhachHang = makh;
            dh.NgayDatHang = DateTime.Now;
            dh.PhiGiao = 0; 
            dh.TenNguoiNhan = tenkh;

            decimal totalOrderPrice = gh.Sum(item => (item.sp.Gia * item.Soluong)) ?? 0; 
            amount = totalOrderPrice;

            if (checkGiaoHang == "0")
            {
                dh.DiaChi = diachigiaohang; 
            }
            else
            {
                dh.DiaChi = diachi; 
            }

            dh.DienThoai = SoDienThoai;
            dh.Email = email;
            dh.TrangThai = false; 

            if (paymentMethod.Equals("VNPay"))
            {
                Session["PendingOrder"] = new
                {
                    DonHang = dh,
                    GioHang = gh
                };
                var model = new PaymentInformationModel()
                {
                    OrderType = "other",
                    Amount = amount,
                    OrderDescription = "Thanh toán đơn hàng",
                    Name = "User",
                };

                var paymentUrl = CreatePaymentUrl(model);
                return Redirect(paymentUrl);
            }
            else if (paymentMethod.Equals("COD"))
            {
                dh.PTTT = "COD";

                db.DonHangs.Add(dh);
                db.SaveChanges();

                int mdh = db.DonHangs.OrderByDescending(x => x.Ma).FirstOrDefault().Ma;

                foreach (var item in gh)
                {
                    ChiTietDonHang ctd = new ChiTietDonHang();
                    ctd.MaDH = mdh;
                    ctd.MaSP = item.sp.Ma;
                    ctd.SoLuong = item.Soluong;
                    ctd.DonGia = item.sp.Gia;
                    db.ChiTietDonHangs.Add(ctd);
                    db.SaveChanges();
                }

                DonHang ttdh = db.DonHangs.Find(mdh);
                Session["TTDonHang"] = ttdh;

                return RedirectToAction("DatHangTC");
            }

            return RedirectToAction("Index", "Home");
        }



        public string CreatePaymentUrl(PaymentInformationModel model)
        {
            string vnp_Version = "2.1.0";
            string vnp_Command = "pay";
            string orderType = "other";
            string bankCode = "NCB";

            string vnp_TxnRef = GetRandomNumber(8);
            string vnp_IpAddr = "127.0.0.1";
            string vnp_TmnCode = "0S7T01T8";

            var vnp_Params = new Dictionary<string, string>
    {
        { "vnp_Version", vnp_Version },
        { "vnp_Command", vnp_Command },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", (Math.Floor(model.Amount * 100)).ToString() },

        { "vnp_CurrCode", "VND" },
        { "vnp_BankCode", bankCode },
        { "vnp_TxnRef", vnp_TxnRef },
        { "vnp_OrderInfo", "Thanhtoan" },
        { "vnp_OrderType", model.OrderType },
        { "vnp_Locale", "vn" },
        { "vnp_ReturnUrl", "https://localhost:44336/GioHangs/PaymentCallback" },
        { "vnp_IpAddr", vnp_IpAddr },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
    };

            DateTime expireDate = DateTime.Now.AddMinutes(15);
            vnp_Params.Add("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));

            var fieldNames = vnp_Params.Keys.ToList();
            fieldNames.Sort();

            StringBuilder hashData = new StringBuilder();
            StringBuilder query = new StringBuilder();

            foreach (var fieldName in fieldNames)
            {
                string fieldValue = vnp_Params[fieldName];
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    hashData.Append(fieldName).Append('=').Append(Uri.EscapeDataString(fieldValue));
                    query.Append(Uri.EscapeDataString(fieldName))
                         .Append('=')
                         .Append(Uri.EscapeDataString(fieldValue));

                    if (fieldNames.IndexOf(fieldName) < fieldNames.Count - 1)
                    {
                        query.Append('&');
                        hashData.Append('&');
                    }
                }
            }

            string secretKey = "BEZLUPOPOTXTDYZHCBGDJBHFJPBLSARL";
            string secureHash = HmacSHA512(secretKey, hashData.ToString());

            query.Append("&vnp_SecureHash=").Append(secureHash);

            string paymentUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?" + query.ToString();

            System.Diagnostics.Debug.WriteLine($"Payment URL: {paymentUrl}");

            return paymentUrl;
        }


        public ActionResult PaymentCallback()
        {
            string vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
            if (vnp_ResponseCode == "00") 
            {
                var pendingOrder = Session["PendingOrder"] as dynamic;

                if (pendingOrder != null)
                {
                    DonHang dh = pendingOrder.DonHang;
                    List<GioHang> gh = pendingOrder.GioHang;

                    dh.TrangThai = true;
                    dh.NgayDatHang = DateTime.Now;
                    dh.PTTT = "VNPay";
                    db.DonHangs.Add(dh);
                    db.SaveChanges();

                    int mdh = dh.Ma;

                    foreach (var item in gh)
                    {
                        ChiTietDonHang ctd = new ChiTietDonHang();
                        ctd.MaDH = mdh;
                        ctd.MaSP = item.sp.Ma;
                        ctd.SoLuong = item.Soluong;
                        ctd.DonGia = item.sp.Gia;
                        db.ChiTietDonHangs.Add(ctd);
                        db.SaveChanges();
                    }
                    Session["PendingOrder"] = null;
                    TempData["SuccessMessage"] = "Đơn hàng được thanh toán thành công!";
                    DonHang ttdh = db.DonHangs.Find(mdh);
                    Session["TTDonHang"] = ttdh;

                    return RedirectToAction("DatHangTC");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng trong session!";
                    return RedirectToAction("Error");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh toán không thành công!";
                return RedirectToAction("Error");
            }
            
        }


        public string GetRandomNumber(int length)
        {
            Random rnd = new Random();
            const string chars = "0123456789";
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[rnd.Next(chars.Length)]);
            }

            return sb.ToString();
        }


        public static string HmacSHA512(string key, string data)
        {
            if (key == null || data == null)
            {
                throw new ArgumentNullException("Key and data must not be null.");
            }

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] hashBytes = hmac.ComputeHash(dataBytes);

                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }

                return sb.ToString();
            }
        }


        public ActionResult LoginAcc()
        {
            List<GioHang> gh = (List<GioHang>)Session[giohang];
            KhachHang kh = (KhachHang)Session["KhachHang"];
            if (gh == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                if(kh != null)
                {
                    return Redirect("CheckOut");
                }
                else
                {
                   return View(gh);
                }
            }
        }
        [HttpPost]
        public ActionResult LoginAcc(string SoDienThoai, string MatKhau)
        {
            List<GioHang> gh = (List<GioHang>)Session[giohang];
            var isKH = db.KhachHangs.Where(x => x.DienThoai == SoDienThoai && x.Password == MatKhau).FirstOrDefault();
            if (isKH != null)
            {
                Session["KhachHang"] = isKH;
                return Redirect("CheckOut");
            }
            else
            {
                ViewBag.mess = "Đăng nhập không thành công";
            }
            return View(gh);
        }
        public ActionResult CreateAcc()
        {
            List<GioHang> gh = (List<GioHang>)Session[giohang];
            if (gh == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return View(gh);
            }
        }

        [HttpPost]
        public ActionResult CreateAcc(string tenkh, string diachi, string sodienthoai, string password, string email, string diachigiaohang, string checkGiaoHang)
        {
            List<GioHang> gh = (List<GioHang>)Session[giohang];

            KhachHang kh = new KhachHang();

            kh.Ten = tenkh;
            kh.DiaChi = diachi;
            kh.DienThoai = sodienthoai;
            kh.Email = email;
            kh.Password = password;
            db.KhachHangs.Add(kh);
            db.SaveChanges();

            var makh = db.KhachHangs.OrderByDescending(x => x.Ma).FirstOrDefault().Ma;

            DonHang dh = new DonHang();
            dh.MaKhachHang = makh;
            dh.NgayDatHang = DateTime.Now;
            dh.PhiGiao = 0;
            dh.TenNguoiNhan = tenkh;
            dh.DienThoai = sodienthoai;
            dh.DiaChi = diachi;
            dh.Email = email;
            if (checkGiaoHang == "0")
            {
                dh.DiaChi = diachigiaohang;
            }
            else
            {
                dh.DiaChi = diachi;
            }
            dh.TrangThai = false;
            db.DonHangs.Add(dh);
            db.SaveChanges();
            int mdh = db.DonHangs.OrderByDescending(x => x.Ma).FirstOrDefault().Ma;
            foreach (var item in gh)
            {
                ChiTietDonHang ctd = new ChiTietDonHang();
                ctd.MaDH = mdh;
                ctd.MaSP = item.sp.Ma;
                ctd.SoLuong = item.Soluong;
                ctd.DonGia = item.sp.Gia;
                db.ChiTietDonHangs.Add(ctd);
                db.SaveChanges();
            }

            DonHang ttdh = db.DonHangs.Find(mdh);
            Session["TTDonHang"] = ttdh;
            return RedirectToAction("DatHangTC");

        }
        public ActionResult DatHangTC()
        {
            var dh = (DonHang)Session["TTDonHang"];

            var ctdh = db.ChiTietDonHangs.Where(x => x.MaDH == dh.Ma).ToList();

            Session["giohang"] = null;

            return View(ctdh);
        }


    }
}