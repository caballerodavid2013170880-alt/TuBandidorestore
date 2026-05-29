"use strict";

// Class definition
var KTPolitica = function () {
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

          'RangoTiempo': {
            validators: {
              notEmpty: {
                message: 'Rango de tiempo es requerido'
              },
              integer: {
                message: 'El valor no es un n&uacute;mero entero v&aacute;lido',
                // The default separators
                thousandsSeparator: '',
                decimalSeparator: '.',
              },
              greaterThan: {
                min: 1,
                message: 'Ingrese un n&uacute;mero mayor de 0'
              }
            }
          },
          'PorcentajeCompensacion': {
            validators: {
              notEmpty: {
                message: 'Porcentaje es requerido'
              },
              integer: {
                message: 'El valor no es un n&uacute;mero entero v&aacute;lido',
                // The default separators
                thousandsSeparator: '',
                decimalSeparator: '.',
              },
              greaterThan: {
                min: 1,
                message: 'Ingrese un n&uacute;mero mayor de 0'
              }
            }
          },
          'TipoCancelacion': {
            validators: {
              notEmpty: {
                message: 'El tipo de cancelaci&oacute;n es requerido'
              }
            }
          },
          'TipoTiempo': {
            validators: {
              notEmpty: {
                message: 'El tipo de tiempo es requerido'
              }
            }
          },

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

  //const handleControls = () => {
  //  const tipoCodigoRadios = document.querySelectorAll('input[name="TipoCodigo"]');
  //  const correosExclusivosGroup = document.getElementById('correosExclusivosGroup');

  //  validator.disableValidator('Correos');
  //  validator.revalidateField('Correos');

  //  if (document.querySelector('input[name="TipoCodigo"]:checked')) {
  //    if (document.querySelector('input[name="TipoCodigo"]:checked').value === '2') {
  //      correosExclusivosGroup.style.display = 'block';
  //    } else if (document.querySelector('input[name="TipoCodigo"]:checked').value === '1') {
  //      correosExclusivosGroup.style.display = 'none';
  //    }
  //  }

  //  tipoCodigoRadios.forEach(function (radio) {
  //    radio.addEventListener('change', function () {
  //      if (radio.value === '2') {
  //        correosExclusivosGroup.style.display = 'block';
  //      } else {
  //        correosExclusivosGroup.style.display = 'none';
  //      }
  //    });



  //  });

  //  $("#Vigencia").daterangepicker({
  //    locale: {
  //      format: "DD/M/yyyy",
  //      "applyLabel": "Aplicar",
  //      "cancelLabel": "Cancelar",
  //      "daysOfWeek": [
  //        "Do",
  //        "Lu",
  //        "Ma",
  //        "Mi",
  //        "Ju",
  //        "Vi",
  //        "Sa"
  //      ],
  //      "monthNames": [
  //        "Enero",
  //        "Febrero",
  //        "Marzo",
  //        "Abril",
  //        "Mayo",
  //        "Junio",
  //        "Julio",
  //        "Agosto",
  //        "Septiembre",
  //        "Octubre",
  //        "Noviembre",
  //        "Diciembre"
  //      ],
  //    }
  //  });




  //  var inputCodigo = document.getElementById('Codigo');
  //  var botonCopiar = document.getElementById('copy-code');
  //  // Agrega un evento de clic al botón
  //  botonCopiar.addEventListener('click', function () {
  //    // Crea un elemento de texto temporal
  //    var elementoTemporal = document.createElement('textarea');
  //    elementoTemporal.value = inputCodigo.value;

  //    // Añade el elemento temporal al DOM
  //    document.body.appendChild(elementoTemporal);

  //    // Selecciona y copia el contenido del textarea
  //    elementoTemporal.select();
  //    document.execCommand('copy');

  //    // Elimina el elemento temporal
  //    document.body.removeChild(elementoTemporal);

  //    // Muestra un mensaje de éxito (puedes personalizar esto según tus necesidades)
  //    notificacion.info('C&oacute;digo copiado.');
  //  });

  //}



  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_politica_in_form');
      submitButton = document.querySelector('#kt_politica_in_submit');

      handleValidation();

      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTPolitica.init();
});
