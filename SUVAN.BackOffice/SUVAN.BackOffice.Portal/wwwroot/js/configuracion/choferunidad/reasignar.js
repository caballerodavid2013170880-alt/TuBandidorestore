"use strict";

var KTReasignarList = function () {
  // Define shared variables
  var table = document.getElementById('kt_reporte_table');
  let horarioChangeList = [];
  var datatable;
  var toolbarBase;
  var toolbarSelected;
  var selectedCount;
  const listadoContainer = document.getElementById('listado-container');
  const btnSeleccionReasignacion = document.getElementById('btn-seleccion-reasignacion');
  const btnRemoverAsingacion = document.getElementById('btn-seleccion-remover');
  const btnReasignar = document.getElementById('btn-guardar-reasignacion');
  const modalAsignar = document.getElementById('kt_modal_reasignar');
  let showModalAsignar = new bootstrap.Modal(modalAsignar, {
    keyboard: false,
    backdrop: 'static'
  });
  const target = document.querySelector("#modal_body_reasignacion");
  const blockUI = new KTBlockUI(target);
  // Private functions
  var initTable = function () {
    // Set date data order
    const tableRows = table.querySelectorAll('tbody tr');

    tableRows.forEach(row => {
      const dateRow = row.querySelectorAll('td');

    });

    // Init datatable --- more info on datatables: https://datatables.net/manual/
    datatable = $(table).DataTable({
      "info": false,
      'order': [],
      "pageLength": 50,
      "lengthChange": false,
      'columnDefs': [
        { orderable: false, targets: 0 }, // Disable ordering on column 6 (actions)                
      ]
    });

    // Re-init functions on every table re-draw -- more info: https://datatables.net/reference/event/draw
    datatable.on('draw', function () {
      initToggleToolbar();
      handleDeleteRows();
      toggleToolbars();
    });
  }

  // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
  var handleSearchDatatable = () => {
    const filterSearch = document.querySelector('[data-kt-reporte-table-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
      datatable.search(e.target.value).draw();
    });
  }

  // Filter Datatable
  var handleFilterDatatable = () => {
    // Select filter options
    const filterForm = document.querySelector('[data-kt-reporte-table-filter="form"]');
    const filterButton = filterForm.querySelector('[data-kt-reporte-table-filter="filter"]');
    const selectOptions = filterForm.querySelectorAll('select');

    // Filter datatable on submit
    filterButton.addEventListener('click', function () {
      var filterString = '';

      // Get filter values
      selectOptions.forEach((item, index) => {
        if (item.value && item.value !== '') {
          if (index !== 0) {
            filterString += ' ';
          }

          // Build filter value options
          filterString += item.value;
        }
      });

      // Filter datatable --- official docs reference: https://datatables.net/reference/api/search()
      datatable.search(filterString).draw();
    });
  }

  // Reset Filter
  var handleResetForm = () => {
    // Select reset button
    const resetButton = document.querySelector('[data-kt-reporte-table-filter="reset"]');

    // Reset datatable
    resetButton.addEventListener('click', function () {
      // Select filter options
      const filterForm = document.querySelector('[data-kt-reporte-table-filter="form"]');
      const selectOptions = filterForm.querySelectorAll('select');

      // Reset select2 values -- more info: https://select2.org/programmatic-control/add-select-clear-items
      selectOptions.forEach(select => {
        $(select).val('').trigger('change');
      });

      // Reset datatable --- official docs reference: https://datatables.net/reference/api/search()
      datatable.search('').draw();
    });
  }


  // Delete subscirption
  var handleDeleteRows = () => {
    // Select all delete buttons
    const deleteButtons = table.querySelectorAll('[data-kt-reporte-table-filter="delete_row"]');

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
          text: `¿Esta seguro que desea eliminar el conductor ${contenidoName}?`,
          icon: "warning",
          showCancelButton: true,
          buttonsStyling: false,
          confirmButtonText: "Si, eliminar!",
          cancelButtonText: "No, cancelar",
          customClass: {
            confirmButton: "btn fw-bold btn-danger",
            cancelButton: "btn fw-bold btn-active-light-primary"
          },
          preConfirm: async () => {

            const contenidoId = parseInt(d.getAttribute('data-kt-empresa-delete-item'));
            const response = await fetch('/Contenido/EliminarGeneral', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ ContenidoId: contenidoId })
            });
            const data = await response.json();
            console.log(data);


          }
        }).then(function (result) {
          if (result.value) {
            Swal.fire({
              text: `Usted Elimino el conductor ${contenidoName}`,
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
      })
    });
  }

  // Init toggle toolbar
  var initToggleToolbar = () => {
    // Toggle selected action toolbar
    // Select all checkboxes
    const checkboxes = table.querySelectorAll('[type="checkbox"]');

    // Select elements
    toolbarBase = document.querySelector('[data-kt-reporte-table-toolbar="base"]');
    toolbarSelected = document.querySelector('[data-kt-reporte-table-toolbar="selected"]');
    selectedCount = document.querySelector('[data-kt-reporte-table-select="selected_count"]');
    const deleteSelected = document.querySelector('[data-kt-reporte-table-select="delete_selected"]');

    // Toggle delete selected toolbar
    checkboxes.forEach(c => {
      // Checkbox on click event
      c.addEventListener('click', function () {
        setTimeout(function () {
          toggleToolbars();
        }, 50);
      });
    });

    // Deleted selected rows
    deleteSelected.addEventListener('click', function () {
      // SweetAlert2 pop up --- official docs reference: https://sweetalert2.github.io/
      Swal.fire({
        text: "Are you sure you want to delete selected customers?",
        icon: "warning",
        showCancelButton: true,
        buttonsStyling: false,
        confirmButtonText: "Yes, delete!",
        cancelButtonText: "No, cancel",
        customClass: {
          confirmButton: "btn fw-bold btn-danger",
          cancelButton: "btn fw-bold btn-active-light-primary"
        }
      }).then(function (result) {
        if (result.value) {
          Swal.fire({
            text: "You have deleted all selected customers!.",
            icon: "success",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
              confirmButton: "btn fw-bold btn-primary",
            }
          }).then(function () {
            // Remove all selected customers
            checkboxes.forEach(c => {
              if (c.checked) {
                datatable.row($(c.closest('tbody tr'))).remove().draw();
              }
            });

            // Remove header checked box
            const headerCheckbox = table.querySelectorAll('[type="checkbox"]')[0];
            headerCheckbox.checked = false;
          }).then(function () {
            toggleToolbars(); // Detect checked checkboxes
            initToggleToolbar(); // Re-init toolbar to recalculate checkboxes
          });
        } else if (result.dismiss === 'cancel') {
          Swal.fire({
            text: "Selected customers was not deleted.",
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
  }

  // Toggle toolbars
  const toggleToolbars = () => {
    // Select refreshed checkbox DOM elements 
    const allCheckboxes = table.querySelectorAll('tbody [type="checkbox"]');

    // Detect checkboxes state & count
    let checkedState = false;
    let count = 0;

    // Count checked boxes
    allCheckboxes.forEach(c => {
      if (c.checked) {
        checkedState = true;
        count++;
      }
    });

    // Toggle toolbars
    if (checkedState) {
      selectedCount.innerHTML = count;
      toolbarBase.classList.add('d-none');
      toolbarSelected.classList.remove('d-none');
    } else {
      toolbarBase.classList.remove('d-none');
      toolbarSelected.classList.add('d-none');
    }
  }

  const rowCheckSelection = () => {

    $('.rowCheckbox').change(function () {
      const tr = $(this).closest('tr');
      if ($(this).prop('checked')) {


        let objeto = {};

        objeto.corridaAsingnacionId = tr.find('td').eq(0).find('input').val();
        objeto.rutaId = tr.find('td').eq(1).find('input').val();
        objeto.ruta = tr.find('td').eq(1).text().trim();
        objeto.conductorId = tr.find('td').eq(2).find('input').val();
        objeto.conductor = tr.find('td').eq(2).text().trim();
        objeto.unidadId = tr.find('td').eq(3).find('input').val();
        objeto.unidad = tr.find('td').eq(3).text().trim();
        objeto.horarioId = tr.find('td').eq(4).find('input').val();
        objeto.horario = tr.find('td').eq(4).text().trim();
        objeto.fecha = tr.find('td').eq(5).text().trim();


        horarioChangeList.push(objeto);

        console.log("add horarioChange", horarioChangeList);

      } else {


        const horarioId = tr.find('td').eq(0).find('input').val();
        //remove horarioId form horarioChange
        const index = horarioChangeList.findIndex(x => x.corridaAsingnacionId == horarioId);

        horarioChangeList.splice(index, 1);

        console.log("remove horarioChange", horarioChangeList);

      }
    });



  }

  const showSelectedItems = () => {
    listadoContainer.innerHTML = '';
    horarioChangeList.forEach(item => {
      const row = document.createElement('div');
      row.className = "d-flex flex-row gap-4 mb-4";
      row.innerHTML = `<span class="text-gray-900">${item.conductor}</span>
                    <span class="text-gray-500">${item.unidad}</span>
                    <span class="text-gray-500"> Horario: ${item.horario}</span>
                    <span class="text-gray-500"> Fecha: ${item.fecha}</span>`;

      const deleteOption = document.createElement('a');
      deleteOption.className = "ms-auto btn btn-icon btn-active-light-danger w-15px h-15px";
      deleteOption.innerHTML = `<i class="ki-outline ki-trash fs-3"></i>`;
      deleteOption.addEventListener('click', function (event) {

        horarioChangeList = horarioChangeList.filter(x => x.corridaAsingnacionId != item.corridaAsingnacionId);
        showSelectedItems();

      });

      row.appendChild(deleteOption);

      listadoContainer.appendChild(row);
    });
  }

  const removeAsignacion = () => {
    Swal.fire({
      text: 'Esta seguro que desea eliminar la asignación?',
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

        const model = {
          detalle: horarioChangeList,
        };

        const response = await fetch('/ChoferUnidad/EliminarAsignacionChoferUnidad', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(model)
        });
        const data = await response.json();
        console.log(data);

        return data;
      }
    }).then(function (result) {
      console.log(result);
      if (result.value.success) {
        Swal.fire({
          text: 'Se elimino la asignación correctamente',
          icon: "success",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        }).then(function () {
          location.reload();
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
  }

  const handleRemoveAsignacion = () => {
    btnRemoverAsingacion.addEventListener('click', function () {
      if (horarioChangeList.length === 0) {
        notificacion.info("No se ha seleccionado ningun chofer/unidad");
        return;
      }

      removeAsignacion();
    });

  }

  const handleSeleccion = () => {

    btnSeleccionReasignacion.addEventListener('click', function () {
      if (horarioChangeList.length === 0) {
        notificacion.info("No se ha seleccionado ningun chofer/unidad");
        return;
      }

      showModalAsignar.show();
      showSelectedItems();
    });




  }

  const handleGuardar = () => {

    btnReasignar.addEventListener('click', async function () {

      const conductorReasignarId = document.getElementById('ConductorReasignarId').value;
      const vehiculoReasignarId = document.getElementById('VehiculoReasignarId').value;

      if (horarioChangeList.length === 0 || (conductorReasignarId == "" && vehiculoReasignarId == "")) {
        notificacion.info("No se ha seleccionado ningun conductor o unidad");
        return;
      }


      const model = {
        detalle: horarioChangeList,
        conductorReasignarId: conductorReasignarId === null || conductorReasignarId === undefined || conductorReasignarId == "" ? 0 : conductorReasignarId,
        vehiculoReasignarId: vehiculoReasignarId === null || vehiculoReasignarId === undefined || vehiculoReasignarId == "" ? 0 : vehiculoReasignarId
      };
      blockUI.block();

      console.log(model);

      const response = await fetch('/ChoferUnidad/ReasignarChoferUnidad', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(model)
      });
      const data = await response.json();
      console.log(data);

      if (data.success) {

        Swal.fire({
          text: `Se reasigno correctamente.`,
          icon: "success",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        });
        blockUI.release();
        location.reload();

      } else {
        //notificacion.error("Ocurrio un error al reasignar");
        blockUI.release();

        Swal.fire({
          text: data.message,
          icon: "error",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        });
      }

    });

  }

  const intiControls = () => {

    $('#selectAll').click(function () {
      horarioChangeList = [];
      if ($(this).prop('checked')) {
        $('.rowCheckbox').prop('checked', true); // Seleccionar todas las filas
        $('.rowCheckbox').trigger('change');
      } else {
        $('.rowCheckbox').prop('checked', false); // Deseleccionar todas las filas
        //$('.rowCheckbox').trigger('change');

      }
    });

    rowCheckSelection();
    handleSeleccion();
    handleRemoveAsignacion();
    handleGuardar();
  }

  return {
    // Public functions  
    init: function () {
      if (!table) {
        return;
      }

      initTable();
      intiControls();
      handleSearchDatatable();
      //handleDeleteRows();

    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTReasignarList.init();
});