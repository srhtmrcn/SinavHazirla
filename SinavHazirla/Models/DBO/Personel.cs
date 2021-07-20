using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class Personel
    {
        [Key]
        public int ID { get; set; }
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public string Parola { get; set; }
        public bool isSilindi { get; set; }
    }
}