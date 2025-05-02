using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace SUVAN.BackOffice.Service.Seguridad
{
  public class AdminService : IAdminService
  {
    private readonly SuvanDbContext context;
    private readonly IPerfilService perfilService;
    private readonly INotificacionCorreoService notificacionCorreoService;
    private readonly IEmpresasService empresasService;

    public AdminService(SuvanDbContext context, IPerfilService perfilService,
      INotificacionCorreoService notificacionCorreoService,
      IEmpresasService empresasService)
    {
      this.context = context;
      this.perfilService = perfilService;
      this.notificacionCorreoService = notificacionCorreoService;
      this.empresasService = empresasService;
    }

    /// <summary>
    /// Obtener las empresas relacionadas a un usuario
    /// </summary>
    /// <param name="adminId"></param>
    /// <returns></returns>
    public async Task<List<AdminEmpresa>> GetEmpresaUsuario(int adminId)
    {
      var adminEmpresa = await context.AdminEmpresas
        .Include(x => x.EmpresaIdempresaNavigation)
        .Where(x => x.AdminIdadmin == adminId)
        .ToListAsync();

      return adminEmpresa!;
    }

    /// <summary>
    /// Obtiene un usuario del portal por su email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Admin> GetAdmin(string email)
    {
      var admins = await context.Admins
         .Include(x => x.AdminEmpresas)
          .ThenInclude(x => x.EmpresaIdempresaNavigation)
         .FirstOrDefaultAsync(x => x.Email == email);

      return admins!;
    }


    /// <summary>
    ///  Obtiene un usuario del portal por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Admin> GetAdmin(int id)
    {
      var admins = await context.Admins
         .Include(x => x.AdminEmpresas)
         .ThenInclude(x => x.EmpresaIdempresaNavigation)
        .FirstOrDefaultAsync(x => x.Idadmin == id);

      return admins!;
    }

    /// <summary>
    /// Actualizar la empresa principal de un usuario
    /// </summary>
    /// <param name="adminId"></param>
    /// <param name="empresaId"></param>
    /// <returns></returns>
    public async Task<int> CambiarEmpresa(int adminId, int empresaId)
    {
      var adminEmpresaPrincipal = await context.AdminEmpresas
        .FirstOrDefaultAsync(x => x.AdminIdadmin == adminId && x.EmpresaIdempresa == empresaId);
      if (adminEmpresaPrincipal is null)
      {
        throw new Exception("No se encontro la configuración");
      }

      // actuializa todas las empresas en 0 para que no sea principal
      var adminEmpresa = await context.AdminEmpresas
        .Where(x => x.AdminIdadmin == adminId)
        .ExecuteUpdateAsync(x => x.SetProperty(x => x.Principal, (ulong)0));

      // actualiza la empresa principal
      adminEmpresaPrincipal.Principal = 1;
      await context.SaveChangesAsync();

      return adminEmpresaPrincipal!.AdminIdadmin!;
    }

    /// <summary>
    /// Regresa el listado de usuarios del portal
    /// </summary>
    /// <returns></returns>
    public async Task<List<Admin>> GetAdmins()
    {
      var admins = await context.Admins
        .ToListAsync();

      return admins!;
    }

    /// <summary>
    /// Activacion de cuenta de usuario
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Admin> ActivarAdmin(ActivarUsuarioViewModel model, bool esActivar = true)
    {
      var admin = await context.Admins
        .Include(x => x.AdminEmpresas)
          .ThenInclude(x => x.EmpresaIdempresaNavigation)
          .FirstOrDefaultAsync(x => x.Email == model.Email);

      if (admin is null)
      {
        throw new Exception("Correo inválido!");
      }

      admin.Hashpassword = model.Password.HashTexto();
      if (esActivar)
      {
        admin.Activo = 1;
      }
      admin.Fecharegistro = DateTime.Now;
      context.Admins.Entry(admin);

      await context.SaveChangesAsync();

      return admin;
    }

    /// <summary>
    /// Activar Usuario de la app
    /// </summary>
    /// <param name="model"></param>
    /// <param name="esActivar"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Usuario> ActivarUsuarioApp(ActivarUsuarioViewModel model, bool esActivar = true)
    {
      var usuario = await context.Usuarios
          .FirstOrDefaultAsync(x => x.Email == model.Email);

      if (usuario is null)
      {
        throw new Exception("Correo inválido!");
      }

      usuario.Hashpass = model.Password.GetHashSHA256();
      if (esActivar)
      {
        usuario.Activo = 1;
      }
      usuario.Fecharegistro = DateTime.Now;
      context.Usuarios.Entry(usuario);

      await context.SaveChangesAsync();

      return usuario;
    }

    /// <summary>
    /// reestablece contraseña de un conductor
    /// </summary>
    /// <param name="model"></param>
    /// <param name="esActivar"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Database.Entities.Conductor> ActivarUsuarioAppCond(ActivarUsuarioViewModel model, bool esActivar = true)
    {
      var conductor = await context.Conductors
          .FirstOrDefaultAsync(x => x.Email == model.Email);

      if (conductor is null)
      {
        throw new Exception("Correo inválido!");
      }

      conductor.Hashpass = model.Password.GetHashSHA256();
      if (esActivar)
      {
        conductor.Activo = 1;
      }
      conductor.Fecharegistro = DateTime.Now;
      context.Conductors.Entry(conductor);

      await context.SaveChangesAsync();

      return conductor;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> OlvidoContra(string email)
    {
      var admin = await context.Admins.FirstOrDefaultAsync(x => x.Email == email);

      if (admin is null)
      {
        throw new Exception("Correo inválido!");
      }

      var token = TokenCorreoPortal.GeneraToken(email, 1);// se da una vigencia de 1 dias al token en el alta
      var result = await notificacionCorreoService.OlvidoPassword(email, admin.Nombre!, token);

      return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> OlvidoContraUsuarioApp(string email)
    {
      var usuario = await context.Usuarios
        .FirstOrDefaultAsync(x => x.Email == email);

      if (usuario is null)
      {
        throw new Exception("Correo inválido!");
      }

      var token = TokenCorreoPortal.GeneraToken(email, 1);// se da una vigencia de 1 dias al token en el alta
      var result = await notificacionCorreoService.OlvidoPasswordApp(email, usuario.Nombre!, token);

      return result;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<int> AgregarUsuario(AgregarUsuarioViewModel model)
    {
      Admin admin = new Admin();


      if (model.AdminId == 0)
      {
        var existUser = await context.Admins.FirstOrDefaultAsync(x => x.Email == model.Email);
        if (existUser != null)
        {
          throw new Exception("Un usuario ya existe con este correo electrónico");
        }
      }

      if (model.AdminId > 0)
      {
        admin = await context.Admins.FirstOrDefaultAsync(x => x.Idadmin == model.AdminId);
      }

      admin.Nombre = model.Nombre;
      admin.Email = model.Email;
      //admin.PerfilIdperfil = (int)model.PerfilId!;
      admin.Activo = (ulong?)(model.Activo ? 1 : 0);
      admin.Fecharegistro = DateTime.Now;

      if (model.AdminId > 0)
      {
        context.Admins.Entry(admin);
      }
      else
      {
        context.Admins.Add(admin);
      }

      await context.SaveChangesAsync();

      var empresas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmpresaUsuarioViewModel>>(model.EmpresasUsuario);

      await GuardaAdminEmpresa(empresas!, admin.Idadmin);

      //:: TODO enviar correo de alta de usuario
      if (model.AdminId == 0)
      {
        await SendAltaEmail(model.Email, model.Nombre);
      }

      return admin.Idadmin;
    }

    /// <summary>
    /// Envia correo de alta de usuario
    /// </summary>
    /// <param name="email"></param>
    /// <param name="nombre"></param>
    /// <returns></returns>
    private async Task<bool> SendAltaEmail(string email, string nombre)
    {
      var token = TokenCorreoPortal.GeneraToken(email, 7);// se da una vigencia de 7 dias al token en el alta
      var result = await notificacionCorreoService.EnviarCorreoActivacionPortal(email, nombre, token);

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adminId"></param>
    /// <returns></returns>
    public async Task<AgregarUsuarioViewModel> GetAdminViewModel(int adminId)
    {
      Admin? admin = new Admin();
      List<Perfil> perfiles = await perfilService.GetPerfiles();
      List<Empresa> empresas = await empresasService.GetEmpresas();
      if (adminId > 0)
      {
        admin = await context.Admins.FirstOrDefaultAsync(x => x.Idadmin == adminId);
      }

      AgregarUsuarioViewModel viewModel = new AgregarUsuarioViewModel();


      viewModel.Perfiles = perfiles.Select(x => new PerfilViewModel
      {
        PerfilId = x.Idperfil,
        Nombre = x.Nombre!,

      }).ToList();

      viewModel.Empresas = empresas.Select(x => new EmpresaViewModel
      {
        EmpresaId = x.Idempresa,
        Nombre = x.Nombre!,

      }).ToList();

      if (admin != null)
      {
        viewModel.AdminId = adminId;
        viewModel.Nombre = admin?.Nombre ?? string.Empty;
        viewModel.Email = admin?.Email ?? string.Empty;
        viewModel.PerfilId = admin?.PerfilIdperfil ?? 0;
        viewModel.Activo = adminId == 0 ? true : admin?.Activo == 1;

        viewModel.EmpresasSeleccion = await ObternerEmpresasByAdmin(adminId)
       .ContinueWith(x => x.Result.Select(x => new EmpresaUsuarioViewModel
       {
         empresaId = x.EmpresaIdempresa,
         empresaNombre = x.EmpresaIdempresaNavigation.Nombre!,
         perfilId = x.PerfilIdperfil,
         perfilNombre = x.PerfilIdperfilNavigation.Nombre!,
         esPrincipal = x.Principal == 1 ? true : false
       }).ToList());
      }

      if (viewModel.EmpresasSeleccion.Any())
      {
        viewModel.EmpresasUsuario = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.EmpresasSeleccion);

      }
      return viewModel!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adminId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> EliminarAdmin(int adminId)
    {
      var admin = await context.Admins.FirstOrDefaultAsync(x => x.Idadmin == adminId);

      if (admin is null)
      {
        throw new Exception("No se encontro el usuario");
      }

      admin.Activo = 0;
      admin.Fecharegistro = DateTime.Now;
      context.Admins.Entry(admin);

      await context.SaveChangesAsync();

      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<Admin> AutenticarUsuario(string email, string password)
    {
      var admin = await context.Admins
        .Include(x => x.AdminEmpresas)
        .ThenInclude(x => x.EmpresaIdempresaNavigation)
        .FirstOrDefaultAsync(x => x.Email == email
              && x.Hashpassword == password.HashTexto()
              && x.Activo == 1);

      return admin!;
    }


    /// <summary>
    /// Guarda las empresas relacionadas a un usuario y su perfil
    /// </summary>
    /// <param name="empresasUsuario"></param>
    /// <param name="adminId"></param>
    /// <returns></returns>
    private async Task<int> GuardaAdminEmpresa(List<EmpresaUsuarioViewModel> empresasUsuario, int adminId)
    {
      await EleminiarAdminEmpresa(adminId);

      var esRegistroUnico = empresasUsuario.Count() == 1 ? true : false;
      foreach (var empresa in empresasUsuario)
      {
        AdminEmpresa adminEmpresa = new();
        adminEmpresa.AdminIdadmin = adminId;
        adminEmpresa.EmpresaIdempresa = empresa.empresaId;
        adminEmpresa.PerfilIdperfil = empresa.perfilId;
        if (esRegistroUnico)
          adminEmpresa.Principal = 1;
        else
          adminEmpresa.Principal = (ulong?)(empresa.esPrincipal ? 1 : 0);

        context.AdminEmpresas.Add(adminEmpresa);
      }

      var result = await context.SaveChangesAsync();

      return result;

    }

    /// <summary>
    /// Elimina las empresas relacionadas a un usuario
    /// </summary>
    /// <param name="adminId"></param>
    /// <returns></returns>
    private async Task<int> EleminiarAdminEmpresa(int adminId)
    {
      var adminEmpresa = await context.AdminEmpresas
        .Where(x => x.AdminIdadmin == adminId)
        .ExecuteDeleteAsync();

      return adminEmpresa;

    }

    /// <summary>
    /// obtiene las empresas relacionadas a un usuario y su perfil
    /// </summary>
    /// <param name="adminId"></param>
    /// <returns></returns>
    private async Task<List<AdminEmpresa>> ObternerEmpresasByAdmin(int adminId)
    {
      var adminEmpresa = await context.AdminEmpresas
        .Include(x => x.EmpresaIdempresaNavigation)
        .Include(x => x.PerfilIdperfilNavigation)
        .Where(x => x.AdminIdadmin == adminId)
        .ToListAsync();

      return adminEmpresa;
    }
  }
}
