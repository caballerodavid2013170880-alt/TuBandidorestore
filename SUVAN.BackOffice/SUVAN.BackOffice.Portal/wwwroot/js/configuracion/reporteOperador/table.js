"use strict";

var KTReporteOperadorList = function () {
  // Define shared variables
  var table = document.getElementById('kt_table_reporte');
  var datatable;

  // Private functions
  var initReporOperadorTable = function () {
    // Set date data order


    // Init datatable --- more info on datatables: https://datatables.net/manual/
    datatable = $(table).DataTable({
      "info": false,
      'order': [],
      "pageLength": 20,
      "lengthChange": false,

    });


  }

  // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
  var handleSearchDatatable = () => {
    const filterSearch = document.querySelector('[data-kt-reporte-table-filter="search"]');
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

      initReporOperadorTable();
      handleSearchDatatable();

    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTReporteOperadorList.init();
});