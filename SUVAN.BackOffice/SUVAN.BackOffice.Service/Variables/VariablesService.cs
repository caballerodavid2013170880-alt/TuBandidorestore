using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Variables;
using SUVAN.BackOffice.Service.Pago;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Service.UnlimintPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.UnlimintPay.Pago.RespuestaValidacionPagoWS;
using static SUVAN.BackOffice.Service.Pago.PagoService;

namespace SUVAN.BackOffice.Service.Variables
{
    public class VariablesService : IVariablesService
    {
        private readonly SuvanDbContext _context;

        public VariablesService(SuvanDbContext context)
        {
            _context = context;
        }

        public async Task<SuVanResponse<ObtenValorVariableResponse>> ObtenValorVariable(string codigo, int empresaid)
        {
            SuVanResponse<ObtenValorVariableResponse> response = new();
            #region Validamos datos de entrada
            //Tipo transaccion "Pago o Recarga"
            var VariablesResult = await _context.Variables
                .Where(x => x.Codigo == codigo)
                .FirstOrDefaultAsync();

            if (VariablesResult == null)
            {
                //Ocurrió un error al generar token 
                response.CodigoMensaje = "400";
                response.Mensaje = "Código incorrecto";
                return response;
            }

            if (VariablesResult.TipovariableIdtipovariable == Convert.ToInt32(eTipoVariable.POR_EMPRESA))
            {
                if (empresaid <= 0)
                {
                    //Ocurrió un error al generar token 
                    response.CodigoMensaje = "400";
                    response.Mensaje = "El parámetro empresaid es obligatorio";
                    return response;
                }
            }
            #endregion

            using (var context = _context)
            {
                if (VariablesResult.TipovariableIdtipovariable == Convert.ToInt32(eTipoVariable.GLOBAL))
                {
                    var result = await (from vg in context.Variableglobals 
                                        where vg.VariableIdvariable == VariablesResult.Idvariable
										select new ObtenValorVariableResponse()
                                        {
                                            valor= vg.Valor
                                        }).FirstOrDefaultAsync();
                    response.Data = result;
                }
                else if (VariablesResult.TipovariableIdtipovariable == Convert.ToInt32(eTipoVariable.POR_EMPRESA))
                {
                    var result = await (from  ve in context.Variableempresas 
                                        where ve.VariableIdvariable == VariablesResult.Idvariable 
                                        && ve.EmpresaIdempresa == empresaid
										select new ObtenValorVariableResponse()
                                        {
                                            valor = ve.Valor
                                        }).FirstOrDefaultAsync();
                    response.Data = result;
                }
            }

            response.CodigoMensaje = "200";
            response.Mensaje = "Búsqueda exitosa";
            return response;
        }


        public enum eTipoVariable
        {
            GLOBAL = 1,
            POR_EMPRESA = 2
        }
    }
}
