using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Template_DoAn_LTW.Controllers
{
    public class BookController : Controller
    {
        QL_SachEntities ql = new QL_SachEntities();
        public ActionResult Index()
        {
            List<Sach> sach = ql.Saches.OrderByDescending(s => s.Gia).ToList();
            return View(sach);
        }
        public ActionResult ChiTietSP(int masp)
        {
            var sach = ql.Saches.FirstOrDefault(s => s.MaSach == masp);
            if (sach == null) return HttpNotFound();

            ViewBag.SachLienQuan_DanhMuc = ql.Saches
                .Where(s => s.MaDanhMuc == sach.MaDanhMuc && s.MaSach != masp)
                .Take(8)
                .ToList();

            ViewBag.SachLienQuan_NXB = ql.Saches
                .Where(s => s.MaNXB == sach.MaNXB && s.MaSach != masp)
                .Take(8)
                .ToList();

            ViewBag.DanhSachNguoiBinhLuan = ql.BinhLuans
                .Where(b => b.MaSach == masp)
                .Select(b => new {
                    BinhLuan = b,
                    NguoiDung = ql.NguoiDungs.FirstOrDefault(nd => nd.MaND == b.MaND)
                })
                .OrderByDescending(x => x.BinhLuan.NgayBL)
                .ToList();

            return View(sach);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Bắt buộc phải đăng nhập mới được gửi
        public ActionResult ThemBinhLuan(int MaSach, int SoSao, string NoiDung)
        {
            if (ModelState.IsValid)
            {
                // Lấy MaND của người dùng hiện tại (từ tài khoản đăng nhập)
                var nguoiDung = ql.NguoiDungs.FirstOrDefault(nd => nd.Email == User.Identity.Name);
                if (nguoiDung == null)
                {
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }

                var binhLuan = new BinhLuan
                {
                    MaND = nguoiDung.MaND,
                    MaSach = MaSach,
                    SoSao = SoSao,
                    NoiDung = NoiDung,
                    NgayBL = DateTime.Today // Vì cột là DATE
                };

                ql.BinhLuans.Add(binhLuan);
                ql.SaveChanges();

                // Sau khi thêm thành công, quay lại trang chi tiết sách
                return RedirectToAction("ChiTietSP", new { masp = MaSach });
            }

            // Nếu có lỗi, quay lại trang chi tiết (có thể thêm TempData thông báo lỗi)
            return RedirectToAction("ChiTietSP", new { masp = MaSach });
        }
        public ActionResult LocSP(int idloc, int type)
        {
            List<Sach> listsach = new List<Sach>();
            if (type == 1)
                listsach = ql.Saches.Where(s => s.MaDanhMuc == idloc).ToList();
            else if (type == 2)
                listsach = ql.Saches.Where(s => s.MaNXB == idloc).ToList();
            return View("Index", listsach);
        }

        public ActionResult TimKiem(string kw, int? chude, string[] gia)
        {
            List<Sach> listSach = new List<Sach>();
            if (!string.IsNullOrEmpty(kw))
            {
                listSach = ql.Saches.Where(s => s.TenSach.Contains(kw.ToLower())).ToList();

            }
            if (chude != null)
            {
                listSach = ql.Saches.Where(s => s.MaDanhMuc == chude).ToList();
            }
            if (gia != null && gia.Length > 0)
            {
                var kqgia = new List<Sach>();
                foreach (string g in gia)
                {
                    if (g.Contains("-"))
                    {
                        var arr = g.Split('-');
                        int min = int.Parse(arr[0]);
                        int max = int.Parse(arr[1]);
                        kqgia.AddRange(listSach.Where(s => s.Gia >= min && s.Gia <= max).ToList());
                    }
                    else if (g.Contains(">"))
                    {
                        int min = int.Parse(g.Replace(">", ""));
                        kqgia.AddRange(listSach.Where(s => s.Gia > min).ToList());
                    }
                }
                listSach = kqgia.Distinct().ToList();
            }
            return View("Index", listSach);
        }

    }
}