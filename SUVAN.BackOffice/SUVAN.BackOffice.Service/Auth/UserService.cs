using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ObjectUsuarios;

namespace SUVAN.BackOffice.Service
{
    public class UserService : IUserService
    {
        private readonly SuvanDbContext _context;
        public UserService(SuvanDbContext context)
        {
            _context = context;
        }

        public void ValidateUser()
        {

        }

        public async Task<ObjectUsuario> dataUser(Usuario data)
        {
            ObjectUsuario result = new ObjectUsuario
            {
                Idusuario = data.Idusuario,
                Email = data.Email,
                Hashpass = data.Hashpass,
                CodigopaisIdcodigopais = data.CodigopaisIdcodigopais,
                Telefono = data.Telefono,
                CodigoAuth = data.CodigoAuth,
                Validado = data.Validado,
                Activo = data.Activo,
                Nombre = data.Nombre,
                CodigoExp = data.CodigoExp  
            };

            return result;
        }

        public async Task<ObjectConductor> dataConductor(Database.Entities.Conductor data)
        {
            ObjectConductor result = new ObjectConductor
            {
                Idconductor = data.Idconductor,
                Email = data.Email,
                Hashpass = data.Hashpass,
                CodigopaisIdcodigopais = data.CodigopaisIdcodigopais,
                Telefono = data.Telefono,
                CodigoAuth = data.CodigoAuth,
                Validado = data.Validado,
                Activo = data.Activo,
                Nombre = data.Nombre,
                Rfc = data.Rfc,
                CodigoExp = data.CodigoExp
            };

            return result;
        }
    }
}
