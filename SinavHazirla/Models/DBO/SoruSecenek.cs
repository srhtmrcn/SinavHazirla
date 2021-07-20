using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class SoruSecenek
    {
        [Key]
        public int ID { get; set; }
        public int SoruID { get; set; }
        public string Metin { get; set; }
        public int Puan { get; set; }


        [ForeignKey("SoruID")]
        public virtual Soru Soru { get; set; }
    }
}