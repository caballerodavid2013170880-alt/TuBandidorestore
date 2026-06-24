"use strict";

var KTDetalleMOTable = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay vehículos coincidentes generados",
                "zeroRecords": "No se encontraron resultados",
                "lengthMenu": "Mostrar _MENU_ registros",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "Siguiente",
                    "previous": "Anterior"
                }
            }
        });
    };

    var handleSearchDatatable = function () {
        var filterSearch = document.querySelector('[data-kt-mo-table-filter="search"]');
        if (!filterSearch) return;
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    return {
        init: function () {
            table = document.querySelector('#kt_table_detalle_mo');
            if (!table) return;
            initDatatable();
            handleSearchDatatable();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalleMOTable.init();
});