using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class Soru
    {
        [Key]
        public int ID { get; set; }
        public int PersonelID { get; set; }
        public DateTime Tarih { get; set; }
        public int KonuID { get; set; }
        public string SoruMetni { get; set; }
        public bool isCoktanSecmeli { get; set; }
        public bool isBoslukDoldurma { get; set; }
        public bool isKlasik { get; set; }
        public bool isSilindi { get; set; }




        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }

        [ForeignKey("KonuID")]
        public virtual Konu Konu { get; set; }

        public virtual ICollection<SoruSecenek> SoruSecenekler { get; set; }

        public virtual ICollection<SinavSoru> SinavSorular { get; set; }
        public virtual ICollection<SoruAnahtar> SoruAnahtarlar { get; set; }

    }
}