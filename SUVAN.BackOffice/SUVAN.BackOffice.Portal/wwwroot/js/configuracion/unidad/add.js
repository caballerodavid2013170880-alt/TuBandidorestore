"use strict";

// Class definition
var KTUnidad = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  let serviciosList = [];
  const serviciosJson = document.getElementById('ServiciosJson');

  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {
          'TipoUnidadId': {
            validators: {
              notEmpty: {
                message: 'Tipo de unidad requerida'
              }
            }
          },
          'Placas': {
            validators: {
              notEmpty: {
                message: 'Placas requerida'
              },
              stringLength: {
                min: 4,
                max: 8,
                message: 'deben tener entre 4 y 8 caracteres',
              }
            }
          },
          'Vin': {
            validators: {
              regexp: {
                regexp: /^[A-HJ-NPR-Z0-9]{17}$/,
                message: 'Ingrese un formato de VIN v&aacute;lido',
              },
              //vin: {
              //  message: 'Ingrese un formato de VIN v&aacute;lido'
              //}
            }
          },
          'NumeroPoliza': {
            validators: {
              notEmpty: {
                message: 'N&uacute;mero de p&oacute;liza requerida'
              },
              stringLength: {
                min: 6,
                max: 255,
                message: 'deben tener entre 6 y 255 caracteres',
              }
            }
          },
          'FechaFinSeguro': {
            validators: {
              notEmpty: {
                message: 'Fecha fin seguro requerida'
              }
            }
          },
          'Marca': {
            validators: {
              notEmpty: {
                message: 'Marca requerida'
              },
              stringLength: {
                min: 3,
                max: 255,
                message: 'deben tener entre 3 y 255 caracteres',
              }
            }
          },
          'Modelo': {
            validators: {
              notEmpty: {
                message: 'Modelo requerido'
              },
              stringLength: {
                min: 1,
                max: 6,
                message: 'deben tener entre 1 y 6 caracteres',
              }
            }
          },
          'NumeroEconomico': {
            validators: {
              notEmpty: {
                message: 'N&uacute;mero econ&oacute;mico requerida'
              }
            }
          },
          'NumeroMotor': {
            validators: {
              notEmpty: {
                message: 'N&uacute;mero motor requerido'
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

      if (serviciosList.length > 0) {
        serviciosJson.value = JSON.stringify(serviciosList);
      }

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

  const initControls = () => {

    if (serviciosJson.value != "") {
      serviciosList = JSON.parse(serviciosJson.value);
      buldServiciosList();
    }

    const agregarServicioBtn = document.getElementById('agregar-servicio');
    const detalleInput = document.getElementById('detalle-servicio');
    const fechaServicioInput = document.getElementById('fecha-servicio');
    agregarServicioBtn.addEventListener('click', () => {

      if (detalleInput.value == "" || fechaServicioInput.value == "") {
        notificacion.info("Ingresar Detalle y fecha de servicio");
        return;
      }

      const data = {
        detalle: detalleInput.value,
        fechaServicio: fechaServicioInput.value
      };

      serviciosList.push(data);
      buldServiciosList();
      detalleInput.value = "";
      fechaServicioInput.value = "";
    });
  }

  const buldServiciosList = () => {
    const servicioListContainer = document.getElementById('servicios-list-container');
    servicioListContainer.innerHTML = "";

    serviciosList.forEach((item, index) => {
      const row = document.createElement('div');
      row.className = "d-flex flex-row gap-4 mb-4";
      row.innerHTML = `<span class="text-gray-900">${item.detalle}</span>
                    <span class="text-gray-500">${new Date(item.fechaServicio).toISOString().split('T')[0]}</span>`;

      const deleteOption = document.createElement('a');
      deleteOption.className = "ms-auto btn btn-icon btn-active-light-danger w-15px h-15px";
      deleteOption.innerHTML = `<i class="ki-outline ki-trash fs-3"></i>`;
      deleteOption.addEventListener('click', function (event) {

        serviciosList.splice(index, 1);
        buldServiciosList();

      });

      row.appendChild(deleteOption);

      servicioListContainer.appendChild(row);


    });

  }

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_unidad_in_form');
      submitButton = document.querySelector('#kt_unidad_in_submit');

      initControls();
      handleValidation();

      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTUnidad.init();
});

document.addEventListener("DOMContentLoaded", function () {
    const mostrar = document.getElementById("mostrarConfirmacionDetalle").value === 'True';
    const idDetalle = document.getElementById("idVehiculoDetalle")?.value;
    if (mostrar && idDetalle && idDetalle !== "0") {
        Swal.fire({
            title: "\u00bfDesea a\u00f1adir los detalles del veh\u00edculo?",
            icon: "question",
            showCancelButton: true,
            buttonsStyling: false,
            confirmButtonText: "S\u00ed",
            cancelButtonText: "No",
            customClass: {
                confirmButton: "btn fw-bold btn-primary",
                cancelButton: "btn fw-bold btn-secondary"
            }
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = `/VehiculoDetalle/NavegacionVehiculoDetalle/${idDetalle}`;
            } else {
                window.location.href = '/Configuracion/Unidades';
            }
        });
    }
});
