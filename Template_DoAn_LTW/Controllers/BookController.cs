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