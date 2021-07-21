using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla.Controllers
{
    public class SinavController : Controller
    {
        #region Sınav İşlemleri


        [OturumKontrolFilter]
        public ActionResult List(int brans = 0)
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

            var ss = db.Sinav.AsNoTracking()
                       .Where(x => x.BransID == brans)
                       .Select(x => new SinavDTO()
                       {
                           SinavID = x.ID,
                           SinavAd = x.Ad,
                           Hazirayan = x.Personel.AdSoyad,
                           Tarih = x.Tarih,
                       })
                       .ToList();

            ViewBag.brans = brans;
            ViewBag.bransAd = brans == 0 ? "" : db.Brans.Where(x => x.ID == brans).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();
            ViewBag.branslar = bs;
            return View(ss);
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

            var k = new Sinav();
            k.BransID = brans;

            if (id != 0)
                k = db.Sinav.AsNoTracking().Where(x => x.ID == id).FirstOrDefault();

            ViewBag.bransAd = db.Brans.Where(x => x.ID == brans).Select(x => x.Ad).DefaultIfEmpty("").FirstOrDefault();

            return View(k);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [OturumKontrolFilter]
        public ActionResult Save(Sinav model)
        {
            var aktif = Hizli.GetPersonel();
            var db = new DatabaseContext();
            model.Tarih = DateTime.Now;
            model.PersonelID = aktif.ID;

            if (model.ID == 0)
            {
                db.Sinav.Add(model);
                db.SaveChanges();
            }
            else
            {
                var k = db.Sinav.Where(x => x.ID == model.ID).FirstOrDefault();
                if (k != null)
                {
                    k.Ad = model.Ad;
                    k.Aciklama = model.Aciklama;
                    db.SaveChanges();
                }
                else
                {
                    Hizli.SetHata("İşlem yapılacak Sınav bilgisine ulaşılamadı! Lütfen Sınav listesini kontrol edip tekrar deneyin.");
                }
            }

            return RedirectToAction("List", new { brans = model.BransID });
        }


        [OturumKontrolFilter]
        public ActionResult Delete(int id)
        {
            var db = new DatabaseContext();
            var s = db.Sinav.FirstOrDefault(x => x.ID == id);
            if (s == null)
            {
                Hizli.SetHata("İşlem yapılacak Sınav bulunamadı! Lütfen sayfayı yenileyip tekrar deneyin.");
            }
            else
            {
                Hizli.SetHata(s.Ad + " isimli Sınav silindi." + s.ID);
                var sss = db.SinavSoru.Where(x => x.SinavID == id);

                db.SinavSoru.RemoveRange(sss);
                db.Sinav.Remove(s);
                db.SaveChanges();
            }
            return RedirectToAction("List", new { brans = s.BransID });
        }

        #endregion

        #region Sınava Soru Ekleme İşlemleri

        [OturumKontrolFilter]
        public ActionResult ExamQuestions(int id)
        {
            var db = new DatabaseContext();

            var s = db.Sinav.FirstOrDefault(x => x.ID == id);
            if (s == null)
            {
                Hizli.SetHata("İşlem yapılacak sınav bulunamadı!");
                return RedirectToAction("List");
            }

            ViewBag.sinavID = id;
            ViewBag.sinavAd = s.Ad;
            ViewBag.personelID = s.PersonelID;
            ViewBag.bransID = s.BransID;
            ViewBag.bransAd = s.Brans.Ad;
            ViewBag.anahtarlar = db.AnahtarKelime.Select(x => x.Kelime).ToList();

            var sorular = db.SinavSoru.AsNoTracking()
                .Where(ss => ss.SinavID == id)
                .Select(ss => new SoruDTO()
                {
                    SoruID = ss.SoruID,
                    SoruNo = ss.SoruNo,
                    Tarih = ss.Soru.Tarih,
                    PersonelAd = ss.Soru.Personel.AdSoyad,
                    isBoslukDoldurma = ss.Soru.isBoslukDoldurma,
                    isCoktanSecmeli = ss.Soru.isCoktanSecmeli,
                    isKlasik = ss.Soru.isKlasik,
                    KullanimSayisi = ss.Soru.SinavSorular.Count(),
                    AnahtarKelimeler = ss.Soru.SoruAnahtarlar.Select(xx => xx.AnahtarKelime.Kelime).ToList(),
                    Puan = ss.Soru.isCoktanSecmeli
                        ? ss.Soru.SoruSecenekler.Select(xx => xx.Puan).DefaultIfEmpty(0).Max()
                        : ss.Soru.SoruSecenekler.Select(xx => xx.Puan).DefaultIfEmpty(0).Sum()
                })
                .OrderBy(x=>x.SoruNo)
              .ToList();

            return View(sorular);
        }

        [OturumKontrolFilter]
        public PartialViewResult GetQuestionsByKeys(int id, int brans, string csv)
        {
            var db = new DatabaseContext();

            csv = csv.Replace(", ", ","); // kelime zincirindeki ayraçtan sonra gelen boşlukları kaldır.
            csv = csv.ToLower(); // anahtar kelimeler küçük harf olarak kayıt edildi.

            var soruIDs = db.Database.SqlQuery<int>($"exec dbo.AnahtariUyanSorular '{csv}'").ToList();

            var sorular = db.Soru.AsNoTracking()
                            .Where(s => soruIDs.Contains(s.ID)
                                    && !s.SinavSorular.Any(x=>x.SinavID == id)
                                    && s.Konu.BransID == brans
                            )
                            .Select(s=> new SoruDTO()
                           {
                               SoruID = s.ID,
                               Tarih = s.Tarih,
                               PersonelAd = s.Personel.AdSoyad,
                               isBoslukDoldurma = s.isBoslukDoldurma,
                               isCoktanSecmeli = s.isCoktanSecmeli,
                               isKlasik = s.isKlasik,
                               KullanimSayisi = s.SinavSorular.Count(),
                               AnahtarKelimeler = s.SoruAnahtarlar.Select(xx => xx.AnahtarKelime.Kelime).ToList(),
                               Puan = s.isCoktanSecmeli
                                    ? s.SoruSecenekler.Select(xx => xx.Puan).DefaultIfEmpty(0).Max()
                                    : s.SoruSecenekler.Select(xx => xx.Puan).DefaultIfEmpty(0).Sum()
                           })
                          .ToList();

            return PartialView(sorular);
        }

        [OturumKontrolFilter]
        public JsonResult UpsertQuesitons(int id, List<int> qids)
        {
            if(id>0 && qids.Count() > 0)
            {
                var sss = new List<SinavSoru>();

                var db = new DatabaseContext();
                foreach (var q in qids)
                {
                    if (!db.SinavSoru.Any(x=>x.SinavID == id && x.SoruID == q))
                        sss.Add(new SinavSoru() { SinavID = id, SoruID = q, SoruNo = 1 });
                }

                if (sss.Count() > 0)
                {
                    SinavAciklamaEkle(id, "Sınava " + sss.Count() + " soru eklendi.");
                    db.SinavSoru.AddRange(sss);
                    db.SaveChanges();
                }

                return Hizli.Jr(true, $"Seçilen soru{(qids.Count()>0?"lar":"")} sınava eklendi!");
            }
            else
                return Hizli.Jr(false, "Gerekli parametreler hatalı! Lütfen sayfayı yenileyip tekrar deneyin.");
        }

        [OturumKontrolFilter]
        public JsonResult RemoveQuestion(int id, int qid)
        { 
            var db = new DatabaseContext();
            var q = db.SinavSoru.FirstOrDefault(x => x.SinavID == id && x.SoruID == qid);

            if (q != null)
            {
                SinavAciklamaEkle(id, "Sınavdan " + q.SoruID + " ID'li soru çıkarıldı.");
                db.SinavSoru.Remove(q);
                db.SaveChanges();
            }

            return Hizli.Jr(true, "Seçilen soru sınavdan çıkarıldı!");
        }

        [OturumKontrolFilter]
        public JsonResult SetQuestionNumber(int id, int qid, int qnumber)
        {
            var db = new DatabaseContext();
            var q = db.SinavSoru.FirstOrDefault(x => x.SinavID == id && x.SoruID == qid);

            if (q != null)
            {
                q.SoruNo = qnumber;
                db.SaveChanges();
            }

            return Hizli.Jr(true, "Soru numarası güncellendi!");
        }

        #endregion

        public PartialViewResult ShowQuestion(int id)
        {
            ViewBag.id = id;
            return PartialView();
        }

        public JsonResult AddDescription(int id, string aciklama)
        {
            SinavAciklamaEkle(id, aciklama);
            return Hizli.Jr(true,"");
        }

        private void SinavAciklamaEkle(int sinavID, string aciklama)
        {
            var aktif = Hizli.GetPersonel();
            var ack = new SinavAciklama()
            {
                PersonelID = aktif.ID,
                SinavID = sinavID,
                Aciklama = aciklama,
                Tarih = DateTime.Now
            };
            var db = new DatabaseContext();
            db.SinavAciklama.Add(ack);
            db.SaveChanges();
        }


        public PartialViewResult ShowDescriptions(int id)
        {
            ViewBag.id = id;
            return PartialView();
        }


        #region Sınav Yazdırma İşlemleri


        public ActionResult GetExam(int id)
        {
            SinavAciklamaEkle(id, "Sınav PDF indirildi");
            return new ActionAsPdf("GetPDF", new { id, isCevapAnahtari = false }) { FileName = "Sinav.pdf" };
        }

        public ActionResult GetSolutions(int id)
        {
            SinavAciklamaEkle(id, "Cevap Anahtarı PDF indirildi");
            return new ActionAsPdf("GetPDF", new { id, isCevapAnahtari = true }) { FileName = "Cevaplar.pdf" };
        }

        public ActionResult GetPDF(int id, bool isCevapAnahtari = false)
        {
            ViewBag.id = id;
            ViewBag.isCevapAnahtari = isCevapAnahtari;
            return View();
        }

        #endregion
    }
}