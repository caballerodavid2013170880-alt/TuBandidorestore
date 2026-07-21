"use strict";

var KTZonaTable = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay zonas registradas",
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
                { "orderable": false, "targets": 6 }
            ]
        });

        datatable.on('draw', function () {
            handleDeleteRows();
        });
    };

    var handleSearchDatatable = function () {
        var filterSearch = document.querySelector('[data-kt-zona-table-filter="search"]');
        if (!filterSearch) return;
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };

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

    var handleDeleteRows = () => {
        const deleteButtons = table.querySelectorAll('[data-kt-zona-table-filter="delete_row"]');
        deleteButtons.forEach(d => {
            d.addEventListener('click', function (e) {
                e.preventDefault();
                const parent = e.target.closest('tr');
                const zonaName = parent.querySelectorAll('td')[2].innerText;

                Swal.fire({
                    text: `¿Esta seguro que desea eliminar la zona ${zonaName}?`,
                    icon: "warning",
                    showCancelButton: true,
                    buttonsStyling: false,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "No, cancelar",
                    customClass: {
                        confirmButton: "btn fw-bold btn-danger",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    }
                }).then(function (result) {
                    if (result.value) {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                        const zonaId = parseInt(d.getAttribute('data-kt-zona-delete-item'));

                        // fetch nativo para enviar un FormData al endpoint EliminarZona
                        var formData = new FormData();
                        formData.append("IdZona", zonaId);
                        if (token) formData.append("__RequestVerificationToken", token);

                        fetch('/Administrativo/EliminarZona', {
                            method: 'POST',
                            body: formData
                        })
                            .then(response => response.json())
                            .then(data => {
                                if (data.success) {
                                    Swal.fire({
                                        text: data.message || `Usted eliminó la zona ${zonaName}`,
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Aceptar",
                                        customClass: { confirmButton: "btn fw-bold btn-primary" }
                                    }).then(function () {
                                        datatable.row($(parent)).remove().draw();
                                    });
                                } else {
                                    Swal.fire({
                                        text: data.message || "Ocurrió un error",
                                        icon: "error",
                                        confirmButtonText: "Aceptar",
                                        customClass: { confirmButton: "btn fw-bold btn-primary" }
                                    });
                                }
                            });
                    }
                });
            })
        });
    }

    return {
        init: function () {
            table = document.querySelector('#kt_table_zona');
            if (!table) return;
            initDatatable();
            handleSearchDatatable();
            handleTempDataMessage();
            handleDeleteRows();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTZonaTable.init();
});