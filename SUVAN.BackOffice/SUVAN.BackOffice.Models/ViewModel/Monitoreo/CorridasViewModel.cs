using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.ViewModel.Configuracion
{
    public class RastreoViewModel
    {
        public List<CorridasViewModel> Corridas { get; set; } = new List<CorridasViewModel>();
        public List<AgregarRutaViewModel> Rutas { get; set; } = new List<AgregarRutaViewModel>();
    }
    public class CorridasViewModel
    {
        public int CorridaAsignacionId { get; set; }
        public string Placa { get; set; } = null!;
        public string Conductor { get; set; } = null!;
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }

    public class CorridasAsignadasJSONModel
    {
        public int IdcorridaAsignacion {get;set;}
        public int IdVehiculo {get;set;}
        public string? Placa { get; set; }
        public int IdConductor {get;set;}
        public string? NombreConductor { get; set; }
        /*public decimal Latitude {get;set;}
        public decimal Longitude {get;set;}*/
        public int IdRuta { get; set; }
    }

    public class RutasJSONModel
    {
        public int Idruta { get; set; }
        public string? Nombre { get; set; }
        public string? GeoRuta { get; set; }
        public required List<RutaParadaJSONModel> Paradas { get; set; }
    }

    public class RutaParadaJSONModel
    {
        public int IdParada { get; set; }
        public string? Nombre { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class RastreoJSONModel
    {
        public List<CorridasAsignadasJSONModel>? Corridas { get; set; }
        public List<RutasJSONModel>? Rutas { get; set; }
    }
}
