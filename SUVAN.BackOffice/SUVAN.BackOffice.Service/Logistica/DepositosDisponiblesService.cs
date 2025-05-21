using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.DepositosDisponiblesViewModel;

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
            var depositos = await context.Depositosdisponibles.Include(d => d.Zona).Include(d => d.Taller).ToListAsync();

            return depositos!;
        }

        /// <summary>
        /// Obtiene el ViewModel del depósito específico.
        /// </summary>
        /// <param name="id">Identificador del deposito.</param>
        /// <returns>ViewModel para el depósito especifico.</returns>
        /// 

        public async Task<DepositosDisponiblesViewModel> GetDepositoViewModel(int depositoId)
        {
            DepositosDisponiblesViewModel depositosUnidadViewModel = await GetDeposito(depositoId);

            return depositosUnidadViewModel;

        }

        public async Task<DepositosDisponiblesViewModel> GetDeposito(int id)
        {
            var deposito = await context.Depositosdisponibles
                .Where(x => x.IdDeposito == id)
                .Select(d => new DepositosDisponiblesViewModel
                {
                    DepositoId = d.IdDeposito,
                    NombreDeposito = d.DepositoNombre!,
                    ZonaId = d.ZonaId,
                    TallerId = d.TallerId,
                    Activo = d.Activo == true
                })
                .FirstOrDefaultAsync();

            var zonas = await context.Zonas
                .Select(z => new ZonasViewModel
                {
                    ZonaId = z.IdZona,
                    ZonaNombre = z.NombreZona,
                    Talleres = context.Tallers
                        .Where(t => t.ZonaIdzona == z.IdZona)
                        .Select(t => new TalleresViewModel
                        {
                            IdTaller = t.IdTaller,
                            NombreTaller = t.NombreTaller
                        }).ToList()
                }).ToListAsync();

            if (deposito != null)
            {
                deposito.ZonasView = zonas;
                return deposito;
            }

            return new DepositosDisponiblesViewModel { ZonasView = zonas };
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
            deposito.DepositoNombre = model.NombreDeposito;
            deposito.TallerId = model.TallerId;
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

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Depositosdisponibles
              .Where(x => x.IdDeposito == DepositoId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }


        /// <summary>
        /// Obtiene el ViewModel del Taller
        /// </summary>
        /// <param name="zonaId">Identificador del Taller.</param>
        /// <returns>ViewModel para el Taller especifico.</returns>

        public List<DepositosDisponiblesViewModel.TalleresViewModel> ObtenerTaller(int zonaId)
        {
            var talleres = context.Tallers
                .Where(t => t.ZonaIdzona == zonaId)
                .Select(t => new DepositosDisponiblesViewModel.TalleresViewModel
                {
                    IdTaller = t.IdTaller,
                    NombreTaller = t.NombreTaller
                }).ToList();

            return talleres;
        }

    }
}
