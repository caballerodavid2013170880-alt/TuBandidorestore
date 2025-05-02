using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ObjectUsuarios;

namespace SUVAN.BackOffice.Service
{
    public interface IUserService
    {
        public void ValidateUser();


        Task<ObjectUsuario> dataUser(Usuario data);

        Task<ObjectConductor> dataConductor(Database.Entities.Conductor data);

    }
}
