using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.CatalogsViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class CatalogsService : ICatalogsService
    {
        private readonly SuvanDbContext context;

        public CatalogsService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<dynamic> GetCatalog(string catalogName, int IdEmpresa, int nClaveFiltro)
        {
            if (string.IsNullOrWhiteSpace(catalogName))
                throw new ArgumentException("El nombre del catálogo no puede ser vacío.");

            catalogName = catalogName.Trim().ToLower();

            switch (catalogName)
            {
                case "tipoeje":
                    return await ObtenerTipoEje();

                case "vehiculo":
                    return await ObtenerVehiculo(IdEmpresa);

                case "marca":
                    return await ObtenerMarcas();

                case "modelo":
                    return await ObtenerModelos(nClaveFiltro);

                case "tipovehiculo":
                    return await ObtenerTipoVehiculo();

                case "zona":
                    return await ObtenerZona(IdEmpresa);

                case "deposito":
                    return await ObtenerDepositos(nClaveFiltro);

                default:
                    throw new ArgumentException($"Catálogo '{catalogName}' no reconocido.");
            }
        }

        public async Task<List<TiposEjeViewModel>> ObtenerTipoEje()
        {
            var resultado = await (from t in context.TipoEjes
                                   select new TiposEjeViewModel
                                   {
                                       Id = t.IdTipoEje,
                                       Descripcion = t.Descripcion,

                                   }).ToListAsync();
            return resultado;
        }

        public async Task<List<VehiViewModel>> ObtenerVehiculo(int IdEmpresa)
        {
            var resultado = await (from t in context.Vehiculos
                                   where t.EmpresaIdempresa == IdEmpresa
                                   select new VehiViewModel
                                   {
                                       Id = t.IdVehiculo,
                                       Placas = t.Placas,
                                       Vin = t.Vin,
                                       Numeroeconomico = t.Numeroeconomico,
                                       Numeromotor = t.Numeromotor

                                   }).ToListAsync();
            return resultado;
        }

        public async Task<List<MarcasViewModel>> ObtenerMarcas()
        {
            var marca = await (from m in context.Marcas
                               select new MarcasViewModel()
                               {
                                   Id = m.IdMarca,
                                   Descripcion = m.Descripcion,
                                   Modelos = context.Modelos
                                   .Where(d => d.IdMarca == m.IdMarca)
                                   .Select(d => new ModelosViewModel
                                   {
                                       Id = d.IdModelo,
                                       Descripcion = d.Descripcion
                                   }).ToList()
                               }).ToListAsync();

            return marca;
        }

        public async Task<List<ModelosViewModel>> ObtenerModelos(int marcaId)
        {
            var resultado = await (from t in context.Modelos
                                   where
                                   (t.IdMarca == marcaId)
                                   select new ModelosViewModel
                                   {
                                       Id = t.IdModelo,
                                       Descripcion = t.Descripcion

                                   }).ToListAsync();
            return resultado;
        }

        public async Task<List<TipoVehiculosViewModel>> ObtenerTipoVehiculo()
        {
            var resultado = await (from t in context.Tipovehiculos
                                   select new TipoVehiculosViewModel
                                   {
                                       Id = t.Idtipovehiculo,
                                       Descripcion = t.Nombre,

                                   }).ToListAsync();
            return resultado;
        }

        public async Task<List<ZonasViewModel>> ObtenerZona(int IdEmpresa)
        {
            var zona = await (from m in context.Zonas
                              where m.IdEmpresa == IdEmpresa
                              select new ZonasViewModel()
                              {
                                  Id = m.IdZona,
                                  Descripcion = m.NombreZona,
                                  Depositos = context.Depositosdisponibles
                                  .Where(d => d.ZonaId == m.IdZona)
                                  .Select(d => new DepositosViewModel
                                  {
                                      Id = d.IdDeposito,
                                      Descripcion = d.DepositoNombre
                                  }).ToList()
                              }).ToListAsync();

            return zona;
        }

        public async Task<List<DepositosViewModel>> ObtenerDepositos(int IdZona)
        {
            var resultado = await (from t in context.Depositosdisponibles
                                   where
                                   (t.ZonaId == IdZona)
                                   select new DepositosViewModel
                                   {
                                       Id = t.IdDeposito,
                                       Descripcion = t.DepositoNombre

                                   }).ToListAsync();
            return resultado;
        }
    }
}
