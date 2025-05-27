"use strict";

// Class definition
var KTReparacion = function () {
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

                    'Descripcion': {
                        validators: {
                            notEmpty: {
                                message: 'Descripción requerida'
                            },
                        }
                    },
                    'Grupo': {
                        validators: {
                            notEmpty: {
                                message: 'Grupo requerido'
                            },
                        }
                    },
                    'Valor': {
                        validators: {
                            notEmpty: {
                                message: 'Valor requerido'
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
            form = document.querySelector('#kt_reparacion_in_form');
            submitButton = document.querySelector('#kt_reparacion_in_submit');

            document.querySelector("#Valor").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9.]/g, '');
                if ((this.value.match(/\./g) || []).length > 1) {
                    this.value = this.value.slice(0, -1);
                }
            });

            document.querySelector("#Grupo").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });


            handleValidation();

            handleSubmitValidation(); // use for form validation submit

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTReparacion.init();
});
