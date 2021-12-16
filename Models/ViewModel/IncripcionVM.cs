using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursos.Models.ViewModel
{
    public class IncripcionVM
    {
        public int IdEstudiante { get; set; }
        public string Codigo { get; set; }

        public Periodo Periodo { get; set; }
        public Curso Curso { get; set; }
    }
}
