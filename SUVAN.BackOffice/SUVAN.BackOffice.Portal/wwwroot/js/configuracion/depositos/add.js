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

          'IdRegion': {
            validators: {
              callback: {
                message: 'Debes seleccionar una región',
                    callback: function (input) {
                          return input.value !== '0' && input.value !== "";
                      }
                  }
                }
          },

          'IdPlanta': {
            validators: {
              callback: {
                message: 'Debes seleccionar una planta',
                    callback: function (input) {
                        return input.value !== '0' && input.value !== "";
                    }
                  }
                }
          },


          'IdZona': {
            validators: {
              callback: {
                message: 'Debes seleccionar una zona',
                    callback: function (input) {
                        return input.value !== '0' && input.value !== "";
                    }
                }
            }
          },


          'NombreDeposito': {
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

          'Direc': {
            validators: {
              notEmpty: {
                message: 'Dirección requerida'
              },
              stringLength: {
                min: 10,
                max: 250,

                message: 'Deben tener entre 10 y 250 caracteres',
              },
            }
          },


          'Ciudad': {
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


          'Respon': {
            validators: {
              notEmpty: {
                message: 'Respon requerido'
              },
              stringLength: {
                min: 4,
                max: 100,

                message: 'Deben tener entre 4 y 100 caracteres',
              },
            }
          },


          'Tel': {
            validators: {
              notEmpty: {
                message: 'Teléfono requerido'
              },
              stringLength: {
                min: 10,
                max: 12,

                message: 'Debe tener entre 10 y 12 caracteres',
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
          'Cp': {
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


      //LOGICA COMBOS EN CASCADA

      //cuando se cambia el combo de region, se cargan las plantas
      $(form.querySelector('[name="IdRegion"]')).on('change', function(){
          var regionId = $(this).val();
          var $plantaSelect = $(form.querySelector('[name="IdPlanta"]'));
          var $zonaSelect = $(form.querySelector('[name="IdZona"]'));

          //limpiar combos hijo y mensaje de espera
          $plantaSelect.empty().append('<option value="0">Cargando plantas...</option>');
          $zonaSelect.empty().append('<option value="0">Selecciona una zona...</option>');

          if (regionId > 0) {
              //peticionAJAX al conytroller
              $.getJSON('/Configuracion/ObtenerPlantas', { regionId: regionId }, function (data) {
                  $plantaSelect.empty().append('<option value="0">Selecciona una planta...</option>');
                  $.each(data, function (index, item) {
                      $plantaSelect.append('<option value="' + item.id + '">' + item.nombre + '</option>');
                  });
              });
          }


          //Revalidación (quita mensaje de error rojo)
          validator.revalidateField('IdRegion');
      });


      //Cuando cambia planta, caga las zonas
      $(form.querySelector('[name="IdPlanta"]')).on('change', function(){
          var plantaId = $(this).val();
          var $zonaSelect = $(form.querySelector('[name="IdZona"]'));

          //limpia combo zona
          $zonaSelect.empty().append('<option value="0">Cargando Zonas...</option');

          if (plantaId > 0) {
              //Petición AJAX al Controller
              $.getJSON('/Configuracion/ObtenerZonas', { plantaId: plantaId }, function (data) {
                  $zonaSelect.empty().append('<option value="0">Selecciona una zona...</option>');
                  $.each(data, function (index, item) {
                      $zonaSelect.append('<option value="' + item.id + '">' + item.nombre + '</option>');
                  });
              });
          }


          //Revalidación (quita mensaje de error rojo)
          validator.revalidateField('IdPlanta');
      });


      //Cuando cambia la zona, solo revalidacion
      $(form.querySelector('[name="IdZona"]')).on('change', function(){
          validator.revalidateField('IdZona');
        });

      ////revalidacion al cambiar el combo
      ////esto quita mensaje rojo en cuanto se selecciona algo del combo
      //$(form.querySelector('[name="id_region"]')).on('change', function(){
      //    validator.revalidateField('id_region');
      //});

      //$(form.querySelector('[name="id_planta"]')).on('change', function(){
      //    validator.revalidateField('id_planta');
      //});

      //$(form.querySelector('[name="id_zona"]')).on('change', function(){
      //    validator.revalidateField('id_zona');
      //});


    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTDeposito.init();
});
