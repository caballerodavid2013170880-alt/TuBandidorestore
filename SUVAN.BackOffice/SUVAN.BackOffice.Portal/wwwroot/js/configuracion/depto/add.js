"use strict";

var KTDeptoAdd = function () {
    var submitButton;
    var form;
    var validator;

    var initValidation = function () {
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
                    'id_deposi': {
                        validators: {
                            notEmpty: {
                                message: 'El depósito es requerido'
                            }
                        }
                    },
                    'descrip': {
                        validators: {
                            notEmpty: {
                                message: 'La descripción es requerida'
                            }
                        }
                    },
                    'responsable': {
                        validators: {
                            notEmpty: {
                                message: 'El responsable es requerido'
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

    var handleForm = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            if (validator) {
                validator.validate().then(function (status) {
                    if (status == 'Valid') {
                        submitButton.setAttribute('data-kt-indicator', 'on');
                        submitButton.disabled = true;
                        form.submit();
                    } else {
                        Swal.fire({
                            text: "Lo sentimos, parece que se han detectado algunos errores, inténtelo de nuevo.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "¡Ok, entiendo!",
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
            form = document.querySelector('#kt_depto_form');
            submitButton = document.getElementById('kt_depto_submit');

            if (!form || !submitButton) {
                return;
            }

            initValidation();
            handleForm();
        }
    }
}();

KTUtil.onDOMContentLoaded(function () {
    KTDeptoAdd.init();
});
