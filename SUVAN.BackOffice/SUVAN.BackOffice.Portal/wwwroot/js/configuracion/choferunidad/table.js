"use strict";

var KTChoferUnidadList = function () {
  // Define shared variables
  let form;
  let submitButton;
  let validator;
  const table = document.getElementById('kt_reporte_table');
  let datatable;

  const rutaSelect = document.getElementById('RutaId');
  const horarioSelect = document.getElementById('HorarioId');
  let rutaConfiguration = [];
  let corridasConfiguration = [];
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {
          'RutaId': {
            validators: {
              notEmpty: {
                message: 'Ruta requerida'
              },
            }
          }
        },
        plugins: {
          trigger: new FormValidation.plugins.Trigger(),
          bootstrap: new FormValidation.plugins.Bootstrap5({
            rowSelector: '.fv-row',
            eleInvalidClass: '',  // comment to enable invalid state icons
            eleValidClass: '' // comment to enable valid state icons
          })
        }
      }
    );
  }

  var handleSubmitValidation = function (e) {
    // Handle form submit
    submitButton.addEventListener('click', function (e) {
      // Prevent button default action
      e.preventDefault();

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Disable button to avoid multiple click
          submitButton.setAttribute('data-kt-indicator', 'on');
          submitButton.disabled = true;
          form.submit();
        }
      });
    });
  }

  // Private functions
  var initReporteTable = function () {
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
      //'columnDefs': [
      //  { orderable: false, targets: 4 }, // Disable ordering on column 6 (actions)                
      //]
    });

    // Re-init functions on every table re-draw -- more info: https://datatables.net/reference/event/draw

  }



  // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
  var handleSearchDatatable = () => {
    const filterSearch = document.querySelector('[data-kt-reporte-table-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
      datatable.search(e.target.value).draw();
    });
  }

  // limpiar valores de calendario y fechas seleccionadas
  const limpiaValores = () => {
    corridasConfiguration = [];
  }

  const initRutaAndHoarios = () => {


    $("#RutaId").on('change', function (event) {

      //const rutaId = event.target.value;
      const rutaId = $(this).val();
      if (!rutaId) {
        return;
      }
      const ruta = rutaConfiguration.find(r => r.RutaId == rutaId);

      // limpia valores
      horarioSelect.innerHTML = "";
      limpiaValores();

      const optionSeleccione = document.createElement('option');
      optionSeleccione.value = "";
      optionSeleccione.textContent = "Selecciona un horario";
      horarioSelect.appendChild(optionSeleccione);
      if (ruta.Corridas.length == 0) {
        notificacion.info("No hay horarios disponibles para la ruta seleccionada");
        return;
      }
      corridasConfiguration = ruta.Corridas;
      ruta.Corridas.forEach(horario => {
        const option = document.createElement('option');
        option.value = horario.CorridaId;
        option.textContent = horario.Horas;
        horarioSelect.appendChild(option);
      });

    });

    horarioSelect.addEventListener('change', function (event) {

      const corridaId = event.target.value;
      const horario = corridasConfiguration.find(r => r.CorridaId == corridaId);


    });


    $("#RutaId").trigger("change");
    const horarioSeleccionado = document.getElementById('IdHoSeleccionado').value;
    if (horarioSeleccionado) {
      horarioSelect.value = horarioSeleccionado;
    }

  }

  const initControls = () => {
    initRutaAndHoarios();

  }

  const initData = () => {
    const rutaJsonInput = document.getElementById('RutasJson');
    rutaConfiguration = JSON.parse(rutaJsonInput.value);
    console.log(rutaConfiguration);
  }


  return {
    // Public functions  
    init: function () {
      if (!table) {
        return;
      }
      form = document.querySelector('#kt_reporte_in_form');
      submitButton = document.querySelector('#kt_reporte_in_submit');
      initData();
      initControls();

      handleValidation();

      handleSubmitValidation(); // use for form validation submit
      initReporteTable();
      handleSearchDatatable();


    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTChoferUnidadList.init();
});