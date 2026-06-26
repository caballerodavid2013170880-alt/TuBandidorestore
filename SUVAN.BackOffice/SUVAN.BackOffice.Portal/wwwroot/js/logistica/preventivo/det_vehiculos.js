"use strict";
var KTDetalleVehiculos = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        table = document.querySelector('#kt_table_det_prev_mo');
        if (!table) return;

        datatable = $(table).DataTable({
            "info": true,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No se encontraron vehículos coincidentes.",
                "zeroRecords": "No se encontraron coincidencias en la búsqueda.",
            }
        });
    };

    var handleSearch = function () {
        var filterSearch = document.querySelector('[data-kt-vehiculos-filter="search"]');
        if (!filterSearch) return;
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    return {
        init: function () {
            initDatatable();
            handleSearch();
        }
    };
}();
KTUtil.onDOMContentLoaded(function () { KTDetalleVehiculos.init(); });