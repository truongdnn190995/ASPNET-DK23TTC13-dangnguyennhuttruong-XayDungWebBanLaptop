using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLaptop.Models;

namespace WebLaptop.Controllers
{
    public class wishlistController : Controller
    {
        // GET: wishlist
        WebLaptopEntities db = new WebLaptopEntities();
        string wl = "wishlist";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThemWishList(int masp)
        {

            var gh = (List<wishlist>)Session[wl];
            var sp = db.SanPhams.Find(masp);

            // kiem tra 

            if (gh == null)
            {
                var gio_hang_mois = new List<wishlist>();

                wishlist gh_moi = new wishlist();
                gh_moi.sp = sp;
                gio_hang_mois.Add(gh_moi);
                Session[wl] = gio_hang_mois;
            }
            else
            {
                if (gh.Exists(x => x.sp.Ma == masp))
                {

                }
                else
                {
                    wishlist gh_moi = new wishlist();
                    gh_moi.sp = sp;
                    gh.Add(gh_moi);
                    Session[wl] = gh;

                }

            }

            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult XemWishList()
        {
            if (Session[wl] == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                List<wishlist> gh = (List<wishlist>)Session[wl];
                return View(gh);
            }
        }
        public ActionResult XoaSP(int masp)
        {

            var gh = (List<wishlist>)Session[wl];
            var sp = db.SanPhams.Find(masp);
            if (gh.Exists(x => x.sp.Ma == masp))
            {
                var sanpham = gh.Where(s => s.sp.Ma == masp).FirstOrDefault();
                gh.Remove(sanpham);
                Session[wl] = gh;
            }
            return Redirect(Request.UrlReferrer.ToString());

        }
    }
}