using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla.Controllers
{
    public class KonuController : Controller
    {

        #region Konu İşlemleri


        [OturumKontrolFilter]
        public ActionResult List(int brans=0)
        {
            var db = new DatabaseContext();

            var bs = db.Brans.AsNoTracking()
                             .Where(x => !x.isSilindi)
                             .Select(x => new SelectListItem()
                             {
                                 Value = x.ID.ToString(),
                                 Text = x.Ad,
                                 Selected = x.ID == brans
                             }).ToList();

            var ks = db.Konu.AsNoTracking().Where(x => x.BransID == brans && !x.isSilindi).ToList();

            ViewBag.brans = brans;
            ViewBag.bransAd = brans == 0 ? "" : db.Brans.Where(x => x.ID == brans).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();
            ViewBag.branslar = bs;
            return View(ks);
        }


        [OturumKontrolFilter]
        public ActionResult Edit(int brans, int id = 0)
        {
            if (brans == 0)
            {
                Hizli.SetHata("Lütfen öncelikle bir Branş seçin ve tekrar deneyin.");
                return RedirectToAction("List");
            }

            var db = new DatabaseContext();
            
            var k = new Konu();
            k.BransID = brans;
            
            if (id != 0)
                k = db.Konu.AsNoTracking().Where(x => x.ID == id && !x.isSilindi).FirstOrDefault();

            ViewBag.bransAd = db.Brans.Where(x => x.ID == brans).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();

            return View(k);
        }

        [ValidateAntiForgeryToken]
        [OturumKontrolFilter]
        public ActionResult Save(Konu model)
        {
            var db = new DatabaseContext();
            if (model.ID == 0)
            {
                db.Konu.Add(model);
                db.SaveChanges();
            }
            else
            {
                var k = db.Konu.Where(x => x.ID == model.ID && !x.isSilindi).FirstOrDefault();
                if (k != null)
                {
                    k.Ad = model.Ad;
                    db.SaveChanges();
                }
                else
                {
                    Hizli.SetHata("İşlem yapılacak Konu bilgisine ulaşılamadı! Lütfen Konu listesini kontrol edip tekrar deneyin.");
                }
            }

            return RedirectToAction("List", new { brans = model.BransID });
        }


        [OturumKontrolFilter]
        public ActionResult Delete(int id)
        {
            var db = new DatabaseContext();
            var k = db.Konu.FirstOrDefault(x => x.ID == id);
            if (k == null)
            {
                Hizli.SetHata("İşlem yapılacak Konu bulunamadı! Lütfen sayfayı yenileyip tekrar deneyin.");
            }
            else
            {
                Hizli.SetHata(k.Ad + " isimli Konu silindi.");
                k.isSilindi = true;
                db.SaveChanges();
            }
            return RedirectToAction("List", new { brans=k.BransID });
        }

        #endregion

    }
}