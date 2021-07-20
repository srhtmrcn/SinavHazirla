using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class AnahtarKelime
    {
        [Key]
        public int ID { get; set; }
        public string Kelime { get; set; }
    }
}