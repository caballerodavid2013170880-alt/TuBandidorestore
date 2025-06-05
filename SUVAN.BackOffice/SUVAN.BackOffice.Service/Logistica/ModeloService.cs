using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class ModeloService : IModeloService
    {
        private readonly SuvanDbContext context;

        public ModeloService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Modelo>> GetModelos()
        {
            return await context.Modelos.ToListAsync();
        }

        public async Task<Modelo> GetModeloViewModel(int id)
        {
            var modelo = await context.Modelos.FirstOrDefaultAsync(x => x.IdModelo == id);
            return modelo ?? new Modelo();
        }

        public async Task<bool> AgregarModelo(Modelo model)
        {
            Modelo modelo = model.IdModelo > 0
                ? await context.Modelos.FirstOrDefaultAsync(x => x.IdModelo == model.IdModelo)
                : new Modelo();

            if (modelo == null)
                throw new Exception("No se encontró el modelo");

            var modeloExistente = await context.Modelos.FirstOrDefaultAsync(x =>
                x.Descrip!.ToLower() == model.Descrip!.ToLower() &&
                x.IdModelo != model.IdModelo);

            if (modeloExistente is not null)
                throw new Exception("Ya existe un modelo con el mismo nombre");

            modelo.Descrip = model.Descrip;

            if (model.IdModelo > 0)
                context.Modelos.Update(modelo);
            else
                context.Modelos.Add(modelo);

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarModelo(int idModelo)
        {
            var modelo = await context.Modelos.FirstOrDefaultAsync(x => x.IdModelo == idModelo);
            if (modelo is null)
                throw new Exception("No se encontró el modelo");

            context.Modelos.Remove(modelo);
            await context.SaveChangesAsync();

            return true;
        }

        public Task EliminarModelo(short idModelo)
        {
            throw new NotImplementedException();
        }
    }
}