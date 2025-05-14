using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface ITipoServicioService
    {
        /// <summary>
        /// Obtiene el listado de Depositos desde la base de datos.
        /// </summary>
        /// <returns>Lista de Depositos.</returns>
        Task<List<TipoServicio>> GetTipoService();
    }
}
