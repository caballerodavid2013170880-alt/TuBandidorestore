"use strict";

// Definición de la clase
var KTModelo = function () {
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
                    'Id_marca': {
                        validators: {
                            notEmpty: {
                                message: 'El ID de marca es requerido'
                            }
                        }
                    },
                    'Id_modelo': {
                        validators: {
                            notEmpty: {
                                message: 'El ID de modelo es requerido'
                            }
                        }
                    },
                    'a_desde': {
                        validators: {
                            notEmpty: {
                                message: 'El año desde es requerido'
                            },
                            numeric: {
                                message: 'Debe ser un número válido'
                            }
                        }
                    },
                    'a_hasta': {
                        validators: {
                            notEmpty: {
                                message: 'El año hasta es requerido'
                            },
                            numeric: {
                                message: 'Debe ser un número válido'
                            }
                        }
                    },
                    'Id_tipo_v': {
                        validators: {
                            notEmpty: {
                                message: 'El tipo de vehículo es requerido'
                            }
                        }
                    },
                    'descrip': {
                        validators: {
                            notEmpty: {
                                message: 'La descripción es requerida'
                            },
                            stringLength: {
                                min: 3,
                                max: 50,
                                message: 'Debe tener entre 3 y 50 caracteres'
                            }
                        }
                    },
                    'km_garan': {
                        validators: {
                            notEmpty: {
                                message: 'El kilometraje de garantía es requerido'
                            },
                            numeric: {
                                message: 'Debe ser un número válido'
                            }
                        }
                    },
                    'mes_gara': {
                        validators: {
                            notEmpty: {
                                message: 'Los meses de garantía son requeridos'
                            },
                            numeric: {
                                message: 'Debe ser un número válido'
                            }
                        }
                    },
                    'tipo_eje': {
                        validators: {
                            notEmpty: {
                                message: 'El tipo de eje es requerido'
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
            form = document.querySelector('#kt_modelo_in_form');
            submitButton = document.querySelector('#kt_modelo_in_submit');

            handleValidation(); // Configurar validaciones
            handleSubmitValidation(); // Manejar envío del formulario
        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTModelo.init();
});