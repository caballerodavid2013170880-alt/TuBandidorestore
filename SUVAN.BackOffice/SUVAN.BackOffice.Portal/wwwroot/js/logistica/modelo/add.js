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
                    'IdMarca': {
                        validators: {
                            notEmpty: {
                                message: 'Marca requerida',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'IdTipoV': {
                        validators: {
                            notEmpty: {
                                message: 'Tipo de Vehículo requerido',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'AnioDesde': {
                        validators: {
                            notEmpty: {
                                message: 'Año Desde requerido'
                            },
                            regexp: {
                                regexp: /^\d{4}$/,
                                message: 'El año debe tener 4 dígitos'
                            }
                        }
                    },
                    'AnioHasta': {
                        validators: {
                            notEmpty: {
                                message: 'Año Desde requerido'
                            },
                            regexp: {
                                regexp: /^\d{4}$/,
                                message: 'El año debe tener 4 dígitos'
                            }
                        }
                    },
                    'Descripcion': {
                        validators: {
                            notEmpty: {
                                message: 'Descripción requerida'
                            },
                            stringLength: {
                                min: 7,
                                max: 60,

                                message: 'deben tener entre 7 y 60 caracteres',
                            },
                        }
                    },
                    'KmGarantia': {
                        validators: {
                            notEmpty: {
                                message: 'Kilómetros de Garantía requerido'
                            }
                        }
                    },
                    'MesGarantia': {
                        validators: {
                            notEmpty: {
                                message: 'Meses de Garantía requerida'
                            }
                        }
                    },
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

            document.querySelector("#MesGarantia").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#AnioDesde").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#AnioHasta").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#KmGarantia").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9.]/g, '');
                if ((this.value.match(/\./g) || []).length > 1) {
                    this.value = this.value.slice(0, -1);
                }
            });

            handleValidation();

            handleSubmitValidation();

        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTModelo.init();
});