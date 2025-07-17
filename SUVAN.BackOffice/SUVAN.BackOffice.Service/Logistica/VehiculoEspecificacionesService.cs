using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
                    IdEspecificaciones = x.IdEspecificaciones,
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
            if (archivosImagen?.Count == 0)
                return;

            string carpetaModelo = Path.Combine(webRootPath, "assets", "img", model.IdMarca.ToString(), model.IdModelo.ToString());
            Directory.CreateDirectory(carpetaModelo); 

            int maxConsecutivo = await context.VehiculoEspecificacionesImgs
                .Where(x => x.IdEspecificaciones == model.IdEspecificaciones)
                .MaxAsync(x => (int?)x.Consecutivo) ?? 0;

            foreach (var archivo in archivosImagen.Where(f => f?.Length > 0))
            {
                string extension = Path.GetExtension(archivo.FileName);
                string nombreArchivo = $"{Guid.NewGuid():N}{extension}";
                string rutaFisica = Path.Combine(carpetaModelo, nombreArchivo);
                string rutaRelativa = $"/assets/img/{model.IdMarca}/{model.IdModelo}/{nombreArchivo}";

                await using var stream = new FileStream(rutaFisica, FileMode.Create);
                await archivo.CopyToAsync(stream);

                context.VehiculoEspecificacionesImgs.Add(new VehiculoEspecificacionesImg
                {
                    IdEspecificaciones = model.IdEspecificaciones,
                    Consecutivo = ++maxConsecutivo,
                    Ruta = rutaRelativa
                });
            }

            await context.SaveChangesAsync();
        }


        public async Task<List<VehiculoEspecificacionesViewModel>> ObtenerDetalleModal(int IdEspecificaciones)
        {
            var resultado = await context.VehiculoEspecificaciones
                .Where(v => v.IdEspecificaciones == IdEspecificaciones)
                .Select(v => new VehiculoEspecificacionesViewModel
                {
                    IdEspecificaciones = v.IdEspecificaciones,
                    IdMarca = v.IdMarca,
                    IdModelo = v.IdModelo,
                    Ancho = v.Ancho,
                    Largo = v.Largo,
                    Altura = v.Altura,
                    PesoBruto = v.PesoBruto,
                    ToneladasCarga = v.ToneladasCarga,
                    MetrosCubCarga = v.MetrosCubCarga,
                    Pallets = v.Pallets,
                    TipoMotor = v.TipoMotor,
                    PotenciaMotor = v.PotenciaMotor,
                    NoCilindros = v.NoCilindros,
                    CapacidadAceite = v.CapacidadAceite,
                    CapacidadCombu = v.CapacidadCombu,
                    RenEsp = v.RenEsp,
                    TipoCombustible = v.TipoCombustible,
                    Transmision = v.Transmision,
                    Traccion = v.Traccion,
                    TipoEje = v.TipoEje,
                    CargaPorEje = v.CargaPorEje,
                    CargaMax = v.CargaMax,
                    TotalLlantas = v.TotalLlantas,
                    LlantasRepuesto = v.LlantasRepuesto,
                    DimensionLlantas = v.DimensionLlantas,
                    PulCub = v.PulCub,
                    Origen = v.Origen,
                    Observaciones = v.Observaciones,

                    Imagenes = context.VehiculoEspecificacionesImgs
                        .Where(img => img.IdEspecificaciones == v.IdEspecificaciones)
                        .OrderBy(img => img.Consecutivo)
                        .Select(img => new EspecificacionesImgView
                        {
                            IdEspecificaciones = img.IdEspecificaciones,
                            Ruta = img.Ruta,
                            Consecutivo = img.Consecutivo
                        }).ToList()
                })
                .ToListAsync();
            return resultado;
        }

        public async Task EliminarImagenesAsync(List<(int idEspecificacion, int consecutivo)> idsParaEliminar, string webRootPath)
        {
            if (idsParaEliminar == null || !idsParaEliminar.Any())
                return;

            foreach (var (idEspecificacion, consecutivo) in idsParaEliminar)
            {
                var imagen = await context.VehiculoEspecificacionesImgs
                    .FirstOrDefaultAsync(x => x.IdEspecificaciones == idEspecificacion && x.Consecutivo == consecutivo);

                if (imagen != null)
                {
                    var rutaFisica = Path.Combine(webRootPath, imagen.Ruta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(rutaFisica))
                    {
                        try
                        {
                            File.Delete(rutaFisica);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    context.VehiculoEspecificacionesImgs.Remove(imagen);
                }
            }

            await context.SaveChangesAsync();
        }

    }
}
