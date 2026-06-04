"use strict";
/**
 * Gestiona la DataTable, la búsqueda en tiempo real, el mensaje TempData mediante modal Bootstrap y SweetAlert.
 * Patrón de eliminación: modal Bootstrap para confirmación visual + fetch POST.
 * @module KTDeptoTable
 */

/**
 * @namespace KTDeptoTable
 * @description Módulo principal para el manejo de la tabla de Departamentos.
 */
    var KTDeptoTable = function () {
        /** @type {HTMLElement} Referencia al elemento table del DOM. */
        var table;
        /** @type {object} Instancia de DataTable inicializada. */
        var datatable;

    /**
     * Inicializa la DataTable con configuración de columnas, idioma en español
     * y desactivación de ordenamiento en la columna de Acciones.
     * Columnas: Depósito (0), Departamento (1), Responsable (2), Estatus (3), Acciones (4).
     * @private
     */
    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay departamentos registrados",
                "zeroRecords": "No se encontraron resultados",
                "lengthMenu": "Mostrar _MENU_ registros",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "Siguiente",
                    "previous": "Anterior"
                }
            },
            "columnDefs": [
                { "orderable": false, "targets": 4 }  // Columna Acciones no ordenable
            ]
        });
    };

    /**
     * Enlaza el campo de búsqueda para filtrar las filas de la tabla en tiempo real.
     * Usa el atributo <c>data-kt-depto-table-filter="search"</c> del input.
     * @private
     */
    var handleSearchDatatable = function () {
        var filterSearch = document.querySelector('[data-kt-depto-table-filter="search"]');
        if (!filterSearch) return;
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    /**
     * Muestra el mensaje TempData mediante SweetAlert si existe.
     * Se invoca al cargar la página tras un redireccionamiento post-guardado.
     * @private
     */
    var handleTempDataMessage = function () {
        var mensaje = document.getElementById('mensajeTempData');
        if (mensaje && mensaje.value) {
            Swal.fire({
                text: mensaje.value,
                icon: 'success',
                buttonsStyling: false,
                confirmButtonText: 'Aceptar',
                customClass: { confirmButton: 'btn btn-primary' }
            });
        }
    };

    /**
     * Obtiene el token CSRF de la página para usarlo en peticiones fetch POST.
     * @returns {string} Valor del token antifalsificación.
     * @private
     */
    var getCsrfToken = function () {
        var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    };
    
    // ── API pública del módulo ────────────────────────────────────────
    return {
        /**
         * Punto de entrada: inicializa la DataTable, el buscador,
         * el mensaje TempData y el modal de eliminación.
         */
        init: function () {
            table = document.querySelector('#kt_table_depto');
            if (!table) return;
            initDatatable();
            handleSearchDatatable();
            handleTempDataMessage();
        }
    };
}();

// Ejecutar al cargar el DOM
KTUtil.onDOMContentLoaded(function () {
    KTDeptoTable.init();
});
