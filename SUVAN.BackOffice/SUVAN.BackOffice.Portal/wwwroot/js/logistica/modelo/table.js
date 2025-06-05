"use strict";

var KTModeloList = function () {
    // Define variables compartidas
    var table = document.getElementById('kt_table_modelo');
    var datatable;

    // Inicializar la tabla Modelo
    var initModeloTable = function () {
        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            "pageLength": 10,
            "lengthChange": false,
            'columnDefs': [
                { orderable: false, targets: 1 } // Desactivar orden en la columna de acciones
            ]
        });

        datatable.on('draw', function () {
            handleDeleteRows();
        });
    };

    // Manejo de búsqueda en la tabla
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-modelo-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    // Manejo de eliminación de registros
    var handleDeleteRows = () => {
        const deleteButtons = table.querySelectorAll('[data-kt-modelo-table-filter="delete_row"]');

        deleteButtons.forEach(d => {
            d.addEventListener('click', function (e) {
                e.preventDefault();

                const parent = e.target.closest('tr');
                const descrip = parent.querySelectorAll('td')[5].innerText; // Obtener descripción del modelo

                Swal.fire({
                    text: `¿Está seguro que desea eliminar el modelo "${descrip}"?`,
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "No, cancelar",
                    preConfirm: async () => {
                        const modeloId = parseInt(d.getAttribute('data-kt-modelo-delete-item'));
                        const response = await fetch('/Modelo/EliminarModelo', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ id_modelo: modeloId })
                        });

                        return response.json();
                    }
                }).then(function (result) {
                    if (result.value) {
                        Swal.fire({
                            text: `El modelo "${descrip}" ha sido eliminado`,
                            icon: "success",
                            confirmButtonText: "Aceptar"
                        }).then(function () {
                            datatable.row($(parent)).remove().draw();
                        });
                    }
                });
            });
        });
    };

    return {
        init: function () {
            if (!table) {
                return;
            }

            initModeloTable();
            handleSearchDatatable();
            handleDeleteRows();
        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTModeloList.init();
});