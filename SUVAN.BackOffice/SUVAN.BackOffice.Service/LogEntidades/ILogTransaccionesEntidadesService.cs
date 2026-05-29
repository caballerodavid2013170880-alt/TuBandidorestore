using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.LogEntidades
{
  public interface ILogTransaccionesEntidadesService
  {
    Task AddBitacora(Logtransaccionesentidade bitacora);
  }
}