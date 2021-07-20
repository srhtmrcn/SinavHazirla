using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class SinavSoru
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 0)]
        public int SinavID { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 1)]
        public int SoruID { get; set; }
        public int SoruNo { get; set; }


        [ForeignKey("SinavID")]
        public virtual Sinav Sinav { get; set; }


        [ForeignKey("SoruID")]
        public virtual Soru Soru { get; set; }


    }
}