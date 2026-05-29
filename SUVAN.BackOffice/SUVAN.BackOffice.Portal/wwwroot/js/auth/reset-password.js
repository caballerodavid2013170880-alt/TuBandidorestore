"use strict";

// Class Definition
var KTAuthResetPassword = function () {
  // Elements
  var form;
  var submitButton;
  var validator;

  var handleForm = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {
          'Email': {
            validators: {
              regexp: {
                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: 'No es un correo electr&oacutenico valido',
              },
              notEmpty: {
                message: 'Correo electr&oacutenico es requerido'
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
    submitButton.addEventListener('click', function (e) {
      e.preventDefault();

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Show loading indication
          submitButton.setAttribute('data-kt-indicator', 'on');

          // Disable button to avoid multiple click
          submitButton.disabled = true;
          form.submit();


        }
      });
    });
  }



  // Public Functions
  return {
    // public functions
    init: function () {
      form = document.querySelector('#kt_password_reset_form');
      submitButton = document.querySelector('#kt_password_reset_submit');

      handleForm();

      handleSubmitValidation(); // used for demo purposes only

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTAuthResetPassword.init();
});
