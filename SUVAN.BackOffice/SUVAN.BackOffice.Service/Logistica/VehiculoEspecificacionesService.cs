using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

            var query = from modelo in context.Modelos
                        join marca in context.Marcas on modelo.IdMarca equals marca.IdMarca
                        join espec in context.VehiculoEspecificaciones
                            on modelo.IdModelo equals espec.IdModelo into especGroup
                        from espec in especGroup.DefaultIfEmpty()
                        where espec == null || espec.IdMarca == marca.IdMarca
                        select new VehiculoEspecifiDescripcionViewModel
                        {
                            IdEspecificaciones = espec.IdEspecificaciones,
                            IdMarca = marca.IdMarca,
                            DescripcionMarca = marca.Descripcion,
                            IdModelo = modelo.IdModelo,
                            DescripcionModelo = modelo.Descripcion,
                            CapacidadCombu = espec.CapacidadCombu,
                            TipoMotor = espec.TipoMotor,
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

            var imagenes = await context.VehiculoEspecificacionesImgs.Where(x => x.IdEspecificaciones == id).OrderBy(x => x.Consecutivo)
                .Select(x => new EspecificacionesImgView
                {
                    Ruta = x.Ruta,
                    Consecutivo = x.Consecutivo
                })
                .ToListAsync();

            return new VehiculoEspecificacionesViewModel
            {
                IdEspecificaciones = especifi.IdEspecificaciones,
                IdMarca = especifi.IdMarca,
                IdModelo = especifi.IdModelo,
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
                Observaciones = especifi.Observaciones,
                Imagenes = imagenes
            };
        }



        /// <summary>
        /// Agrega o actualiza un registro Vehiculo Especificaciones en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de Vehiculo Especificaciones.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarVehiculoEspecificaciones(VehiculoEspecificacionesViewModel model)
        {
            VehiculoEspecificacione vehiculo;

            if (model.IdEspecificaciones > 0)
            {
                vehiculo = await context.VehiculoEspecificaciones.FirstOrDefaultAsync(x => x.IdEspecificaciones == model.IdEspecificaciones);

                if (vehiculo == null)
                    throw new Exception("No se encontraron registros");

            }
            else
            {
                vehiculo = new VehiculoEspecificacione();
            }

            vehiculo.IdEspecificaciones = model.IdEspecificaciones;
            vehiculo.IdMarca = model.IdMarca;
            vehiculo.IdModelo = model.IdModelo;
            vehiculo.Ancho = model.Ancho;
            vehiculo.Largo = model.Largo;
            vehiculo.Altura = model.Altura;
            vehiculo.PesoBruto = model.PesoBruto;
            vehiculo.ToneladasCarga = model.ToneladasCarga;
            vehiculo.MetrosCubCarga = model.MetrosCubCarga;
            vehiculo.Pallets = model.Pallets;
            vehiculo.TipoMotor = model.TipoMotor;
            vehiculo.PotenciaMotor = model.PotenciaMotor;
            vehiculo.NoCilindros = model.NoCilindros;
            vehiculo.CapacidadAceite = model.CapacidadAceite;
            vehiculo.CapacidadCombu = model.CapacidadCombu;
            vehiculo.RenEsp = model.RenEsp;
            vehiculo.TipoCombustible = model.TipoCombustible;
            vehiculo.Transmision = model.Transmision;
            vehiculo.Traccion = model.Traccion;
            vehiculo.TipoEje = model.TipoEje;
            vehiculo.CargaPorEje = model.CargaPorEje;
            vehiculo.CargaMax = model.CargaMax;
            vehiculo.TotalLlantas = model.TotalLlantas;
            vehiculo.LlantasRepuesto = model.LlantasRepuesto;
            vehiculo.DimensionLlantas = model.DimensionLlantas;
            vehiculo.PulCub = model.PulCub;
            vehiculo.Origen = model.Origen;
            vehiculo.Observaciones = model.Observaciones;

            if (model.IdEspecificaciones > 0)
            {
                context.VehiculoEspecificaciones.Entry(vehiculo);

                await context.SaveChangesAsync();
            }
            else
            {
                context.VehiculoEspecificaciones.Add(vehiculo);
                await context.SaveChangesAsync();

            }
            return true;
        }

        public async Task GuardarImagenesAsync(VehiculoEspecificacionesViewModel model, List<IFormFile> archivosImagen, string webRootPath)
        {
            if (archivosImagen == null || archivosImagen.Count == 0)
                return;

            string carpetaMarca = Path.Combine(webRootPath, "assets", "img", model.IdMarca.ToString());
            string carpetaModelo = Path.Combine(carpetaMarca, model.IdModelo.ToString());

            if (!Directory.Exists(carpetaModelo))
                Directory.CreateDirectory(carpetaModelo);

            var consecutivos = await context.VehiculoEspecificacionesImgs.Where(x => x.IdEspecificaciones == model.IdEspecificaciones).Select(x => x.Consecutivo)
                .ToListAsync();

            int maxConsecutivo = (consecutivos.Any() ? consecutivos.Max() : 0);

            foreach (var archivo in archivosImagen)
            {
                if (archivo == null || archivo.Length == 0)
                    continue;

                string extension = Path.GetExtension(archivo.FileName);
                string archivoNuevo = $"{Guid.NewGuid():N}{extension}";
                string rutaFisica = Path.Combine(carpetaModelo, archivoNuevo);
                string rutaRelativa = $"/assets/img/{model.IdMarca}/{model.IdModelo}/{archivoNuevo}";

                await using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                maxConsecutivo++;

                var nuevaImg = new VehiculoEspecificacionesImg
                {
                    IdEspecificaciones = model.IdEspecificaciones,
                    Consecutivo = maxConsecutivo,
                    Ruta = rutaRelativa
                };

                context.VehiculoEspecificacionesImgs.Add(nuevaImg);
            }

            await context.SaveChangesAsync();
        }



    }
}
