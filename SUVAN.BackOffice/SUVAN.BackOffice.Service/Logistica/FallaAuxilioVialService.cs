using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class FallaAuxilioVialService : IFallaAuxilioVial
    {
        private readonly SuvanDbContext context;

        public FallaAuxilioVialService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de motivos de auxilio desde la base de datos.
        /// </summary>
        /// <returns>Lista de Motivos de Auxilios.</returns>
        public async Task<List<FallaAuxilioVial>> GetFallaAuxilioVial()
        {
            var falla_auxilio = await context.FallaAuxilioVials.ToListAsync();

            return falla_auxilio!;

        }

        /// <summary>
        /// Obtiene el ViewModel del Motivo de Auxilio específico.
        /// </summary>
        /// <param name="id">Identificador del Motivo.</param>
        /// <returns>ViewModel para el Motivo de Auxilio especifico.</returns>
        public async Task<FallaAuxilioVialViewModel> GetFallaAuxilioViewModel(int id)
        {
            FallaAuxilioVialViewModel vRet = new FallaAuxilioVialViewModel();
            var falla = await context.FallaAuxilioVials.FirstOrDefaultAsync(x => x.Idfalla == id);

            if (falla == null)
                return vRet;
            else
            {
                vRet = new FallaAuxilioVialViewModel
                {
                    FallaId = falla.Idfalla,
                    Nombre = falla.Nombre!

                };
            }

            return vRet;

        }

        /// <summary>
        /// Agrega o actualiza un depósito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del depósito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarFallaAuxilioVial(FallaAuxilioVialViewModel model)
        {
            FallaAuxilioVial falla;

            if (model.FallaId > 0)
            {
                falla = await context.FallaAuxilioVials.FirstOrDefaultAsync(x => x.Idfalla == model.FallaId);

                if (falla == null)
                    throw new Exception("No se encontro el Depósito");

            }
            else
            {
                falla = new FallaAuxilioVial();
            }

            // valida si un motivo existe con el mismo nombre
            var fallaExistente = await context.FallaAuxilioVials.FirstOrDefaultAsync(x =>
            x.Nombre!.ToLower() == model.Nombre!.ToLower()
            && x.Idfalla != model.FallaId);

            if (fallaExistente is not null)
                throw new Exception("Ya existe una falla con el mismo nombre");

            falla.Idfalla = model.FallaId;
            falla.Nombre = model.Nombre;

            if (model.FallaId > 0)
            {
                context.FallaAuxilioVials.Entry(falla);

                await context.SaveChangesAsync();
            }
            else
            {
                context.FallaAuxilioVials.Add(falla);
                await context.SaveChangesAsync();

                await context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> EliminarFallaAuxilioVial(int FallaId)
        {
            var falla = await context.FallaAuxilioVials.FirstOrDefaultAsync(x => x.Idfalla == FallaId);

            if (falla is null)
            {
                throw new Exception("No se encontro la Falla de Auxilio");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.FallaAuxilioVials
              .Where(x => x.Idfalla == FallaId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
