using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SUVAN.BackOffice.Service.Logistica
{
    public class VehiculoDetalleService : IVehiculoDetalleService
    {
        private readonly SuvanDbContext context;

        public VehiculoDetalleService(SuvanDbContext context)
        {
            this.context = context;
        }

        // 📌 Obtener todos los vehículos
        public async Task<List<VehiculoDetalle>> GetVehiculos()
            => await context.VehiculosDetalle.ToListAsync();

        // 📌 Obtener un vehículo por ID
        public async Task<VehiculoDetalle> GetVehiculoViewModel(int id)
        {
            var vehiculo = await context.VehiculosDetalle.FirstOrDefaultAsync(x => x.IdVehicDetalle == id);
            return vehiculo ?? new VehiculoDetalle();
        }

        // 📌 Agregar o actualizar un vehículo
        public async Task<bool> AgregarVehiculo(VehiculoDetalle model)
        {
            VehiculoDetalle vehiculo = model.IdVehicDetalle > 0
                ? await context.VehiculosDetalle.FirstOrDefaultAsync(x => x.IdVehicDetalle == model.IdVehicDetalle)
                ?? throw new Exception("No se encontró el vehículo")
                : new VehiculoDetalle();

            // Validación de duplicados por PlacaPe
            var placaPeLower = model.PlacaPe?.ToLower();

            var vehiculoExistente = await context.VehiculosDetalle
                .Where(x => x.PlacaPe != null && x.PlacaPe.ToLower() == placaPeLower && x.IdVehicDetalle != model.IdVehicDetalle)
                .FirstOrDefaultAsync();

            if (vehiculoExistente is not null)
                throw new Exception("Ya existe un vehículo con la misma placa");

            // Actualizar propiedades del vehículo
            vehiculo.IdZona = model.IdZona;
            vehiculo.IdDeposito = model.IdDeposito;
            vehiculo.IdMarca = model.IdMarca;
            vehiculo.IdModelo = model.IdModelo;
            vehiculo.Color = model.Color ?? vehiculo.Color;
            vehiculo.PlacaPe = model.PlacaPe ?? vehiculo.PlacaPe;
            vehiculo.Anio = model.Anio ?? vehiculo.Anio;
            vehiculo.Costo = model.Costo ?? vehiculo.Costo;
            vehiculo.VigenciaPermisoAceite = model.VigenciaPermisoAceite ?? vehiculo.VigenciaPermisoAceite;
            vehiculo.VigenciaTarjetaCircula = model.VigenciaTarjetaCircula ?? vehiculo.VigenciaTarjetaCircula;

            if (model.IdVehicDetalle > 0)
                context.VehiculosDetalle.Update(vehiculo);
            else
                context.VehiculosDetalle.Add(vehiculo);

            await context.SaveChangesAsync();
            return true;
        }// 📌 Eliminar un vehículo
        public async Task<bool> EliminarVehiculo(int idVehicDetalle)
        {
            var vehiculo = await context.VehiculosDetalle.FirstOrDefaultAsync(x => x.IdVehicDetalle == idVehicDetalle);
            if (vehiculo is null)
                throw new Exception("No se encontró el vehículo");

            context.VehiculosDetalle.Remove(vehiculo);
            await context.SaveChangesAsync();

            return true;
        }
    }
}