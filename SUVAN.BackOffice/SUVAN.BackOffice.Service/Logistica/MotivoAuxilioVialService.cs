using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class MotivoAuxilioVialService : IMotivoAuxilioVialService
    {
        private readonly SuvanDbContext context;

        public MotivoAuxilioVialService(SuvanDbContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Obtiene el listado de motivos de auxilio desde la base de datos.
        /// </summary>
        /// <returns>Lista de Motivos de Auxilios.</returns>
        public async Task<List<MotivoAuxilioVial>> GetMotivoAuxilioVial()
        {
            var motivo_auxilio = await context.MotivoAuxilioVials.ToListAsync();

            return motivo_auxilio!;

        }

      
    }
}
