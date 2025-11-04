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
            if (id == null) return RedirectToAction("Index");
            var dsSachCD = data.SACHes.Where(s => s.MACD == id).ToList();
            return View(dsSachCD);
        }

        public ActionResult DSMenu_NXB()
        {
            List<NHAXUATBAN> dsNXB = data.NHAXUATBANs.Take(10).ToList();
            return PartialView(dsNXB);
        }
        public ActionResult NXBTheoTen(string ten)
        {
            var nxbTheoTen = data.NHAXUATBANs.Where(s => s.TENNXH == ten).ToList();
            return View(nxbTheoTen);
        }
        public ActionResult Index()
        {
            List<SACH> dsSach = data.SACHes.OrderByDescending(s => s.NGAYCAPNHAT).Take(5).ToList();
            return View(dsSach);
        }
        public ActionResult ChiTietSach(int id)
        {
            var sach = data.SACHes.Include(s => s.CHUDE).Include(s => s.NHAXUATBAN).SingleOrDefault(s => s.MASACH == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
    }
}