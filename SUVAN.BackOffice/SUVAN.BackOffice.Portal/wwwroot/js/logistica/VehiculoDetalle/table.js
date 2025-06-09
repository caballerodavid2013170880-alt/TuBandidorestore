"use strict";

var KTVehiculoList = function () {
    var table = document.getElementById('kt_table_vehiculo_detalle');
    var datatable;

    var initVehiculoTable = function () {
        const tableRows = table.querySelectorAll('tbody tr');

        tableRows.forEach(row => {
            const dataRow = row.querySelectorAll('td');
        });

        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            "pageLength": 10,
            "lengthChange": false,
            'columnDefs': [
                { orderable: false, targets: 4 } // Deshabilitar orden en columna de acciones
            ]
        });

        datatable.on('draw', function () {
            handleDeleteRows();
        });
    }

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-vehiculo-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    var handleDeleteRows = () => {
        const deleteButtons = table.querySelectorAll('[data-kt-vehiculo-table-filter="delete_row"]');

        deleteButtons.forEach(d => {
            d.addEventListener('click', function (e) {
                e.preventDefault();
                const parent = e.target.closest('tr');
                const vehiculoName = parent.querySelectorAll('td')[3].innerText; // Placa

                Swal.fire({
                    text: `¿Está seguro que desea eliminar el vehículo con placa ${vehiculoName}?`,
                    icon: "warning",
                    showCancelButton: true,
                    buttonsStyling: false,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "No, cancelar",
                    customClass: {
                        confirmButton: "btn fw-bold btn-danger",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    },
                    preConfirm: async () => {
                        const vehiculoId = parseInt(d.getAttribute('data-kt-vehiculo-delete-item'));
                        const response = await fetch('/VehiculoDetalle/EliminarVehiculo', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ vehiculoId: vehiculoId })
                        });
                        const data = await response.json();
                        console.log(data);
                    }
                }).then(function (result) {
                    if (result.value) {
                        Swal.fire({
                            text: `Usted eliminó el vehículo con placa ${vehiculoName}`,
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary",
                            }
                        }).then(function () {
                            datatable.row($(parent)).remove().draw();
                        });
                    }
                });
            })
        });
    }

    return {
        init: function () {
            if (!table) {
                return;
            }
            initVehiculoTable();
            handleSearchDatatable();
            handleDeleteRows();
        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    KTVehiculoList.init();
});