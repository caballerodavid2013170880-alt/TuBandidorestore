using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Administrativo
{
    public interface IDepositoService
    {

        /// <summary>
        /// Agrega o actualiza un deposito en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos del deposito.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        Task<bool> AgregarDeposito(DepositoViewModel model, int idEmpresa);

        /// <summary>
        /// Obtiene el listado de depositos desde la base de datos.
        /// </summary>
        /// <returns>Lista de depositos.</returns>
        Task<List<Deposito>> GetDepositos(int id_empresa);
        /*
        //se agregan los metodos de Regi�n Plantas y zonas para btener esos cat�logos y mostrarlos en el combo box
        Task<List<DepositoViewModel.CatalogItemViewModel>> GetRegions(int id_empresa);
        Task<List<DepositoViewModel.CatalogItemViewModel>> GetPlantas(int id_empresa);
        Task<List<DepositoViewModel.CatalogItemViewModel>> GetZonas(int id_empresa);

        //se agregan los metodos para obtener filtrados por regi�n planta y zona (filtrado en cascada)
        Task<List<DepositoViewModel.CatalogItemViewModel>> GetPlantasByRegion(int id_empresa, int id_region);
        Task<List<DepositoViewModel.CatalogItemViewModel>> GetZonasByPlanta(int id_empresa, int id_planta);
        */

        /// <summary>
        /// Obtiene el ViewModel para el depósito específico.
        /// </summary>
        ///  <param name="id_empresa">Identificador de la empresa.</param>
        ///  <param name="id_region">Identificador de la región.</param>
        ///  <param name="id_planta">Identificador de la planta.</param>
        ///  <param name="id_zona">Identificador de la zona.</param>
        /// <param name="id_deposito">Identificador del depósito.</param>
        /// <returns>ViewModel para el depósito específico.</returns>
        /// 
        Task<DepositoViewModel> GetDepositoViewModel(int idEmpresa, int idDeposito);

        //agregado para usar eliminado logico
        //Task<bool> EliminarDeposito(int idEmpresa, int idDeposito);


    }
}
