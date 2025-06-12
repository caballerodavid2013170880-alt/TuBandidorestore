/*
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class TipoSiniestroService : ITipoSiniestroService
    {
        private readonly SuvanDbContext context;

        public TipoSiniestroService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de los Tipos de Siniestro desde la base de datos.
        /// </summary>
        /// <returns>Lista de Tipos de Siniestro.</returns>
        public async Task<List<TipoSiniestroService>> GetTipoSiniestro()
        {
            return await context.TipoSiniestros.ToListAsync();
        }

        /// <summary>
        /// Obtiene el ViewModel de un siniestro específico.
        /// </summary>
        /// <param name="id">Identificador del siniestro.</param>
        /// <returns>ViewModel para el siniestro específico.</returns>
        public async Task<CausaSiniestroViewModel.TipoSiniestroViewModel> GetTipoSiniestroViewModel(int id)
        {
            var siniestro = await context.TipoSiniestros.FirstOrDefaultAsync(x => x.Id_tipo_siniestro == id);
            if (siniestro == null) return new CausaSiniestroViewModel.TipoSiniestroViewModel();

            return new CausaSiniestroViewModel.TipoSiniestroViewModel
            {
                Id_tipo_siniestro = siniestro.Id_tipo_siniestro,
                Id_causa_siniestro = siniestro.Id_causa_siniestro,
                Descripcion = siniestro.Descripcion
            };
        }

        /// <summary>
        /// Agrega o actualiza un siniestro en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del siniestro.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTipoSiniestro(CausaSiniestroViewModel.TipoSiniestroViewModel model)
        {
            TipoSiniestro siniestro;

            if (model.Id_tipo_siniestro > 0)
            {
                siniestro = await context.TipoSiniestros.FirstOrDefaultAsync(x => x.Id_tipo_siniestro == model.Id_tipo_siniestro);
                if (siniestro == null) throw new Exception("No se encontró el Siniestro");
            }
            else
            {
                siniestro = new TipoSiniestro();
            }

            var siniestroExistente = await context.TipoSiniestros.FirstOrDefaultAsync(x =>
                x.Descripcion.ToLower() == model.Descripcion.ToLower() && x.Id_tipo_siniestro != model.Id_tipo_siniestro);

            if (siniestroExistente is not null)
                throw new Exception("Ya existe un Siniestro con la misma descripción");

            siniestro.Id_causa_siniestro = model.Id_causa_siniestro;
            siniestro.Descripcion = model.Descripcion;

            if (model.Id_tipo_siniestro > 0)
            {
                context.TipoSiniestros.Update(siniestro);
            }
            else
            {
                context.TipoSiniestros.Add(siniestro);
            }

            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina un siniestro en la base de datos.
        /// </summary>
        /// <param name="IdTipoSiniestro">Identificador del siniestro.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> EliminarTipoSiniestro(int IdTipoSiniestro)
        {
            var siniestro = await context.TipoSiniestros.FirstOrDefaultAsync(x => x.Id_tipo_siniestro == IdTipoSiniestro);

            if (siniestro is null) throw new Exception("No se encontró el Siniestro");

            var delete = await context.TipoSiniestros.Where(x => x.Id_tipo_siniestro == IdTipoSiniestro).ExecuteDeleteAsync();
            await context.SaveChangesAsync();

            return delete > 0;
        }

        Task<List<TipoSiniestro>> ITipoSiniestroService.GetTipoSiniestro()
        {
            throw new NotImplementedException();
        }
    }
}
*/