"use strict";

var KTPoliticaList = function () {
  // Define shared variables
  var table = document.getElementById('kt_table_politica');
  var datatable;
  var toolbarBase;
  var toolbarSelected;
  var selectedCount;

  // Private functions
  var initEstacionTable = function () {
    // Set date data order
    const tableRows = table.querySelectorAll('tbody tr');

    tableRows.forEach(row => {
      const dateRow = row.querySelectorAll('td');

    });

    // Init datatable --- more info on datatables: https://datatables.net/manual/
    datatable = $(table).DataTable({
      "info": false,
      'order': [],
      "pageLength": 10,
      "lengthChange": false,
      'columnDefs': [
        { orderable: false, targets: 4 }, // Disable ordering on column 6 (actions)                
      ]
    });

    // Re-init functions on every table re-draw -- more info: https://datatables.net/reference/event/draw
    datatable.on('draw', function () {


    });
  }

  // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
  var handleSearchDatatable = () => {
    const filterSearch = document.querySelector('[data-kt-politica-table-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
      datatable.search(e.target.value).draw();
    });
  }





  return {
    // Public functions  
    init: function () {
      if (!table) {
        return;
      }

      initEstacionTable();
      handleSearchDatatable();

    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTPoliticaList.init();
});