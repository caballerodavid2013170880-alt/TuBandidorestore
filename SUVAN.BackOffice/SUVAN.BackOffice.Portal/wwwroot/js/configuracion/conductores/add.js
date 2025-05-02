"use strict";

// Class definition
var KTConductor = function () {
  // Elements
  var form;
  var submitButton;
  var validator;

  const dataURItoBlob = (dataURI) => {
    var byteString = atob(dataURI.split(',')[1]);
    var ab = new ArrayBuffer(byteString.length);
    var ia = new Uint8Array(ab);
    for (var i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }
    return new Blob([ab], { type: 'image/png' });
  }

  const loadImageBase64 = (base64String) => {

    const fileInput = document.getElementById('Imagen');

    var blob = dataURItoBlob(base64String);
    var file = new File([blob], "consulta.png", { type: "image/png" });

    // Crea un nuevo FileList y asigna el archivo
    var fileList = new DataTransfer();
    fileList.items.add(file);

    // Asigna el archivo al input file
    //fileInput.files[0] = [file];
    fileInput.files = fileList.files;

    $("#Imagen").trigger('change');

    //.image-input.image-input-changed

    // toggle class image-input-changed to image-input
    const imageInput = document.querySelector('.image-input');
    imageInput.classList.remove('image-input-empty');
    imageInput.classList.add('image-input-changed');

  }

  const initImage64 = () => {

    const image64 = document.getElementById('Imagen64').value;
    if (image64) {
      const imageWrapper = document.getElementById('image-conductor-wrapper');

      const base64String = `data:image/png;base64,${image64}`;
      imageWrapper.style.backgroundImage = `url(${base64String})`;

      loadImageBase64(base64String);

    }

  }

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
          'RFC': {
            validators: {
              notEmpty: {
                message: 'RFC requerido'
              },
              regexp: {
                regexp: /^([A-Z&Ññ]{3,4})(\d{6})([A-V1-9])([A-Z\d]{2,3})$/,
                message: 'El RFC no es valido ',
              }
            }
          },
          'Curp': {
            validators: {
              notEmpty: {
                message: 'CURP requerido'
              },
              regexp: {
                regexp: /^[A-Z]{4}\d{6}[HM][A-Z]{5}\d{2}$/,
                message: 'El CURP no es valido ',
              }
            }
          },
          'Ine': {
            validators: {
              notEmpty: {
                message: 'INE requerido'
              }
            }
          },
          'Correo': {
            validators: {
              regexp: {
                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: 'No es un correo electr&oacutenico valido',
              },
              notEmpty: {
                message: 'Correo requerido'
              }
            }
          },
          'Direccion': {
            validators: {
              notEmpty: {
                message: 'Direcci&oacute;n requerido'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'TipoSangre': {
            validators: {
              notEmpty: {
                message: 'Tipo sangre requerido'
              },
              stringLength: {
                min: 2,
                max: 10,

                message: 'deben tener entre 2 y 10 caracteres',
              },
            }
          },
          'NumeroLicencia': {
            validators: {
              notEmpty: {
                message: 'N&uacutemero licencia requerido'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'TipoLicencia': {
            validators: {
              notEmpty: {
                message: 'Tipo licencia requerido'
              }
            }
          },
          'RegimenFiscalId': {
            validators: {
              notEmpty: {
                message: 'Regimen fiscal requerido'
              }
            }
          },
          'Cif': {
            validators: {
              notEmpty: {
                message: 'CIF requerido'
              },
              stringLength: {
                min: 4,
                max: 25,

                message: 'deben tener entre 4 y 25 caracteres',
              },
            }
          },
          'NombreContacto': {
            validators: {
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Imagen': {
            validators: {
              file: {
                extension: 'jpeg,jpg,png',
                type: 'image/jpeg,image/png',
                maxSize: 2097152, // 2048 * 1024
                message: 'El archivo seleccionado no es valido',
              },
            }
          },
          comision: {
            selector: '.comisionValidator',
            validators: {
              callback: {
                message: 'Por favor ingrese al menos una de las comisiones',
                callback: function (input) {
                  let isEmpty = true;
                  const comisionElements = validator.getElements('comision');
                  for (const i in comisionElements) {
                    if (comisionElements[i].value !== '') {
                      isEmpty = false;
                      break;
                    }
                  }

                  if (!isEmpty) {
                    // Update the status of callback validator for all fields
                    validator.updateFieldStatus('comision', 'Valid', 'callback');
                    return true;
                  }

                  return false;
                }
              }
            },
          }
          //'Comisionfija': {
          //  validators: {
          //    notEmpty: {
          //      message: 'Por favor ingrese al menos uno de los campos'
          //    }
          //  }
          //},
          //'ComisionvariableKm': {
          //  validators: {
          //    notEmpty: {
          //      message: 'Por favor ingrese al menos uno de los campos'
          //    }
          //  }
          //},
          //'ComisionvariableIngresos': {
          //  validators: {
          //    notEmpty: {
          //      message: 'Por favor ingrese al menos uno de los campos'
          //    }
          //  }
          //}
        },
        //options: {
        //  verbose: false,
        //  fields: {
        //    Comisionfija: {
        //      validators: {
        //        oneOf: {
        //          message: 'Por favor ingrese al menos uno de los campos',
        //          field: ['Comisionfija', 'ComisionvariableKm', 'ComisionvariableIngresos']
        //        }
        //      }
        //    }
        //  }
        //},
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
      form = document.querySelector('#kt_conductor_in_form');
      submitButton = document.querySelector('#kt_conductor_in_submit');


      handleValidation();
      initImage64();
      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTConductor.init();
});
