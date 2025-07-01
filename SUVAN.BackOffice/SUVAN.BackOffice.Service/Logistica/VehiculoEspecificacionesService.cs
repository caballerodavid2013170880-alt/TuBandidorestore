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
            List<ModeloEspecifiViewModel> modelos = await ObtenerModelo(model.IdMarca);

            model.Marcas = marcas;
            model.Modelos = modelos;

            return model;

        }

        public async Task<VehiculoEspecificacionesViewModel> ObtenerEspecificaciones(VehiculoEspecificacionesViewModel model, int idModelo)
        {
            model.Marcas = await ObtenerMarcas();
            model.Modelos = await ObtenerModelo(model.IdMarca);

            var search = await (from e in context.VehiculoEspecificaciones
                                join m in context.Marcas on e.IdMarca equals m.IdMarca
                                join n in context.Modelos on e.IdModelo equals n.IdModelo
                                where e.IdModelo == idModelo select new VehiculoEspecifiDescripcionViewModel
                                { 
                                    IdMarca = m.IdMarca,
                                    IdModelo = n.IdModelo,
                                    CapacidadCombu = e.CapacidadCombu,
                                    TipoMotor = e.TipoMotor,
                                }).ToListAsync();

            model.Descripcion = search;

            return model;
        }
    }
}
