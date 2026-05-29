"use strict";

// Class definition
var KTCorridas = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  var submitAgregarButton;
  var table = document.getElementById('table-horarios');


  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {
          'Vigencia': {
            validators: {
              notEmpty: {
                message: 'Vigencias es requerida'
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
      //validator.validate().then(function (status) {
      //  if (status == 'Valid') {
      // Disable button to avoid multiple click
      submitButton.setAttribute('data-kt-indicator', 'on');
      submitButton.disabled = true;
      //form.submit();
      //  }
      //});
    });

    submitAgregarButton.addEventListener('click', function (e) {
      // Prevent button default action
      e.preventDefault();

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Disable button to avoid multiple click
          submitAgregarButton.setAttribute('data-kt-indicator', 'on');
          submitAgregarButton.disabled = true;
          from.value = "agregarCorrida";
          form.submit();
        }
      });
    });
  }

  const handleControls = () => {
    //let contadorCorridas = 0;
    //document.getElementById('agregarCorrida').addEventListener('click', function () {
    //  contadorCorridas++;
    //  var template = `
    //            <div>
    //                <h2>Corrida ${contadorCorridas}</h2>
    //                <label asp-for="Detalles[${contadorCorridas - 1}].Inicio">Hora de inicio</label>
    //                <input asp-for="Detalles[${contadorCorridas - 1}].Inicio" type="time" />

    //                <label asp-for="Detalles[${contadorCorridas - 1}].Fin">Hora de fin</label>
    //                <input asp-for="Detalles[${contadorCorridas - 1}].Fin" type="time" />

    //                <h3>Días de la semana</h3>

    //            </div>
    //        `;
    //  document.querySelector('form').insertAdjacentHTML('beforeend', template);
    //});
  }


  const handleDeleteNewRow = () => {
    const deleteButtons = table.querySelectorAll('[data-kt-corridas-horario-table-filter="delete_new_row"]');
    deleteButtons.forEach(d => {
      d.addEventListener('click', function (e) {
        document.getElementById('agregarCorrida').value = "-";
        form.submit();
      });
    });
  }

  const handleAddNewRow = () => {
    const addButtons = document.querySelectorAll('[data-kt-corridas-horario-table-filter="add_new_row"]');
    addButtons.forEach(d => {
      d.addEventListener('click', function (e) {
        document.getElementById('agregarCorrida').value = "+";
        form.submit();
      });
    });
  }

  const handleSaveButton = () => {
    const addButtons = document.querySelectorAll('[data-kt-corridas-horario-table-filter="save_all"]');
    addButtons.forEach(d => {
      d.addEventListener('click', function (e) {
        document.getElementById('agregarCorrida').value = "Guardar";
        form.submit();
      });
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
        document.getElementById('agregarCorrida').value = horarioId;
        Swal.fire({
          text: 'Esta seguro que desea eliminar la el horario seleccionado?',
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
            form.submit();
            //const response = await fetch('/Corridas/EliminarHorarioCorrida', {
            //  method: 'POST',
            //  headers: {
            //    'Content-Type': 'application/json'
            //  },
            //  body: JSON.stringify({ RutaId: horarioId })
            //});
            //const data = await response.json();
            //console.log(data);

            return false;
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

              parent.remove();
              location.reload();


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
  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_corridas_in_form');
      submitButton = document.querySelector('#kt_corridas_in_submit');
      submitAgregarButton = document.querySelector('input[name="agregarCorrida"]');


      handleControls();
      handleValidation();
      handleDeleteHorarios();
      handleDeleteNewRow();
      handleAddNewRow();
      handleSaveButton();
      //handleSubmitValidation(); // use for form validation submit
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTCorridas.init();
});
