"use strict";

var KTDepositoAdd = function () {
    var submitButton;
    var validator;
    var form;

    var handleForm = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'id_region': {
                        validators: {
                            notEmpty: {
                                message: 'La región es requerida'
                            }
                        }
                    },
                    'id_planta': {
                        validators: {
                            notEmpty: {
                                message: 'La planta es requerida'
                            }
                        }
                    },
                    'id_zona': {
                        validators: {
                            notEmpty: {
                                message: 'La zona es requerida'
                            }
                        }
                    },
                    'descripcion': {
                        validators: {
                            notEmpty: {
                                message: 'La descripción es requerida'
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

        submitButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {
                        submitButton.setAttribute('data-kt-indicator', 'on');
                        submitButton.disabled = true;

                        form.submit();
                    }
                });
            }
        });
    }

    return {
        init: function () {
            form = document.querySelector('#kt_deposito_in_form');
            submitButton = document.querySelector('#kt_deposito_in_submit');

            if (!form) {
                return;
            }

            handleForm();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDepositoAdd.init();
});
