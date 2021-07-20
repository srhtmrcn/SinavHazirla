using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class Konu
    {
        [Key]
        public int ID { get; set; }
        public int BransID { get; set; }
        public string Ad { get; set; }
        public bool isSilindi { get; set; }


        [ForeignKey("BransID")]
        public virtual Brans Brans { get; set; }

        public virtual ICollection<Soru> Sorular { get; set; }
    }
}