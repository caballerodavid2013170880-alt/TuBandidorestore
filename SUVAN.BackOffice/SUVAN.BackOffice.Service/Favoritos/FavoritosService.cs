using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.Favoritos;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Service.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUVAN.BackOffice.Service.Favoritos
{
    public class FavoritosService : IFavoritosService
    {
        private readonly SuvanDbContext _context;
        public FavoritosService(SuvanDbContext context)
        {
            _context = context;

        }
        public async Task<SuVanResponse<List<FavoritoModel>>> ConsultaFavoritos(string userId)
        {
            SuVanResponse<List<FavoritoModel>> response = new();
            using (var context = _context)
            {
                var result = (from o in context.Favoritos
                              where o.UsuarioIdusuario == int.Parse(userId)
                              && o.Activo == 1
                              select new FavoritoModel()
                              {
                                  TipoFavorito = o.TipofavoritoIdtipofavorito,
                                  Direccion = o.Direccion,
                                  Latitud = o.Latitud,
                                  Longitud = o.Longitud,
                                  Activo = (int?)o.Activo
                              }).ToList();
                response.Data = result;
            }
            response.CodigoMensaje = "200";
            response.Mensaje = "Búsqueda exitosa";
            return response;
        }
        public async Task<SuVanResponse<FavoritoModel>> EstableceFavorito(string usuarioId, FavoritoModel model)
        {
            SuVanResponse<FavoritoModel> response = new();
            Favorito? FavoritoEntity = _context.Favoritos.FirstOrDefault(x => x.TipofavoritoIdtipofavorito == model.TipoFavorito && x.UsuarioIdusuario == int.Parse(usuarioId));
            if (FavoritoEntity == null)
            {
                var entity = new Favorito()
                {
                    TipofavoritoIdtipofavorito = model.TipoFavorito,
                    Latitud = model.Latitud,
                    Longitud = model.Longitud,
                    Direccion = model.Direccion,
                    UsuarioIdusuario = int.Parse(usuarioId),
                    Activo = (ulong?)model.Activo,
                    Fecharegistro = DateTime.Now
                };
                _context.Favoritos.Add(entity);
                model.TipoFavorito = entity.TipofavoritoIdtipofavorito;
            }
            else
            {
                FavoritoEntity.Latitud = model.Latitud;
                FavoritoEntity.Longitud = model.Longitud;
                FavoritoEntity.Direccion = model.Direccion;
                FavoritoEntity.Activo = (ulong?)model.Activo;
                FavoritoEntity.Fecharegistro = DateTime.Now;
                _context.Favoritos.Entry(FavoritoEntity);
                model.TipoFavorito = FavoritoEntity.TipofavoritoIdtipofavorito;

            }
            await _context.SaveChangesAsync();            
            response.Data = model;
            response.CodigoMensaje = "200";
            response.Mensaje = "Solicitud exitosa";
            return response;
        }

        public async Task<SuVanResponse<FavoritoPersonalResponse>> CreaFavoritoPersonal(string usuarioId, FavoritoPersonalModel model)
        {
            SuVanResponse<FavoritoPersonalResponse> response = new();

            FavoritoPersonalResponse? result = new FavoritoPersonalResponse();

            var entity = new Favoritopersonal()
            {
                Nombre  = model.Nombre,
                Latitud = model.Latitud,
                Longitud = model.Longitud,
                Direccion = model.Direccion,
                UsuarioIdusuario = int.Parse(usuarioId),
                Activo = (ulong?)model.Activo,
                Fecharegistro = DateTime.Now
            };
            //if (entity.Idfavoritopersonal == 0)
                _context.Favoritopersonals.Add(entity);
            //else
            //     _context.Favoritopersonals.Entry(entity);
            await _context.SaveChangesAsync();
            result.Id = entity.Idfavoritopersonal;
            result.Nombre = model.Nombre;
            result.Latitud = model.Latitud;
            result.Longitud = model.Longitud;
            result.Direccion = model.Direccion;
            result.Activo = model.Activo;
            response.Data = result;

            response.CodigoMensaje = "200";
            response.Mensaje = "Solicitud exitosa";
            return response;
        }

        public async Task<SuVanResponse<List<FavoritoPersonalResponse>>> FavoritosPersonal(string userId)
        {
            SuVanResponse<List<FavoritoPersonalResponse>> response = new();
            using (var context = _context)
            {
               response.Data = await (from o in context.Favoritopersonals
                              where o.UsuarioIdusuario == int.Parse(userId) 
                              && o.Activo == 1
                              select new FavoritoPersonalResponse()
                              {
                                  Id = o.Idfavoritopersonal,
                                  Nombre = o.Nombre,
                                  Direccion = o.Direccion,
                                  Latitud = o.Latitud,
                                  Longitud = o.Longitud,
                                  Activo = (int?)o.Activo,
                              }).ToListAsync();
            }
            response.CodigoMensaje = "200";
            response.Mensaje = "Búsqueda exitosa";
            return response;
        }

        public async Task<SuVanResponse<string>> RemueveFavorito(string favoritopersonalId, string usuarioId)
        {
            SuVanResponse<string> response = new();

            var favoritopersonalEntity = await _context.Favoritopersonals.FirstOrDefaultAsync(
                x => x.Idfavoritopersonal == int.Parse(favoritopersonalId) 
                && x.UsuarioIdusuario == int.Parse(usuarioId));

            if (favoritopersonalEntity == null)
            {
                response.CodigoMensaje = "401";
                response.Mensaje = "No se encontro información";
                response.Data = null;
                return response;
            }
            _context.Favoritopersonals.Remove(favoritopersonalEntity);
            await _context.SaveChangesAsync();
            response.CodigoMensaje = "200";
            response.Mensaje = "Registro eliminado";
            response.Data = null;
            return response;
        }
    }
}
