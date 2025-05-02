using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Pago;
using SUVAN.BackOffice.Models.UnlimintPay.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SUVAN.BackOffice.Service.UnlimintPay
{
    public interface IUnlimintPayService
    {
        RespuestaGeneracionToken GenerarToken(int userId, PeticionGeneracionToken pmtPeticion);


        RespuestaGeneracionPago GenerarUrlPago(PeticionGeneracionPago pmtPeticion);


        RespuestaValidacionPago ValidarPago(PeticionValidacionPago pmtPeticion);

        PeticionGeneracionToken ArmaRequestToken(string grant_type, int userId = 0);


        PeticionGeneracionPago ArmaRequestPeticionGeneracionPago(string tokenPay, string gPeticionId, string gOrdenId, string emailUser, decimal Monto, string Moneda);


        PeticionValidacionPago ArmaRequestPeticionValidaPago(string tokenPay, string OrdenId, string PeticionId);

    }
}
