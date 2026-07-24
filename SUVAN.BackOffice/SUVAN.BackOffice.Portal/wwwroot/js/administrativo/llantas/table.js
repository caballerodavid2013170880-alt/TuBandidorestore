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

    return {
        init: function () {
            table = document.querySelector("#kt_table_llantas");

            if (!table) {
                return;
            }

            initDatatable();
            handleSearchDatatable();
            handleSuccessMessage();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDatatablesLlantas.init();
});
