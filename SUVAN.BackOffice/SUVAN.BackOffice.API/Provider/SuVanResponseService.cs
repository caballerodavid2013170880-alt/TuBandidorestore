using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using System.Net;

namespace SUVAN.BackOffice.API.Provider
{
    public class SuVanResponseService : ISuVanResponseService
    {
        public ActionResult Handle<TResponse>(SuVanResponse<TResponse> response,HttpStatusCode httpStatusCode=HttpStatusCode.OK)
        {
            var actionresult = new ObjectResult(response);
            actionresult.StatusCode = (int)httpStatusCode;
            return actionresult;
        }
    }
}
