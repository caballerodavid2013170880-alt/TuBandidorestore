using SUVAN.BackOffice.Models.Variables;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Variables
{
    public interface IVariablesService
    {
        Task<SuVanResponse<ObtenValorVariableResponse>> ObtenValorVariable(string codigo, int empresaid);

    }
}
