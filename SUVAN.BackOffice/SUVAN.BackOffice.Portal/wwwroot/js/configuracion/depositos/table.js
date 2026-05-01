"use strict";

var KTDepositoTable = function () {
    var table;
    var datatable;

    var initDatatable = function () {
        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            'pageLength': 10,
            'columnDefs': [
                { orderable: false, targets: -1 }, // Disable ordering on 'Actions' column
            ]
        });

        datatable.on('draw', function () {
            // handle events after draw
        });
    }

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-deposito-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    return {
        init: function () {
            table = document.querySelector('#kt_table_deposito');

            if (!table) {
                return;
            }

            initDatatable();
            handleSearchDatatable();
        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    KTDepositoTable.init();
});
