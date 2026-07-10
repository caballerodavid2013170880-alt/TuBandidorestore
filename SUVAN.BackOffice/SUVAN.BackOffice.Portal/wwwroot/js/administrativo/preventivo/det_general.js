"use strict";
var KTDetalleGeneral = function () {
    var initDatatable = function () {
        var table = document.querySelector('#kt_table_det_prev');
        if (!table) return;

        $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay registros globales generados.",
            },
            "columnDefs": [{ "orderable": false, "targets": 4 }] // Desactiva orden en 'Acciones'
        });
    };

    var handleSearchDatatable = function () {
        var filterSearch = document.querySelector('[data-kt-det_prev-table-filter="search"]');
        if (!filterSearch) return;
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    return {
        init: function () {
            table = document.querySelector('#kt_table_det_prev');
            if (!table) return;
            initDatatable();
            handleSearchDatatable();
        }
    };
}();
KTUtil.onDOMContentLoaded(function () { KTDetalleGeneral.init(); });