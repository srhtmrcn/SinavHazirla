using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla.Controllers
{
    public class PersonelController : Controller
    {
        #region Giriş ve Anasayfa

        public ActionResult Giris()
        {
            return View();
        }
        
        public ActionResult OturumAc(string email, string parola)
        {
            var db = new DatabaseContext();
            var personel = db.Personel.FirstOrDefault(x => x.Email == email && x.Parola == parola && !x.isSilindi);
            if (personel == null)
            {
                Hizli.SetPersonel(null);
                Hizli.SetHata("Kullanıcı adı veya parola hatalı!");
                return RedirectToAction("Giris");
            }
            else
            {
                Hizli.SetPersonel(personel);
                return RedirectToAction("AnaSayfa");
            }
        }

        public ActionResult OturumuKapat()
        {
            Session.Abandon();
            return RedirectToAction("Giris");
        }


        [OturumKontrolFilter]
        public ActionResult AnaSayfa()
        {
            return View();
        }

        #endregion


        #region Personel İşlemleri


        [OturumKontrolFilter]
        public ActionResult List()
        {
            var db = new DatabaseContext();
            var p = db.Personel.AsNoTracking().Where(x => !x.isSilindi).ToList();
            return View(p);
        }


        [OturumKontrolFilter]
        public ActionResult Edit(int id=0)
        {
            var db = new DatabaseContext();
            var p = new Personel();
            if (id != 0)
                p = db.Personel.AsNoTracking().Where(x => x.ID == id && !x.isSilindi).FirstOrDefault();

            return View(p);
        }

        [ValidateAntiForgeryToken]
        [OturumKontrolFilter]
        public ActionResult Save(Personel personel)
        {
            var db = new DatabaseContext();
            if (personel.ID == 0)
            {
                db.Personel.Add(personel);
                db.SaveChanges();
            }
            else
            {
                var p = db.Personel.Where(x => x.ID == personel.ID && !x.isSilindi).FirstOrDefault();
                if (p != null)
                {
                    p.AdSoyad = personel.AdSoyad;
                    p.Email = personel.Email;
                    p.Parola = personel.Parola;
                    db.SaveChanges();
                }
                else
                {
                    Hizli.SetHata("İşlem yapılacak personel bilgisine ulaşılamadı! Lütfen personel listesini kontrol edip tekrar deneyin.");
                }
            }

            return RedirectToAction("List");
        }


        [OturumKontrolFilter]
        public ActionResult Delete(int id)
        {
            var db = new DatabaseContext();
            var p = db.Personel.FirstOrDefault(x => x.ID == id);
            if (p == null)
            {
                Hizli.SetHata("İşlem yapılacak personel bulunamadı! Lütfen sayfayı yenileyip tekrar deneyin.");
            }
            else
            {
                Hizli.SetHata(p.AdSoyad + " isimli personel silindi.");
                p.isSilindi = true;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        #endregion

    }
}