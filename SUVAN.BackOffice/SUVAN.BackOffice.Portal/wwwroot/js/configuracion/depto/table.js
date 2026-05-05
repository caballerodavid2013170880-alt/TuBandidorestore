"use strict";

var KTDepartamentoList = function () {
    // Define shared variables
    var table = document.getElementById('kt_table_departamento');
    var datatable;

    // Private functions
    var initDepartamentoTable = function () {
        if (!table) {
            return;
        }

        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            'columnDefs': [
                { orderable: false, targets: 4 }, // Disable ordering on column 4 (actions)
            ]
        });

        datatable.on('draw', function () {
            // Re-init any custom functions here if needed
        });
    }

    // Search Datatable
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-departamento-table-filter="search"]');
        if (filterSearch) {
            filterSearch.addEventListener('keyup', function (e) {
                datatable.search(e.target.value).draw();
            });
        }
    }

    return {
        // Public functions
        init: function () {
            initDepartamentoTable();
            handleSearchDatatable();
        }
    }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTDepartamentoList.init();
});
