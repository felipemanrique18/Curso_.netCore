using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursos.Models.ViewModel
{
    public class MatriculaEstudianteVM
    {
        public int IdPeriodo { get; set; }

        public int periodo { get; set; }

        public List<EstudiantesVM> Estudiantes{ get; set; }
    }
}
