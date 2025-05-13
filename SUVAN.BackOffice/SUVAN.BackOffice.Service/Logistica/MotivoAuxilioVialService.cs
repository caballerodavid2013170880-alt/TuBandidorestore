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
    public class MotivoAuxilioVialService : IMotivoAuxilioVialService
    {
        private readonly SuvanDbContext context;

        public MotivoAuxilioVialService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de motivos de auxilio desde la base de datos.
        /// </summary>
        /// <returns>Lista de Motivos de Auxilios.</returns>
        public async Task<List<MotivoAuxilioVial>> GetMotivoAuxilioVial()
        {
            var motivo_auxilio = await context.MotivoAuxilioVials.ToListAsync();

            return motivo_auxilio!;

        }

        /// <summary>
        /// Obtiene el ViewModel del Motivo de Auxilio específico.
        /// </summary>
        /// <param name="id">Identificador del Motivo.</param>
        /// <returns>ViewModel para el Motivo de Auxilio especifico.</returns>
        public async Task<MotivoAuxilioVialViewModel> GetMotivoAuxilioViewModel(int id)
        {
            MotivoAuxilioVialViewModel vRet = new MotivoAuxilioVialViewModel();
            var motivo = await context.MotivoAuxilioVials.FirstOrDefaultAsync(x => x.Idmotivo == id);

            if (motivo == null)
                return vRet;
            else
            {
                vRet = new MotivoAuxilioVialViewModel
                {
                    MotivoId = motivo.Idmotivo,
                    Nombre = motivo.Nombre!

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
        public async Task<bool> AgregarMotivoAuxilioVial(MotivoAuxilioVialViewModel model)
        {
            MotivoAuxilioVial motivo;

            if (model.MotivoId > 0)
            {
                motivo = await context.MotivoAuxilioVials.FirstOrDefaultAsync(x => x.Idmotivo == model.MotivoId);

                if (motivo == null)
                    throw new Exception("No se encontro el Depósito");

            }
            else
            {
                motivo = new MotivoAuxilioVial();
            }

            // valida si un motivo existe con el mismo nombre
            var motivoExistente = await context.MotivoAuxilioVials.FirstOrDefaultAsync(x =>
            x.Nombre!.ToLower() == model.Nombre!.ToLower()
            && x.Idmotivo != model.MotivoId);

            if (motivoExistente is not null)
                throw new Exception("Ya existe un motivo con el mismo nombre");

            motivo.Idmotivo = model.MotivoId;
            motivo.Nombre = model.Nombre;

            if (model.MotivoId > 0)
            {
                context.MotivoAuxilioVials.Entry(motivo);

                await context.SaveChangesAsync();
            }
            else
            {
                context.MotivoAuxilioVials.Add(motivo);
                await context.SaveChangesAsync();

                await context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> EliminarMotivoAuxilioVial(int MotivoId)
        {
            var motivo = await context.MotivoAuxilioVials.FirstOrDefaultAsync(x => x.Idmotivo == MotivoId);

            if (motivo is null)
            {
                throw new Exception("No se encontro el Motivo de Auxilio");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.MotivoAuxilioVials
              .Where(x => x.Idmotivo == MotivoId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

    }
}
