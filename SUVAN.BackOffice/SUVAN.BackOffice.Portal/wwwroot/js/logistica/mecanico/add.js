"use strict";

// Class definition
var KTMecanico = function () {
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
                    'Nombre': {
                        validators: {
                            notEmpty: {
                                message: 'El Nombre es requerido'
                            }
                        }
                    },
                    'Apellido': {
                        validators: {
                            notEmpty: {
                                message: 'Los Apellidos son requeridos'
                            }
                        }
                    },
                    'IdDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Debes de seleccionar una Zona',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'Numero': {
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
                    'FechaIngreso': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de Ingreso es requerida'
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

        $("#FechaIngreso").daterangepicker({
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
            startDate: moment(),
            minDate: moment()
        });

        document.getElementById("Numero").addEventListener("input", function () {
            this.value = this.value.replace(/\D/g, "").slice(0, 10);
        });

        document.getElementById("Nombre").addEventListener("input", function () {
            this.value = this.value.toUpperCase();
        });
        document.getElementById("Apellido").addEventListener("input", function () {
                this.value = this.value.toUpperCase();
        });
    }

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_mecanico_in_form');
            submitButton = document.querySelector('#kt_mecanico_in_submit');

            handleValidation();
            handleControls();

            handleSubmitValidation();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTMecanico.init();
});