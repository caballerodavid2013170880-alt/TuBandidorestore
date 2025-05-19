using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Logistica
{
    public class MotivoAuxilioVialViewModel
    {
        public uint MotivoId { get; set; }

        [Required(ErrorMessage = "El nombre del motivo es necesario.")]
        public string Nombre { get; set; } = null!;
    }
}
