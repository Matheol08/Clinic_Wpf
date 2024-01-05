using WpfApp08.Models3;
using WpfApp08.Models2;
using WpfApp08.Models1;
using WpfApp08.Models4;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace WpfApp08
{
    public class AnnuaireContext : DbContext
    {
        public AnnuaireContext(DbContextOptions<AnnuaireContext> options)
            : base(options)
        {

        }

        public DbSet<Salaries> Salaries { get; set; }
        public DbSet<Specialites> Specialites { get; set; }
        public DbSet<Medecins> Medecins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.\SQLExpress;Database=Annuaire;Trusted_Connection=True;TrustServerCertificate=true;");
            }

        }
    }
}
