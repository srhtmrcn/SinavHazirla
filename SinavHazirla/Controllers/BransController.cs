using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla.Controllers
{
    public class BransController : Controller
    {

        #region Brans İşlemleri


        [OturumKontrolFilter]
        public ActionResult List()
        {
            var db = new DatabaseContext();
            var bs = db.Brans.AsNoTracking().Where(x => !x.isSilindi).ToList();
            return View(bs);
        }


        [OturumKontrolFilter]
        public ActionResult Edit(int id = 0)
        {
            var db = new DatabaseContext();
            var b = new Brans();
            if (id != 0)
                b = db.Brans.AsNoTracking().Where(x => x.ID == id && !x.isSilindi).FirstOrDefault();

            return View(b);
        }

        [ValidateAntiForgeryToken]
        [OturumKontrolFilter]
        public ActionResult Save(Brans model)
        {
            var db = new DatabaseContext();
            if (model.ID == 0)
            {
                db.Brans.Add(model);
                db.SaveChanges();
            }
            else
            {
                var b = db.Brans.Where(x => x.ID == model.ID && !x.isSilindi).FirstOrDefault();
                if (b != null)
                {
                    b.Ad = model.Ad;
                    db.SaveChanges();
                }
                else
                {
                    Hizli.SetHata("İşlem yapılacak Branş bilgisine ulaşılamadı! Lütfen Branş listesini kontrol edip tekrar deneyin.");
                }
            }

            return RedirectToAction("List");
        }


        [OturumKontrolFilter]
        public ActionResult Delete(int id)
        {
            var db = new DatabaseContext();
            var b = db.Brans.FirstOrDefault(x => x.ID == id);
            if (b == null)
            {
                Hizli.SetHata("İşlem yapılacak Brans bulunamadı! Lütfen sayfayı yenileyip tekrar deneyin.");
            }
            else
            {
                Hizli.SetHata(b.Ad + " isimli Branş silindi.");
                b.isSilindi = true;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        #endregion

    }
}