using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class CausaSiniestroService : ICausaSiniestroService
    {
        private readonly SuvanDbContext context;

        public CausaSiniestroService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CausaSiniestro>> GetCausaSiniestro()
        {
            return await context.CausaSiniestros.ToListAsync();
        }

        public async Task<CausaSiniestroViewModel> GetCausaSiniestroViewModel(int id)
        {
            var causa = await context.CausaSiniestros.FirstOrDefaultAsync(x => x.Id_causa_siniestro == id);
            if (causa == null) return new CausaSiniestroViewModel();

            return new CausaSiniestroViewModel
            {
                Id_causa_siniestro = causa.Id_causa_siniestro,
                Descripcion = causa.Descripcion
            };
        }

        public async Task<bool> AgregarCausaSiniestro(CausaSiniestroViewModel model)
        {
            CausaSiniestro causa;

            if (model.Id_causa_siniestro > 0)
            {
                causa = await context.CausaSiniestros.FirstOrDefaultAsync(x => x.Id_causa_siniestro == model.Id_causa_siniestro);
                if (causa == null) throw new Exception("No se encontró la causa del siniestro");
            }
            else
            {
                causa = new CausaSiniestro();
            }

            var causaExistente = await context.CausaSiniestros.FirstOrDefaultAsync(x =>
                x.Descripcion.ToLower() == model.Descripcion.ToLower() && x.Id_causa_siniestro != model.Id_causa_siniestro);

            if (causaExistente is not null)
                throw new Exception("Ya existe una causa de siniestro con la misma descripción");

            causa.Descripcion = model.Descripcion;

            if (model.Id_causa_siniestro > 0)
            {
                context.CausaSiniestros.Update(causa);
            }
            else
            {
                context.CausaSiniestros.Add(causa);
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarCausaSiniestro(int IdCausaSiniestro)
        {
            var causa = await context.CausaSiniestros.FirstOrDefaultAsync(x => x.Id_causa_siniestro == IdCausaSiniestro);

            if (causa is null) throw new Exception("No se encontró la causa del siniestro");

            var delete = await context.CausaSiniestros.Where(x => x.Id_causa_siniestro == IdCausaSiniestro).ExecuteDeleteAsync();
            await context.SaveChangesAsync();

            return delete > 0;
        }

        Task<List<CausaSiniestro>> ICausaSiniestroService.GetCausaSiniestro()
        {
            throw new NotImplementedException();
        }
    }
}