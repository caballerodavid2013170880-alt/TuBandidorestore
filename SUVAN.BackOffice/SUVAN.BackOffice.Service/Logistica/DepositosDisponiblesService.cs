using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class DepositosDisponiblesService : IDepositosDisponibles
    {
        private readonly SuvanDbContext context;

        public DepositosDisponiblesService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Depositosdisponible>> GetDepositos()
        {
            var depositos = await context.Depositosdisponibles.ToListAsync();

            return depositos!;
        }

        /// <summary>
        /// Obtiene el ViewModel del depósito específico.
        /// </summary>
        /// <param name="id">Identificador del deposito.</param>
        /// <returns>ViewModel para el depósito especifico.</returns>
        public async Task<DepositosDisponiblesViewModel> GetDepositoViewModel(int id)
        {
            DepositosDisponiblesViewModel vRet = new DepositosDisponiblesViewModel();
            var deposito = await context.Depositosdisponibles.FirstOrDefaultAsync(x => x.IdDeposito == id);

            if (deposito == null)
                return vRet;
            else
            {
                vRet = new DepositosDisponiblesViewModel
                {
                    DepositoId = deposito.IdDeposito,
                    ZonaId = deposito.ZonaId!,
                    ZonaNombre = deposito.ZonaNombre!,
                    NombreDeposito = deposito.DepositoNombre!,
                    TallerId = deposito.TalleId!,
                    NombreTaller = deposito.TallerNombre!,
                    Activo = deposito.Activo == true,

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
        public async Task<bool> AgregarDeposito(DepositosDisponiblesViewModel model)
        {
            Depositosdisponible deposito;

            if (model.DepositoId > 0)
            {
                deposito = await context.Depositosdisponibles.FirstOrDefaultAsync(x => x.IdDeposito == model.DepositoId);

                if (deposito == null)
                    throw new Exception("No se encontro el Depósito");

            }
            else
            {
                deposito = new Depositosdisponible();
            }

            // valida si un depósito existe con el mismo nombre
            var depositoExistente = await context.Depositosdisponibles.FirstOrDefaultAsync(x =>
            x.DepositoNombre!.ToLower() == model.NombreDeposito!.ToLower()
            && x.IdDeposito != model.DepositoId);

            if (depositoExistente is not null)
                throw new Exception("Ya existe un depósito con el mismo nombre");

            deposito.ZonaId = model.ZonaId;
            deposito.ZonaNombre = model.ZonaNombre;
            deposito.DepositoNombre = model.NombreDeposito;
            deposito.TalleId = model.TallerId;
            deposito.TallerNombre = model.NombreTaller;
            deposito.Activo = model.Activo ? true : false;

            if (model.DepositoId > 0)
            {
                context.Depositosdisponibles.Entry(deposito);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Depositosdisponibles.Add(deposito);
                await context.SaveChangesAsync();

                await context.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Elimina un depósito en la base de datos.
        /// </summary>
        /// <param name="DepositoId">Identificador del deposito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarDeposito(int DepositoId)
        {
            var deposito = await context.Depositosdisponibles.FirstOrDefaultAsync(x => x.IdDeposito == DepositoId);

            if (deposito is null)
            {
                throw new Exception("No se encontro el Depósito");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Depositosdisponibles
              .Where(x => x.IdDeposito == DepositoId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }
    }
}
