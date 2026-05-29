using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.AppSettingsModels
{
  public class GlobalConfigsOptions
    {
        public string apiURL { get; set; } = string.Empty;
        public string accTokenFactory { get; set; } = string.Empty;
        public string authFCM { get; set; } = string.Empty;
    }
}
