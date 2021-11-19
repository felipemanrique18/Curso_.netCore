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
    public class MatriculaController : ControllerBase
    {
        CursosCTX ctx;

        public MatriculaController(CursosCTX _ctx)
        {
            ctx = _ctx;
        }

        public async Task<List<Matricula>> Get()
        {
            return await ctx.Matricula.Include(x => x.Estudiante).Include(x => x.Periodo).ToListAsync();
        }

        [HttpGet("{periodo}/{estudiante}")]
        public async Task<IActionResult> Get(int peridodo, int estudiante)
        {
            var Matricula = await ctx.Matricula.Include(x => x.Periodo).Include(x => x.Estudiante).Where(x => x.IdEstudiante == estudiante && x.IdPeriodo == peridodo).SingleOrDefaultAsync();
            if (Matricula == null)
            {
                return NotFound(ErrorHelper.Response(404, $"El estudiante con identicador {estudiante} no existe en el periodo {peridodo}"));
            }

            return Ok(Matricula);
        }

        [HttpPost("{periodo}/{estudiante}")]
        public async Task<IActionResult> Post(int periodo, int estudiante)
        {
            var Periodo = await ctx.Periodo.AsNoTracking().Where(x => x.IdPeriodo == periodo).SingleOrDefaultAsync();

            if(Periodo == null)
            {
                return NotFound(ErrorHelper.Response(404, "Perdodo no encontrado"));
            }

            if (!Periodo.Estado.Value)
            {
                return BadRequest(ErrorHelper.Response(400, "El periodo se encuentra cerrado"));
            }

            var Estudiante = await ctx.Estudiante.AsNoTracking().Where(x => x.IdEstudiante == estudiante).SingleOrDefaultAsync();

            if(Estudiante == null)
            {
                return NotFound(ErrorHelper.Response(404, "Estudiante no encontrado"));
            }

            if(await ctx.Matricula.Where(x => x.IdPeriodo == periodo && x.IdEstudiante == estudiante).AsNoTracking().AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, "El estudiante ya se encuentra matriculado"));

            }

            ctx.Matricula.Add(new Matricula()
            {
                IdPeriodo = periodo,
                IdEstudiante = estudiante,
                Fecha = DateTime.Now
            });

            await ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { periodo = periodo, estudiante = estudiante },null);
        }

        [HttpDelete("{periodo}/{estudiante}")]
        public async Task<IActionResult> Delete(int periodo, int estudiante)
        {
            var matricula = await ctx.Matricula.FindAsync(estudiante,periodo);
            if(matricula == null) 
            {
                return NotFound(ErrorHelper.Response(404, "El estudiante no se encuentra matriculado"));

            }
            if(!await ctx.Periodo.Where(x => x.IdPeriodo == periodo && x.Estado == true).AsNoTracking().AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, "El periodo se encuentra cerrado no puede eliminar la matricula"));

            }

            if(await ctx.InscripcionCurso.Where(x => x.IdPeriodo == periodo && x.IdEstudiante == estudiante).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, "No se puede elimiar matricula, el estudiante tiene cursos incritos"));
            }

            ctx.Matricula.Remove(matricula);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
