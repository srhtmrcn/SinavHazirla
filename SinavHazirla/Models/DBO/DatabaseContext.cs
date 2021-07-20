using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SinavHazirla
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DbConnectStr")
        {

        }

        public DbSet<AnahtarKelime> AnahtarKelime { get; set; }
        public DbSet<Brans> Brans { get; set; }
        public DbSet<Konu> Konu { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Personel> Personel { get; set; }
        public DbSet<Sinav> Sinav { get; set; }
        public DbSet<SinavAciklama> SinavAciklama { get; set; }
        public DbSet<SinavSoru> SinavSoru { get; set; }
        public DbSet<Soru> Soru { get; set; }
        public DbSet<SoruAnahtar> SoruAnahtar { get; set; }
        public DbSet<SoruSecenek> SoruSecenek { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}