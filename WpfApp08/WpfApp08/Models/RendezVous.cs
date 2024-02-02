using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp08.Models1;
using WpfApp08.Models2;

namespace WpfApp08.Models3
{
    public class RendezVous
    {
        [Required]
        [Key]
        public int IdRendezVous { get; set; }
        [ForeignKey("Patients")]   
        public int PatientId { get; set; }
        public virtual Patients Patients { get; set; }

        [ForeignKey("Medecins")]
        public int MedecinId { get; set; }

        public virtual Medecins Medecins { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string InfosComplementaires { get; set; }
    }

    public class ajoutRendezVous
    {
        public int IdRendezVous { get; set; }
        [ForeignKey("Patients")]
        public int PatientId { get; set; }

        [ForeignKey("Medecins")]

        public int MedecinId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string InfosComplementaires { get; set; }
    }
}
