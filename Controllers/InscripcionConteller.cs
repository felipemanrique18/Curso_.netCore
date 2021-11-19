using Cursos.Helper;
using Cursos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InscripcionConteller : ControllerBase
    {

        public readonly CursosCTX ctx;

        public InscripcionConteller(CursosCTX _ctx)
        {
            ctx = _ctx;
        }

        public async Task<List<InscripcionCurso>> Get()
        {
            return await ctx.InscripcionCurso.Include(X=> X.Curso).Include(x => x.Matricula).Include(x => x.Periodo).ToListAsync();
        }

        [HttpGet("{periodo}/{estudiante/}curso}")]
        public async Task<IActionResult> Get(int periodo, int estudiante, string curso)
        {
            var inscripcion = await ctx.InscripcionCurso.Include(X => X.Curso).Include(x => x.Matricula).Include(x => x.Periodo)
                .Where(x => x.IdEstudiante == estudiante && x.Curso.Codigo == curso && x.IdPeriodo == periodo).SingleOrDefaultAsync();

            if(inscripcion == null)
            {
                return NotFound(ErrorHelper.Response(404, " El curso no se encuentra inscrito"));
            }

            return Ok(inscripcion);
        }


    }
}
