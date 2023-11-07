using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

namespace CNTT_CLC2_NguyenDucThang63135414.Models
{
    public class NhanViensController : Controller
    {
        private QLNV63135414Entities db = new QLNV63135414Entities();
        public ActionResult TimKiem(int? page, string maNV)
        {
            int pageSize = 5; // Số lượng bản ghi trên mỗi trang
            int pageNumber = (page ?? 1);

            var nhanViens = db.NhanViens.Where(abc => string.IsNullOrEmpty(maNV) || abc.MaNV == maNV)
                .OrderBy(abc => abc.MaNV)
                .ToPagedList(pageNumber, pageSize);
            if (nhanViens.Count() == 0)
            {
                ViewBag.TB = "Không có thông tin tìm kiếm.";
            }
            return View(nhanViens);
        }
        [HttpGet]
        public ActionResult TimKiemNC(string maNV = "", string tenNV = "", string gioiTinh = "", string luongMin = "", string luongMax = "", string diaChi = "", string maPB = "")
        {
            string min = luongMin, max = luongMax;
            if (gioiTinh != "1" && gioiTinh != "0")
                gioiTinh = null;
            ViewBag.maNV = maNV;
            ViewBag.hoTen = tenNV;
            ViewBag.gioiTinh = gioiTinh;
            if (luongMin == "")
            {
                ViewBag.luongMin = "";
                min = "0";
            }
            else
            {
                ViewBag.luongMin = luongMin;
                min = luongMin;
            }
            if (max == "")
            {
                max = Int32.MaxValue.ToString();
                ViewBag.luongMax = "";// Int32.MaxValue.ToString(); 
            }
            else
            {
                ViewBag.luongMax = luongMax;
                max = luongMax;
            }
            ViewBag.diaChi = diaChi;
            ViewBag.MaPB = new SelectList(db.PhongBans, "MaPB", "TenPB");
            var nhanViens = db.NhanViens.SqlQuery("NhanVien_TimKiem'" + maNV + "','" + tenNV + "','" + gioiTinh + "','" + min + "','" + max + "',N'" + diaChi + "','" + maPB + "'");
            if (nhanViens.Count() == 0)
                ViewBag.TB = "Không có thông tin tìm kiếm.";
            return View(nhanViens.ToList());
        }


        // GET: NhanViens
        public ActionResult Index(int? page)
        {
            var nhanViens = db.NhanViens.Include(n => n.PhongBan).OrderBy(nv => nv.TenNV);
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            IPagedList<NhanVien> listNhanViens = nhanViens.ToPagedList(pageNumber, pageSize);
            return View(listNhanViens);
        }
        // GET: NhanViens/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);

            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }
        public static string getMoneys(long m)
        {
            string text = string.Empty;
            long num = m / 1000 + 1;
            for (int i = 0; i < num; i++)
            {
                if (m >= 1000)
                {
                    long num2 = m % 1000;
                    text = ((num2 == 0) ? (".000" + text) : ((num2 < 10) ? (".00" + num2 + text) : ((num2 < 100) ? (".0" + num2 + text) : ("." + num2 + text))));
                    m /= 1000;
                    continue;
                }
                text = m + text;
                break;
            }
            return text;
        }
        // GET: NhanViens/Create
        public ActionResult Create()
        {
            ViewBag.MaNV = LayMaNV();
            ViewBag.MaPB = new SelectList(db.PhongBans, "MaPB", "TenPB");
            return View();
        }

        // POST: NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaNV,HoNV,TenNV,GioiTinh,NgaySinh,Luong,AnhNV,DiaChi,MaPB")] NhanVien nhanVien)
        {
            //System.Web.HttpPostedFileBase Avatar;
            var imgNV = Request.Files["Avatar"];
            //Lấy thông tin từ input type=file có tên Avatar
            string postedFileName = System.IO.Path.GetFileName(imgNV.FileName);
            //Lưu hình đại diện về Server
            var path = Server.MapPath("/Images/" + postedFileName);
            imgNV.SaveAs(path);
            if (ModelState.IsValid)
            {
                nhanVien.MaNV = LayMaNV();
                nhanVien.AnhNV = postedFileName;
                db.NhanViens.Add(nhanVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaPB = new SelectList(db.PhongBans, "MaPB", "TenPB", nhanVien.MaPB);
            return View(nhanVien);
        }

        // GET: NhanViens/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaPB = new SelectList(db.PhongBans, "MaPB", "TenPB", nhanVien.MaPB);
            return View(nhanVien);
        }

        // POST: NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNV,HoNV,TenNV,GioiTinh,NgaySinh,Luong,AnhNV,DiaChi,MaPB")] NhanVien nhanVien)
        {
            var imgNV = Request.Files["Avatar"];
            try
            {
                //Lấy thông tin từ input type=file có tên Avatar
                string postedFileName = System.IO.Path.GetFileName(imgNV.FileName);
                //Lưu hình đại diện về Server
                var path = Server.MapPath("/Images/" + postedFileName);
                imgNV.SaveAs(path);
            }
            catch
            {
            }
            if (ModelState.IsValid)
            {
                db.Entry(nhanVien).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaPB = new SelectList(db.PhongBans, "MaPB", "TenPB", nhanVien.MaPB);
            return View(nhanVien);
        }

        // GET: NhanViens/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            NhanVien nhanVien = db.NhanViens.Find(id);
            db.NhanViens.Remove(nhanVien);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        string LayMaNV()
        {
            var maMax = db.NhanViens.ToList().Select(n => n.MaNV).Max();
            int maNV = int.Parse(maMax.Substring(2)) + 1;
            string NV = String.Concat("000", maNV.ToString());
            return "NV" + NV.Substring(maNV.ToString().Length - 1);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult PrintList()
        {
            var nhanViens = db.NhanViens.Include(n => n.PhongBan).OrderBy(n => n.TenNV);
            return PartialView(nhanViens.ToList());
        }
    }
}
