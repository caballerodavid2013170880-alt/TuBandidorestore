"use strict";

var KTReporteIngresosList = function () {
  // Define shared variables
  let form;
  let submitButton;
  let validator;
  const table = document.getElementById('kt_reporte_table');
  let datatable;

  //calendarios
  const flatpickrOptions = {
    dateFormat: "d/m/Y",
  };
  const datepickerDesde = $("#desdeview").flatpickr(flatpickrOptions);
  const datepickerHasta = $("#hastaview").flatpickr(flatpickrOptions);

  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {

          'Hasta': {
            validators: {
              callback: {
                message: "La fecha hasta debe ser igual o posterior a la fecha desde",
                callback: function (input) {
                  var desde = datepickerDesde.selectedDates[0];
                  var hasta = datepickerHasta.selectedDates[0];

                  if (desde && hasta) {
                    return hasta >= desde;
                  }

                  return true;
                }
              }
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

  // Hook export buttons
  const exportButtons = () => {
    const documentTitle = 'Reporte Ingresos';
    var buttons = new $.fn.dataTable.Buttons(table, {
      buttons: [

        {
          extend: 'excelHtml5',
          title: documentTitle
        }

      ]
    }).container().appendTo($('#kt_datatable_example_buttons'));

    // Hook dropdown menu click event to datatable export buttons
    const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
    exportButtons.forEach(exportButton => {
      exportButton.addEventListener('click', e => {
        e.preventDefault();

        // Get clicked export value
        const exportValue = e.target.getAttribute('data-kt-export');
        const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

        // Trigger click event on hidden datatable export buttons
        target.click();
      });
    });
  }

  // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
  var handleSearchDatatable = () => {
    const filterSearch = document.querySelector('[data-kt-reporte-table-filter="search"]');
    filterSearch.addEventListener('keyup', function (e) {
      datatable.search(e.target.value).draw();
    });
  }




  const initControls = () => {
    const perdiodoInput = document.getElementById('Periodo');
    if (perdiodoInput.value === '0') {
      perdiodoInput.value = '1';
    }
    // 
    // Manejar el cambio en los radio buttons
    var radioButtons = document.querySelectorAll('input[name="periodo_type"]');
    const dateDesde = document.getElementById('Desde').value;
    const dateHasta = document.getElementById('Hasta').value;

    radioButtons.forEach(function (radio) {


      radio.addEventListener("change", function () {
        radioButtons.forEach(function (subradio) {
          subradio.closest("label").classList.remove("active");
        });
        this.closest("label").classList.add("active");
        var periodoSeleccionado = this.value;
        perdiodoInput.value = periodoSeleccionado;
        // Configurar Flatpickr según el periodo seleccionado

        if (dateDesde) {

          datepickerDesde.setDate(dateDesde);
          datepickerHasta.setDate(dateHasta);

        }
        switch (periodoSeleccionado) {
          case '1':
            configurarFlatpickrFormato("d/m/Y");
            break;
          case '2':
            configurarFlatpickrFormato("m/Y");
            break;
          case '3':
            configurarFlatpickrFormato("Y");
            break;
        }
      });
    });

    function configurarFlatpickrFormato(formato) {
      datepickerDesde.set("dateFormat", formato);
      datepickerHasta.set("dateFormat", formato);
    }

    if (perdiodoInput.value === '1') {
      const radio = $('input[name="periodo_type"][value="1"]');
      //radio.prop('checked', true);
      const label = radio.closest('label.btn-outline');
      label.click();

    }
    if (perdiodoInput.value === '2') {
      const radio = $('input[name="periodo_type"][value="2"]');
      //radio.prop('checked', true);
      const label = radio.closest('label.btn-outline');
      label.click();
    }
    if (perdiodoInput.value === '3') {
      const radio = $('input[name="periodo_type"][value="3"]');
      //radio.prop('checked', true);
      const label = radio.closest('label.btn-outline');
      label.click();
    }

  }




  return {
    // Public functions  
    init: function () {
      if (!table) {
        return;
      }
      form = document.querySelector('#kt_reporte_in_form');
      submitButton = document.querySelector('#kt_reporte_in_submit');

      initControls();
      handleValidation();

      handleSubmitValidation(); // use for form validation submit
      initReporteTable();
      exportButtons();
      handleSearchDatatable();


    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTReporteIngresosList.init();
});