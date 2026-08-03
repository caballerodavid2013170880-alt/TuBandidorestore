using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public class LlantaMarcaService : ILlantaMarcaService
    {
        private readonly SuvanDbContext context;

        public LlantaMarcaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<LlantaMarcaViewModel>> GetMarcas()
        {
            return await context.LlantaMarcas
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.Nombre)
                .Select(x => new LlantaMarcaViewModel
                {
                    IdMarcaLlanta = x.IdMarcaLlanta,
                    Nombre = x.Nombre,
                    Activo = x.Activo == true,
                    FechaCreacion = x.FechaCreacion,
                    FechaModificacion = x.FechaModificacion
                })
                .ToListAsync();
        }

        public async Task<LlantaMarcaViewModel> GetMarcaViewModel(uint idMarcaLlanta)
        {
            var marca = await context.LlantaMarcas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdMarcaLlanta == idMarcaLlanta);

            if (marca == null)
            {
                return new LlantaMarcaViewModel();
            }

            return new LlantaMarcaViewModel
            {
                IdMarcaLlanta = marca.IdMarcaLlanta,
                Nombre = marca.Nombre,
                Activo = marca.Activo == true,
                FechaCreacion = marca.FechaCreacion,
                FechaModificacion = marca.FechaModificacion
            };
        }

        public async Task<bool> CrearMarca(LlantaMarcaViewModel model, int idUsuario)
        {
            var nombre = await ValidarNombreMarca(model);

            var marca = new LlantaMarca
            {
                Nombre = nombre,
                Activo = true,
                FechaCreacion = DateTime.Now,
                CreadoPor = (uint)Math.Max(idUsuario, 0)
            };

            context.LlantaMarcas.Add(marca);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarMarca(LlantaMarcaViewModel model, int idUsuario)
        {
            if (model.IdMarcaLlanta == 0)
            {
                throw new Exception("No se encontró la marca de llanta.");
            }

            var marca = await context.LlantaMarcas
                .FirstOrDefaultAsync(x => x.IdMarcaLlanta == model.IdMarcaLlanta);

            if (marca == null)
            {
                throw new Exception("No se encontró la marca de llanta.");
            }

            var nombre = await ValidarNombreMarca(model);

            marca.Nombre = nombre;
            marca.FechaModificacion = DateTime.Now;
            marca.ModificadoPor = (uint)Math.Max(idUsuario, 0);

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarMarca(uint idMarcaLlanta, int idUsuario)
        {
            var marca = await context.LlantaMarcas
                .FirstOrDefaultAsync(x => x.IdMarcaLlanta == idMarcaLlanta);

            if (marca == null)
            {
                throw new Exception("No se encontró la marca de llanta.");
            }

            if (marca.Activo == false)
            {
                throw new Exception("La marca de llanta ya se encuentra inactiva.");
            }

            marca.Activo = false;
            marca.FechaModificacion = DateTime.Now;
            marca.ModificadoPor = (uint)Math.Max(idUsuario, 0);

            await context.SaveChangesAsync();

            return true;
        }

        private async Task<string> ValidarNombreMarca(LlantaMarcaViewModel model)
        {
            var nombre = model.Nombre?.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("La marca es requerida.");
            }

            if (nombre.Length > 100)
            {
                throw new Exception("La marca no debe exceder 100 caracteres.");
            }

            var nombreNormalizado = nombre.ToLower();
            var existeMarca = await context.LlantaMarcas
                .AsNoTracking()
                .AnyAsync(x => x.Nombre.ToLower() == nombreNormalizado
                            && x.IdMarcaLlanta != model.IdMarcaLlanta);

            if (existeMarca)
            {
                throw new Exception("Ya existe una marca de llanta con el mismo nombre.");
            }

            return nombre;
        }
    }
}
