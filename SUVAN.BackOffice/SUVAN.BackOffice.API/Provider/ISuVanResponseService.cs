using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using System.Net;

namespace SUVAN.BackOffice.API.Provider
{
    public interface ISuVanResponseService
    {
        ActionResult Handle<TResponse>(SuVanResponse<TResponse> response, HttpStatusCode httpStatusCode = HttpStatusCode.OK);
    }
}
