using SUVAN.BackOffice.Models.ViewModel.Logistica;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly SuvanDbContext context;

        public MantenimientoService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TallerViewModel>> ObtenerTaller(int IdEmpresa)
        {
            var resultado = await (from t in context.Tallers
                                   where t.ZonaIdzonaNavigation.IdEmpresaNavigation.Idempresa == IdEmpresa
                                   select new TallerViewModel
                                   {
                                       NombreTaller = t.NombreTaller,
                                       NombreZona = t.ZonaIdzonaNavigation.NombreZona,
                                       NombreDeposito = t.IdDepositoNavigation.DepositoNombre,
                                       Domicilio = t.Domicilio,
                                       Telefono = t.Telefono,
                                   }).ToListAsync();

            return resultado;
        }

        public async Task<List<MecanicoViewModel>> ObtenerMecanico(int IdEmpresa)
        {
            var resultado = await (from m in context.Mecanicos
                                   where m.IdDepositoNavigation.IdEmpresaNavigation.Idempresa == IdEmpresa
                                   select new MecanicoViewModel
                                   {
                                       Nombre = m.Nombre,
                                       NombreDeposito = m.IdDepositoNavigation.DepositoNombre,
                                       NombreTaller = m.IdTallerNavigation.NombreTaller,
                                       Puesto = m.Puesto,
                                       
                                   }).ToListAsync();

            return resultado;
        }


    }
}
