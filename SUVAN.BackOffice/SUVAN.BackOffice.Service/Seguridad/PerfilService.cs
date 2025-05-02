using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Seguridad
{
  public class PerfilService : IPerfilService
  {
    private readonly SuvanDbContext context;
    private readonly IPermisoService permisoService;

    public PerfilService(SuvanDbContext context, IPermisoService permisoService)
    {
      this.context = context;
      this.permisoService = permisoService;
    }

    /// <summary>
    /// Regresa el listado de perfiles
    /// </summary>
    /// <returns></returns>
    public async Task<List<Perfil>> GetPerfiles()
    {
      var perfils = await context.Perfils.ToListAsync();

      return perfils!;
    }


    /// <summary>
    /// Elimina un perfil
    /// </summary>
    /// <param name="perfilId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> EliminarPerfil(int perfilId)
    {
      var perfil = await context.Perfils.FirstOrDefaultAsync(x => x.Idperfil == perfilId);

      if (perfil == null)
      {
        throw new Exception("No se encontro el perfil");
      }

      await permisoService.DeletePermisoByPerfil(perfil.Idperfil);

      context.Perfils.Remove(perfil);

      await context.SaveChangesAsync();

      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<int> AgregarPerfil(AgregarPerfilViewModel model)
    {
      Perfil perfil = new Perfil();


      if (model.PerfilId > 0)
      {
        perfil = await context.Perfils.FirstOrDefaultAsync(x => x.Idperfil == model.PerfilId);

        if (perfil == null)
        {
          throw new Exception("No se encontro el perfil");
        }
        perfil.Nombre = model.Nombre;
        perfil.Activo = (ulong?)(model.Activo ? 1 : 0);
        perfil.Fecharegistro = DateTime.Now;

        context.Perfils.Entry(perfil);
      }
      else
      {
        perfil = new Perfil
        {
          Nombre = model.Nombre,
          Activo = (ulong?)(model.Activo ? 1 : 0),
          Fecharegistro = DateTime.Now
        };

        context.Perfils.Add(perfil);
      }

      await context.SaveChangesAsync();

      var permisos = new List<Permiso>();
      foreach (var opcion in model.Opciones.Where(x => x.Agregar || x.Editar || x.Eliminar || x.Ejecutar))
      {
        var permiso = new Permiso
        {
          MenuIdmenu = opcion.MenuId,
          PerfilIdperfil = perfil.Idperfil,
          Agregar = (ulong?)(opcion.Agregar ? 1 : 0),
          Modificar = (ulong?)(opcion.Editar ? 1 : 0),
          Eliminar = (ulong?)(opcion.Eliminar ? 1 : 0),
          Ejecutar = (ulong?)(opcion.Ejecutar ? 1 : 0),
          Activo = 1,
          Fecharegistro = DateTime.Now
        };

        permisos.Add(permiso);
      }

      await permisoService.DeletePermisoByPerfil(perfil.Idperfil);
      await permisoService.AgregarPermisoByPerfil(permisos);

      return perfil.Idperfil;
    }

    /// <summary>
    /// Carga el modelo para agregar o editar un perfil
    /// </summary>
    /// <param name="perfilId"></param>
    /// <returns></returns>
    public async Task<AgregarPerfilViewModel> GetPerfilViewModel(int perfilId)
    {
      List<Permiso> permisos = new List<Permiso>();
      Perfil? perfil = new Perfil();
      var menus = await context.Menus
        .Include(x => x.MenuIdpadreNavigation)
        .Where(x => x.MenuIdpadre != null && x.Activo == 1)
        .ToListAsync();


      if (perfilId > 0)
      {
        perfil = await context.Perfils.FirstOrDefaultAsync(x => x.Idperfil == perfilId);
        permisos = await permisoService.GetPermisoByUsuario(perfilId);
      }


      var model = new AgregarPerfilViewModel
      {
        Opciones = menus.OrderBy(x => x.MenuIdpadre)
        .Select(x => new OpcionPerfilViewModel
        {
          MenuId = x.Idmenu,
          MenuNombre = x.Titulo!,
          MenuSeccion = x.MenuIdpadreNavigation!.Titulo!,
          Agregar = false,
          Editar = false,
          Eliminar = false,
          Ejecutar = false
        }).ToList()
      };

      GetOpcionesPerfil(perfilId, permisos, perfil, model);

      return model;
    }

    /// <summary>
    /// Carga las opciones seleccionadas del perfil que se esta consultando
    /// </summary>
    /// <param name="perfilId"></param>
    /// <param name="permisos"></param>
    /// <param name="perfil"></param>
    /// <param name="model"></param>
    private void GetOpcionesPerfil(int perfilId, List<Permiso> permisos, Perfil? perfil, AgregarPerfilViewModel model)
    {
      if (perfilId > 0 && permisos.Any() && perfil != null)
      {
        model.PerfilId = perfilId;
        model.Nombre = perfil.Nombre!;
        model.Activo = perfil.Activo == 1;
        foreach (var opcion in model.Opciones)
        {
          var permiso = permisos.FirstOrDefault(x => x.MenuIdmenu == opcion.MenuId);

          if (permiso != null)
          {
            opcion.Agregar = permiso.Agregar == 1;
            opcion.Editar = permiso.Modificar == 1;
            opcion.Eliminar = permiso.Eliminar == 1;
            opcion.Ejecutar = permiso.Ejecutar == 1;
          }
        }

      }
    }
  }

}

