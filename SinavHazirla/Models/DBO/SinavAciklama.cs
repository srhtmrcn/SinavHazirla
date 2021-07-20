using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class SinavAciklama
    {
        [Key]
        public int ID { get; set; }
        public int SinavID { get; set; }
        public int PersonelID { get; set; }
        public DateTime Tarih { get; set; }
        public string Aciklama { get; set; }


        [ForeignKey("SinavID")]
        public virtual Sinav Sinav { get; set; }


        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }
    }
}