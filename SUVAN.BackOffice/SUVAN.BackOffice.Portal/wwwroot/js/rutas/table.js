"use strict";

var KTRutaList = function () {
  // Define shared variables
  var table = document.getElementById('kt_table_ruta');
  var datatable;
  var toolbarBase;
  var toolbarSelected;
  var selectedCount;

  // Private functions
  var initRutaTable = function () {
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
        { orderable: false, targets: 1 }, // Disable ordering on column 6 (actions)                
      ]
    });

    // Re-init functions on every table re-draw -- more info: https://datatables.net/reference/event/draw
    datatable.on('draw', function () {

      handleDeleteRows();
    });
  }

  // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
  var handleSearchDatatable = () => {
    const filterSearch = document.querySelector('[data-kt-ruta-table-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
      datatable.search(e.target.value).draw();
    });
  }



  // Delete subscirption
  var handleDeleteRows = () => {
    // Select all delete buttons
    const deleteButtons = table.querySelectorAll('[data-kt-ruta-table-filter="delete_row"]');

    deleteButtons.forEach(d => {
      // Delete button on click
      d.addEventListener('click', function (e) {
        e.preventDefault();

        // Select parent row
        const parent = e.target.closest('tr');

        // Get user name
        const contenidoName = parent.querySelectorAll('td')[0].innerText;

        // SweetAlert2 pop up --- official docs reference: https://sweetalert2.github.io/
        Swal.fire({
          text: `¿Esta seguro que desea eliminar la ruta ${contenidoName}?`,
          icon: "warning",
          showCancelButton: true,
          buttonsStyling: false,
          confirmButtonText: "Si, eliminar",
          cancelButtonText: "No, cancelar",
          customClass: {
            confirmButton: "btn fw-bold btn-danger",
            cancelButton: "btn fw-bold btn-active-light-primary"
          },
          preConfirm: async () => {

            const rutaId = parseInt(d.getAttribute('data-kt-ruta-delete-item'));
            const response = await fetch('/Rutas/EliminarRuta', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ RutaId: rutaId })
            });
            const data = await response.json();
            console.log(data);
            return data;

          }
        }).then(function (result) {
          if (result.value.success) {
            Swal.fire({
              text: `Usted Elimino la ruta ${contenidoName}`,
              icon: "success",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn fw-bold btn-primary",
              }
            }).then(function () {
              // Remove current row
              datatable.row($(parent)).remove().draw();
              //location.reload();
            });
          } else {
            Swal.fire({
              text: result.value.message,
              icon: "error",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn fw-bold btn-primary",
              }
            });
          }
        });
      })
    });
  }

  return {
    // Public functions  
    init: function () {
      if (!table) {
        return;
      }

      initRutaTable();
      handleSearchDatatable();
      handleDeleteRows();

    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTRutaList.init();
});