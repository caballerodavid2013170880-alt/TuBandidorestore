"use strict";

var KTCorridasList = function () {
  // Define shared variables
  var table = document.getElementById('kt_table_corridas');
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
    const filterSearch = document.querySelector('[data-kt-corridas-table-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
      datatable.search(e.target.value).draw();
    });
  }

  const handleDeleteHorarios = () => {
    // Select all delete buttons
    const deleteButtons = table.querySelectorAll('[data-kt-corridas-horario-table-filter="delete_row"]');

    deleteButtons.forEach(d => {

      d.addEventListener('click', function (e) {

        e.preventDefault();
        // Select parent row
        const parent = e.target.closest('tr');

        // Get user name
        const tdContnent = parent.querySelectorAll('td')[0];
        const horarioId = parseInt(d.getAttribute('data-kt-corridas-horario-delete-item'));

        Swal.fire({
          text: '¿Esta seguro que desea eliminar la el horario seleccionado?',
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

            const response = await fetch('/Corridas/EliminarHorarioCorrida', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ RutaId: horarioId })
            });
            const data = await response.json();
            console.log(data);

            return data;
          }
        }).then(function (result) {
          console.log(result);
          if (result.value.success) {
            Swal.fire({
              text: 'Se elimino el horario correctamente',
              icon: "success",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn fw-bold btn-primary",
              }
            }).then(function () {
              // Remove current row
              //const divsDetail = tdContnent.querySelectorAll('div');

              d.parentElement.parentElement.remove();
              //divsDetail.forEach(d => {
              //  d.remove();
              //});
            });
          } else {
            Swal.fire({
              text: result.value.message,
              icon: "warning",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn fw-bold btn-primary",
              }
            });
          }
        });



      });
    });

  }

  // Delete subscirption
  var handleDeleteRows = () => {
    // Select all delete buttons
    const deleteButtons = table.querySelectorAll('[data-kt-corridas-horario-table-filter="delete_row"]');

    deleteButtons.forEach(d => {
      // Delete button on click
      d.addEventListener('click', function (e) {
        e.preventDefault();

        // Select parent row
        const parent = e.target.closest('tr');

        // Get user name
        const tdContnent = parent.querySelectorAll('td')[0];
        const tdContentDiv = tdContnent.querySelectorAll('div');

        if (tdContentDiv.length == 0) {
          notificacion.info("No se puede eliminar la informaci&oacute;n, ya que no tiene corridas asociadas");
          return;
        }

        const contenidoName = tdContnent.querySelector('span').innerText;
        // SweetAlert2 pop up --- official docs reference: https://sweetalert2.github.io/
        Swal.fire({
          text: `¿Esta seguro que desea eliminar las corridas de la ruta ${contenidoName}?`,
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

            const rutaId = parseInt(d.getAttribute('data-kt-corridas-delete-item'));
            const response = await fetch('/Corridas/EliminarCorrida', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ RutaId: rutaId })
            });
            const data = await response.json();
            console.log(data);


          }
        }).then(function (result) {
          if (result.value) {
            Swal.fire({
              text: `Se eliminaron las corridas de la ruta ${contenidoName}`,
              icon: "success",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn fw-bold btn-primary",
              }
            }).then(function () {
              // Remove current row
              const divsDetail = tdContnent.querySelectorAll('div');
              divsDetail.forEach(d => {
                d.remove();
              });
              /* datatable.row($(parent)).remove().draw();*/
              //location.reload();
            }).then(function () {
              // Detect checked checkboxes
              toggleToolbars();
            });
          } else if (result.dismiss === 'cancel') {
            Swal.fire({
              text: customerName + " was not deleted.",
              icon: "error",
              buttonsStyling: false,
              confirmButtonText: "Ok, got it!",
              customClass: {
                confirmButton: "btn fw-bold btn-primary",
              }
            });
          }
        });


      });

    });

    handleDeleteHorarios();
  }

  return {
    // Public functions  
    init: function () {
      if (!table) {
        return;
      }
      handleDeleteRows();
      initEstacionTable();
      handleSearchDatatable();


    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTCorridasList.init();
});