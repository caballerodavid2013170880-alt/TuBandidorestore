"use strict";

// Class definition
var KTSigninGeneral = function () {
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
                message: 'Nombre de perfil requerido'
              },
              regexp: {
                regexp: /^[a-zA-ZÀ-ÿ\u00f1\u00d1]+(\s*[a-zA-ZÀ-ÿ\u00f1\u00d1]*)*[a-zA-ZÀ-ÿ\u00f1\u00d1]+$/i,
                message: 'El Nombre de perfil solo debe conteneres alfabeticos ',
              },
              stringLength: {
                min: 4,
                max: 45,

                message: 'deben tener entre 4 y 45 caracteres',
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

  var handleSubmitAjax = function (e) {
    // Handle form submit
    submitButton.addEventListener('click', function (e) {
      // Prevent button default action
      e.preventDefault();

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Show loading indication
          submitButton.setAttribute('data-kt-indicator', 'on');

          // Disable button to avoid multiple click
          submitButton.disabled = true;

          // Check axios library docs: https://axios-http.com/docs/intro
          axios.post(submitButton.closest('form').getAttribute('action'), new FormData(form)).then(function (response) {
            if (response) {
              form.reset();

              // Show message popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
              Swal.fire({
                text: "You have successfully logged in!",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                  confirmButton: "btn btn-primary"
                }
              });

              const redirectUrl = form.getAttribute('data-kt-redirect-url');

              if (redirectUrl) {
                location.href = redirectUrl;
              }
            } else {
              // Show error popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
              Swal.fire({
                text: "Sorry, the email or password is incorrect, please try again.",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                  confirmButton: "btn btn-primary"
                }
              });
            }
          }).catch(function (error) {
            Swal.fire({
              text: "Lo sentimos, se han detectado algunos errores. Int&eacute;ntalo de nuevo.",
              icon: "error",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn btn-primary"
              }
            });
          }).then(() => {
            // Hide loading indication
            submitButton.removeAttribute('data-kt-indicator');

            // Enable button
            submitButton.disabled = false;
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

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_perfil_in_form');
      submitButton = document.querySelector('#kt_perfil_in_submit');

      handleValidation();


      //handleSubmitAjax(); // use for ajax submit
      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTSigninGeneral.init();
});
