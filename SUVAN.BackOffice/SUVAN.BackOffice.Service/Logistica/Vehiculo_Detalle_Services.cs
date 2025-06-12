using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class VehiculoDetalleService : IVehiculoDetalleService
    {
        private readonly SuvanDbContext context;

        public VehiculoDetalleService(SuvanDbContext context) => this.context = context;

        //  Obtener todos los vehículos
        public async Task<List<VehiculoDetalle>> GetVehiculos()
        {
            return await context.VehiculoDetalles.ToListAsync();
        }

        //  Obtener un vehículo por ID
        public async Task<VehiculoDetalle> GetVehiculoById(int id)
        {
            return await context.VehiculoDetalles.FindAsync(id) ?? new VehiculoDetalle();
        }

        //  Agregar o actualizar un vehículo
        public async Task<bool> AgregarVehiculo(VehiculoDetalle model)
        {
            // Validamos y normalizamos la placa antes de la consulta
            string placaNormalizada = model.PlacaPe?.Trim().ToLower();

            // 📌 Validación de duplicados (corregida)
            var vehiculoExistente = await context.VehiculoDetalles
                .Where(x => !string.IsNullOrWhiteSpace(x.PlacaPe) &&
                            x.PlacaPe.ToLower() == placaNormalizada &&
                            x.IdVehicDetalle != model.IdVehicDetalle)
                .FirstOrDefaultAsync();
            //  Obtenemos el vehículo existente o creamos uno nuevo
            var vehiculo = await context.VehiculoDetalles.FindAsync(model.IdVehicDetalle) ?? new VehiculoDetalle();


            // Asignamos valores sin sobrescribir datos existentes con null
            vehiculo.IdZona = model.IdZona != default ? model.IdZona : vehiculo.IdZona;
            vehiculo.IdDeposito = model.IdDeposito != default ? model.IdDeposito : vehiculo.IdDeposito;
            vehiculo.IdMarca = model.IdMarca != default ? model.IdMarca : vehiculo.IdMarca;
            vehiculo.IdModelo = model.IdModelo != default ? model.IdModelo : vehiculo.IdModelo;
            vehiculo.Color = !string.IsNullOrEmpty(model.Color) ? model.Color : vehiculo.Color;
            vehiculo.PlacaPe = !string.IsNullOrEmpty(model.PlacaPe) ? model.PlacaPe : vehiculo.PlacaPe;
            vehiculo.Anio = model.Anio ?? vehiculo.Anio;  // Anio es nullable
            vehiculo.Costo = model.Costo ?? vehiculo.Costo;  // Costo es nullable
            vehiculo.VigenciaPermisoAceite = model.VigenciaPermisoAceite ?? vehiculo.VigenciaPermisoAceite;
            vehiculo.VigenciaTarjetaCircula = model.VigenciaTarjetaCircula ?? vehiculo.VigenciaTarjetaCircula;

            // Definimos si es un nuevo registro o una actualización
            context.Entry(vehiculo).State = model.IdVehicDetalle > 0 ? EntityState.Modified : EntityState.Added;
            await context.SaveChangesAsync();

            return true;
        }

        //  Eliminar un vehículo
        public async Task<bool> EliminarVehiculo(int idVehicDetalle)
        {
            var vehiculo = await context.VehiculoDetalles.FindAsync(idVehicDetalle);

            if (vehiculo is null)
                return false;

            context.VehiculoDetalles.Remove(vehiculo);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarVehiculo(VehiculoDetalle model)
        {
            return false;
        }

        public async Task<VehiculoDetalle> WGetVehiculoById(int id)
        {
            return null;
        }

        public Task<VehiculoDetalle?> GetVehiculoViewModel(int id)
        {
            return null;
        }
    }
}