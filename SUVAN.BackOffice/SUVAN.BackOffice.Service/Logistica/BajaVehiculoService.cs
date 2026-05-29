using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class BajaVehiculoService : IBajaVehiculoService
    {
        private readonly SuvanDbContext context;

        public BajaVehiculoService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<BajaVehi>> GetBajaVehiculo()
        {

            var baja = await context.BajaVehis.ToListAsync();

            return baja!;
        }

        /// <summary>
        /// Obtiene el ViewModel de la Baja del Vehiculo específico.
        /// </summary>
        /// <param name="idBaja">Identificador de la Baja del Vehiculo.</param>
        /// <returns>ViewModel de la Baja especifica.</returns>
        public async Task<BajaVehiViewModel> GetBajaVehiculoViewModel(int idBaja)
        {
            BajaVehiViewModel vRet = new BajaVehiViewModel();
            var baja = await context.BajaVehis.FirstOrDefaultAsync(x => x.IdBaja == idBaja);

            if (baja == null)
                return vRet;
            else
            {
                vRet = new BajaVehiViewModel
                {
                    IdBaja = baja.IdBaja!,
                    Descripcion = baja.Descripcion!,
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza una baja en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de la baja.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarBajaVehiculo(BajaVehiViewModel model)
        {
            BajaVehi baja;

            if (model.IdBaja > 0)
            {
                baja = await context.BajaVehis.FirstOrDefaultAsync(x => x.IdBaja == model.IdBaja);

                if (baja == null)
                    throw new Exception("No se encontro la Baja");

            }
            else
            {
                baja = new BajaVehi();
            }

            // Valida si la descripción de la baja esta duplicado
            var bajaExistenteDescripcion = await context.BajaVehis.FirstOrDefaultAsync(x =>
            x.Descripcion!.ToLower() == model.Descripcion!.ToLower()
            && x.IdBaja != model.IdBaja);

            if (bajaExistenteDescripcion is not null)
                throw new Exception("Ya existe una Baja con la misma descripción");

            baja.Descripcion = model.Descripcion;


            if (model.IdBaja > 0)
            {
                context.BajaVehis.Entry(baja);

                await context.SaveChangesAsync();
            }
            else
            {
                context.BajaVehis.Add(baja);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina una baja en la base de datos.
        /// </summary>
        /// <param name="IdBaja">Identificador de la baja.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarBajaVehiculo(int IdBaja)
        {
            var baja = await context.BajaVehis.FirstOrDefaultAsync(x => x.IdBaja == IdBaja);

            if (baja is null)
            {
                throw new Exception("No se encontro la Baja");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.BajaVehis
              .Where(x => x.IdBaja == IdBaja)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
