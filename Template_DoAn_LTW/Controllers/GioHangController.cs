using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Template_DoAn_LTW.Models;

namespace Template_DoAn_LTW.Controllers
{
    public class GioHangController : Controller
    {
        private QL_SachEntities db = new QL_SachEntities();
        public ActionResult Index()
        {
            if (Session["GioHang"] == null)
                return RedirectToAction("Index", "Book");

            var gioHangSession = Session["GioHang"] as List<GioHangViewModel>;
            if (gioHangSession == null || !gioHangSession.Any())
                return RedirectToAction("Index", "Book");

            return View(gioHangSession);
        }
        private List<GioHangViewModel> LayGioHangSession()
        {
            var ds = Session["GioHang"] as List<GioHangViewModel>;
            if (ds == null)
            {
                ds = new List<GioHangViewModel>();
                Session["GioHang"] = ds;
            }
            return ds;
        }
        public ActionResult ThemGioHang(int MaSach, string url, int Soluong = 1)
        {
            var ds = LayGioHangSession();
            var item = ds.FirstOrDefault(s => s.MaSach == MaSach);

            if (item == null)
            {
                var sach = db.Saches.Find(MaSach);
                if (sach != null)
                {
                    var newItem = new GioHangViewModel
                    {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        HinhAnh = sach.HinhAnh,
                        Gia = sach.Gia ?? 0,
                        SoLuong = Soluong,
                        ThanhTien = (sach.Gia ?? 0) * Soluong
                    };
                    ds.Add(newItem);
                }
            }
            else
            {
                item.SoLuong += Soluong;
                item.ThanhTien = item.SoLuong * item.Gia;
            }

            Session["SoLuong"] = ds.Sum(s => s.SoLuong);
            Session["GioHang"] = ds;

            return Redirect(url);
        }
        public ActionResult CapNhatGioHang(int MaSach, FormCollection f)
        {
            var ds = LayGioHangSession();
            var item = ds.FirstOrDefault(s => s.MaSach == MaSach);

            if (item != null)
            {
                int soluong;
                if (!int.TryParse(f["soluong"], out soluong))
                    soluong = item.SoLuong;

                string action = f["action"];
                if (action == "plus")
                    soluong++;
                else if (action == "minus" && soluong > 1)
                    soluong--;

                if (soluong <= 0)
                {
                    ds.Remove(item);
                }
                else
                {
                    item.SoLuong = soluong;
                    item.ThanhTien = soluong * item.Gia;
                }

                Session["GioHang"] = ds;
                Session["SoLuong"] = ds.Sum(s => s.SoLuong);
            }

            return RedirectToAction("Index", "GioHang");
        }

        public ActionResult XoaGioHang(int MaSach)
        {
            var ds = LayGioHangSession();
            var item = ds.FirstOrDefault(s => s.MaSach == MaSach);

            if (item != null)
            {
                ds.Remove(item);
                Session["GioHang"] = ds;
                Session["SoLuong"] = ds.Sum(s => s.SoLuong);

                if (ds.Count == 0)
                    return RedirectToAction("Index", "Book");
            }

            return RedirectToAction("Index", "GioHang");
        }
        public ActionResult XoaToanBoGioHang()
        {
            Session["GioHang"] = null;
            Session["SoLuong"] = 0;
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index", "Book");
        }
        public ActionResult LuuVaoDatabase(int? MaND = null)
        {
            if (Session["GioHang"] == null)
            {
                TempData["Message"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            if (MaND == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để lưu giỏ hàng!";
                return RedirectToAction("Login", "Account");
            }

            var ds = Session["GioHang"] as List<GioHangViewModel>;

            foreach (var item in ds)
            {
                var existing = db.GioHangs.FirstOrDefault(g => g.MaND == MaND && g.MaSach == item.MaSach);

                if (existing != null)
                {
                    existing.SoLuong = (existing.SoLuong ?? 0) + item.SoLuong;
                    existing.NgayThem = DateTime.Now;
                }
                else
                {
                    var gioHangDB = db.GioHangs.Create();
                    gioHangDB.MaND = MaND;
                    gioHangDB.MaSach = item.MaSach;
                    gioHangDB.SoLuong = item.SoLuong;
                    gioHangDB.NgayThem = DateTime.Now;

                    db.GioHangs.Add(gioHangDB);
                }
            }

            try
            {
                db.SaveChanges();
                Session["GioHang"] = null;
                Session["SoLuong"] = 0;

                TempData["Success"] = "Đã lưu giỏ hàng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi lưu giỏ hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}