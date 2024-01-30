using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp08.Models3;
using WpfApp08.Models4;
using Newtonsoft.Json;
using System.Windows.Navigation;

namespace WpfApp08.Models1
{
    public class Medecins
    {

        [Required]
        [Key]

        public int IdMedecin { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        [ForeignKey("Specialites")]
        public int SpecialiteId { get; set; }

        public virtual  Specialites Specialites { get; set; }

    }
}