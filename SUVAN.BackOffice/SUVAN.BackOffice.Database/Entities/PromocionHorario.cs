using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class PromocionHorario
{
    public int IdpromocionHorario { get; set; }

    public int PromocionIdpromocion { get; set; }

    public TimeOnly? Horadesde { get; set; }

    public TimeOnly? Horahasta { get; set; }

    public virtual Promocion PromocionIdpromocionNavigation { get; set; } = null!;
}
