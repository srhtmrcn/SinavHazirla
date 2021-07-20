using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla.Controllers
{
    public class SoruController : Controller
    {

        #region Soru İşlemleri

        [OturumKontrolFilter]
        public ActionResult List(int brans = 0, int konu=0)
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

            var ks = db.Konu.AsNoTracking()
                            .Where(x => x.BransID == brans && !x.isSilindi)
                             .Select(x => new SelectListItem()
                             {
                                 Value = x.ID.ToString(),
                                 Text = x.Ad + " ("+(x.Sorular.Where(xx=>!xx.isSilindi).Count())+" soru mevcut)",
                                 Selected = x.ID == konu
                             }).ToList();

            ViewBag.brans = brans;
            ViewBag.konu = konu;
            ViewBag.konuAd = konu == 0 ? "" : db.Konu.Where(x => x.ID == konu).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();
            ViewBag.konular = ks;

            ViewBag.bransAd = brans == 0 ? "" : db.Brans.Where(x => x.ID == brans).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();
            ViewBag.branslar = bs;

            var ss = db.Soru.Where(x => x.KonuID == konu && !x.isSilindi)
                        .Select(x => new SoruDTO()
                        {
                            SoruID = x.ID,
                            Tarih = x.Tarih,
                            PersonelAd = x.Personel.AdSoyad,
                            isBoslukDoldurma = x.isBoslukDoldurma,
                            isCoktanSecmeli = x.isCoktanSecmeli,
                            isKlasik = x.isKlasik,
                            KullanimSayisi = x.SinavSorular.Count(),
                            AnahtarKelimeler = x.SoruAnahtarlar.Select(xx=>xx.AnahtarKelime.Kelime).ToList(),
                            Puan = x.isCoktanSecmeli 
                                    ? x.SoruSecenekler.Select(xx=>xx.Puan).DefaultIfEmpty(0).Max() 
                                    : x.SoruSecenekler.Select(xx=>xx.Puan).DefaultIfEmpty(0).Sum()
                        }).ToList();

            return View(ss);
        }


        [OturumKontrolFilter]
        public ActionResult Edit(int konu, int id = 0)
        {
            if (konu == 0)
            {
                Hizli.SetHata("Lütfen öncelikle bir konu seçin ve tekrar deneyin.");
                return RedirectToAction("List");
            }

            var db = new DatabaseContext();

            var k = new Soru();
            k.KonuID = konu;

            if (id != 0)
                k = db.Soru.AsNoTracking().Where(x => x.ID == id && !x.isSilindi).FirstOrDefault();

            ViewBag.bransAd = db.Konu.Where(x => x.ID == konu).Select(x => x.Brans.Ad).DefaultIfEmpty("").FirstOrDefault();
            ViewBag.konuAd = db.Konu.Where(x => x.ID == konu).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();

            return View(k);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [OturumKontrolFilter]
        public ActionResult Save(Soru model)
        {
            var aktif = Hizli.GetPersonel();

            var db = new DatabaseContext();
            model.Tarih = DateTime.Now;
            model.PersonelID = aktif.ID;
            
            if (model.ID == 0)
            {
                db.Soru.Add(model);
                db.SaveChanges();

                if (model.isKlasik)
                {
                    // klasik sorularda sadece bir tane cevap girişi olacağı için seçenek ekle butonu görünmeyecek
                    // sorunun cevaı olan seçenek otomatik olarak eklenecek
                    db.SoruSecenek.Add(new SoruSecenek()
                    {
                        SoruID = model.ID,
                        Metin = "",
                        Puan = 0
                    });
                    db.SaveChanges();
                }
            }
            else
            {
                var q = db.Soru.Where(x => x.ID == model.ID && !x.isSilindi).FirstOrDefault();
                if (q != null)
                {
                    q.SoruMetni = model.SoruMetni;
                    q.isCoktanSecmeli = model.isCoktanSecmeli;
                    q.isBoslukDoldurma = model.isBoslukDoldurma;
                    q.isKlasik = model.isKlasik;
                    db.SaveChanges();
                }
                else
                {
                    Hizli.SetHata("İşlem yapılacak Soru bilgisine ulaşılamadı! Lütfen Soru listesini kontrol edip tekrar deneyin.");
                }
            }
            
            return RedirectToAction("Edit", new { id = model.ID, konu=model.KonuID});
        }


        [OturumKontrolFilter]
        public ActionResult Delete(int id)
        {
            var db = new DatabaseContext();
            var q = db.Soru.FirstOrDefault(x => x.ID == id);
            if (q == null)
            {
                Hizli.SetHata("İşlem yapılacak Soru bulunamadı! Lütfen sayfayı yenileyip tekrar deneyin.");
            }
            else
            {
                Hizli.SetHata(q.ID + " ID ile kayıtlı Soru silindi.");
                q.isSilindi = true;
                db.SaveChanges();
            }

            return RedirectToAction("List", new { brans = q == null ? 0 : q.Konu.BransID, konu = q == null ? 0 : q.KonuID });
        }

        #endregion


        #region Seçenek İşlemleri

        [OturumKontrolFilter]
        public JsonResult SetQuestionKeys(int id, string csv)
        {
            try
            {
                if (id == 0)
                    throw new Exception("Soru bilgisi bulunamadı! Lütfen sayfayı yenileyip tekrar deneyin.");

                var db = new DatabaseContext();
                var oncekiAnahtarlar = db.SoruAnahtar.Where(x => x.SoruID == id);
                db.SoruAnahtar.RemoveRange(oncekiAnahtarlar);
                db.SaveChanges();

                var parts = csv.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var soruAnahtarlar = new List<SoruAnahtar>();


                foreach (var p in parts)
                {
                    var kelime = p.Trim().ToLower();
                    var ak = db.AnahtarKelime.FirstOrDefault(x => x.Kelime == kelime);
                    if (ak == null)
                    {
                        ak = new AnahtarKelime()
                        {
                            Kelime = kelime
                        };
                        db.AnahtarKelime.Add(ak);
                        db.SaveChanges();
                    }
                    soruAnahtarlar.Add(new SoruAnahtar() { SoruID = id, AnahtarKelimeID = ak.ID });
                }
                if (soruAnahtarlar.Any())
                {
                    db.SoruAnahtar.AddRange(soruAnahtarlar);
                    db.SaveChanges();
                }

                return Hizli.Jr(true, "Anahtarlar eklendi");
            }
            catch (Exception ex)
            {
                return Hizli.Jr(false, ex.Message);
            }
        }


        [ValidateInput(false)]
        [OturumKontrolFilter]
        public JsonResult UpsertOption(SoruSecenek model) // => SecenekID
        {
            try
            {
                var db = new DatabaseContext();
                if (model.ID > 0)
                {
                    var onceki = db.SoruSecenek.FirstOrDefault(x=>x.ID==model.ID);
                    if (onceki == null)
                        throw new Exception("İşlem yapılacak seçenek bulunamadı. Lütfen sayfayı yenileyip tekrar deneyin.");

                    onceki.Metin = model.Metin;
                    onceki.Puan = model.Puan;
                    db.SaveChanges();
                }
                else
                {
                    if (model.SoruID == 0) 
                        throw new Exception("Seçenek için soru bilgisi bulunamadı!");

                    if (string.IsNullOrEmpty(model.Metin))
                        throw new Exception("Seçenek metni oluşturulmamış!");

                    db.SoruSecenek.Add(model);
                    db.SaveChanges();
                }

                return Hizli.Jr(true, model.ID.ToString());
            }
            catch (Exception ex)
            {
                return Hizli.Jr(false, ex.Message);
            }
        }


        [OturumKontrolFilter]
        public JsonResult RemoveOption(int id)
        {
            try
            {
                var db = new DatabaseContext();

                var onceki = db.SoruSecenek.FirstOrDefault(x => x.ID == id);
                if (onceki == null)
                    throw new Exception("İşlem yapılacak seçenek bulunamadı. Lütfen sayfayı yenileyip tekrar deneyin.");

                db.SoruSecenek.Remove(onceki);
                db.SaveChanges();

                return Hizli.Jr(true, "Seçenek silindi.");
            }
            catch (Exception ex)
            {
                return Hizli.Jr(false, ex.Message);
            }
        }

        #endregion
    }
}