"use strict";
var datatableVehiculos;
var vehiculosDataGlobal = [];

var KTDetalleConsolidado = function () {
    var initSelect = function () {
        $('#selectPreventivoGlobal').on('change', function () {
            var id = $(this).val();
            // Limpiar pantalla y filtros al cambiar de preventivo
            $('#detalleCompletoContainer').hide();
            $('#fvPlacas').val('');
            $('#fvServicio').val('');
            $('[data-kt-vehiculos-filter="search"]').val('');

            if (id) { loadDatos(id); }
        });

        var initId = $('#selectPreventivoGlobal').val();
        if (initId) loadDatos(initId);
    };

    var loadDatos = function (id) {
        fetch('/MantenimientoPreventivo/GetDatosDetalleView?id=' + id).then(res => res.json()).then(data => {
            if (data.success) {
                vehiculosDataGlobal = data.vehiculos;
                var g = data.general;
                $('#lblNombre').text(g.nombrePreventivo);
                $('#lblUbicacion').text(g.planta + ' / ' + g.deposito);
                $('#lblVehiculo').text(g.marca + ' / ' + g.modelo);
                $('#lblFechaPrev').text(new Date(g.fechaPrev).toLocaleDateString('en-GB'));
                $('#lblFechaRegistro').text(new Date(g.fechaRegistro).toLocaleDateString('en-GB'));
                $('#lblCostoBase').text('$' + g.costoUnitario.toFixed(2));
                $('#lblIvaUnidad').text('$' + g.ivaUnitario.toFixed(2));
                $('#lblGranTotal').text('$' + g.costoTotal.toFixed(2));

                // 2. Destruir DataTables previo si existe para evitar el error "Cannot reinitialise"
                if ($.fn.DataTable.isDataTable('#kt_table_vehiculos_consolidado')) {
                    $('#kt_table_vehiculos_consolidado').DataTable().destroy();
                }

                var tbody = $('#tbodyVehiculos');
                tbody.empty();
                data.vehiculos.forEach(v => {
                    tbody.append(`<tr>
                        <td>${v.manoObra}</td>
                        <td><b>${v.placas}</b><br/><span class="text-muted">VIN: ${v.vin}</span></td>
                        <td><span class="badge badge-light-info">$${v.costoTotalUnitario.toFixed(2)}</span></td>
                        <td class="text-end">
                            <button type="button" class="btn btn-icon btn-light btn-active-light-primary w-30px h-30px" onclick="verInfoVehiculo(${v.idPrevMo || v.IdPrevMo})" title="Ver Detalle de vehículo">
                                <i class="ki-outline ki-eye fs-3"></i>
                            </button>
                        </td>
                    </tr>`);
                });

                // Reinicializar DataTables sobre la tabla limpia
                datatableVehiculos = $('#kt_table_vehiculos_consolidado').DataTable({
                    "info": true,
                    "order": [],
                    "columnDefs": [{ "orderable": false, "targets": 3 }] // Ajustado a 3 porque quitamos el IVA
                });

                $('#detalleCompletoContainer').show();
            }
        });
    };

    var initFilters = function () {
        // Asegura que los filtros de la segunda sección funcionen comprobando la variable viva
        $('[data-kt-vehiculos-filter="search"]').on('keyup', function (e) {
            if (datatableVehiculos) datatableVehiculos.search(e.target.value).draw();
        });
        $('#fvPlacas').on('keyup', function () {
            if (datatableVehiculos) datatableVehiculos.column(1).search(this.value).draw();
        });
        $('#fvServicio').on('keyup', function () {
            if (datatableVehiculos) datatableVehiculos.column(0).search(this.value).draw();
        });
    };

    return { init: function () { initSelect(); initFilters(); } };
}();

window.verInfoVehiculo = function (idMo) {
    var v = vehiculosDataGlobal.find(x => x.idPrevMo === idMo || x.IdPrevMo === idMo);
    if (v) {
        var html = `
            <div class="mb-4"><b>Servicio:</b> ${v.manoObra}</div>
            <div class="mb-4"><b>Vehículo:</b> ${v.placas} <br><span class="text-muted">VIN: ${v.vin}</span></div>
            <div class="mb-4"><b>Fecha Prevista:</b> ${new Date(v.fechaPrev).toLocaleDateString('en-GB')}</div>
            <div class="mb-4"><b>Monto IVA:</b> $${(v.iva || 0).toFixed(2)}</div>
            <div class="mb-4"><b>Costo Total Unitario:</b> <span class="text-success fw-bold">$${v.costoTotalUnitario.toFixed(2)}</span></div>
        `;
        document.getElementById('modalInfoVehiculoBody').innerHTML = html;
        new bootstrap.Modal(document.getElementById('modalInfoVehiculo')).show();
    }
};

KTUtil.onDOMContentLoaded(function () { KTDetalleConsolidado.init(); });