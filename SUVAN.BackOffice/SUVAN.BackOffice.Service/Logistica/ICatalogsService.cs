using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface ICatalogsService
    {
        Task<dynamic> GetCatalog(string catalogName, int IdEmpresa, int nClaveFiltro);

    }
}
