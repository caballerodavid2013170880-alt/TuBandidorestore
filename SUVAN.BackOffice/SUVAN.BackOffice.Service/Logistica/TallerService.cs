using FacturacionPegaso;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.ZonaViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class TallerService : ITallerService
    {
        private readonly SuvanDbContext context;

        public TallerService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Taller>> GetTaller()
        {
            var talleres = await context.Tallers.Include(t => t.ZonaIdzonaNavigation).Include(t => t.IdDepositoNavigation).ToListAsync();

            return talleres;
        }

        /// <summary>
        /// Obtiene el ViewModel del taller específico.
        /// </summary>
        /// <param name="id">Identificador del taller.</param>
        /// <returns>ViewModel para el taller especifico.</returns>
        public async Task<TallerViewModel> GetTallerViewModel(int id, int IdEmpresa)
        {

            var taller = await context.Tallers
                .Where(x => x.IdTaller == id)
                .Select(d => new TallerViewModel
                {
                    IdTaller = d.IdTaller,
                    NombreTaller = d.NombreTaller!,
                    Domicilio = d.Domicilio,
                    Contacto = d.Contacto,
                    Telefono = d.Telefono,
                    Email = d.Email,
                    ZonaIdzona = d.ZonaIdzona,
                    IdDeposito = d.IdDeposito,
                })
                .FirstOrDefaultAsync();

            //var zonas = await context.Zonas
            //    .Select(z => new TallerViewModel.ZonasViewModel
            //    {
            //        ZonaId = z.IdZona,
            //        ZonaNombre = z.NombreZona,
            //        Depositos = context.Depositosdisponibles
            //            .Where(t => t.ZonaId == z.IdZona)
            //            .Select(t => new TallerViewModel.DepositosViewModel
            //            {
            //                DepositoId = t.IdDeposito,
            //                NombreDeposito = t.DepositoNombre
            //            }).ToList()
            //    }).ToListAsync();

            var zonas = await (from z in context.Zonas where z.IdEmpresa == IdEmpresa
                             select new TallerViewModel.ZonasViewModel() {
                                 ZonaId = z.IdZona,
                                 ZonaNombre = z.NombreZona,
                                 Depositos = context.Depositosdisponibles
                                 .Where(d => d.ZonaId == z.IdZona)
                                 .Select(d => new TallerViewModel.DepositosViewModel
                                 {
                                    DepositoId = d.IdDeposito,
                                    NombreDeposito = d.DepositoNombre
                                 }).ToList()
                             }).ToListAsync();

            if (taller != null)
            {
                taller.ZonaView = zonas;
                return taller;
            }

            return new TallerViewModel { ZonaView = zonas };
        }

        /// <summary>
        /// Agrega o actualiza un taller en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarTaller(TallerViewModel model)
        {
            Taller taller;

            if (model.IdTaller > 0)
            {
                taller = await context.Tallers.FirstOrDefaultAsync(x => x.IdTaller == model.IdTaller);

                if (taller == null)
                    throw new Exception("No se encontro el Taller");

            }
            else
            {
                taller = new Taller();
            }

            // valida si un depósito existe con el mismo nombre
            var tallerExistente = await context.Tallers.FirstOrDefaultAsync(x =>
            x.NombreTaller!.ToLower() == model.NombreTaller!.ToLower()
            && x.IdTaller != model.IdTaller);

            if (tallerExistente is not null)
                throw new Exception("Ya existe un taller con el mismo nombre");

            taller.IdTaller = model.IdTaller;
            taller.NombreTaller = model.NombreTaller;
            taller.Domicilio = model.Domicilio;
            taller.Contacto = model.Contacto;
            taller.Telefono = model.Telefono;
            taller.Email = model.Email;
            taller.ZonaIdzona = model.ZonaIdzona;
            taller.IdDeposito = model.IdDeposito;

            if (model.IdTaller > 0)
            {
                context.Tallers.Entry(taller);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Tallers.Add(taller);

                await context.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Elimina un taller en la base de datos.
        /// </summary>
        /// <param name="TallerId">Identificador del taller.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarTaller(int TallerId)
        {
            var taller = await context.Tallers.FirstOrDefaultAsync(x => x.IdTaller == TallerId);

            if (taller is null)
            {
                throw new Exception("No se encontro el Taller");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Tallers
              .Where(x => x.IdTaller == TallerId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

        public List<TallerViewModel.DepositosViewModel> ObtenerDeposito(int zonaId)
        {
            var deposito = context.Depositosdisponibles
                .Where(t => t.ZonaId == zonaId)
                .Select(t => new TallerViewModel.DepositosViewModel
                {
                    DepositoId = t.IdDeposito,
                    NombreDeposito = t.DepositoNombre
                }).ToList();

            return deposito;
        }
    }
}
