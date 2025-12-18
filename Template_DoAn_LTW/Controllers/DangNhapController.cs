using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Template_DoAn_LTW.Controllers
{
    public class DangNhapController : Controller
    {
        QL_SachEntities ql = new QL_SachEntities();

        // GET: DangNhap
        public ActionResult Index()
        {
            ViewBag.Url = Url.Action("Index", "Book");
            return View();
        }
        [HttpPost]
        public ActionResult XulyFormDN(FormCollection form, string url)
        {
            string email = form["email"];
            string pass = form["password"];

            var kq = ql.NguoiDungs
                .FirstOrDefault(nd => nd.Email == email && nd.MatKhau == pass);

            if (kq != null)
            {
                FormsAuthentication.SetAuthCookie(kq.Email, true);

                Session["HoTen"] = kq.HoTen;
                Session["MaND"] = kq.MaND;
                Session["AnhDaiDien"] = kq.AnhDaiDien;

                return Redirect(url ?? Url.Action("Index", "Book"));
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
            ViewBag.Url = Url.Action("Index", "Book");
            return View("Index");
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}