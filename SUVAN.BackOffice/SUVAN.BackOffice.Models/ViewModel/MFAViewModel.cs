using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel
{
  public class MFAViewModel
  {
    //[Required(ErrorMessage = "El Id requerido")]
    //public int UserId { get; set; }
    [Required(ErrorMessage = "El Email requerido")]
    public string Email { get; set; } = string.Empty;

    public string OTP { get; set; } = string.Empty;
  }
}
