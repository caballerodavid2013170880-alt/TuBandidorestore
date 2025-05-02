using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Favoritos
{
    public class FavoritoModelParent
    {
        [Required]
        public string? Direccion { get; set; }
        [Required]
        public decimal? Latitud { get; set; }
        [Required]
        public decimal? Longitud { get; set; }
        [Required]
        public int? Activo { get; set; }
    }

    public class FavoritoModel: FavoritoModelParent
    {
        public int TipoFavorito { get; set; }
    }

    public class FavoritoModelRequest : FavoritoModelParent
    {
        [Required]
        public string? Nombre { get; set; }
    }


    public class FavoritoPersonalModel: FavoritoModelParent
    {
        [Required]
        public string? Nombre { get; set; }
    }

    public class FavoritoPersonalResponse: FavoritoModelParent
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
    }
}
