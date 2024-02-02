using WpfApp08.Models3;
using WpfApp08.Models2;
using WpfApp08.Models1;
using WpfApp08.Models4;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace WpfApp08
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(DbContextOptions<ClinicContext> options)
            : base(options)
        {

        }

        public DbSet<RendezVous> RendezVous { get; set; }
        public DbSet<Specialites> Specialites { get; set; }
        public DbSet<Medecins> Medecins { get; set; }
        public DbSet<Patients> Patients { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-4QTSD8Q;Database=Clinic;Trusted_Connection=True;TrustServerCertificate=true" );
            }

        }
    }
}
