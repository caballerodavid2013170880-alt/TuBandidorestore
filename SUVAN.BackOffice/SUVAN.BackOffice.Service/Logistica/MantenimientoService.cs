using SUVAN.BackOffice.Models.ViewModel.Logistica;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
                                       IdTaller = t.IdTaller,
                                       NombreTaller = t.NombreTaller,
                                       NombreZona = t.ZonaIdzonaNavigation.NombreZona,
                                       NombreDeposito = t.IdDepositoNavigation.DepositoNombre,
                                       Domicilio = t.Domicilio,
                                       Telefono = t.Telefono,
                                   }).ToListAsync();

            return resultado;
        }

        public async Task<List<MecanicoViewModel>> ObtenerMecanico(int tallerId)
        {
            var mecanicos = await context.Mecanicos.Where(m => m.IdTaller == tallerId).Select(m => new MecanicoViewModel
            {
                    IdTaller = m.IdTaller,
                    Nombre = m.Nombre,
                    Puesto = m.Puesto,
                    NombreTaller = m.IdTallerNavigation.NombreTaller,
                    NombreDeposito = m.IdDepositoNavigation.DepositoNombre

            }).ToListAsync();

            return mecanicos;
        }
    }
}
