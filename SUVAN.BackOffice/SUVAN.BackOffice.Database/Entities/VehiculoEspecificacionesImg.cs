using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class VehiculoEspecificacionesImg
{
    public int IdEspecificaciones { get; set; }

    public int Consecutivo { get; set; }

    public string? Ruta { get; set; }
}
