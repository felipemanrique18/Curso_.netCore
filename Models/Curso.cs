using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cursos.Models
{
    public partial class Curso
    {
        public Curso()
        {
            InscripcionCurso = new HashSet<InscripcionCurso>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCurso { get; set; }

        [StringLength(10)]
        [MaxLength (10, ErrorMessage = "El codigo debe ser de maximo de 10 caracteres")]
        [MinLength(2, ErrorMessage = "El codigo debe ser de minimo de 2 caracteres")]
        public string Codigo { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        public bool? Estado { get; set; }

        public virtual ICollection<InscripcionCurso> InscripcionCurso { get; set; }
    }
}
