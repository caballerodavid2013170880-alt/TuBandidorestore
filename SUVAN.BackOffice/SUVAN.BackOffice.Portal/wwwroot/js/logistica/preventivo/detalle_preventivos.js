"use strict";
var datatableVehiculos;

var KTDetalleConsolidado = function () {
    var initSelect = function () {
        $('#selectPreventivoGlobal').on('change', function () {
            var id = $(this).val();
            if (id) { loadDatos(id); } else { $('#detalleCompletoContainer').hide(); }
        });

        // Si hay un ID seleccionado desde ViewBag, dispáralo al inicio
        var initId = $('#selectPreventivoGlobal').val();
        if (initId) loadDatos(initId);
    };

    var loadDatos = function (id) {
        fetch('/MantenimientoPreventivo/GetDatosDetalleView?id=' + id)
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    $('#detalleCompletoContainer').show();
                    var g = data.general;
                    $('#lblNombre').text(g.nombrePreventivo);
                    $('#lblUbicacion').text(g.planta + ' / ' + g.deposito);
                    $('#lblVehiculo').text(g.marca + ' / ' + g.modelo);
                    $('#lblFechaPrev').text(new Date(g.fechaPrev).toLocaleDateString('en-GB'));
                    $('#lblFechaRegistro').text(new Date(g.fechaRegistro).toLocaleDateString('en-GB'));
                    $('#lblCostoBase').text('$' + g.costoUnitario.toFixed(2));
                    $('#lblIvaUnidad').text('$' + g.ivaUnitario.toFixed(2));
                    $('#lblGranTotal').text('$' + g.costoTotal.toFixed(2));

                    // Llenar tabla
                    var tbody = $('#tbodyVehiculos');
                    tbody.empty();
                    data.vehiculos.forEach(v => {
                        tbody.append(`<tr>
                            <td>${v.manoObra}</td>
                            <td><b>${v.placas}</b><br/><span class="text-muted">${v.vin}</span></td>
                            <td>$${(v.iva || 0).toFixed(2)}</td>
                            <td><span class="badge badge-light-info">$${v.costoTotalUnitario.toFixed(2)}</span></td>
                        </tr>`);
                    });

                    // Reiniciar DataTable
                    if (datatableVehiculos) { datatableVehiculos.destroy(); }
                    datatableVehiculos = $('#kt_table_vehiculos_consolidado').DataTable({ "info": true, "order": [] });
                }
            });
    };

    var initFilters = function () {
        document.querySelector('[data-kt-vehiculos-filter="search"]').addEventListener('keyup', function (e) {
            if (datatableVehiculos) datatableVehiculos.search(e.target.value).draw();
        });
        $('#fvPlacas').on('keyup', function () { if (datatableVehiculos) datatableVehiculos.column(1).search(this.value).draw(); });
        $('#fvServicio').on('keyup', function () { if (datatableVehiculos) datatableVehiculos.column(0).search(this.value).draw(); });
    };

    return { init: function () { initSelect(); initFilters(); } };
}();
KTUtil.onDOMContentLoaded(function () { KTDetalleConsolidado.init(); });