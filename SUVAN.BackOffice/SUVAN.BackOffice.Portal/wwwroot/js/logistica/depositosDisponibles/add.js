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

                    'ZonaId': {
                        validators: {
                            notEmpty: {
                                message: 'Zona ID requerido'
                            },
                            stringLength: {
                                min: 1,
                                max: 10,

                                message: 'deben tener entre 1 y 10 digitos',
                            },
                        }
                    },

                    'ZonaNombre': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre de la Zona requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 20,

                                message: 'deben tener entre 7 y 20 caracteres',
                            },
                        }
                    },

                    'NombreDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Deposito requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 20,

                                message: 'deben tener entre 7 y 20 caracteres',
                            },
                        }
                    },

                    'TallerId': {
                        validators: {
                            notEmpty: {
                                message: 'Zona ID requerido'
                            },
                            stringLength: {
                                min: 1,
                                max: 10,

                                message: 'deben tener entre 1 y 10 digitos',
                            },
                        }
                    },

                    'NombreTaller': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Taller requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 20,

                                message: 'deben tener entre 7 y 20 caracteres',
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

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_empresa_in_form');
            submitButton = document.querySelector('#kt_empresa_in_submit');


            handleValidation();

            handleSubmitValidation(); // use for form validation submit

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTDeposito.init();
});
