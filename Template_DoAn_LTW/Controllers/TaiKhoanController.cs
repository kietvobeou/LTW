using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Template_DoAn_LTW.Controllers
{
    [Authorize]
    public class TaiKhoanController : Controller
    {
        private readonly QL_SachEntities ql = new QL_SachEntities();
        public ActionResult HoSo()
        {
            if (Session["MaND"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }

            int maND = (int)Session["MaND"];

            var nguoiDung = ql.NguoiDungs.FirstOrDefault(nd => nd.MaND == maND);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }

            return View(nguoiDung);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoSo(
            NguoiDung model,
            HttpPostedFileBase AnhDaiDienUpload,
            string AnhDaiDienUrl 
        )
        {
            if (Session["MaND"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }

            int maND = (int)Session["MaND"];
            var nguoiDung = ql.NguoiDungs.FirstOrDefault(nd => nd.MaND == maND);

            if (nguoiDung == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(nguoiDung);
            }
            nguoiDung.HoTen = model.HoTen;
            nguoiDung.SDT = model.SDT;
            nguoiDung.DiaChi = model.DiaChi;
            if (!string.IsNullOrWhiteSpace(AnhDaiDienUrl) &&
                (AnhDaiDienUrl.StartsWith("http://") || AnhDaiDienUrl.StartsWith("https://")))
            {
                nguoiDung.AnhDaiDien = AnhDaiDienUrl;
            }
            else if (AnhDaiDienUpload != null && AnhDaiDienUpload.ContentLength > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(AnhDaiDienUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận file ảnh: jpg, jpeg, png, gif.");
                    return View(nguoiDung);
                }

                var fileName = Guid.NewGuid() + extension;
                var savePath = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), fileName);
                AnhDaiDienUpload.SaveAs(savePath);

                if (!string.IsNullOrEmpty(nguoiDung.AnhDaiDien) &&
                    !nguoiDung.AnhDaiDien.StartsWith("http") &&
                    nguoiDung.AnhDaiDien != "default-avatar.jpg")
                {
                    var oldPath = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), nguoiDung.AnhDaiDien);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                nguoiDung.AnhDaiDien = fileName;
            }

            ql.SaveChanges();

            Session["HoTen"] = nguoiDung.HoTen;
            Session["AnhDaiDien"] = nguoiDung.AnhDaiDien;

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("HoSo");
        }
    }
}
