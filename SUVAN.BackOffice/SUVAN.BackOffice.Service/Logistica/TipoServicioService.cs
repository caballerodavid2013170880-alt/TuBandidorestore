using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class TipoServicioService : ITipoServicioService
    {
        private readonly SuvanDbContext context;

        public TipoServicioService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TipoServicio>> GetTipoService()
        {
            var tipo = await context.TipoServicios.ToListAsync();

            return tipo!;
        }
    }
}
