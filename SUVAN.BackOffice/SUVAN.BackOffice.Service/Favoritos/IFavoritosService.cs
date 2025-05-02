using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Favoritos;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Favoritos
{
    public interface IFavoritosService
    {
        public  Task<SuVanResponse<List<FavoritoModel>>> ConsultaFavoritos(string userId);
        public  Task<SuVanResponse<FavoritoModel>> EstableceFavorito(string usuarioId, FavoritoModel model);
        public  Task<SuVanResponse<List<FavoritoPersonalResponse>>> FavoritosPersonal(string userId);
        public Task<SuVanResponse<FavoritoPersonalResponse>> CreaFavoritoPersonal(string usuarioId, FavoritoPersonalModel model);
        public Task<SuVanResponse<string>> RemueveFavorito(string favoritopersonalId, string usuarioId);

    }
}
