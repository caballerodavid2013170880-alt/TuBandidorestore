using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Enums
{
    public enum EnumEstatusPago : int
    {
        IN_PROGRESS = 1,
        COMPLETED = 2,
        DECLINED = 3,
        CANCEL = 4,
        AUTHORIZED = 5
    }
}
