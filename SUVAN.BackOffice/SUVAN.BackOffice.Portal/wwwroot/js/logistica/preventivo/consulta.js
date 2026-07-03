"use strict";

var KTConsultaPreventivos = function () {
    var datatable;

    var initDatatable = function () {
        var table = document.querySelector('#kt_table_consulta');
        if (!table) return;

        datatable = $(table).DataTable({
            "info": true,
            "order": [[0, 'asc']], // Orden inicial por nombre
            "columnDefs": [{ "orderable": false, "targets": 6 }] // (Columnas 6)
        });

        // Eventos de Filtro Texto
        $('#fcNombre').on('keyup', function () {
            datatable.column(0).search(this.value).draw();
        });

        // Lógica de Filtro Combinado para Mes y Año con Regex en C 4
        var aplicarFiltroFecha = function () {
            var mes = $('#filtroMes').val();
            var anio = $('#filtroAnio').val();

            // Si no hay seleccionado nada, limpiar filtro de la columna
            if (!mes && !anio) {
                datatable.column(4).search('').draw();
                return;
            }

            // Construye Regex. Formato de celda visto por el usuario: dd/MM/yyyy
            var regex = '^.{2}/' + (mes ? mes : '.{2}') + '/' + (anio ? anio : '.{4}') + '$';

            // Buscar usando regex (true) y sin smart search (false)
            datatable.column(4).search(regex, true, false).draw();
        };

        $('#filtroMes, #filtroAnio').on('change', aplicarFiltroFecha);

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