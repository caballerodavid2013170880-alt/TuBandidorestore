"use strict";
var datatable;
var KTPreventivoTable = function () {
    var initDatatable = function () {
        var table = document.querySelector('#kt_table_preventivo');
        if (!table) return;

        // Inicializamos Datatables (Quitando el orden de la columna Acciones, que ahora es la 9)
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "columnDefs": [{ "orderable": false, "targets": 9 }]
        });

        // Filtro: Fecha Prevista (Columna 8)
        $('#filtroFecha').on('change', function () {
            let v = this.value;
            if (v) {
                let p = v.split('-');
                datatable.column(8).search(p[2] + '/' + p[1] + '/' + p[0]).draw();
            } else {
                datatable.column(8).search('').draw();
            }
        });

        // Filtro: Búsqueda General
        document.querySelector('[data-kt-preventivo-table-filter="search"]').addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });

        // Funcionalidad: Ordenamiento Dinámico
        $('#btnSort').on('click', function () {
            var columnIdx = parseInt($('#sortColumn').val(), 10);
            var direction = $('#sortDirection').val();
            datatable.order([columnIdx, direction]).draw();
        });
    };
    return { init: function () { initDatatable(); } };
}();