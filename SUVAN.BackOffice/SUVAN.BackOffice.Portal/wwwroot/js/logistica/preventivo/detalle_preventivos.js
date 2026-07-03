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
            $('#fvVin').val('');

            if (id) { loadDatos(id); }
        });

        var initId = $('#selectPreventivoGlobal').val();
        if (initId) loadDatos(initId);
    };

    var loadDatos = function (id) {
        fetch('/MantenimientoPreventivo/GetDatosDetalleView?id=' + id)
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    vehiculosDataGlobal = data.vehiculos || []; // Prevenir nulos
                    var g = data.general;

                    // Validar que 'general' no sea nulo
                   
                        $('#lblNombre').text(g.nombrePreventivo || 'Sin nombre');
                        $('#lblUbicacion').text((g.planta || '') + ' / ' + (g.deposito || ''));
                        $('#lblVehiculo').text((g.marca || '') + ' / ' + (g.modelo || ''));
                        $('#lblFechaPrev').text(g.fechaPrev ? new Date(g.fechaPrev).toLocaleDateString('en-GB') : 'N/A');
                        $('#lblFechaRegistro').text(g.fechaRegistro ? new Date(g.fechaRegistro).toLocaleDateString('en-GB') : 'N/A');
                        $('#lblCostoBase').text('$' + (g.costoUnitario || 0).toFixed(2));
                        $('#lblIvaUnidad').text('$' + (g.ivaUnitario || 0).toFixed(2));
                        $('#lblGranTotal').text('$' + (g.costoTotal || 0).toFixed(2));
                    

                    // Limpiar DataTables previo si existe para evitar el error "Cannot reinitialise"
                    if ($.fn.DataTable.isDataTable('#kt_table_vehiculos_consolidado')) {
                        $('#kt_table_vehiculos_consolidado').DataTable().destroy();
                    }

                    var tbody = $('#tbodyVehiculos');
                    tbody.empty();

                    // Iterar de forma segura sobre los vehículos
                    if (vehiculosDataGlobal.length > 0) {
                        vehiculosDataGlobal.forEach(v => {
                            tbody.append(`<tr>
                            <td>${v.manoObra || 'N/A'}</td>
                            <td><b>${v.placas || 'N/A'}</b></td>
                            <td>${v.vin || 'N/A'}</td>
                            <td><span class="badge badge-light-info">$${(v.costoTotalUnitario || 0).toFixed(2)}</span></td>
                            <td class="text-end">
                                <button type="button" class="btn btn-icon btn-light btn-active-light-primary w-30px h-30px" onclick="verInfoVehiculo(${v.idPrevMo || v.IdPrevMo})" title="Ver Detalle de vehículo">
                                    <i class="ki-outline ki-eye fs-3"></i>
                                </button>
                            </td>
                        </tr>`);
                        });
                    } else {
                        tbody.append(`<tr><td colspan="5" class="text-center text-muted">No hay vehículos registrados para este preventivo.</td></tr>`);
                    }

                    // Reinicializar DataTables sobre la tabla limpia
                    datatableVehiculos = $('#kt_table_vehiculos_consolidado').DataTable({
                        "info": true,
                        "order": [],
                        "columnDefs": [{ "orderable": false, "targets": 4 }]
                    });

                    $('#detalleCompletoContainer').show();
                }
            })
            .catch(error => {
                console.error("Error al obtener el detalle del preventivo:", error);
            });
    };

    var initFilters = function () {
        $('#fvPlacas').on('keyup', function () {
            if (datatableVehiculos) datatableVehiculos.column(1).search(this.value).draw();
        });
        $('#fvVin').on('keyup', function () {
            if (datatableVehiculos) datatableVehiculos.column(2).search(this.value).draw();
        });
    };

    var initSorting = function () {
        var clearSorts = function (exceptId) {
            if (exceptId !== 'sortPlacas') $('#sortPlacas').val('');
            if (exceptId !== 'sortVin') $('#sortVin').val('');
        };

        $('#sortPlacas').on('change', function () {
            var dir = $(this).val();
            if (dir) {
                clearSorts('sortPlacas');
                if (datatableVehiculos) datatableVehiculos.order([1, dir]).draw();
            }
        });

        $('#sortVin').on('change', function () {
            var dir = $(this).val();
            if (dir) {
                clearSorts('sortVin');
                if (datatableVehiculos) datatableVehiculos.order([2, dir]).draw();
            }
        });

        $('#btnLimpiarOrden').on('click', function () {
            clearSorts('');
            if (datatableVehiculos) datatableVehiculos.order([]).draw();
        });
    };

    return { init: function () { initSelect(); initFilters(); initSorting(); } };
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