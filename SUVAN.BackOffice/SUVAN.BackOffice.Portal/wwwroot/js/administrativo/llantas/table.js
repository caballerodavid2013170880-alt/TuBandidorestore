"use strict";

var KTDatatablesLlantas = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        datatable = $(table).DataTable({
            info: false,
            order: [],
            pageLength: 10,
            lengthChange: false
        });
    };

    var handleSearchDatatable = function () {
        const filterSearch = document.querySelector('[data-kt-llantas-table-filter="search"]');
        filterSearch.addEventListener("keyup", function (e) {
            datatable.search(e.target.value).draw();
        });
    };

    var handleSuccessMessage = function () {
        const mensaje = document.querySelector("#mensajeTempData");

        if (!mensaje || !mensaje.value) {
            return;
        }

        Swal.fire({
            text: mensaje.value,
            icon: "success",
            buttonsStyling: false,
            confirmButtonText: "Aceptar",
            customClass: {
                confirmButton: "btn btn-primary"
            }
        });
    };

    var handleDeleteRows = function () {
        const deleteButtons = table.querySelectorAll('[data-kt-llantas-table-filter="delete_row"]');

        deleteButtons.forEach(function (button) {
            button.addEventListener("click", function (e) {
                e.preventDefault();

                const parent = e.target.closest("tr");
                const codigoLlanta = button.getAttribute("data-kt-llanta-delete-name") || parent.querySelectorAll("td")[0].innerText;
                const idLlanta = button.getAttribute("data-kt-llanta-delete-item");

                Swal.fire({
                    text: `¿Está seguro que desea eliminar la llanta ${codigoLlanta}?`,
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
                    if (!result.value) {
                        return;
                    }

                    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                    const formData = new FormData();
                    formData.append("id", idLlanta);
                    if (token) {
                        formData.append("__RequestVerificationToken", token);
                    }

                    fetch("/llantas/eliminar", {
                        method: "POST",
                        body: formData
                    })
                        .then(function (response) {
                            return response.json();
                        })
                        .then(function (data) {
                            if (data.success) {
                                Swal.fire({
                                    text: data.message || `Llanta ${codigoLlanta} eliminada correctamente.`,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary"
                                    }
                                }).then(function () {
                                    datatable.row($(parent)).remove().draw();
                                });
                                return;
                            }

                            Swal.fire({
                                text: data.message || "No fue posible eliminar la llanta.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            });
                        })
                        .catch(function () {
                            Swal.fire({
                                text: "No fue posible eliminar la llanta.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            });
                        });
                });
            });
        });
    };

    return {
        init: function () {
            table = document.querySelector("#kt_table_llantas");

            if (!table) {
                return;
            }

            initDatatable();
            handleSearchDatatable();
            handleSuccessMessage();
            handleDeleteRows();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDatatablesLlantas.init();
});
