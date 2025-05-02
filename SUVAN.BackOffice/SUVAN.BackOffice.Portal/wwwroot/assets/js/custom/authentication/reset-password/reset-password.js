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

          // Simulate ajax request
          setTimeout(function () {
            // Hide loading indication
            submitButton.removeAttribute('data-kt-indicator');

            // Enable button
            submitButton.disabled = false;

            // Show message popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
            Swal.fire({
              text: "We have send a password reset link to your email.",
              icon: "success",
              buttonsStyling: false,
              confirmButtonText: "Ok, got it!",
              customClass: {
                confirmButton: "btn btn-primary"
              }
            }).then(function (result) {
              if (result.isConfirmed) {
                form.querySelector('[name="email"]').value = "";
                //form.submit();

                var redirectUrl = form.getAttribute('data-kt-redirect-url');
                if (redirectUrl) {
                  location.href = redirectUrl;
                }
              }
            });
          }, 1500);
        } else {
          // Show error popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
          Swal.fire({
            text: "Lo sentimos, se han detectado algunos errores. Int&eacute;ntalo de nuevo.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Aceptar",
            customClass: {
              confirmButton: "btn btn-primary"
            }
          });
        }
      });
    });
  }


  var isValidUrl = function (url) {
    try {
      new URL(url);
      return true;
    } catch (e) {
      return false;
    }
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
