"use strict";

var KTVehiculoDetalleList = function () {
    var table = document.getElementById('kt_table_vehiculo_detalle');
    var datatable;

    var initVehiculoTable = function () {
        datatable = $(table).DataTable({
            info: false,
            order: [],
            pageLength: 10,
            lengthChange: false,
            columnDefs: [
                { orderable: false, targets: -1 } // Última columna (acciones)
            ]
        });

        datatable.on('draw', function () {
            handleDeleteRows();
        });
    };

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-vehiculo-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    var handleDeleteRows = () => {
        const deleteButtons = table.querySelectorAll('[data-kt-vehiculo-table-filter="delete_row"]');

        deleteButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();

                const parent = e.target.closest('tr');
                const IdVehicDetalle = button.getAttribute('data-kt-vehiculo-delete-item');
                const nombreVehiculo = parent.querySelector('td').innerText;

                Swal.fire({
                    text: `¿Deseas eliminar el vehículo "${nombreVehiculo}"?`,
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "Cancelar",
                    buttonsStyling: false,
                    customClass: {
                        confirmButton: "btn fw-bold btn-danger",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    },
                    preConfirm: async () => {
                        try {
                            const response = await fetch('/VehiculoDetalle/EliminarVehiculo', {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify({ IdVehicDetalle: parseInt(IdVehicDetalle) })
                            });
                            if (!response.ok) throw new Error('No se pudo eliminar');
                            return await response.json();
                        } catch (error) {
                            Swal.showValidationMessage(`Error: ${error.message}`);
                        }
                    }
                }).then(result => {
                    if (result.isConfirmed) {
                        Swal.fire({
                            text: "Vehículo eliminado correctamente",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary",
                            }
                        }).then(() => {
                            datatable.row($(parent)).remove().draw();
                        });
                    }
                });
            });
        });
    };

    return {
        init: function () {
            if (!table) return;
            initVehiculoTable();
            handleSearchDatatable();
            handleDeleteRows();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTVehiculoDetalleList.init();
});