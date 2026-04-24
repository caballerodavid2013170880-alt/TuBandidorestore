"use strict";

var KTPlanta = function () {
    var form;
    var submitButton;
    var validator;

    var handleValidation = function (e) {
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
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
    }

    var handleSubmitValidation = function (e) {
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
            form = document.querySelector('#kt_planta_in_form');
            submitButton = document.querySelector('#kt_planta_in_submit');

            handleValidation();
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTPlanta.init();
});