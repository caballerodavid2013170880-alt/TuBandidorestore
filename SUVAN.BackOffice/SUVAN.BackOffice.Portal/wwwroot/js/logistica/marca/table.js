"use strict";

var KTMarcaList = function () {
    // Define variables compartidas
    var table = document.getElementById('kt_table_marca');
    var datatable;

    // Inicializar la tabla Marca
    var initMarcaTable = function () {
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
        const filterSearch = document.querySelector('[data-kt-marca-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    // Manejo de eliminación de registros
    var handleDeleteRows = () => {
        const deleteButtons = table.querySelectorAll('[data-kt-marca-table-filter="delete_row"]');

        deleteButtons.forEach(d => {
            d.addEventListener('click', function (e) {
                e.preventDefault();

                const parent = e.target.closest('tr');
                const descrip = parent.querySelectorAll('td')[1].innerText; // Obtener descripción de la marca

                Swal.fire({
                    text: `¿Está seguro que desea eliminar la marca "${descrip}"?`,
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "No, cancelar",
                    preConfirm: async () => {
                        const marcaId = parseInt(d.getAttribute('data-kt-marca-delete-item'));
                        const response = await fetch('/Marca/EliminarMarca', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ id_marca: marcaId })
                        });

                        return response.json();
                    }
                }).then(function (result) {
                    if (result.value) {
                        Swal.fire({
                            text: `La descripcion "${descrip}" ha sido eliminada`,
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

            initMarcaTable();
            handleSearchDatatable();
            handleDeleteRows();
        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTMarcaList.init();
});