"use strict";

var KTPreventivoTable = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "pageLength": 10,
            "language": {
                "emptyTable": "No hay mantenimientos preventivos registrados",
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
                { "orderable": false, "targets": 3 }
            ]

        });
    };

    var handleSearchDatatable = function () {
        var filterSearch = document.querySelector('[data-kt-preventivo-table-filter="search"]');
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



    return {
        init: function () {
            table = document.querySelector('#kt_table_preventivo');
            if (!table) return;
            initDatatable();
            handleSearchDatatable();
            handleTempDataMessage();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTPreventivoTable.init();
});