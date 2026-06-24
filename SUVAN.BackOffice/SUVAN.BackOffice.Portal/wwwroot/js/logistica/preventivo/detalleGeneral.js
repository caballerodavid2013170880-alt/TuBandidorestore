"use strict";

var KTDetalleGeneralTable = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay registros masivos generados",
                "lengthMenu": "Mostrar _MENU_ registros",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "Siguiente",
                    "previous": "Anterior"
                }
            },
            "columnDefs": [
                { "orderable": false, "targets": 4 }
            ]
        });
    };

    return {
        init: function () {
            table = document.querySelector('#kt_table_detalle_general');
            if (!table) return;
            initDatatable();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalleGeneralTable.init();
});