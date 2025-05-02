using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Pdf.Content.Objects;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ActivacionUsuario;
using SUVAN.BackOffice.Models.ActualizaFotografia;
using SUVAN.BackOffice.Models.ActualizaPassword;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.Conductor;
using SUVAN.BackOffice.Models.Corrida;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Models.Parada;
using SUVAN.BackOffice.Models.RecuperaPassword;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Utilities;
using System.Globalization;
using System.Text;

namespace SUVAN.BackOffice.Service.Conductor
{
  public class ConductorService : IConductorService
  {
    private readonly SuvanDbContext context;

    public ConductorService(SuvanDbContext context)
    {
      this.context = context;

    }


    public async Task<SuVanResponse<string>> Registro(ConductorRegistroRequest data)
    {
      SuVanResponse<string> _result = new();
      //ObjectParentResponse resultWS = new ObjectParentResponse();
      //resultWS.Resultado = true;
      Codigopai? CodigopaiEntity = await GetCodigopais(data.codigopais);
      if (CodigopaiEntity == null)
      {
        _result.CodigoMensaje = "400";
        _result.Mensaje = "Código de país incorrecto";
        return _result;
      }

      Empresa? empresa = await GetEmpresa(data.empresa);
      if (empresa == null)
      {
        _result.CodigoMensaje = "400";
        _result.Mensaje = "Código de empresa incorrecto";
        return _result;
      }


      DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
      var entity = new Database.Entities.Conductor()
      {
        Email = data.email,
        EmpresaIdempresa = data.empresa,
        CodigopaisIdcodigopais = data.codigopais,
        Telefono = data.telefono,
        Fecharegistro = DateTime.Now,
        Activo = 1,
        Validado = 0,
        CodigoExp = expiracion,
        Nombre = data.nombre,
        Rfc = data.rfc
      };
      context.Conductors.Add(entity);
      await context.SaveChangesAsync();

      _result.CodigoMensaje = "200";
      _result.Mensaje = "Registro exitoso";
      _result.Data = null;
      return _result;
    }


    public async Task<SuVanResponse<string>> Firebase(int UserID, string firebaseid)
    {
      SuVanResponse<string> _result = new();
      Database.Entities.Conductor? info = await context.Conductors.Where(x => x.Idconductor == UserID).FirstOrDefaultAsync();
      if (info != null)
      {
        info.FirebaseId = firebaseid;
        context.SaveChanges();
        _result.CodigoMensaje = "200";
        _result.Mensaje = "Activación exitosa";
        _result.Data = null;
      }
      return _result;
    }

    public async Task<SuVanResponse<string>> SolicitaActivacion(SolicitaActivacionRequest data)
    {
      SuVanResponse<string> _result = new();
      Database.Entities.Conductor? info = await context.Conductors.Where(x => x.Email == data.email).FirstOrDefaultAsync();
      if (info != null)
      {
        string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);

        DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
        info.CodigoAuth = code;
        info.CodigoExp = expiracion;
        context.SaveChanges();

        _result.CodigoMensaje = "200";
        _result.Mensaje = "Código generado exitoso";
        _result.Data = null;
      }
      return _result;
    }

    public async Task<Codigopai?> GetCodigopais(int? codigopais)
    {
      return await context.Codigopais.Where(x => x.Idcodigopais == codigopais).FirstOrDefaultAsync();
    }

    public async Task<Empresa?> GetEmpresa(int? empresa)
    {
      return await context.Empresas.Where(x => x.Idempresa == empresa).FirstOrDefaultAsync();
    }

    public async Task<SuVanResponse<string>> Activacion(ActivacionUsuarioRequest data)
    {
      SuVanResponse<string> _result = new();
      Database.Entities.Conductor? info = await context.Conductors.Where(x => x.Email == data.email).FirstOrDefaultAsync();
      if (info != null)
      {
        info.Validado = 1;
        info.CodigoAuth = null;
        info.CodigoExp = null;
        info.Hashpass = data.password.GetHashSHA256();
        context.SaveChanges();
        _result.CodigoMensaje = "200";
        _result.Mensaje = "Activación exitosa";
        _result.Data = null;
      }
      return _result;
    }

    public async Task<SuVanResponse<string>> GeneraCodigo(GeneraCodigoRequest data)
    {
      SuVanResponse<string> _result = new();
      Database.Entities.Conductor? info = await context.Conductors.Where(x => x.Email == data.email && x.Hashpass == data.password.GetHashSHA256()).FirstOrDefaultAsync();
      if (info != null)
      {
        string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
        DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
        info.CodigoAuth = code;
        info.CodigoExp = expiracion;
        context.SaveChanges();

        _result.CodigoMensaje = "200";
        _result.Mensaje = "Código generado exitoso";
        _result.Data = null;
      }
      return _result;
    }

    public async Task<SuVanResponse<string>> RecuperaPassword(RecuperaPasswordRequest data)
    {
      SuVanResponse<string> _result = new();
      _result.CodigoMensaje = "200";
      _result.Mensaje = "Recuperación exitosa";
      _result.Data = null;
      return _result;
    }

    public async Task<SuVanResponse<string>> ActualizaPassword(ActualizaPasswordRequest data)
    {
      SuVanResponse<string> _result = new();
      Database.Entities.Conductor? info = await context.Conductors.Where(x => x.Email == data.email).FirstOrDefaultAsync();
      if (info != null)
      {
        info.Hashpass = data.nuevopassword.GetHashSHA256();
        context.SaveChanges();
        _result.CodigoMensaje = "200";
        _result.Mensaje = "Actualización exitosa";
        _result.Data = null;
      }
      return _result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Database.Entities.Conductor> getInfoEmail(string email)
    {
      Database.Entities.Conductor conductor = await context.Conductors
          .Where(x => x.Email == email).FirstOrDefaultAsync();

      return conductor!;
    }

    public async Task<Database.Entities.Conductor> getInfo(string email, string password)
    {
      Database.Entities.Conductor conductor = await context.Conductors
          .Where(x => (x.Email == email)
                   && (x.Hashpass == (!string.IsNullOrEmpty(password) ? password.GetHashSHA256() : x.Hashpass))
          ).FirstOrDefaultAsync();

      return conductor!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Database.Entities.Conductor> getInfoID(int ConductorID)
    {
      Database.Entities.Conductor conductor = await context.Conductors
          .Where(x => x.Idconductor == ConductorID)
          .FirstOrDefaultAsync();

      return conductor!;
    }

    public async Task<SuVanResponse<PerfilConductorModel>> ObtenerPerfil(int ConductorId)
    {
      SuVanResponse<PerfilConductorModel> response = new();
      PerfilConductorModel? result = await (from o in context.Conductors
                                            where o.Idconductor == ConductorId
                                            && o.Activo == 1
                                            && o.Validado == 1
                                            select new PerfilConductorModel()
                                            {
                                              Idconductor = o.Idconductor,
                                              Nombre = o.Nombre,
                                              Telefono = o.Telefono,
                                              Email = o.Email,
                                              Codigopais = o.CodigopaisIdcodigopais,
                                              FotoConductor = context.FotoConductors.Where(k => k.ConductorIdconductor == ConductorId).Select(x => x.Imagen).FirstOrDefault()
                                            }).FirstOrDefaultAsync();


      response.CodigoMensaje = "200";
      response.Mensaje = "Búsqueda exitosa";

      response.Data = result;
      return response;
    }

    public async Task<SuVanResponse<ActualizaPerfilRequest>> ActualizaPerfil(int ConductorId, ActualizaPerfilRequest model, string code)
    {
      SuVanResponse<ActualizaPerfilRequest> response = new();
      Database.Entities.Conductor? perfilEntity = context.Conductors.FirstOrDefault(x => x.Idconductor == ConductorId);

      model.CodigoCambioTelefono = !model.CodigoCambioTelefono.Equals("0000") ? model.CodigoCambioTelefono : string.Empty;
      ActualizaPerfilRequest? result = new ActualizaPerfilRequest();

      if (perfilEntity != null)
      {
        if ((perfilEntity.Telefono != model.Telefono || perfilEntity.CodigopaisIdcodigopais != model.Codigopais) && perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
        //if ((perfilEntity.Telefono != model.Telefono || perfilEntity.CodigopaisIdcodigopais != model.Codigopais) && !string.IsNullOrEmpty(perfilEntity.CodigoAuth))
        {
          //1 validamos que el codigo que nos mandan no venga vacio
          if (!string.IsNullOrEmpty(model.CodigoCambioTelefono))
          {
            //2 Validamos si los códigos son diferentes de nulo o vacios
            if (!string.IsNullOrEmpty(perfilEntity.CodigoAuth) && !string.IsNullOrEmpty(model.CodigoCambioTelefono))
            {
              //3 Si el código expiro y son diferentes mandamos que el codigo ha sido expirado
              if (DateTime.Now > perfilEntity.CodigoExp && perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
              {
                response.CodigoMensaje = "400";
                response.Mensaje = "Código expirado";
                return response;
              }

              //4 Validamos la expiración sigue vigente pero son codigos diferentes
              if (perfilEntity.CodigoExp > DateTime.Now && perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
              {
                response.CodigoMensaje = "400";
                response.Mensaje = "Código no válido";
                return response;
              }
            }
          }


          DateTime expiracion = SUVAN.BackOffice.Utilities.GeneraCodigos.ExpiracionCodigoActivacion();
          //enviamos correo y actualizamos en bd el codigo que mandamos
          perfilEntity.CodigoAuth = code;
          perfilEntity.CodigoExp = expiracion;
          response.Mensaje = "Solicitud de actualización exitosa";
          response.CodigoMensaje = "206";

          result.Nombre = perfilEntity.Nombre;
          result.Email = perfilEntity.Email;
          result.Telefono = perfilEntity.Telefono;
          result.Codigopais = perfilEntity.CodigopaisIdcodigopais;
          response.Data = result;
        }
        else
        {
          if ((!string.IsNullOrEmpty(model.CodigoCambioTelefono) && model.CodigoCambioTelefono != "0000") && perfilEntity.CodigoAuth != null)
          {
            if (perfilEntity.CodigoAuth != model.CodigoCambioTelefono)
            {
              response.CodigoMensaje = "400";
              response.Mensaje = "Código no válido";
              return response;
            }
            if (DateTime.Now > perfilEntity.CodigoExp)
            {
              response.CodigoMensaje = "400";
              response.Mensaje = "Código expirado";
              return response;
            }
          }
          perfilEntity.Nombre = model.Nombre;
          perfilEntity.Email = model.Email;
          perfilEntity.Telefono = !string.IsNullOrEmpty(model.CodigoCambioTelefono) ? model.Telefono : perfilEntity.Telefono;
          perfilEntity.CodigopaisIdcodigopais = model.Codigopais;
          perfilEntity.CodigoAuth = null;
          perfilEntity.CodigoExp = null;

          result.Nombre = model.Nombre;
          result.Email = model.Email;
          result.Telefono = model.Telefono;
          result.Codigopais = model.Codigopais;
          response.Data = result;
          response.Mensaje = "Actualización exitosa";
          response.CodigoMensaje = "200";
        }
      }

      context.Conductors.Entry(perfilEntity);
      await context.SaveChangesAsync();
      return response;
    }

    public async Task<SuVanResponse<string>> ActualizaFotografia(int ConductorId, ActualizaFotografiaRequest data)
    {
      SuVanResponse<string> response = new();
      FotoConductor? perfilEntity = context.FotoConductors.FirstOrDefault(x => x.ConductorIdconductor == ConductorId);
      if (perfilEntity == null)
      {
        var fotografiaEntity = new Database.Entities.FotoConductor()
        {
          ConductorIdconductor = ConductorId,
          Imagen = data.Imagen,
          Fecharegistro = DateTime.Now,
          Activo = 1
        };
        context.FotoConductors.Add(fotografiaEntity);
      }
      else
      {
        perfilEntity.Imagen = data.Imagen;
        context.FotoConductors.Entry(perfilEntity);
      }
      await context.SaveChangesAsync();

      response.Data = null;
      response.Mensaje = "Actualización exitosa";
      response.CodigoMensaje = "200";

      return response;
    }



    public async Task<SuVanResponse<List<AdministradorConversacionModel>>> ObtineAdministradoresPorEmpresa(int empresaId)
    {
      SuVanResponse<List<AdministradorConversacionModel>> response = new();

      var administradores = await (from a in context.Admins
                                   join b in context.AdminEmpresas on a.Idadmin equals b.AdminIdadmin
                                   where b.EmpresaIdempresa == empresaId && (b.PerfilIdperfil == 1 || b.PerfilIdperfil == 8)
                                   select new AdministradorConversacionModel
                                   {
                                     AdministradorId = a.Idadmin,
                                     Nombre = a.Nombre!,
                                   }).ToListAsync();


      // remueve administradores repetidos
      administradores = administradores.GroupBy(x => x.AdministradorId).Select(x => x.First()).ToList();

      response.Data = administradores;
      response.Mensaje = "Consulta exitosa";
      response.CodigoMensaje = "200";

      return response;
    }





    public async Task<SuVanResponse<List<ConductorCorridas>>> ProximasCorridas(int conductorId)
    {
      SuVanResponse<List<ConductorCorridas>> response = new();


      DateTime hoy = DateTime.Now.Date;

      using (var ctx = context)
      {
        var list = (from a in ctx.CorridaAsignacions
                    join b in ctx.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                    join c in ctx.Ruta on b.RutaIdruta equals c.Idruta
                    where a.ConductorIdconductor == conductorId
                    && (a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) ||
                    a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                    )
                    select new ConductorCorridas()
                    {
                      IdEstatusCorridaAsignacion = a.EstatusviajeIdestatusviaje ?? (sbyte)EnumEstatusViaje.RESERVANDO,
                      idCorridaAsignacion = a.IdcorridaAsignacion,
                      idRuta = c.Idruta,
                      ruta = c.Nombre,
                      fecha = DateOnly.FromDateTime(a.Fecha ?? DateTime.Now),
                      horaInicio = b.HoraInicio,
                      horaFin = b.HoraFin,
                      paradaInicio = (
                            (from a in ctx.RutaParada
                             join b in ctx.Parada on a.ParadaIdparada equals b.Idparada
                             orderby a.Orden
                             select new ParadaCorrida()
                             {
                               idRuta = a.RutaIdruta,
                               idParada = b.Idparada,
                               nombre = b.Nombre,
                               parada = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                               longitud = b.Longitud,
                               latitud = b.Latitud,
                               orden = a.Orden
                             }).Where(x => x.idRuta == c.Idruta).First()
                        ),
                      paradaTermino = (
                            (from a in ctx.RutaParada
                             join b in ctx.Parada on a.ParadaIdparada equals b.Idparada
                             orderby a.Orden descending
                             select new ParadaCorrida()
                             {
                               idRuta = a.RutaIdruta,
                               idParada = b.Idparada,
                               nombre = b.Nombre,
                               parada = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                               longitud = b.Longitud,
                               latitud = b.Latitud,
                               orden = a.Orden
                             }).Where(x => x.idRuta == c.Idruta).First()
                        ),
                      pasajeros = (
                            (from z in ctx.Viajes
                             join y in ctx.Estatusviajes on z.EstatusviajeIdestatusviaje equals y.Idestatusviaje
                             where z.CorridaAsignacionIdcorridaAsignacion == a.IdcorridaAsignacion
                            && (z.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_ESPERA) ||
                            z.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO) ||
                            z.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
                            //|| z.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.PERDIDO)
                            )
                             select z.Numeropasajeros).Sum() ?? 0
                        )
                    }).Distinct().Where(x => x.fecha >= DateOnly.FromDateTime(hoy)).OrderBy(x => x.fecha).ThenBy(x => x.horaInicio).Take(10).ToList();

        response.Data = list;
      }
      response.CodigoMensaje = "200";
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontraron proximos viajes" : "Búsqueda exitosa";

      return response;
    }

    public async Task<SuVanResponse<List<RutaCorridaAsignada>>> RutasCorridaAsignada(int conductorId)
    {
      SuVanResponse<List<RutaCorridaAsignada>> response = new();

      using (var ctx = context)
      {
        var list = (from a in ctx.CorridaAsignacions
                    join b in ctx.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                    join c in ctx.Ruta on b.RutaIdruta equals c.Idruta
                    where a.ConductorIdconductor == conductorId
                    select new RutaCorridaAsignada()
                    {
                      idRuta = c.Idruta,
                      ruta = c.Nombre
                    }).Distinct().ToList();

        list.ForEach(x => x.horarios = (
                            (from a in ctx.CorridaAsignacions
                             join b in ctx.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                             join c in ctx.Ruta on b.RutaIdruta equals c.Idruta
                             where a.ConductorIdconductor == conductorId
                             select new
                             {
                               idRuta = c.Idruta,
                               horario = b.HoraInicio
                             }).Where(z => z.idRuta == x.idRuta).Distinct().Select(x => x.horario.ToString()).ToList()
                        )
                    );

        response.Data = list;
      }
      response.CodigoMensaje = "200";
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontraron rutas asignadas a corridas de este conductor" : "Búsqueda exitosa";

      return response;
    }

    public async Task<SuVanResponse<List<ConductorCorridas>>> BusquedaCorridasAsignadas(BusquedaCorridasRequest data, int conductorId)
    {
      SuVanResponse<List<ConductorCorridas>> response = new();

      using (var ctx = context)
      {
        List<ConductorCorridas> list = (from a in ctx.CorridaAsignacions
                                        join b in ctx.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                                        join c in ctx.Ruta on b.RutaIdruta equals c.Idruta
                                        where a.ConductorIdconductor == conductorId
                                        select new ConductorCorridas()
                                        {
                                          IdEstatusCorridaAsignacion = a.EstatusviajeIdestatusviaje,
                                          idCorridaAsignacion = a.IdcorridaAsignacion,
                                          idRuta = c.Idruta,
                                          ruta = c.Nombre,
                                          fecha = DateOnly.FromDateTime(a.Fecha ?? DateTime.Now),
                                          horaInicio = b.HoraInicio,
                                          horaFin = b.HoraFin,
                                          paradaInicio = (
                                                (from a in ctx.RutaParada
                                                 join b in ctx.Parada on a.ParadaIdparada equals b.Idparada
                                                 orderby b.Orden
                                                 select new ParadaCorrida()
                                                 {
                                                   idRuta = a.RutaIdruta,
                                                   idParada = b.Idparada,
                                                   nombre = b.Nombre,
                                                   parada = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                                                   longitud = b.Longitud,
                                                   latitud = b.Latitud,
                                                   orden = a.Orden
                                                 }).Where(x => x.idRuta == c.Idruta).OrderBy(x => x.orden).First()
                                            ),
                                          paradaTermino = (
                                                (from a in ctx.RutaParada
                                                 join b in ctx.Parada on a.ParadaIdparada equals b.Idparada
                                                 orderby a.Orden descending
                                                 select new ParadaCorrida()
                                                 {
                                                   idRuta = a.RutaIdruta,
                                                   idParada = b.Idparada,
                                                   nombre = b.Nombre,
                                                   parada = (b.Calle ?? "") + (("No. " + b.Numero) ?? "") + ((", " + b.Colonia) ?? "") + ((", " + b.Codigopostal) ?? "") + ((", " + b.Municipio) ?? "") + ((", " + b.Ciudad) ?? ""),
                                                   longitud = b.Longitud,
                                                   latitud = b.Latitud,
                                                   orden = a.Orden
                                                 }).Where(x => x.idRuta == c.Idruta).OrderByDescending(x => x.orden).First()
                                            ),
                                          pasajeros = (
                                                (from z in ctx.Viajes where z.CorridaAsignacionIdcorridaAsignacion == a.IdcorridaAsignacion select z.Numeropasajeros).Sum() ?? 0
                                            ),
                                          Calificacion = (int)a.Calificacion,
                                          MensajeCalificacion = a.Mensaje
                                        }).Distinct().OrderBy(x => x.fecha).ThenBy(x => x.horaInicio).ToList();

        if (data.idRuta != null)
          list = list.Where(x => x.idRuta == data.idRuta).ToList();

        if (data.fecha != null)
          list = list.Where(x => x.fecha == data.fecha).ToList();

        if (data.horario != null)
          list = list.Where(x => x.horaInicio == data.horario).ToList();


        foreach (var item in list)
        {
          if (item.idRuta > 0)
          {
            var paradas = (
                       from a in ctx.Ruta
                       join b in ctx.RutaParada on a.Idruta equals b.RutaIdruta
                       join c in ctx.Parada on b.ParadaIdparada equals c.Idparada
                       where a.Idruta == item.idRuta
                       && a.Activo == 1
                       && b.Activo == 1
                       && c.Activo == 1
                       select new CorridaEstacion()
                       {
                         Idruta = a.Idruta,
                         Idparada = c.Idparada,
                         Nombre = c.Nombre,
                         Direccion = (c.Calle ?? "") + (("No. " + c.Numero) ?? "") + ((", " + c.Colonia) ?? "") + ((", " + c.Codigopostal) ?? "") + ((", " + c.Municipio) ?? "") + ((", " + c.Ciudad) ?? ""),
                         Latitud = c.Latitud,
                         Longitud = c.Longitud,
                         IdEstatus = 1,
                         Orden = b.Orden
                       }).Distinct().OrderBy(x => x.Orden).ToList();
            item.Estaciones = paradas;
          }
        }

        response.Data = list;
      }
      response.CodigoMensaje = "200";
      response.Mensaje = response.Data.ToArray().Length == 0 ? "No se encontraron corridas" : "Búsqueda exitosa";

      return response;
    }

    public async Task<SuVanResponse<CorridaCalificacion>> CalificaCorrida(CorridaCalificacion data)
    {
      CorridaAsignacion calif = await context.CorridaAsignacions.Where(c => c.IdcorridaAsignacion == data.CorridaId).FirstOrDefaultAsync();

      SuVanResponse<CorridaCalificacion> response = new();
      response.CodigoMensaje = "400";
      response.Mensaje = "Datos incorrectos";

      if (calif != null)
      {
        calif.Calificacion = data.Calificacion;
        calif.Mensaje = data.Mensaje;
        context.CorridaAsignacions.Update(calif);


        var viajeUsuario = await context.Viajes.Where(c => c.CorridaAsignacionIdcorridaAsignacion == calif.IdcorridaAsignacion).ToListAsync();
        foreach (var item in viajeUsuario)
        {
          var nuevaCalificacion = new CalificacionUsuario
          {
            ViajeIdviaje = item.Idviaje,
            UsuarioIdusuario = item.UsuarioIdusuario,
            ConductorIdconductor = calif.ConductorIdconductor,
            Calificacion = data.Calificacion
          };

          context.CalificacionUsuarios.Add(nuevaCalificacion);
        }
        await context.SaveChangesAsync();
        response.Data = data;
        response.CodigoMensaje = "200";
        response.Mensaje = "Calificación de usuario realizada con exito";
      }

      return response;

    }

    public async Task<SuVanResponse<CalificacionesViajeResponse>> ConductorCalificaciones(int corrida_asignacionId)
    {
      SuVanResponse<CalificacionesViajeResponse> response = new();
      CalificacionesViajeResponse? result = new CalificacionesViajeResponse();
      result.Usuarios = new List<ConductorCalificacionesResponse>();
      result.Conductor = new List<ConductorCalificacionesResponse>();

      using (var ctx = context)
      {
        var calificacionUsuarios = await (from a in ctx.Viajes
                                          join b in ctx.CalificacionConductors on a.Idviaje equals b.ViajeIdviaje
                                          where a.CorridaAsignacionIdcorridaAsignacion == corrida_asignacionId
                                          &&
                                          (
                                          a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                                         || a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
                                          )
                                          select new
                                          {
                                            calificacion = b.Calificacion ?? 0,
                                            comentarios = b.Mensaje ?? string.Empty
                                          }).ToListAsync();

        foreach (var item in calificacionUsuarios)
        {
          result.Usuarios.Add(new ConductorCalificacionesResponse()
          {
            calificacion = item.calificacion,
            comentarios = item.comentarios
          });
        }

        var calificacionConductor = await (from a in ctx.Viajes
                                           join b in ctx.CorridaAsignacions on a.CorridaAsignacionIdcorridaAsignacion equals b.IdcorridaAsignacion
                                           where a.CorridaAsignacionIdcorridaAsignacion == corrida_asignacionId
                                           &&
                                           (
                                           a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.EN_CURSO)
                                          || a.EstatusviajeIdestatusviaje == Convert.ToInt32(EnumEstatusViaje.FINALIZADO)
                                           )
                                           select new
                                           {
                                             calificacion = b.Calificacion ?? 0,
                                             comentarios = b.Mensaje ?? string.Empty
                                           }).ToListAsync();

        foreach (var item in calificacionConductor)
        {
          result.Conductor.Add(new ConductorCalificacionesResponse()
          {
            calificacion = item.calificacion,
            comentarios = item.comentarios
          });
        }

        if (calificacionUsuarios.Count == 0 && calificacionConductor.Count == 0)
        {
          response.Mensaje = "No se encontraron calificaciones";
        }
        else
        {
          response.Mensaje = "Búsqueda exitosa";
        }
      }
      response.Data = result;
      response.CodigoMensaje = "200";

      return response;
    }

    public async Task<SuVanResponse<EstadisticasConductorResponse>> EstadisticasConductor(int conductorId, EstatidisticasConductorRequest data)
    {
      SuVanResponse<EstadisticasConductorResponse> response = new();

      DateTime fechaInicio;
      DateTime fechaFin;

      List<DetalleEstadisiticas> detalle = new List<DetalleEstadisiticas>();

      switch (data.Frecuencia)
      {
        case 1:
          DateTime.TryParseExact(data.FechaInicio,
                  "dd-MM-yyyy",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out fechaInicio);

          DateTime.TryParseExact(data.FechaFin,
                  "dd-MM-yyyy",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out fechaFin);

          detalle = EstadisticasConductorPorDia(conductorId, fechaInicio, fechaFin).Result;
          break;
        case 2:
          DateTime.TryParseExact(data.FechaInicio,
                  "MM-yyyy",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out fechaInicio);

          DateTime.TryParseExact(data.FechaFin,
                  "MM-yyyy",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out fechaFin);

          detalle = EstadisticasConductorPorMes(conductorId, fechaInicio, fechaFin).Result;
          break;
        case 3:
          DateTime.TryParseExact(data.FechaInicio,
                  "yyyy",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out fechaInicio);

          DateTime.TryParseExact(data.FechaFin,
                  "yyyy",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out fechaFin);

          detalle = EstadisticasConductorPorAnio(conductorId, fechaInicio, fechaFin).Result;
          break;
      }

      EstadisticasConductorResponse estadisticas = new EstadisticasConductorResponse();
      estadisticas.Detalle = detalle;
      estadisticas.Pasajeros = detalle.Select(x => x.Pasajeros).Sum();
      estadisticas.Viajes = detalle.Select(x => x.Viajes).Sum();
      estadisticas.Kilometros = detalle.Select(x => x.Kilometros).Sum();
      estadisticas.Ingresos = detalle.Select(x => x.Ingresos).Sum();

      response.Data = estadisticas;
      response.CodigoMensaje = detalle.Count() == 0 ? "400" : "200";
      response.Mensaje = detalle.Count() == 0 ? "No se encontraron resultados" : "Busqueda de estadisticas realizada con exito";

      return response;

    }

    private async Task<List<DetalleEstadisiticas>> EstadisticasConductorPorDia(int conductorId, DateTime fechaInicio, DateTime fechaFin)
    {
      List<DetalleEstadisiticas> estadisiticas = new List<DetalleEstadisiticas>();
      List<EstadisiticasConductorFrecuenciaDia> datos = new List<EstadisiticasConductorFrecuenciaDia>();

      datos = await (from a in context.CorridaAsignacions
                     where a.ConductorIdconductor == conductorId
                     && a.Fecha >= fechaInicio && a.Fecha <= fechaFin
                     group a by a.Fecha into corridasAsignadas
                     select new EstadisiticasConductorFrecuenciaDia
                     {
                       Fecha = corridasAsignadas.Key.Value
                     }).OrderBy(x => x.Fecha).ToListAsync();

      foreach (var item in datos)
      {
        DetalleEstadisiticas result = new DetalleEstadisiticas();
        result.Fecha = item.Fecha.ToString("dd/MM/yyyy");

        List<int> corridaAsinacionResult = await (from a in context.CorridaAsignacions
                                                  where a.ConductorIdconductor == conductorId
                                                  && a.Fecha == item.Fecha
                                                  select a.IdcorridaAsignacion).ToListAsync();

        result.Viajes = corridaAsinacionResult.Count();
        result.Ingresos = Ingresos(corridaAsinacionResult).Result;
        result.Pasajeros = NumeroDePasajeros(corridaAsinacionResult).Result;
        result.Kilometros = KilometrosRecorridos(corridaAsinacionResult).Result;

        estadisiticas.Add(result);
      }

      return estadisiticas;
    }

    private async Task<List<DetalleEstadisiticas>> EstadisticasConductorPorMes(int conductorId, DateTime fechaInicio, DateTime fechaFin)
    {
      List<DetalleEstadisiticas> estadisiticas = new List<DetalleEstadisiticas>();
      List<EstadisiticasConductorFrecuenciaMes> datos = new List<EstadisiticasConductorFrecuenciaMes>();

      datos = await (from a in context.CorridaAsignacions
                     where a.ConductorIdconductor == conductorId
                     && a.Fecha.Value.Year >= fechaInicio.Year && a.Fecha.Value.Month >= fechaInicio.Month
                     && a.Fecha.Value.Year <= fechaFin.Year && a.Fecha.Value.Month <= fechaFin.Month
                     group a by new { a.Fecha.Value.Year, a.Fecha.Value.Month } into corridasAsignadas
                     select new EstadisiticasConductorFrecuenciaMes
                     {
                       Mes = corridasAsignadas.Key.Month,
                       Anio = corridasAsignadas.Key.Year

                     }).OrderBy(x => x.Anio).OrderBy(x => x.Mes).ToListAsync();

      foreach (var item in datos)
      {
        DetalleEstadisiticas result = new DetalleEstadisiticas();
        result.Fecha = String.Format("{0}/{1}", item.Mes, item.Anio);

        List<int> corridaAsinacionResult = await (from a in context.CorridaAsignacions
                                                  where a.ConductorIdconductor == conductorId
                                                  && a.Fecha.Value.Year == item.Anio && a.Fecha.Value.Month == item.Mes
                                                  select a.IdcorridaAsignacion).ToListAsync();

        result.Viajes = corridaAsinacionResult.Count();
        result.Ingresos = Ingresos(corridaAsinacionResult).Result;
        result.Pasajeros = NumeroDePasajeros(corridaAsinacionResult).Result;
        result.Kilometros = KilometrosRecorridos(corridaAsinacionResult).Result;
        estadisiticas.Add(result);

      }

      return estadisiticas;
    }

    private async Task<List<DetalleEstadisiticas>> EstadisticasConductorPorAnio(int conductorId, DateTime fechaInicio, DateTime fechaFin)
    {
      List<DetalleEstadisiticas> estadisiticas = new List<DetalleEstadisiticas>();
      List<int> datos = new List<int>();

      datos = await (from a in context.CorridaAsignacions
                     where a.ConductorIdconductor == conductorId
                     && a.Fecha.Value.Year >= fechaInicio.Year && a.Fecha.Value.Year <= fechaFin.Year
                     group a by a.Fecha.Value.Year into corridasAsignadas
                     select corridasAsignadas.Key).OrderBy(x => x).ToListAsync();


      foreach (var item in datos)
      {
        DetalleEstadisiticas result = new DetalleEstadisiticas();
        result.Fecha = item.ToString();

        List<int> corridaAsinacionResult = await (from a in context.CorridaAsignacions
                                                  where a.ConductorIdconductor == conductorId
                                                  && a.Fecha.Value.Year == item
                                                  select a.IdcorridaAsignacion).ToListAsync();

        result.Viajes = corridaAsinacionResult.Count();
        result.Ingresos = Ingresos(corridaAsinacionResult).Result;
        result.Pasajeros = NumeroDePasajeros(corridaAsinacionResult).Result;
        result.Kilometros = KilometrosRecorridos(corridaAsinacionResult).Result;
        estadisiticas.Add(result);
      }

      return estadisiticas;
    }

    private async Task<int> NumeroDePasajeros(List<int> corridaAsignacionIds)
    {
      int pasajeros = 0;
      var status = new string[] { "En espera", "En curso", "Finalizado" };

      pasajeros = await (from a in context.Viajes
                         join b in context.Estatusviajes on a.EstatusviajeIdestatusviaje equals b.Idestatusviaje
                         where corridaAsignacionIds.Contains(a.CorridaAsignacionIdcorridaAsignacion.Value)
                         && status.Contains(b.Estatusviajecol)
                         select a.Numeropasajeros).SumAsync() ?? 0;

      return pasajeros;
    }

    private async Task<decimal> Ingresos(List<int> corridaAsignacionIds)
    {
      decimal ingresos = 0;
      var status = new string[] { "En espera", "En curso", "Finalizado" };

      ingresos = await (from a in context.Viajes
                        join b in context.Estatusviajes on a.EstatusviajeIdestatusviaje equals b.Idestatusviaje
                        join c in context.Transaccions on a.TransaccionIdtransaccion equals c.Idtransaccion
                        where corridaAsignacionIds.Contains(a.CorridaAsignacionIdcorridaAsignacion.Value)
                        && status.Contains(b.Estatusviajecol)
                        select c.Cantidad).SumAsync() ?? 0;

      return ingresos;
    }

    private async Task<decimal> KilometrosRecorridos(List<int> corridaAsignacionIds)
    {
      decimal distanciaRecorrida = 0;

      decimal distancia = await (from a in context.CorridaAsignacions
                                 join b in context.Corrida on a.CorridaIdcorrida equals b.Idcorrida
                                 join c in context.Ruta on b.RutaIdruta equals c.Idruta
                                 where corridaAsignacionIds.Contains(a.IdcorridaAsignacion)
                                 select c.Distanciamts).SumAsync() ?? 0;

      distanciaRecorrida += distancia;

      decimal km = Convert.ToDecimal((distanciaRecorrida / 1000).ToString("#.###"));

      return km;
    }

    public async Task<int> GeneraRecibo(int operadorID, string fechaInicio, string fechaFin)
    {
      DateTime fechaInicioOut;
      DateTime fechaFinOut;

      fechaInicioOut = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.CurrentCulture);
      fechaFinOut = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.CurrentCulture);


      var lLiquidaciones = await (from cl in context.CorridaLiquidacions
                                  join ca in context.CorridaAsignacions on cl.IdCorridaAsignacion equals ca.IdcorridaAsignacion
                                  where cl.Liquidado == false && ca.Fecha!.Value.Date >= fechaInicioOut.Date && ca.Fecha.Value.Date <= fechaFinOut.Date
                                     && ca.ConductorIdconductor == operadorID
                                  select new
                                  {
                                    cl.IdCorridaAsignacion,
                                    ca.Fecha,
                                    cl.MontoPagado,
                                    cl.MontoComision
                                  }
                            ).ToListAsync();

      if (lLiquidaciones.Count > 0)
      {
        decimal vMontoPagado,
        vMontoComision;

        vMontoPagado = lLiquidaciones.Sum(x => x.MontoPagado);
        vMontoComision = lLiquidaciones.Sum(x => x.MontoComision ?? 0);

        #region Inserta en DB
        var vHoy = DateTime.Today;
        LiquidacionCabecera vLiquidacionCab = new()
        {
          Idconductor = operadorID,
          FechaEmision = vHoy,
          FechaInico = fechaInicioOut,
          FechaFin = fechaFinOut,
          MontoPagado = vMontoPagado,
          MontoComision = vMontoComision
        };
        context.LiquidacionCabeceras.Add(vLiquidacionCab);
        await context.SaveChangesAsync();

        List<LiquidacionDetalle> lLiqDet = new List<LiquidacionDetalle>();
        List<CorridaLiquidacion> lCorLiq = new List<CorridaLiquidacion>();
        CorridaLiquidacion vCorLiq = new CorridaLiquidacion();
        foreach (var vLiq in lLiquidaciones)
        {
          lLiqDet.Add(new LiquidacionDetalle()
          {
            IdLiquidacion = vLiquidacionCab.IdLiquidacion,
            IdCorridaAsignacion = vLiq.IdCorridaAsignacion
          });

          vCorLiq = await context.CorridaLiquidacions.FirstOrDefaultAsync(x => x.IdCorridaAsignacion == vLiq.IdCorridaAsignacion);

          if (vCorLiq != null)
          {
            vCorLiq.Liquidado = true;
            lCorLiq.Add(vCorLiq);
          }
        }
        context.LiquidacionDetalles.AddRange(lLiqDet);
        context.CorridaLiquidacions.UpdateRange(lCorLiq);
        await context.SaveChangesAsync();
        #endregion

        return vLiquidacionCab.IdLiquidacion;
      }
      else
      {
        return 0;
      }
    }

    public async Task<byte[]> EmiteRecibo(int liquidacionID)
    {
      var vLiq = (from lc in context.LiquidacionCabeceras
                  where lc.IdLiquidacion == liquidacionID
                  select new
                  {
                    lc.IdconductorNavigation,
                    lc.FechaEmision,
                    lc.FechaInico,
                    lc.FechaFin,
                    lc.MontoPagado,
                    lc.MontoComision
                  }
      ).FirstOrDefault();

      var lLiq = (from ld in context.LiquidacionDetalles
                  join cl in context.CorridaLiquidacions on ld.IdCorridaAsignacion equals cl.IdCorridaAsignacion
                  join ca in context.CorridaAsignacions on ld.IdCorridaAsignacion equals ca.IdcorridaAsignacion
                  join c in context.Corrida on ca.CorridaIdcorrida equals c.Idcorrida
                  join r in context.Ruta on c.RutaIdruta equals r.Idruta
                  where ld.IdLiquidacion == liquidacionID
                  select new
                  {
                    c.Idcorrida,
                    ld.IdCorridaAsignacion,
                    r.Nombre,
                    ca.Fecha,
                    cl.MontoPagado,
                    cl.MontoComision
                  }
      ).ToList();

      var vEmpresa = context.Empresas.Where(x => x.Idempresa == vLiq.IdconductorNavigation.EmpresaIdempresa).FirstOrDefault();

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string HtmlContent = File.ReadAllText($@"{directorioBase}Plantilla\PlantillaRecibo.html");

      // Construir las filas de la tabla
      StringBuilder tableRows = new();
      int vCorrida = 0;
      foreach (var vLiqDet in lLiq)
      {
        if (vCorrida == 0)
        {
          tableRows.AppendFormat("<tr><td style='border-left: 1px solid; border-top: 1px solid;' align='left'>&nbsp;{0}</td><td style='border-top: 1px solid;'>{1}</td><td style='border-top: 1px solid;'>{2}</td><td style='border-right: 1px solid; border-top: 1px solid;'>{3}</td></tr>", vLiqDet.Nombre, vLiqDet.Fecha.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), vLiqDet.MontoPagado.ToString("c"), vLiqDet.MontoComision.Value.ToString("c"));
        }
        else if (vCorrida + 1 == lLiq.Count)
        {
          tableRows.AppendFormat("<tr><td style='border-left: 1px solid; border-bottom: 1px solid;' align='left'>&nbsp;{0}</td><td style='border-bottom: 1px solid;'>{1}</td><td style='border-bottom: 1px solid;'>{2}</td><td style='border-right: 1px solid; border-bottom: 1px solid;'>{3}</td></tr>", vLiqDet.Nombre, vLiqDet.Fecha.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), vLiqDet.MontoPagado.ToString("c"), vLiqDet.MontoComision.Value.ToString("c"));
        }
        else
        {
          tableRows.AppendFormat("<tr><td style='border-left: 1px solid;' align='left'>&nbsp;{0}</td><td>{1}</td><td>{2}</td><td style='border-right: 1px solid;'>{3}</td></tr>", vLiqDet.Nombre, vLiqDet.Fecha.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), vLiqDet.MontoPagado.ToString("c"), vLiqDet.MontoComision.Value.ToString("c"));
        }

        vCorrida++;
      }

      // Reemplazo de los marcadores de posición
      string htmlContent = HtmlContent
          .Replace("{{nombre_empresa}}", vEmpresa.Nombre)
          .Replace("{{fecha_emision}}", vLiq.FechaEmision.ToString("dd/MM/yyyy"))
          .Replace("{{nombre_operador}}", vLiq.IdconductorNavigation.Nombre)
          .Replace("{{curp}}", vLiq.IdconductorNavigation.Curp)
          .Replace("{{rfc}}", vLiq.IdconductorNavigation.Rfc)
          .Replace("{{periodo_liquidacion}}", vLiq.FechaInico.ToString("dd/MM/yyyy") + " al " + vLiq.FechaFin.ToString("dd/MM/yyyy"))
          .Replace("{{monto_pagado}}", vLiq.MontoPagado.Value.ToString("c"))
          .Replace("{{monto_comision}}", vLiq.MontoComision.Value.ToString("c"))
          .Replace("{{filasTabla}}", tableRows.ToString());

      byte[]? bytesPDF = Utilities.PDF.getBytesPDF(htmlContent);
      return bytesPDF;
    }



  }
}
