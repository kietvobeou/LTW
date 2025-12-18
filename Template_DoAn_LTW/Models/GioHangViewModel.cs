using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Template_DoAn_LTW.Models
{
    public class GioHangViewModel
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string HinhAnh { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
    }
}