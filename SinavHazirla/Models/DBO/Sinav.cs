using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SinavHazirla
{
    public class Sinav
    {
        [Key]
        public int ID { get; set; }


        public int BransID { get; set; }
        public string Ad { get; set; }
        public int PersonelID { get; set; }
        public DateTime Tarih { get; set; }
        public string Aciklama { get; set; }

        
        [ForeignKey("BransID")]
        public virtual Brans Brans { get; set; }


        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }

        public virtual ICollection<SinavSoru> SinavSorular { get; set; }

        public virtual ICollection<SinavAciklama> SinavAciklamalar { get; set; }

    }
}