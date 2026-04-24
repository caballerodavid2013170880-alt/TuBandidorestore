"use strict";

// Class definition
var KTPlanta = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'id_region': {
                        validators: {
                            notEmpty: {
                                message: 'Región requerida'
                            }
                        }
                    },
                    'nombre': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre requerido'
                            },
                            stringLength: {
                                min: 2,
                                max: 45,
                                message: 'El nombre debe tener entre 2 y 45 caracteres',
                            },
                        }
                    },
                    'libreria': {
                        validators: {
                            stringLength: {
                                max: 50,
                                message: 'La librería no puede exceder los 50 caracteres',
                            },
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',// comment to enable invalid state icons
                        eleValidClass: '' // comment to enable valid state icons
                    })
                }
            }
        );
    }

    var handleSubmitValidation = function (e) {
        // Handle form submit
        submitButton.addEventListener('click', function (e) {
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
        init: function () {
            // Initialization
            form = document.querySelector('#kt_planta_in_form');
            submitButton = document.querySelector('#kt_planta_in_submit');

            handleValidation();
            handleSubmitValidation(); // use for form validation submit
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTPlanta.init();
});