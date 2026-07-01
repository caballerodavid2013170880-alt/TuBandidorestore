"use strict";

var KTConsultaPreventivos = function () {
    var datatable;

    var initDatatable = function () {
        var table = document.querySelector('#kt_table_consulta');
        if (!table) return;

        datatable = $(table).DataTable({
            "info": true,
            "order": [[0, 'asc']], // Orden inicial por nombre
            "columnDefs": [{ "orderable": false, "targets": 6 }] // Quitar orden en Acciones (C 6)
        });

        // Eventos de Filtro Texto y Fechas
        $('#fcNombre').on('keyup', function () {
            datatable.column(0).search(this.value).draw();
        });

        $('#fcFecha').on('change', function () {
            let v = this.value;
            if (v) {
                let p = v.split('-');
                datatable.column(4).search(p[2] + '/' + p[1] + '/' + p[0]).draw(); // Fecha es la columna 4
            } else {
                datatable.column(4).search('').draw();
            }
        });

        // Limpiar Filtros
        $('#btnClearFilters').on('click', function () {
            $('input, select').val(''); // Vacía controles HTML
            datatable.search('').columns().search('').draw(); // Vacía memoria DataTable
            $('#sortColumn').val('0');
            $('#sortDirection').val('asc');
        });

        // Aplicar Ordenamiento Personalizado
        $('#btnSort').on('click', function () {
            var columnIdx = parseInt($('#sortColumn').val(), 10);
            var direction = $('#sortDirection').val();
            datatable.order([columnIdx, direction]).draw();
        });
    };

    return { init: function () { initDatatable(); } };
}();

KTUtil.onDOMContentLoaded(function () { KTConsultaPreventivos.init(); });