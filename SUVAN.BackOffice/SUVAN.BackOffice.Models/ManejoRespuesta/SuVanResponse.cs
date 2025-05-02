using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ManejoRespuesta
{
        public class SuVanResponse<T>: BaseResponse
        {
            public T Data { get; set; }
        }
}
