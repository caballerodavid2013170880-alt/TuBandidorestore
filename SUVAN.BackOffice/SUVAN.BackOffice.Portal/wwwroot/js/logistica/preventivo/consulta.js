"use strict";

var KTConsultaPreventivos = function () {
    var datatable;

    var initDatatable = function () {
        var table = document.querySelector('#kt_table_consulta');
        if (!table) return;

        datatable = $(table).DataTable({
            "info": true,
            "order": [[0, 'asc']], // Orden inicial por nombre
            "columnDefs": [{ "orderable": false, "targets": 9 }] // Quitar orden en Acciones
        });

        // Eventos de Filtro Texto/Fechas
        $('#fcNombre').on('keyup', function () { datatable.column(0).search(this.value).draw(); });
        $('#fcFecha').on('change', function () {
            let v = this.value;
            if (v) { let p = v.split('-'); datatable.column(7).search(p[2] + '/' + p[1] + '/' + p[0]).draw(); }
            else { datatable.column(7).search('').draw(); }
        });

        // Eventos de Filtro Selects Exactos
        var filterExact = (elementId, colIndex) => {
            $(elementId).on('change', function () {
                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                // Buscamos coincidencia exacta usando expresión regular o vacío
                datatable.column(colIndex).search(val ? '^' + val + '$' : '', true, false).draw();
            });
        };

        filterExact('#fcRegion', 1);
        filterExact('#fcPlanta', 2);
        filterExact('#fcZona', 3);

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