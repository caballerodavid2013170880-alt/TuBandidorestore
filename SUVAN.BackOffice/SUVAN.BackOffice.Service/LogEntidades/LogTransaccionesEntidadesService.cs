using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.LogEntidades
{
  public class LogTransaccionesEntidadesService : ILogTransaccionesEntidadesService
  {
    private readonly SuvanDbContext context;

    public LogTransaccionesEntidadesService(SuvanDbContext context)
    {
      this.context = context;
    }

    public async Task AddBitacora(Logtransaccionesentidade bitacora)
    {
      await context.Logtransaccionesentidades.AddAsync(bitacora);
      await context.SaveChangesAsync();
    }
  }

}
