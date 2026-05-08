"use strict";

// Class definition
var KTDeposito = function () {
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

          'id_region': {
            validators: {
              callback: {
                message: 'Debes seleccionar una región',
                    callback: function (input) {
                          return inpput.value !== '0' && input.value !== "";
                      }
                  }
                }
          },

          'id_planta': {
            validators: {
              callback: {
                message: 'Debes seleccionar una planta',
                    callback: function (input) {
                        return inpput.value !== '0' && input.value !== "";
                    }
                  }
                }
          },


          'id_zona': {
            validators: {
              callback: {
                message: 'Debes seleccionar una zaona',
                    callback: function (input) {
                        return inpput.value !== '0' && input.value !== "";
                    }
                }
            }
          },


          'descrip': {
            validators: {
              notEmpty: {
                message: 'Nombre requerido'
              },
              stringLength: {
                min: 10,
                max: 80,

                message: 'deben tener entre 10 y 80 caracteres',
              },
            }
          },

          'direc': {
            validators: {
              notEmpty: {
                message: 'Dirección requerida'
              },
              stringLength: {
                min: 10,
                max: 70,

                message: 'deben tener entre 10 y 70 caracteres',
              },
            }
          },


          'ciudad': {
            validators: {
              notEmpty: {
                message: 'Ciudad requerida'
              },
              stringLength: {
                min: 3,
                max: 50,

                message: 'deben tener entre 3 y 50 caracteres',
              },
            }
          },


          'respon': {
            validators: {
              notEmpty: {
                message: 'Respon requerido'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 2 y 50 caracteres',
              },
            }
          },



          'direc': {
            validators: {
              notEmpty: {
                message: 'Dirección requerida'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },


          'tel': {
            validators: {
              notEmpty: {
                message: 'Teléfono requerido'
              },
              stringLength: {
                min: 10,
                max: 12,

                message: 'deben tener entre 10 y 12 caracteres',
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
      form = document.querySelector('#kt_deposito_in_form');
      submitButton = document.querySelector('#kt_deposito_in_submit');


      handleValidation();

      handleSubmitValidation(); // use for form validation submit


      //revalidacion al cambiar el combo
      //esto quita mensaje rojo en cuanto se selecciona algo del combo
      $(form.querySelector('[name="id_region"]')).on('change', function(){
          validator.revalidateField('id_region');
      });

      $(form.querySelector('[name="id_planta"]')).on('change', function(){
          validator.revalidateField('id_planta');
      });

      $(form.querySelector('[name="id_zona"]')).on('change', function(){
          validator.revalidateField('id_zona');
      });


    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTDeposito.init();
});
