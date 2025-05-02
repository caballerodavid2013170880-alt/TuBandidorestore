"use strict";

// Class definition
var KTEmpresa = function () {
  // Elements
  var form;
  var submitButton;
  var validator;


  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {

          'Nombre': {
            validators: {
              notEmpty: {
                message: 'Nombre requerido'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },

          'NombreCorto': {
            validators: {
              notEmpty: {
                message: 'Nombre corto requerido'
              },
              stringLength: {
                min: 4,
                max: 8,

                message: 'deben tener entre 4 y 8 caracteres',
              },
            }
          },
          'Rfc': {
            validators: {
              notEmpty: {
                message: 'RFC requerido'
              },
              regexp: {
                regexp: /^([A-Z&Ññ]{3,4})(\d{6})([A-V1-9])([A-Z\d]{1,4})$/,
                message: 'El RFC no es válido ',
              }
            }
          },
          'CP': {
            validators: {
              notEmpty: {
                message: 'Código Postal requerido'
              },
              regexp: {
                regexp: /^\d{4,5}$/,
                message: 'El Código Postal no es válido',
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

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_empresa_in_form');
      submitButton = document.querySelector('#kt_empresa_in_submit');


      handleValidation();

      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTEmpresa.init();
});
