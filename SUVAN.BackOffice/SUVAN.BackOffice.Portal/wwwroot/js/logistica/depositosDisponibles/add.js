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
                    'NombreDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Depósito requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 100,

                                message: 'deben tener entre 7 y 100 caracteres',
                            },
                        }
                    },
                    'Dirección': {
                        validators: {
                            notEmpty: {
                                message: 'Dirección requerida'
                            },
                            stringLength: {
                                min: 7,
                                max: 70,

                                message: 'deben tener entre 7 y 70 caracteres',
                            },
                        }
                    },
                    'Ciudad': {
                        validators: {
                            notEmpty: {
                                message: 'Ciudad requerida'
                            },
                            stringLength: {
                                min: 7,
                                max: 50,

                                message: 'deben tener entre 7 y 50 caracteres',
                            },
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
                    },
                    'Teléfono': {
                        validators: {
                            notEmpty: {
                                message: 'Número telefónico requerido'
                            },
                            regexp: {
                                regexp: /^[0-9]{10}$/,
                                message: 'El número debe tener 10 dígitos'
                            }
                        }
                    },
                    'ZonaId': {
                        validators: {
                            notEmpty: {
                                message: 'Zona requerida',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
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
                    'NombreCorto': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre corto requerido'
                            },
                            stringLength: {
                                min: 4,
                                max: 8,

                                message: 'deben tener entre 4 y 8 caracteres',
                            },
                        }
                    },
                    'Responsable': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre corto requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 50,

                                message: 'deben tener entre 7 y 50 caracteres',
                            },
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

    const initControls = () => {
        handleSubmitValidation();
    };

    // Public functions
    return {
        init: function () {
            form = document.querySelector('#kt_deposito_in_form');
            submitButton = document.querySelector('#kt_deposito_in_submit');

            document.querySelector("#Teléfono").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            handleValidation();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTDeposito.init();
});
