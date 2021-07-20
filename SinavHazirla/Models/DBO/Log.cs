using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class Log
    {
        [Key]
        public int ID { get; set; }
        public DateTime Tarih { get; set; }
        public string Aciklama { get; set; }
        public int PersonelID { get; set; }


        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }
    }
}