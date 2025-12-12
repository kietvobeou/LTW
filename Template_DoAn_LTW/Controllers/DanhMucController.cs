using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Template_DoAn_LTW.Controllers
{
    public class DanhMucController : Controller
    {
        QL_SachEntities bh = new QL_SachEntities();
        public ActionResult _DanhMuc()
        {
            return PartialView();
        }

        public ActionResult _DanhMucTheLoai()
        {
            List<DanhMuc> th = bh.DanhMucs.OrderByDescending(s => s.TenDanhMuc).ToList();
            return PartialView(th);
        }

        public ActionResult _DanhMucNXB()
        {
            List<NhaXuatBan> nxb = bh.NhaXuatBans.OrderByDescending(s => s.TenNXB).ToList();
            return PartialView(nxb);
        }

        public ActionResult _TimKiemNC()
        {
            List<DanhMuc> listLoai = bh.DanhMucs.OrderByDescending(s => s.TenDanhMuc).ToList();

            return PartialView(listLoai);
        }
    }
}