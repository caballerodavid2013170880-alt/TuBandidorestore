using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class MarcaService : IMarcaService
    {
        private readonly SuvanDbContext context;

        public MarcaService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Marca>> GetMarcas()
        {
            return await context.Marcas.ToListAsync();
        }

        public async Task<Marca> GetMarcaViewModel(int id)
        {
            var marca = await context.Marcas.FirstOrDefaultAsync(x => x.IdMarca == id);
            return marca ?? new Marca();
        }

        public async Task<bool> AgregarMarca(Marca model)
        {
            Marca marca = model.IdMarca > 0
                ? await context.Marcas.FirstOrDefaultAsync(x => x.IdMarca == model.IdMarca)
                : new Marca();

            if (marca == null)
                throw new Exception("No se encontró el servicio");

            var marcaExistente = await context.Marcas.FirstOrDefaultAsync(x =>
                x.Descrip!.ToLower() == model.Descrip!.ToLower() &&
                x.IdMarca != model.IdMarca);

            if (marcaExistente is not null)
                throw new Exception("Ya existe un servicio con el mismo nombre");

            marca.Descrip = model.Descrip;

            if (model.IdMarca > 0)
                context.Marcas.Update(marca);
            else
                context.Marcas.Add(marca);

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarMarca(int idMarca)
        {
            var servicio = await context.Marcas.FirstOrDefaultAsync(x => x.IdMarca == idMarca);
            if (servicio is null)
                throw new Exception("No se encontró el servicio");

            context.Marcas.Remove(servicio);
            await context.SaveChangesAsync();

            return true;
        }

        public Task EliminarMarca(short idMarca)
        {
            throw new NotImplementedException();
        }
    }
}