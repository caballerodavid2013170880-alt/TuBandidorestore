using System;
using System.ComponentModel.DataAnnotations;

namespace SUVAN.BackOffice.Database.Entities;

public partial class CausaSiniestro
{
    [Key]
    public int Id_causa_siniestro { get; set; }

    public string? Descripcion { get; set; }
}
