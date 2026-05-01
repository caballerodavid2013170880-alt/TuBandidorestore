using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public class DepositoService : IDepositoService
    {
        private readonly SuvanDbContext context;

        public DepositoService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Deposito>> GetDepositos(short id_emp, short id_region, short id_planta, short id_zona)
        {
            // Opcional: Filtrar por emp, region, planta y zona si son mayores a 0
            var query = context.Depositos.AsQueryable();

            if (id_emp > 0) query = query.Where(x => x.IdEmpresa == id_emp);
            if (id_region > 0) query = query.Where(x => x.IdRegion == id_region);
            if (id_planta > 0) query = query.Where(x => x.IdPlanta == id_planta);
            if (id_zona > 0) query = query.Where(x => x.IdZona == id_zona);

            return await query.ToListAsync();
        }

        public async Task<DepositoViewModel> GetDepositoViewModel(short? id_emp, short id_region, short id_planta, short id_zona, short id_deposito)
        {
            DepositoViewModel vRet = new DepositoViewModel();
            var deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdEmpresa == id_emp && x.IdRegion == id_region && x.IdPlanta == id_planta && x.IdZona == id_zona && x.IdDeposi == id_deposito);

            if (deposito == null)
                return vRet;

            var empresa = await context.Empresas.FirstOrDefaultAsync(e => e.Idempresa == (int?)deposito.IdEmpresa);
            var region = await context.Regions.FirstOrDefaultAsync(r => r.IdRegion == deposito.IdRegion);
            var planta = await context.Planta.FirstOrDefaultAsync(p => p.IdPlanta == deposito.IdPlanta);
            var zona = await context.Zonas.FirstOrDefaultAsync(z => z.IdZona == (int)deposito.IdZona);

            vRet = new DepositoViewModel
            {
                id_empresa = deposito.IdEmpresa,
                id_region = deposito.IdRegion,
                id_planta = deposito.IdPlanta,
                id_zona = deposito.IdZona,
                id_deposito = deposito.IdDeposi,
                descripcion = deposito.Descrip?.Trim() ?? string.Empty,
                direccion = deposito.Direc?.Trim(),
                ciudad = deposito.Ciudad?.Trim(),
                responsable = deposito.Respon?.Trim(),
                telefono = deposito.Tel?.Trim(),
                loc_for = deposito.LocFor?.Trim(),
                r_person = deposito.RPerson?.Trim(),
                desc_corta = deposito.DescCorta?.Trim(),
                rfc = deposito.Rfc?.Trim(),
                cp = deposito.Cp?.Trim(),
                
                nombre_empresa = empresa?.Nombre,
                nombre_region = region?.Nombre,
                nombre_planta = planta?.Nombre,
                nombre_zona = zona?.NombreZona
            };

            return vRet;
        }

        public async Task<bool> AgregarDeposito(DepositoViewModel model)
        {
            // Validación de integridad referencial simulada
            if (model.id_empresa != null && model.id_empresa > 0)
            {
                bool existeEmpresa = await context.Empresas.AnyAsync(e => e.Idempresa == (int?)model.id_empresa);
                if (!existeEmpresa)
                    throw new Exception($"Restricción referencial: La empresa con Id {model.id_empresa} no existe.");
            }

            bool existeRegion = await context.Regions.AnyAsync(r => r.IdRegion == model.id_region);
            if (!existeRegion)
                throw new Exception($"Restricción referencial: La región con Id {model.id_region} no existe.");

            bool existePlanta = await context.Planta.AnyAsync(p => p.IdPlanta == model.id_planta);
            if (!existePlanta)
                throw new Exception($"Restricción referencial: La planta con Id {model.id_planta} no existe.");

            bool existeZona = await context.Zonas.AnyAsync(z => z.IdZona == (int)model.id_zona);
            if (!existeZona)
                throw new Exception($"Restricción referencial: La zona con Id {model.id_zona} no existe.");

            Deposito deposito;

            if (model.id_deposito > 0)
            {
                deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdEmpresa == model.id_empresa && x.IdDeposi == model.id_deposito);

                if (deposito == null)
                    throw new Exception("No se encontró el depósito");

                // Mantiene los identificadores originales en BD para no sobreescribir.
                model.id_region = deposito.IdRegion;
                model.id_planta = deposito.IdPlanta;
                model.id_zona = deposito.IdZona;
            }
            else
            {
                deposito = new Deposito();
                deposito.IdEmpresa = model.id_empresa;
                deposito.IdRegion = model.id_region;
                deposito.IdPlanta = model.id_planta;
                deposito.IdZona = model.id_zona;

                var vLastRow = await context.Depositos.OrderByDescending(x => x.IdDeposi).FirstOrDefaultAsync();
                deposito.IdDeposi = (short)((vLastRow != null ? vLastRow.IdDeposi : 0) + 1);
            }

            var depositoExistente = await context.Depositos.FirstOrDefaultAsync(x => x.Descrip!.ToLower() == model.descripcion!.ToLower()
            && x.IdEmpresa == model.id_empresa && x.IdDeposi != model.id_deposito && x.IdZona == model.id_zona);

            if (depositoExistente is not null)
                throw new Exception("Ya existe un depósito con la misma descripción en esta zona.");

            deposito.Descrip = model.descripcion;
            deposito.Direc = model.direccion;
            deposito.Ciudad = model.ciudad;
            deposito.Respon = model.responsable;
            deposito.Tel = model.telefono;
            deposito.LocFor = model.loc_for;
            deposito.RPerson = model.r_person;
            deposito.DescCorta = model.desc_corta;
            deposito.Rfc = model.rfc;
            deposito.Cp = model.cp;

            if (model.id_deposito > 0)
            {
                context.Depositos.Entry(deposito);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Depositos.Add(deposito);
                await context.SaveChangesAsync();
            }
            return true;
        }
    }
}
