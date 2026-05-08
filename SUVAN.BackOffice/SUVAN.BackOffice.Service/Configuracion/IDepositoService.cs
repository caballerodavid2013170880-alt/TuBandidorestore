using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public interface IDepositoService
    {

        /// <summary>
        /// Agrega o actualiza un deposito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del deposito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarDeposito(DepositoViewModel model);

        /// <summary>
        /// Obtiene el listado de depositos desde la base de datos.
        /// </summary>
        /// <returns>Lista de depositos.</returns>
        Task<List<Deposito>> GetDepositos(int id_empresa);

        //se agregaan los metodos de Región Plantas y zonas para btener esos catálogos y mostrarlos en el combo box
        Task <List<RegionModel>> GetRegions(int id_empresa);
        Task <List<RegionModel>> GetPlantas(int id_empresa);
        Task <List<RegionModel>> GetZonas(int id_empresa);


        /// <summary>
        /// Obtiene el ViewModel para el depósito específico.
        /// </summary>
        ///  <param name="nombre">Nombre del depósito.</param>
        ///  <param name="id_empresa">Identificador de la empresa.</param>
        ///  <param name="id_region">Identificador de la región.</param>
        ///  <param name="id_planta">Identificador de la planta.</param>
        ///  <param name="id_zona">Identificador de la zona.</param>
        /// <param name="id_deposi">Identificador del depósito.</param>
        /// <returns>ViewModel para el depósito específico.</returns>
        /// 
        Task<DepositoViewModel> GetDepositoViewModel(int id_empresa,int id_deposi);
    }
}
