using System;
using System.Collections.Generic;

namespace SUVAN.BackOffice.Database.Entities;

public partial class Politicascompensacion
{
    public int Idpoliticacompensacion { get; set; }

    public int? Tipocancelacion { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Rangotiempo { get; set; }

    public int? Tipotiempo { get; set; }

    public decimal? Porcentajecompensacion { get; set; }

    public ulong? Activa { get; set; }

    public DateTime? Fecharegistro { get; set; }

    public int? EmpresaIdempresa { get; set; }

    public int? Tipopolitica { get; set; }

    public virtual Empresa? EmpresaIdempresaNavigation { get; set; }
}
