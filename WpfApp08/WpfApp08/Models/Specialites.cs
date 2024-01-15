using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp08.Models4
{
   
        public class Specialites
        {
            [Required]
            [Key]
            public int IdSpecialite { get; set; }
            public string Libelle { get; set; }


        }
}

