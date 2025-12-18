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
            // ===== DEBUG BẮT BUỘC =====
            System.Diagnostics.Debug.WriteLine("MASP (Route) = " + masp);

            // ===== LẤY SÁCH =====
            var sach = ql.Saches.FirstOrDefault(s => s.MaSach == masp);
            if (sach == null)
            {
                System.Diagnostics.Debug.WriteLine("KHÔNG TÌM THẤY SÁCH");
                return HttpNotFound();
            }

            System.Diagnostics.Debug.WriteLine("SÁCH: " + sach.MaSach);

            // ===== SÁCH LIÊN QUAN =====
            ViewBag.SachLienQuan_DanhMuc = ql.Saches
                .Where(s => s.MaDanhMuc == sach.MaDanhMuc && s.MaSach != masp)
                .Take(8)
                .ToList();

            ViewBag.SachLienQuan_NXB = ql.Saches
                .Where(s => s.MaNXB == sach.MaNXB && s.MaSach != masp)
                .Take(8)
                .ToList();

            var danhSachBinhLuan = ql.BinhLuans
                .Include("NguoiDung")   
                .Where(b => b.MaSach == sach.MaSach) 
                .OrderByDescending(b => b.NgayBL)
                .ToList();

            System.Diagnostics.Debug.WriteLine("SỐ BÌNH LUẬN = " + danhSachBinhLuan.Count);

            ViewBag.DanhSachBinhLuan = danhSachBinhLuan;
            ViewBag.SoLuongDanhGia = danhSachBinhLuan.Count;

            return View(sach);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ThemBinhLuan(int MaSach, int SoSao, string NoiDung)
        {
            if (Session["MaKH"] == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để bình luận.";
                return RedirectToAction("Index", "DangNhap");
            }

            int maND = (int)Session["MaKH"];

            BinhLuan bl = new BinhLuan
            {
                MaSach = MaSach,
                MaND = maND,
                SoSao = SoSao,
                NoiDung = NoiDung,
                NgayBL = DateTime.Now
            };

            ql.BinhLuans.Add(bl);
            ql.SaveChanges();

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