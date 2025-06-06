using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class MecanicoService : IMecanicoService
    {
        private readonly SuvanDbContext context;

        public MecanicoService(SuvanDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene el listado de los Mecanicos desde la base de datos.
        /// </summary>
        /// <returns>Lista de Mecanicos</returns>
        public async Task<List<Mecanico>> GetMecanico()
        {
            var mecanico = await context.Mecanicos.Include(d => d.IdDepositoNavigation).Include(t => t.IdTallerNavigation).ToListAsync();

            return mecanico!;
        }
        /// <summary>
        /// Obtiene el ViewModel del mecanico específico.
        /// </summary>
        /// <param name="id">Identificador del mecanico.</param>
        /// <returns>ViewModel para el mecanico especifico.</returns>
        public async Task<MecanicoViewModel> GetMecanicoViewModel(int id, int IdEmpresa)
        {
            var mecanico = await context.Mecanicos
                .Where(x => x.IdMecanico == id)
                .Select(d => new MecanicoViewModel
                {
                    IdMecanico = d.IdMecanico,
                    Nombre = d.Nombre!,
                    Puesto = d.Puesto,
                    Activo = d.Activo,
                    IdTaller = d.IdTaller,
                    IdDeposito = d.IdDeposito,
                })
                .FirstOrDefaultAsync();

            var deposito = await (from z in context.Depositosdisponibles
                               where z.IdEmpresa == IdEmpresa
                               select new MecanicoViewModel.DepositosViewModel()
                               {
                                   DepositoId = z.IdDeposito,
                                   NombreDeposito = z.DepositoNombre,
                                   Talleres = context.Tallers
                                   .Where(d => d.IdDeposito == z.IdDeposito)
                                   .Select(d => new MecanicoViewModel.TallerViewModel
                                   {
                                       IdTaller = d.IdTaller,
                                       NombreTaller = d.NombreTaller
                                   }).ToList()
                               }).ToListAsync();

            if (mecanico != null)
            {
                mecanico.DepositoView = deposito;
                return mecanico;
            }

            return new MecanicoViewModel { DepositoView = deposito };
        }

        /// <summary>
        /// Agrega o actualiza un mecanico en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del mecanico.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarMecanico(MecanicoViewModel model)
        {
            Mecanico mecanico;

            if (model.IdMecanico > 0)
            {
                mecanico = await context.Mecanicos.FirstOrDefaultAsync(x => x.IdMecanico == model.IdMecanico);

                if (mecanico == null)
                    throw new Exception("No se encontro el Mecanico");

            }
            else
            {
                mecanico = new Mecanico();
            }
            

            // valida si un Mecanico existe con el mismo nombre
            var mecanicoExistente = await context.Mecanicos.FirstOrDefaultAsync(x =>
            x.Nombre!.ToLower() == model.Nombre!.ToLower()
            && x.IdMecanico != model.IdMecanico);

            if (mecanicoExistente is not null)
                throw new Exception("Ya existe un mecanico con el mismo nombre");

            mecanico.IdMecanico = model.IdMecanico;
            mecanico.Nombre = model.Nombre;
            mecanico.Puesto = model.Puesto;
            mecanico.IdTaller = model.IdTaller;
            mecanico.Activo = model.Activo;
            mecanico.IdDeposito = model.IdDeposito;

            if (model.IdMecanico > 0)
            {
                context.Mecanicos.Entry(mecanico);

                await context.SaveChangesAsync();
            }
            else
            {
                context.Mecanicos.Add(mecanico);

                await context.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Elimina un mecanico en la base de datos.
        /// </summary>
        /// <param name="MecanicoId">Identificador del mecanico.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarMecanico(int MecanicoId)
        {
            var mecanico = await context.Mecanicos.FirstOrDefaultAsync(x => x.IdMecanico == MecanicoId);

            if (mecanico is null)
            {
                throw new Exception("No se encontro el Mecanico");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.Mecanicos
              .Where(x => x.IdMecanico == MecanicoId)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

        public List<MecanicoViewModel.TallerViewModel> ObtenerTaller(int depositoId)
        {
            var taller = context.Tallers
                .Where(t => t.IdDeposito == depositoId)
                .Select(t => new MecanicoViewModel.TallerViewModel
                {
                    IdTaller = t.IdTaller,
                    NombreTaller = t.NombreTaller
                }).ToList();

            return taller;
        }
    }
}
