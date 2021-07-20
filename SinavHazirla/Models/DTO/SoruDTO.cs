using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class SoruDTO
    {
        public int SoruID { get; set; }
        public DateTime Tarih { get; set; }
        public string PersonelAd { get; set; }
        public bool isBoslukDoldurma { get; set; }
        public bool isCoktanSecmeli { get; set; }
        public bool isKlasik { get; set; }
        public int Puan { get; set; }
        public int KullanimSayisi { get; set; }
        public List<string> AnahtarKelimeler { get; set; }
        public int SoruNo { get; set; }
    }
}