using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class SinavDTO
    {
        public int SinavID { get; set; }
        public string SinavAd { get; set; }
        public string Hazirayan { get; set; }
        public DateTime Tarih { get; set; }
    }
}