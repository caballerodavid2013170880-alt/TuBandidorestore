using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class DetPrev
{
    public short IdPrev { get; set; }

    public short IdPrevDet { get; set; }

    public short IdMano { get; set; }
}
