using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class LlantaModelo
{
    public uint IdModeloLlanta { get; set; }

    public uint IdMarcaLlanta { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Medida { get; set; }

    public decimal? PresionMinimaPsi { get; set; }

    public decimal? PresionMaximaPsi { get; set; }

    public decimal? ProfundidadOriginalMm { get; set; }

    public decimal? ProfundidadAlertaMm { get; set; }

    public decimal? ProfundidadMinimaMm { get; set; }

    public uint? VidaUtilEstimadaKm { get; set; }

    public bool? Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public uint CreadoPor { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public uint? ModificadoPor { get; set; }

    public virtual LlantaMarca IdMarcaLlantaNavigation { get; set; } = null!;

    public virtual ICollection<Llantum> Llanta { get; set; } = new List<Llantum>();
}
