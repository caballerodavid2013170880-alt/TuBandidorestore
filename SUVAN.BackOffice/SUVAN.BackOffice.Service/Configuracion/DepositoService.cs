using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public class DepositosService : IDepositoService
    {
        private readonly SuvanDbContext context;

        public DepositosService(SuvanDbContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Obtiene el listado de empresas desde la base de datos.
        /// </summary>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        /// <returns>Lista de empresas.</returns>
        public async Task<List<Deposito>> GetDepositos(int id_empresa)
        {
            var depositos = await context.Depositos.Where(x => x.IdEmpresa == id_empresa).ToListAsync();

            return depositos!;
        }

        /// <summary>
        /// Obtiene el ViewModel para el depósito específico.
        /// </summary>
        /// <param name="nombre">Nombre del depósito.</param>
        /// <param name="id_empresa">Identificador de la empresa.</param>
        ///  <param name="id_region">Identificador de la región.</param>
        ///  <param name="id_planta">Identificador de la planta.</param>
        ///  <param name="id_zona">Identificador de la zona.</param>
        /// <param name="id_deposito">Identificador del depósito.</param>
        /// <returns>ViewModel para el depósito específico.</returns>
        public async Task<DepositoViewModel> GetDepositoViewModel(int id_empresa, int id_deposito)
        {
            DepositoViewModel vRet = new DepositoViewModel();
            var deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdEmpresa == id_empresa & x.IdDeposi == id_deposito);

            if (deposito == null)
                return vRet;
            else
            {
                vRet = new DepositoViewModel
                {
                    id_empresa = deposito.IdEmpresa ?? 0,
                    id_region = deposito.IdRegion,
                    id_planta = deposito.IdPlanta,
                    id_zona = deposito.IdZona,
                    id_deposi = deposito.IdDeposi,
                    descrip = deposito.Descrip,//se usa descripcion para el nombre del deposito no hay un campo nombre en la tabla deposito
                    direc = deposito.Direc,
                    ciudad = deposito.Ciudad,
                    respon = deposito.Respon,
                    tel = deposito.Tel,
                    loc_for = deposito.LocFor,
                    r_person = deposito.RPerson,
                    desc_corta = deposito.DescCorta,
                    rfc = deposito.Rfc,
                    cp = deposito.Cp
                };
            }

            return vRet;

        }
        /// <summary>
        /// Agrega o actualiza un depósito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del depósito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarDeposito(DepositoViewModel model)
        {
            Deposito deposito;


            if (model.id_deposi > 0)
            {
                deposito = await context.Depositos.FirstOrDefaultAsync(x => x.IdEmpresa == model.id_empresa && x.IdDeposi == model.id_deposi);

                if (deposito == null)
                    throw new Exception("No se encontro el deposito. ");
            }
            else
            {
                deposito = new Deposito();
                deposito.IdEmpresa = model.id_empresa;

                //// se agrega el identity manualmente que no esta en la bd
                //var vLastRow = await context.Depositos.OrderBy(x => x.IdEmpresa).LastOrDefaultAsync(x => x.IdEmpresa == model.id_empresa);
                //deposito.IdDeposi = (short)((vLastRow != null ? vLastRow.IdDeposi : 0) + 1);

                // se agrega el identity manualmente que no esta en la bd
                var vLastRow = await context.Depositos.OrderByDescending(x => x.IdDeposi).FirstOrDefaultAsync();
                deposito.IdDeposi = (short)((vLastRow != null ? vLastRow.IdDeposi : 0) + 1);
            }

            // validate if exist one deposit with the same name in the same empresa
            var depositoExistente = await context.Depositos.FirstOrDefaultAsync(x => x.Descrip!.ToLower() == model.descrip!.ToLower()
            && x.IdEmpresa != model.id_empresa);

            if (depositoExistente is not null)
                throw new Exception("Ya existe un depósito con el mismo nombre en la empresa");


            deposito.Descrip = model.descrip;
            deposito.IdRegion = (short)model.id_region;
            deposito.IdPlanta = (short)model.id_planta;
            deposito.IdZona = (short)model.id_zona;
            deposito.Direc = model.direc;
            deposito.Ciudad = model.ciudad;
            deposito.Respon = model.respon;
            deposito.Tel = model.tel;
            deposito.LocFor = model.loc_for;
            deposito.RPerson = model.r_person;
            deposito.DescCorta = model.desc_corta;
            deposito.Rfc = model.rfc;
            deposito.Cp = model.cp;




            if (model.id_deposi > 0)
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



        public async Task<List<RegionModel>> GetRegions(int id_empresa)
        {
            return await context.Regions
                .Where(x => x.IdEmpresa == (short)id_empresa)
                .Select(x => new RegionModel {
                    Id = x.IdRegion,
                    Nombre = x.Nombre
            }).ToListAsync();
        }

        public async Task<List<RegionModel>> GetPlantas(int id_empresa)
        {
            return await context.Planta
                .Where(x => x.IdEmp == (short)id_empresa)
                .Select(x => new RegionModel {
                    Id = x.IdPlanta,
                    Nombre = x.Nombre
                }).ToListAsync();
        }

        public async Task<List<RegionModel>> GetZonas(int id_empresa)
        {
            return await context.Zonas
                .Where(x => x.IdEmpresa == id_empresa)
                .Select(x => new RegionModel {
                    Id = x.IdZona,
                    Nombre = x.NombreZona
                }).ToListAsync();

            //var query = context.Zonas.AsQueryable();//prueba

            //Log para depouracion, si devuelve registros el pdo es el where
            //var todas = await query.ToListAsync();

            //int idPrueba = 1;



            //var zonasBD = await context.Zonas
            //    .Where(x =>x.IdEmpresa == id_empresa)
            //    .Select(x => new RegionModel {
            //        Id = (int)x.IdRegion,
            //        Nombre = x.NombreZona
            //    }).ToListAsync();

            //zonasBD.Add(new RegionModel { Id = -1, Nombre = $"Buscando ID Empresa { id_empresa}" });
            //return zonasBD;


        }
    }
}
