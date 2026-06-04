"use strict";
/**
 * @fileoverview Módulo de tabla de Departamentos para el BackOffice de SUVAN.
 * Gestiona la DataTable, la búsqueda en tiempo real, el mensaje TempData,
 * y la eliminación con confirmación mediante modal Bootstrap y SweetAlert.
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

    /** @type {number} Identificador del departamento a eliminar (leído al abrir el modal). */
    var idDeptoAEliminar = 0;
    /** @type {HTMLElement} Fila de la tabla correspondiente al departamento a eliminar. */
    var rowAEliminar = null;

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

    /**
     * Inicializa el modal Bootstrap de confirmación de eliminación y
     * enlaza los botones de eliminar de cada fila.
     * Al confirmar, envía una petición POST al endpoint EliminarDepto
     * y elimina la fila de la DataTable sin recargar la página.
     * @private
     */
    var handleDeleteModal = function () {
        var modalEl = document.getElementById('kt_modal_eliminar_depto');
        var nombreEl = document.getElementById('modal_depto_nombre');
        var btnConf = document.getElementById('btnConfirmarEliminar');
        if (!modalEl || !btnConf) return;

        var modal = new bootstrap.Modal(modalEl);

        // Enlace de cada botón de eliminación para abrir el modal con datos del depto
        document.querySelectorAll('.btn-eliminar-depto').forEach(function (btn) {
            btn.addEventListener('click', function () {
                idDeptoAEliminar = parseInt(this.getAttribute('data-id'), 10);
                rowAEliminar = this.closest('tr');
                if (nombreEl) {
                    nombreEl.textContent = this.getAttribute('data-nombre') || '';
                }
                modal.show();
            });
        });

        // Confirmación de eliminación: llama al endpoint y actualiza la tabla
        btnConf.addEventListener('click', function () {
            var btn = this;
            btn.setAttribute('data-kt-indicator', 'on');
            btn.disabled = true;

            fetch('/Configuracion/EliminarDepto', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getCsrfToken()
                },
                body: JSON.stringify({ IdDepto: idDeptoAEliminar })
            })
                .then(function (res) { return res.json(); })
                .then(function (data) {
                    btn.removeAttribute('data-kt-indicator');
                    btn.disabled = false;
                    modal.hide();

                    if (data.success) {
                        // Elimina la fila de la DataTable sin recargar la página
                        if (datatable && rowAEliminar) {
                            datatable.row(rowAEliminar).remove().draw();
                        }
                        Swal.fire({
                            text: 'Departamento eliminado correctamente.',
                            icon: 'success',
                            buttonsStyling: false,
                            confirmButtonText: 'Aceptar',
                            customClass: { confirmButton: 'btn btn-primary' }
                        });
                    } else {
                        Swal.fire({
                            text: data.message || 'Ocurrió un error al eliminar el departamento.',
                            icon: 'error',
                            buttonsStyling: false,
                            confirmButtonText: 'Aceptar',
                            customClass: { confirmButton: 'btn btn-primary' }
                        });
                    }
                })
                .catch(function () {
                    btn.removeAttribute('data-kt-indicator');
                    btn.disabled = false;
                    modal.hide();
                    Swal.fire({
                        text: 'Error de conexión. Intente nuevamente.',
                        icon: 'error',
                        buttonsStyling: false,
                        confirmButtonText: 'Aceptar',
                        customClass: { confirmButton: 'btn btn-primary' }
                    });
                });
        });
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
            handleDeleteModal();
        }
    };
}();

// Ejecutar al cargar el DOM
KTUtil.onDOMContentLoaded(function () {
    KTDeptoTable.init();
});
