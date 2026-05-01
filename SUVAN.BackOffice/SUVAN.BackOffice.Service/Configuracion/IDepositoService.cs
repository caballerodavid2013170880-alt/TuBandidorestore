using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public interface IDepositoService
    {
        Task<List<Deposito>> GetDepositos(short id_emp, short id_region, short id_planta, short id_zona);
        Task<DepositoViewModel> GetDepositoViewModel(short? id_emp, short id_region, short id_planta, short id_zona, short id_deposito);
        Task<bool> AgregarDeposito(DepositoViewModel model);
    }
}
