using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public interface IMotivoAuxilioVialService
    {
        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <returns>Lista de empresas.</returns>
        Task<List<MotivoAuxilioVial>> GetMotivoAuxilioVial();
    }
}
