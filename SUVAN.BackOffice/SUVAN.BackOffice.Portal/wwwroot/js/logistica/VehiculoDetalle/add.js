"use strict";

// Class definition
var KTVehiculoDetalle = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    // Handle form validation
    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'PlacaPe': {
                        validators: {
                            notEmpty: {
                                message: 'La placa es requerida'
                            },
                            stringLength: {
                                min: 6,
                                max: 10,
                                message: 'La placa debe tener entre 6 y 10 caracteres'
                            }
                        }
                    },
                    'Color': {
                        validators: {
                            notEmpty: {
                                message: 'El color es requerido'
                            }
                        }
                    },
                    'Modelo': {
                        validators: {
                            notEmpty: {
                                message: 'El modelo es requerido'
                            }
                        }
                    },
                    'Anio': {
                        validators: {
                            notEmpty: {
                                message: 'El año es requerido'
                            },
                            regexp: {
                                regexp: /^[0-9]{4}$/,
                                message: 'Ingrese un año válido (4 dígitos)'
                            }
                        }
                    },
                    'Motor': {
                        validators: {
                            notEmpty: {
                                message: 'Número de motor requerido'
                            }
                        }
                    },
                    'Serie': {
                        validators: {
                            notEmpty: {
                                message: 'Número de serie requerido'
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
    }

    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;
                    form.submit();
                }
            });
        });
    }

    return {
        init: function () {
            form = document.querySelector('#kt_vehiculo_detalle_form');
            submitButton = document.querySelector('#kt_vehiculo_detalle_submit');

            handleValidation();
            handleSubmitValidation();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTVehiculoDetalle.init();
});