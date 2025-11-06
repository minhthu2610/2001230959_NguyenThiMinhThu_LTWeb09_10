using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using LTWeb09_BTM1.Models;

namespace LTWeb09_BTM1.Controllers
{
    public class HomeController : Controller
    {
        BookStore data = new BookStore();
        public ActionResult DSMenu_ChuDe()
        {
            List<CHUDE> dsCD = data.CHUDEs.Take(10).ToList();
            return PartialView(dsCD);
        }
        public ActionResult SachTheoCD(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var dsSach = data.SACHes
                             .Where(s => s.MACD == id)
                             .OrderBy(s => s.GIABAN)
                             .ToList();

            if (dsSach == null || dsSach.Count == 0)
            {
                ViewBag.ThongBao = "Không có sách nào thuộc chủ đề này.";
            }
            else
            {
                ViewBag.TenChuDe = data.CHUDEs
                                      .Where(cd => cd.MACD == id)
                                      .Select(cd => cd.TENCHUDE)
                                      .FirstOrDefault();
            }
            return View(dsSach);
        }

        public ActionResult DSMenu_NXB()
        {
            List<NHAXUATBAN> dsNXB = data.NHAXUATBANs.Take(10).ToList();
            return PartialView(dsNXB);
        }
        public ActionResult SachTheoNXB(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var dsSach = data.SACHes
                             .Where(s => s.MANXB == id)
                             .OrderBy(s => s.GIABAN)
                             .ToList();

            if (dsSach == null || dsSach.Count == 0)
            {
                ViewBag.ThongBao = "Không có sách nào của nhà xuất bản này.";
            }
            else
            {
                ViewBag.TenNXH = data.NHAXUATBANs
                                      .Where(nxb => nxb.MANXB == id)
                                      .Select(nxb => nxb.TENNXH)
                                      .FirstOrDefault();
            }
            return View(dsSach);
        }

        public ActionResult Index()
        {
            List<SACH> dsSach = data.SACHes.OrderByDescending(s => s.NGAYCAPNHAT).Take(5).ToList();
            return View(dsSach);
        }
        public ActionResult ChiTietSach(int id)
        {
            var sach = data.SACHes.Include(s => s.CHUDE).Include(s => s.NHAXUATBAN).Include("THAMGIAs.TACGIA").SingleOrDefault(s => s.MASACH == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        public ActionResult CungChuDe(int id)
        {
            var sach = data.SACHes.SingleOrDefault(s => s.MASACH == id);
            if (sach == null) return HttpNotFound();

            var cungChuDe = data.SACHes
                .Where(s => s.MACD == sach.MACD && s.MASACH != id)
                .Take(4)
                .ToList();

            return PartialView(cungChuDe);
        }
 
        public ActionResult CungNXB(int id)
        {
            var sach = data.SACHes.SingleOrDefault(s => s.MASACH == id);
            if (sach == null) return HttpNotFound();

            var cungNXB = data.SACHes
                .Where(s => s.MANXB == sach.MANXB && s.MASACH != id)
                .Take(4)
                .ToList();

            return PartialView(cungNXB);
        }
        public ActionResult TimKiem(string tukhoa)
        {
            var dsSach = data.SACHes.Where(s => s.TENSACH.Contains(tukhoa) || tukhoa == null).OrderBy(s => s.TENSACH).ToList();
            return View("KetQuaTimKiem", dsSach);
        }
        [HttpGet]
        public ActionResult TimKiemNangCao()
        {
            ViewBag.ChuDeList = new SelectList(data.CHUDEs, "MACD", "TENCHUDE");
            return View();
        }
        [HttpPost]
        public ActionResult KetQuaTimKiem(string tenSach, int? maCD, string mucGia)
        {
            var sach = data.SACHes.AsQueryable();
            if (string.IsNullOrEmpty(tenSach) && !maCD.HasValue && string.IsNullOrEmpty(mucGia))
            {
                ViewBag.ThongBao = "Vui lòng nhập ít nhất một điều kiện tìm kiếm.";
                return View(new List<SACH>());
            }
            if (!string.IsNullOrEmpty(tenSach))
                sach = sach.Where(s => s.TENSACH.Contains(tenSach));
            if (maCD.HasValue) sach = sach.Where(s => s.MACD == maCD.Value);
            if (!string.IsNullOrEmpty(mucGia))
            {
                switch (mucGia)
                {
                    case "duoi70":
                        sach = sach.Where(s => s.GIABAN <= 70000);
                        break;
                    case "71den100":
                        sach = sach.Where(s => s.GIABAN > 70000 && s.GIABAN <= 100000);
                        break;
                    case "tren100":
                        sach = sach.Where(s => s.GIABAN > 100000);
                        break;
                }
            }
            var ketQua = sach.OrderBy(s => s.TENSACH).ToList();

            if (ketQua.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy kết quả nào phù hợp.";
            }

            ViewBag.ChuDe = new SelectList(data.CHUDEs.ToList(), "MACD", "TENCHUDE");
            return View(ketQua);
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(KHACHHANG kh, string matkhau2)
        {
            if (string.IsNullOrEmpty(kh.HOTEN) || string.IsNullOrEmpty(kh.TAIKHOAN) ||
                string.IsNullOrEmpty(kh.MATKHAU) || string.IsNullOrEmpty(matkhau2))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (kh.MATKHAU != matkhau2)
            {
                ViewBag.ThongBao = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            var tonTai = data.KHACHHANGs.FirstOrDefault(k => k.TAIKHOAN == kh.TAIKHOAN);
            if (tonTai != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại.";
                return View();
            }

            data.KHACHHANGs.Add(kh);
            data.SaveChanges();
            ViewBag.ThongBao = "Đăng ký thành công!";
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string taiKhoan, string matKhau)
        {
            var kh = data.KHACHHANGs.SingleOrDefault(k => k.TAIKHOAN == taiKhoan && k.MATKHAU == matKhau);
            if (kh != null)
            {
                Session["TaiKhoan"] = kh;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}