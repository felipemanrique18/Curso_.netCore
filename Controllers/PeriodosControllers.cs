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
    public class PeriodosController : ControllerBase
    {
        public readonly CursosCTX ctx;

        public PeriodosController(CursosCTX _ctx)
        {
            ctx = _ctx;
        }


        public async Task<IEnumerable<Periodo>> Get()
        {
            return await ctx.Periodo.ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var Periodo = await ctx.Periodo.FindAsync(id);

            if (Periodo == null)
            {
                return NotFound(ErrorHelper.Response(404, "El periodo no existe"));
            }
            return Ok(Periodo);
        }

        [HttpGet("activo")]
        public async Task<IActionResult> GetActivo()
        {
            var periodo = await ctx.Periodo.Where(x => x.Estado == true).OrderByDescending(x => x.Anio).FirstOrDefaultAsync();

            if (periodo == null)
            {
                return NotFound(ErrorHelper.Response(404, "No existen periodos abiertos"));
            }

            return Ok(periodo);

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Periodo periodo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }

            if (await ctx.Periodo.Where(x => x.Anio == periodo.Anio).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El año {periodo.Anio} ya existe"));
            }

            ctx.Periodo.Add(periodo);
            await ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = periodo.IdPeriodo }, periodo);
        }

        [HttpPatch("activar/{id}")]
        public async Task<IActionResult> Activar(int id)
        {
            using (var transaction = ctx.Database.BeginTransaction())
            {
                try
                {
                    var periodo = await ctx.Periodo.FindAsync(id);

                    if (periodo == null)
                    {
                        return NotFound(ErrorHelper.Response(404, "No existen este periodo"));
                    }

                    if (periodo.Estado.Value)
                    {
                        await transaction.RollbackAsync();
                        return NoContent();
                    }

                    var periodos = await ctx.Periodo.Where(x => x.IdPeriodo != id).ToListAsync();
                    periodos.ForEach(x => x.Estado = false);
                    periodo.Estado = true;
                    await ctx.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok();

                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, ErrorHelper.Response(500, "Ha ocurrido un error en la base de datos"));
                }
            }
        }

        [HttpPatch("desactivar/{id}")]
        public async Task<IActionResult> Desactivar(int id)
        {

            var periodo = await ctx.Periodo.FindAsync(id);

            if (periodo == null)
            {
                return NotFound(ErrorHelper.Response(404, "No existen este periodo"));
            }

            if (!periodo.Estado.Value)
            {
                return NoContent();
            }

            periodo.Estado = false;
            await ctx.SaveChangesAsync();
            return Ok();

        }
    }
}
