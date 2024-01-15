using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp08.Models3
{
    public class RendezVous
    {
        [Required]
        [Key]
        public int IdRendezVous { get; set; }
        [ForeignKey("IdPatient")]
        public int IdPatient { get; set; }
        [ForeignKey("IdMedecin")]

        public int MedecinId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string InfosComplementaires { get; set; }
    }
}
