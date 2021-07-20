using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class SoruAnahtar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 0)]
        public int SoruID { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 1)]
        public int AnahtarKelimeID { get; set; }


        [ForeignKey("SoruID")]
        public virtual Soru Soru { get; set; }


        [ForeignKey("AnahtarKelimeID")]
        public virtual AnahtarKelime AnahtarKelime{ get; set; }
    }
}