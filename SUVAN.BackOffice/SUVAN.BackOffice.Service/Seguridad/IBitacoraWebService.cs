using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public interface IBitacoraWebService
  {
    Task AddBitacora(Bitacoraloginweb bitacora);
  }
}