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
            ViewBag.Url = Request.UrlReferrer?.ToString() ?? "/";
            return View();
        }

        [HttpPost]
        public ActionResult XulyFormDN(FormCollection form, string url)
        {
            string email = form["email"];
            string pass = form["password"];

            var kq = ql.NguoiDungs
                .Where(nd => nd.Email == email && nd.MatKhau == pass)
                .FirstOrDefault();

            if (kq != null)
            {
                FormsAuthentication.SetAuthCookie(kq.Email, true);

                Session["TenKH"] = kq.HoTen;  // Tên hiển thị
                Session["MaKH"] = kq.MaND;    // ID người dùng

                return Redirect(url);
            }
            else
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
                return View("Index");
            }
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}