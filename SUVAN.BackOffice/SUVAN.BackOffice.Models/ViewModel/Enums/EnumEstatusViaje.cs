using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Enums
{
    public enum EnumEstatusViaje : int
    {
        RESERVANDO = 0,
        EN_ESPERA = 1,
        EN_CURSO = 2,
        FINALIZADO = 3,
        PERDIDO = 4,
        CANCELADO = 5
    }
}
