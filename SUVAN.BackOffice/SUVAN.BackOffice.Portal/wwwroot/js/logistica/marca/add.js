"use strict";

// Definición de la clase
var KTMarca = function () {
    // Elementos
    var form;
    var submitButton;
    var validator;

    // Función para validar el formulario
    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'descrip': {
                        validators: {
                            notEmpty: {
                                message: 'La descripción de la marca es requerida'
                            },
                            stringLength: {
                                min: 3,
                                max: 50,
                                message: 'Debe tener entre 3 y 50 caracteres'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
    };

    // Manejo del envío del formulario
    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault(); // Evita el envío por defecto

            // Validar el formulario
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on'); // Indicador de carga
                    submitButton.disabled = true; // Evita múltiples envíos
                    form.submit(); // Envía el formulario
                }
            });
        });
    };

    // Función pública de inicialización
    return {
        init: function () {
            form = document.querySelector('#kt_marca_in_form');
            submitButton = document.querySelector('#kt_marca_in_submit');

            handleValidation(); // Configurar validaciones
            handleSubmitValidation(); // Manejar envío del formulario
        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTMarca.init();
});