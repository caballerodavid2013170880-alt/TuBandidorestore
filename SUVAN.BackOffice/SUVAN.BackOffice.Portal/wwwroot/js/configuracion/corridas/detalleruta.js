"use strict";

// Class definition
var KTDetalleRuta = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  let cantidadEstaciones = 0;


  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: generateValidationRules(cantidadEstaciones),
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

  const generateValidationRules = (count) => {
    var fields = {};

    for (var i = 0; i < count; i++) {
      fields[`Estaciones[${i}].Horario`] = {
        validators: {
          notEmpty: {
            message: 'El horario es obligatorio'
          },
          //greaterThan: {
          //  message: 'El horario debe ser mayor al anterior',
          //  compare: function () {
          //    var previousHorario = document.querySelector(`#Estaciones_${i}__Horario`).value;
          //    var currentHorario = document.querySelector(`#Estaciones_${i + 1}__Horario`).value;

          //    if (previousHorario === "" || currentHorario === "") {
          //      return true; // No realizar la comparación si alguno de los horarios está vacío
          //    }

          //    return currentHorario > previousHorario;
          //  }
          //}
        }
      };
    }

    return fields;
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

  const handleControls = () => {
    cantidadEstaciones = document.getElementById('CantidadEstaciones').value;
  }


  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_detalleruta_in_form');
      submitButton = document.querySelector('#kt_detalleruta_in_submit');


      handleControls();
      handleValidation();

      handleSubmitValidation(); // use for form validation submit
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTDetalleRuta.init();
});
