"use strict";

// Class definition
var KTZona = function () {
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
                    'ZonaNombre': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre de la Zona requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 225,

                                message: 'deben tener entre 7 y 225 caracteres',
                            },
                        }
                    },
                    'Domicilio': {
                        validators: {
                            notEmpty: {
                                message: 'Domicilio requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 80,

                                message: 'deben tener entre 7 y 80 caracteres',
                            },
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
                    'Responsable': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Responsable requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 60,

                                message: 'deben tener entre 7 y 60 caracteres',
                            },
                        }
                    },
                    'FechaApertura': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de Apertura es requerida'
                            }
                        }
                    },
                    'Telefono1': {
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
                    'Telefono2': {
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

    const handleControls = () => {

        var fechaApertura = $("#FechaApertura").val();

        if (!fechaApertura || fechaApertura === "01/01/0001") {
            fechaApertura = moment().format("DD/MM/YYYY");
        }

        $("#FechaApertura").daterangepicker({
            singleDatePicker: true,
            locale: {
                format: "DD/MM/YYYY",
                applyLabel: "Aceptar",
                cancelLabel: "Cancelar",
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: [
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                ]
            },
            startDate: moment(fechaApertura, "DD/MM/YYYY")
        });
    }

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_zona_in_form');
            submitButton = document.querySelector('#kt_zona_in_submit');

            document.querySelector("#Telefono1").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#Telefono2").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });


            handleValidation();
            handleControls();

            handleSubmitValidation(); // use for form validation submit

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTZona.init();
});