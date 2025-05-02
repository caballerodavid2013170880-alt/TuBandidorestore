using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public class BitacoraWebService : IBitacoraWebService
  {
    private readonly SuvanDbContext context;

    public BitacoraWebService(SuvanDbContext context)
    {
      this.context = context;
    }

    public async Task AddBitacora(Bitacoraloginweb bitacora)
    {
      await context.Bitacoraloginwebs.AddAsync(bitacora);
      await context.SaveChangesAsync();
    }
  }
}
