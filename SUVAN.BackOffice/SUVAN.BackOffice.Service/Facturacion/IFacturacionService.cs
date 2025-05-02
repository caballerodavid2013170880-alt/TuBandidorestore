using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Pago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Facturacion
{
    public interface IFacturacionService
    {
        Task<SuVanResponse<GeneraFacturaResponse>> GenerarFactura(int userId, string emailUser , GeneraFacturaRequest data);

        Task<SuVanResponse<GuardaDatosFacturacionReceptorResponse>> GuardaDatosFacturacionReceptor(int userId, GuardaDatosFacturacionReceptorRequest data);

        Task<SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.RegimenFiscalReceptorResponse>>> ObtenRegimenFiscalReceptor(int userId, string rfc);

        Task<SuVanResponse<List<DatosRegimenFiscalUsoCFDIResponse.UsoCFDIReceptorResponse>>> ObtenUsoCFDIReceptor(int userId, string rfc, int idregimenfiscalreceptor);

        Task<SuVanResponse<List<ObtenDatosFacturacionReceptorResponse>>> ObtenDatosFacturacionReceptor(int userId);

        public Task<SuVanResponse<string>> RemueveDatosFacturacionReceptor(int datosfacturacionreceptorId, int userId);


    }
}
