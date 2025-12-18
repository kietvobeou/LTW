using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Template_DoAn_LTW.Models;

namespace Template_DoAn_LTW.Controllers
{
    public class TrangChuController : Controller
    {
        QL_SachEntities ql = new QL_SachEntities();

        // GET: TrangChu
        public ActionResult Index()
        {
            var sachNgauNhien = ql.Saches
                .OrderBy(r => Guid.NewGuid())
                .Take(8)
                .ToList();

            return View(sachNgauNhien);
        }
    }
}