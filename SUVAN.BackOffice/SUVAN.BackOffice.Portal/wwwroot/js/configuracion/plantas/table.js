"use strict";

var KTPlantaList = function () {
    var datatable;
    var table;

    var initPlantaTable = function () {
        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            'pageLength': 10,
            'columnDefs': [
                { orderable: false, targets: 4 },
            ]
        });
    }

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-planta-table-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    return {
        init: function () {
            table = document.querySelector('#kt_table_planta');

            if (!table) {
                return;
            }

            initPlantaTable();
            handleSearchDatatable();
        }
    }
}();

    KTUtil.onDOMContentLoaded(function () {
    KTPlantaList.init();
    }
);