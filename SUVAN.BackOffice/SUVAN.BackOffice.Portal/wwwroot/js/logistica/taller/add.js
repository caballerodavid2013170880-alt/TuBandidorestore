"use strict";

// Class definition
var KTTaller = function () {
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
                    'ZonaIdzona': {
                        validators: {
                            notEmpty: {
                                message: 'Debes de seleccionar una Zona',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
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

    const initControls = () => {
        handleSubmitValidation();
    };

    // Public functions
    return {
        init: function () {
            form = document.querySelector('#kt_taller_in_form');
            submitButton = document.querySelector('#kt_taller_in_submit');

            handleValidation();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTTaller.init();
});