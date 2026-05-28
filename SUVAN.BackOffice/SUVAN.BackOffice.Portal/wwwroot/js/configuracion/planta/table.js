"use strict";
/**
 * Módulo principal para el manejo de la tabla de Plantas.
 * @namespace KTPlantaTable
 */
var KTPlantaTable = function () {
    /** @type {HTMLElement} Referencia a la tabla DataTable */
    var table;
    /** @type {object} Instancia de DataTable */
    var datatable;
    /**
     * Inicializa la DataTable con configuración de columnas y paginación.
     * @private
     */
    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay plantas registradas",
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
     * Enlaza el campo de búsqueda para filtrar filas de la tabla en tiempo real.
     * Usa el atributo data-kt-planta-table-filter="search" del input.
     * @private
     */
    var handleSearchDatatable = function () {
        var filterSearch = document.querySelector('[data-kt-planta-table-filter="search"]');
        if (!filterSearch) return;
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };
    /**
     * Muestra el mensaje TempData si existe (confirmación de guardado).
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
                customClass: {
                    confirmButton: 'btn btn-primary'
                }
            });
        }
    };
    // API pública
    return {
        /**
         * Punto de entrada: inicializa todos los componentes de la tabla.
         */
        init: function () {
            table = document.querySelector('#kt_table_planta');
            if (!table) return;
            initDatatable();
            handleSearchDatatable();
            handleTempDataMessage();
        }
    };
}();
// Ejecutar al cargar el DOM
KTUtil.onDOMContentLoaded(function () {
    KTPlantaTable.init();
});
