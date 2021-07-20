using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class Brans
    {
        [Key]
        public int ID { get; set; }
        public string Ad { get; set; }
        public bool isSilindi { get; set; }

        public virtual ICollection<Konu> Konular{ get; set; }

    }
}