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

        public async Task<List<Depositosdisponible>> GetDepositos(int IdEmpresa)
        {
            var depositos = await context.Depositosdisponibles.Where(e => e.IdEmpresa == IdEmpresa).Include(em => em.IdEmpresaNavigation).Include(d => d.Zona).ToListAsync();

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

            DepositosDisponiblesViewModel vRet = new DepositosDisponiblesViewModel();
            var deposito = await context.Depositosdisponibles.FirstOrDefaultAsync(x => x.IdDeposito == id);

            if (deposito == null)
                return vRet;
            else
            {
                vRet = new DepositosDisponiblesViewModel
                {
                    DepositoId = deposito.IdDeposito!,
                    NombreDeposito = deposito.DepositoNombre!,
                    ZonaId = deposito.ZonaId!,
                    Activo = deposito.Activo!,
                    Dirección = deposito.Dirección!,
                    Ciudad = deposito.Ciudad!,
                    Responsable = deposito.Responsable!,
                    Teléfono = deposito.Teléfono!,
                    NombreCorto = deposito.NombreCorto!,
                    Rfc = deposito.Rfc!,
                    Cp = deposito.Cp!,
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
        public async Task<bool> AgregarDeposito(DepositosDisponiblesViewModel model, int IdEmpresa)
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
                throw new Exception("Ya existe un Depósito con el mismo nombre");

            // Valida si el RFC esta duplicado en la misma empresa
            var zonaExistenteRFC = await context.Depositosdisponibles.FirstOrDefaultAsync(x =>
                x.Rfc!.Trim().ToLower() == model.Rfc!.Trim().ToLower() &&
                x.IdEmpresa == IdEmpresa &&
                x.IdDeposito != model.DepositoId);

            if (zonaExistenteRFC is not null)
                throw new Exception("Ya existe un Depósito con el mismo RFC");

            deposito.IdDeposito = model.DepositoId!;
            deposito.DepositoNombre = model.NombreDeposito!;
            deposito.ZonaId = model.ZonaId!;
            deposito.Activo = model.Activo ? true : false;
            deposito.Dirección = model.Dirección!;
            deposito.Ciudad = model.Ciudad!;
            deposito.Responsable = model.Responsable!;
            deposito.Teléfono = model.Teléfono!;
            deposito.NombreCorto = model.NombreCorto!;
            deposito.IdEmpresa = IdEmpresa;
            deposito.LocFor = model.LocFor;
            deposito.RPerson = model.RPerson;
            deposito.Rfc = model.Rfc!;
            deposito.Cp = model.Cp!;


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
        /// Obtiene el ViewModel de la Zona
        /// </summary>
        /// <param name="zonaId">Identificador del Taller.</param>
        /// <returns>ViewModel para el Taller especifico.</returns>

        public List<DepositosDisponiblesViewModel.ZonasViewModel> ObtenerZona(int IdEmpresa)
        {
            var resul = (from o in context.Zonas
                         where o.IdEmpresa == IdEmpresa
                         select new DepositosDisponiblesViewModel.ZonasViewModel()
                         {
                             ZonaId = o.IdZona,
                             ZonaNombre = o.NombreZona
                         }).ToList();

            return resul;
        }

    }
}
