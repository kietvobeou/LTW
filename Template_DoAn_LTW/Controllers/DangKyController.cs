using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Template_DoAn_LTW.Controllers
{
    public class DangKyController : Controller
    {
        private QL_SachEntities ql = new QL_SachEntities();

        // GET: DangKy
        public ActionResult Index()
        {
            return View();
        }

        // POST: DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(
            string HoTen,
            string Email,
            string MatKhau,
            string SDT,
            string DiaChi,
            HttpPostedFileBase AnhDaiDienUpload)
        {
            if (ql.NguoiDungs.Any(x => x.Email == Email))
            {
                ViewBag.Error = "Email đã tồn tại!";
                return View();
            }
            string avatar = "default-avatar.jpg";
            if (AnhDaiDienUpload != null && AnhDaiDienUpload.ContentLength > 0)
            {
                var ext = Path.GetExtension(AnhDaiDienUpload.FileName).ToLower();
                string[] allowExt = { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowExt.Contains(ext))
                {
                    ViewBag.Error = "Chỉ cho phép ảnh jpg, png, webp";
                    return View();
                }

                avatar = Guid.NewGuid() + ext;
                string path = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), avatar);
                AnhDaiDienUpload.SaveAs(path);
            }

            NguoiDung nd = new NguoiDung
            {
                HoTen = HoTen,
                Email = Email,
                MatKhau = MatKhau,         
                SDT = SDT,
                DiaChi = DiaChi,
                AnhDaiDien = avatar,
                MaVaiTro = 2              
            };

            ql.NguoiDungs.Add(nd);
            ql.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Index", "DangNhap");
        }
    }
}
