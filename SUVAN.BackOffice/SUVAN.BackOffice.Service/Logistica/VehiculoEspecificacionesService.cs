using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoEspecificacionesViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class VehiculoEspecificacionesService : IVehiculoEspecificacionesService
    {
        private readonly SuvanDbContext context;

        public VehiculoEspecificacionesService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<MarcaEspecifiViewModel>> ObtenerMarcas()
        {
            var marca = await (from m in context.Marcas
                               select new MarcaEspecifiViewModel()
                               {
                                   IdMarca = m.IdMarca,
                                   Descripcion = m.Descripcion,
                                   Modelos = context.Modelos
                                   .Where(d => d.IdMarca == m.IdMarca)
                                   .Select(d => new ModeloEspecifiViewModel
                                   {
                                       IdModelo = d.IdModelo,
                                       Descripcion = d.Descripcion
                                   }).ToList()
                               }).ToListAsync();

            return marca;
        }

        public async Task<List<ModeloEspecifiViewModel>> ObtenerModelo(int marcaId)
        {
            var resultado = await (from t in context.Modelos
                                   where
                                   (t.IdMarca == marcaId)
                                   select new ModeloEspecifiViewModel
                                   {
                                       IdModelo = t.IdModelo,
                                       Descripcion = t.Descripcion

                                   }).ToListAsync();
            return resultado;
        }

        public async Task<VehiculoEspecificacionesViewModel> GetVehiculoEspecifi()
        {
            var model = new VehiculoEspecificacionesViewModel();

            List<MarcaEspecifiViewModel> marcas = await ObtenerMarcas();
            List<ModeloEspecifiViewModel> modelos = await ObtenerModelo(model.IdMarca ?? 0);

            model.Marcas = marcas;
            model.Modelos = modelos;

            return model;

        }

        public async Task<VehiculoEspecificacionesViewModel> ObtenerEspecificaciones(VehiculoEspecificacionesViewModel model)
        {
            model.Marcas = await ObtenerMarcas();

            if (model.IdMarca.HasValue && model.IdMarca > 0)
                model.Modelos = await ObtenerModelo(model.IdMarca.Value);
            else
                model.Modelos = new List<ModeloEspecifiViewModel>();

            var query = from e in context.VehiculoEspecificaciones
                        join m in context.Marcas on e.IdMarca equals m.IdMarca
                        join n in context.Modelos on e.IdModelo equals n.IdModelo
                        select new VehiculoEspecifiDescripcionViewModel
                        {
                            IdEspecificaciones = e.IdEspecificaciones, 
                            IdMarca = m.IdMarca,
                            IdModelo = n.IdModelo,
                            CapacidadCombu = e.CapacidadCombu,
                            TipoMotor = e.TipoMotor,
                        };

            if (model.IdMarca.HasValue && model.IdMarca > 0)
                query = query.Where(x => x.IdMarca == model.IdMarca.Value);

            if (model.IdModelo.HasValue && model.IdModelo > 0)
                query = query.Where(x => x.IdModelo == model.IdModelo.Value);

            model.Descripcion = await query.ToListAsync();

            return model;
        }


        /// <summary>
        /// Obtiene el ViewModel del VehiculoEspecificaciones específico.
        /// </summary>
        /// <param name="id">Identificador del VehiculoEspecificaciones.</param>
        /// <returns>ViewModel para el VehiculoEspecificaciones especifico.</returns>
        public async Task<VehiculoEspecificacionesViewModel> GetVehiculoEspecifiViewModel(int id)
        {
            var especifi = await context.VehiculoEspecificaciones.FirstOrDefaultAsync(x => x.IdEspecificaciones == id);

            if (especifi == null)
                return new VehiculoEspecificacionesViewModel();

            return new VehiculoEspecificacionesViewModel
            {
                IdEspecificaciones = especifi.IdEspecificaciones,
                Ancho = especifi.Ancho,
                Largo = especifi.Largo,
                Altura = especifi.Altura,
                PesoBruto = especifi.PesoBruto,
                ToneladasCarga = especifi.ToneladasCarga,
                MetrosCubCarga = especifi.MetrosCubCarga,
                Pallets = especifi.Pallets,
                TipoMotor = especifi.TipoMotor,
                PotenciaMotor = especifi.PotenciaMotor,
                NoCilindros = especifi.NoCilindros,
                CapacidadAceite = especifi.CapacidadAceite,
                CapacidadCombu = especifi.CapacidadCombu,
                RenEsp = especifi.RenEsp,
                TipoCombustible = especifi.TipoCombustible,
                Transmision = especifi.Transmision,
                Traccion = especifi.Traccion,
                TipoEje = especifi.TipoEje,
                CargaPorEje = especifi.CargaPorEje,
                CargaMax = especifi.CargaMax,
                TotalLlantas = especifi.TotalLlantas,
                LlantasRepuesto = especifi.LlantasRepuesto,
                DimensionLlantas = especifi.DimensionLlantas,
                PulCub = especifi.PulCub,
                Origen = especifi.Origen,
                Observaciones = especifi.Observaciones
            };
        }

    }
}
