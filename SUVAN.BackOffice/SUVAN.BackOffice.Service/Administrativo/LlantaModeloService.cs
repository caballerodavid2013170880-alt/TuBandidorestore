using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class LlantaModeloService : ILlantaModeloService
    {
        private readonly SuvanDbContext context;

        public LlantaModeloService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<LlantaModeloViewModel>> GetModelos()
        {
            return await context.LlantaModelos
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.IdMarcaLlantaNavigation.Nombre)
                .ThenBy(x => x.Nombre)
                .ThenBy(x => x.Medida)
                .Select(x => new LlantaModeloViewModel
                {
                    IdModeloLlanta = x.IdModeloLlanta,
                    IdMarcaLlanta = x.IdMarcaLlanta,
                    Marca = x.IdMarcaLlantaNavigation.Nombre,
                    Nombre = x.Nombre,
                    Medida = x.Medida,
                    PresionMinimaPsi = x.PresionMinimaPsi,
                    PresionMaximaPsi = x.PresionMaximaPsi,
                    ProfundidadOriginalMm = x.ProfundidadOriginalMm,
                    VidaUtilEstimadaKm = x.VidaUtilEstimadaKm,
                    Activo = x.Activo == true,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion
                })
                .ToListAsync();
        }

        public async Task<LlantaModeloViewModel> GetModeloViewModel(uint idModeloLlanta)
        {
            var modelo = await context.LlantaModelos
                .AsNoTracking()
                .Where(x => x.IdModeloLlanta == idModeloLlanta)
                .Select(x => new LlantaModeloViewModel
                {
                    IdModeloLlanta = x.IdModeloLlanta,
                    IdMarcaLlanta = x.IdMarcaLlanta,
                    Marca = x.IdMarcaLlantaNavigation.Nombre,
                    Nombre = x.Nombre,
                    Medida = x.Medida,
                    PresionMinimaPsi = x.PresionMinimaPsi,
                    PresionMaximaPsi = x.PresionMaximaPsi,
                    ProfundidadOriginalMm = x.ProfundidadOriginalMm,
                    VidaUtilEstimadaKm = x.VidaUtilEstimadaKm,
                    Activo = x.Activo == true,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion
                })
                .FirstOrDefaultAsync();

            return await GetCrearViewModel(modelo ?? new LlantaModeloViewModel());
        }

        public async Task<LlantaModeloViewModel> GetCrearViewModel(LlantaModeloViewModel? model = null)
        {
            var vRet = model ?? new LlantaModeloViewModel();

            vRet.Marcas = await context.LlantaMarcas
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.Nombre)
                .Select(x => new LlantaModeloViewModel.CatalogItemViewModel
                {
                    Id = x.IdMarcaLlanta,
                    Nombre = x.Nombre
                })
                .ToListAsync();

            return vRet;
        }

        public async Task<bool> CrearModelo(LlantaModeloViewModel model, int idUsuario)
        {
            var nombre = await ValidarModelo(model);

            var modelo = new LlantaModelo
            {
                IdMarcaLlanta = model.IdMarcaLlanta,
                Nombre = nombre,
                Medida = LimpiarTextoOpcional(model.Medida),
                PresionMinimaPsi = model.PresionMinimaPsi,
                PresionMaximaPsi = model.PresionMaximaPsi,
                ProfundidadOriginalMm = model.ProfundidadOriginalMm,
                VidaUtilEstimadaKm = model.VidaUtilEstimadaKm,
                Activo = true,
                FechaCreacion = DateTime.Now,
                CreadoPor = (uint)Math.Max(idUsuario, 0)
            };

            context.LlantaModelos.Add(modelo);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarModelo(LlantaModeloViewModel model, int idUsuario)
        {
            if (model.IdModeloLlanta == 0)
            {
                throw new Exception("No se encontró el modelo de llanta.");
            }

            var modelo = await context.LlantaModelos
                .FirstOrDefaultAsync(x => x.IdModeloLlanta == model.IdModeloLlanta);

            if (modelo == null)
            {
                throw new Exception("No se encontró el modelo de llanta.");
            }

            var nombre = await ValidarModelo(model);

            modelo.IdMarcaLlanta = model.IdMarcaLlanta;
            modelo.Nombre = nombre;
            modelo.Medida = LimpiarTextoOpcional(model.Medida);
            modelo.PresionMinimaPsi = model.PresionMinimaPsi;
            modelo.PresionMaximaPsi = model.PresionMaximaPsi;
            modelo.ProfundidadOriginalMm = model.ProfundidadOriginalMm;
            modelo.VidaUtilEstimadaKm = model.VidaUtilEstimadaKm;
            modelo.FechaModificacion = DateTime.Now;
            modelo.ModificadoPor = (uint)Math.Max(idUsuario, 0);

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarModelo(uint idModeloLlanta, int idUsuario)
        {
            var modelo = await context.LlantaModelos
                .FirstOrDefaultAsync(x => x.IdModeloLlanta == idModeloLlanta);

            if (modelo == null)
            {
                throw new Exception("No se encontró el modelo de llanta.");
            }

            if (modelo.Activo == false)
            {
                throw new Exception("El modelo de llanta ya se encuentra inactivo.");
            }

            modelo.Activo = false;
            modelo.FechaModificacion = DateTime.Now;
            modelo.ModificadoPor = (uint)Math.Max(idUsuario, 0);

            await context.SaveChangesAsync();

            return true;
        }

        private async Task<string> ValidarModelo(LlantaModeloViewModel model)
        {
            var nombre = model.Nombre?.Trim();
            var medida = LimpiarTextoOpcional(model.Medida);

            if (model.IdMarcaLlanta == 0)
            {
                throw new Exception("La marca es requerida.");
            }

            var marcaActiva = await context.LlantaMarcas
                .AsNoTracking()
                .AnyAsync(x => x.IdMarcaLlanta == model.IdMarcaLlanta && x.Activo == true);

            if (!marcaActiva)
            {
                throw new Exception("La marca seleccionada no existe o no está activa.");
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("El modelo es requerido.");
            }

            if (nombre.Length > 150)
            {
                throw new Exception("El modelo no debe exceder 150 caracteres.");
            }

            if (medida?.Length > 50)
            {
                throw new Exception("La medida no debe exceder 50 caracteres.");
            }

            ValidarRangoDecimal(model.PresionMinimaPsi, "La presión mínima");
            ValidarRangoDecimal(model.PresionMaximaPsi, "La presión máxima");
            ValidarRangoDecimal(model.ProfundidadOriginalMm, "La profundidad original");

            if (model.PresionMinimaPsi.HasValue
                && model.PresionMaximaPsi.HasValue
                && model.PresionMinimaPsi > model.PresionMaximaPsi)
            {
                throw new Exception("La presión mínima no puede ser mayor a la presión máxima.");
            }

            var nombreNormalizado = nombre.ToLower();
            var medidaNormalizada = medida?.ToLower();
            var existeModelo = await context.LlantaModelos
                .AsNoTracking()
                .AnyAsync(x => x.IdMarcaLlanta == model.IdMarcaLlanta
                            && x.IdModeloLlanta != model.IdModeloLlanta
                            && x.Nombre.ToLower() == nombreNormalizado
                            && (x.Medida ?? string.Empty).ToLower() == (medidaNormalizada ?? string.Empty));

            if (existeModelo)
            {
                throw new Exception("Ya existe un modelo de llanta con la misma marca, nombre y medida.");
            }

            return nombre;
        }

        private static string? LimpiarTextoOpcional(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        private static void ValidarRangoDecimal(decimal? valor, string campo)
        {
            if (valor < 0 || valor > 999.99m)
            {
                throw new Exception($"{campo} debe estar entre 0 y 999.99.");
            }
        }
    }
}
