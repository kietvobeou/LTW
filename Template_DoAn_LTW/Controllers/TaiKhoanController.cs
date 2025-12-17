using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Template_DoAn_LTW.Controllers
{
    [Authorize] // Tất cả action trong controller này đều yêu cầu phải đăng nhập
    public class TaiKhoanController : Controller
    {
        private readonly QL_SachEntities ql = new QL_SachEntities();

        // GET: /TaiKhoan/HoSo
        public ActionResult HoSo()
        {
            // Lấy MaND từ Session (được set khi đăng nhập ở DangNhapController)
            if (Session["MaKH"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }

            int maND = (int)Session["MaKH"];
            var nguoiDung = ql.NguoiDungs.FirstOrDefault(nd => nd.MaND == maND);

            if (nguoiDung == null)
            {
                return HttpNotFound();
            }

            return View(nguoiDung);
        }

        // POST: /TaiKhoan/HoSo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoSo(NguoiDung model, HttpPostedFileBase AnhDaiDienUpload)
        {
            if (Session["MaKH"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }

            int maND = (int)Session["MaKH"];
            var nguoiDung = ql.NguoiDungs.FirstOrDefault(nd => nd.MaND == maND);

            if (nguoiDung == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                // Cập nhật các trường văn bản
                nguoiDung.HoTen = model.HoTen;
                nguoiDung.SDT = model.SDT;
                nguoiDung.DiaChi = model.DiaChi;

                // Xử lý upload ảnh đại diện mới (nếu có)
                if (AnhDaiDienUpload != null && AnhDaiDienUpload.ContentLength > 0)
                {
                    // Kiểm tra định dạng file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(AnhDaiDienUpload.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file ảnh: jpg, jpeg, png, gif.");
                        return View(nguoiDung);
                    }

                    // Tạo tên file mới duy nhất
                    var fileName = Guid.NewGuid() + extension;
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), fileName);

                    AnhDaiDienUpload.SaveAs(path);

                    // Xóa ảnh cũ nếu không phải ảnh mặc định
                    if (!string.IsNullOrEmpty(nguoiDung.AnhDaiDien) && nguoiDung.AnhDaiDien != "default-avatar.jpg")
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

                // Cập nhật lại Session để header hiển thị tên và ảnh mới ngay lập tức
                Session["TenKH"] = nguoiDung.HoTen;
                Session["AnhDaiDien"] = nguoiDung.AnhDaiDien;

                TempData["Success"] = "Cập nhật thông tin thành công!";
            }

            return View(nguoiDung);
        }
    }
}