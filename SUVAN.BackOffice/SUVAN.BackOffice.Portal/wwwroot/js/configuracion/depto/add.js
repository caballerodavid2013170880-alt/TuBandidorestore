"use strict";

var KTDepartamentoAdd = function () {
    var form;
    var submitButton;
    var validator;

    var initValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'id_region': {
                        validators: {
                            notEmpty: {
                                message: 'La Región es requerida'
                            }
                        }
                    },
                    'id_planta': {
                        validators: {
                            notEmpty: {
                                message: 'La Planta es requerida'
                            }
                        }
                    },
                    'id_zona': {
                        validators: {
                            notEmpty: {
                                message: 'La Zona es requerida'
                            }
                        }
                    },
                    'id_deposi': {
                        validators: {
                            notEmpty: {
                                message: 'El Depósito es requerido'
                            }
                        }
                    },
                    'descripcion': {
                        validators: {
                            notEmpty: {
                                message: 'La Descripción es requerida'
                            }
                        }
                    },
                    'responsable': {
                        validators: {
                            notEmpty: {
                                message: 'El Responsable es requerido'
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

    var handleFormSubmit = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();

            if (validator) {
                validator.validate().then(function (status) {
                    if (status == 'Valid') {
                        submitButton.setAttribute('data-kt-indicator', 'on');
                        submitButton.disabled = true;

                        // Re-enable Disabled selects before submit so they get posted
                        $(form).find('select:disabled').prop('disabled', false);

                        form.submit();
                    } else {
                        Swal.fire({
                            text: "Por favor llene o corrija los campos indicados del formulario.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "De acuerdo",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });
    }

    return {
        init: function () {
            form = document.querySelector('#kt_departamento_in_form');
            submitButton = document.querySelector('#kt_departamento_in_submit');

            if (!form || !submitButton) {
                return;
            }

            initValidation();
            handleFormSubmit();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDepartamentoAdd.init();
});
