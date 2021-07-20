using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinavHazirla
{
    public static class Hizli
    {
        public static void SetPersonel(Personel personel)
        {
            HttpContext.Current.Session["personel"] = personel;
        }

        public static Personel GetPersonel()
        {
            if (HttpContext.Current == null) return new Personel();
            var p = HttpContext.Current.Session["personel"];
            return p == null ? new Personel() : (Personel)p;
        }
        
        public static void SetHata(string mesaj)
        {
            HttpContext.Current.Session["hata"] = mesaj;
        }

        public static string GetHata()
        {
            if (HttpContext.Current == null) return "";
            var p = HttpContext.Current.Session["hata"];
            SetHata(null);
            return p == null ? "" : (string)p;
        }

        public static JsonResult Jr(bool Durum, string Mesaj)
        {
            return new JsonResult()
            {
                Data = new { Durum = Durum, Mesaj = Mesaj },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}