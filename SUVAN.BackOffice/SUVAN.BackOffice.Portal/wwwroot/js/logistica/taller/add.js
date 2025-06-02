"use strict";

// Class definition
var KTTaller = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    const depositoSelect = document.getElementById('IdDeposito');
    const zonaSelect = document.getElementById('ZonaIdzona');
    const zonaJsonInput = document.getElementById('ZonaJson');

    let zonaConfiguration = {};
    let depositoConfiguration = {};

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
                                max: 100,

                                message: 'deben tener entre 7 y 100 caracteres',
                            },
                        }
                    },
                    'ZonaIdzona': {
                        validators: {
                            notEmpty: {
                                message: 'Zona requerida',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'IdDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Depósito requerido',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'Contacto': {
                        validators: {
                            notEmpty: {
                                message: 'Contacto requerido'
                            }
                        }
                    },
                    'Telefono': {
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
                    'Email': {
                        validators: {
                            regexp: {
                                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                message: 'No es un correo electr&oacutenico valido',
                            },
                            notEmpty: {
                                message: 'Correo requerido'
                            }
                        }
                    },
                    'Domicilio': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Taller requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 255,

                                message: 'deben tener entre 7 y 50 caracteres',
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

    const initData = () => {
        try {
            zonaConfiguration = JSON.parse(zonaJsonInput.value);
        } catch (e) {
        }
    };

    const clearSelect = (select) => {
        while (select.options.length > 0) {
            select.remove(0);
        }
    };

    const initZonaAndDeposito = () => {
        zonaSelect.addEventListener('change', function (event) {
            const zonaId = parseInt(event.target.value);
            const zona = zonaConfiguration.find(z => z.ZonaId === zonaId);

            clearSelect(depositoSelect);

            const optionSeleccione = document.createElement('option');
            optionSeleccione.value = "";
            optionSeleccione.textContent = "Selecciona un depósito";
            depositoSelect.appendChild(optionSeleccione);

            depositoConfiguration = zona.Depositos;

            zona.Depositos.forEach(t => {
                const option = document.createElement('option');
                option.value = t.DepositoId;
                option.textContent = t.DepositoNombreId;
                depositoSelect.appendChild(option);
            });
        });

        depositoSelect.addEventListener('change', function (event) {
            const depositoId = parseInt(event.target.value);
            const deposito = depositoConfiguration.find(t => t.DepositoId === depositoId);
            console.log("Depósito seleccionado:", deposito);
        });
    };

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
        initZonaAndDeposito();
        handleSubmitValidation();
    };

    // Public functions
    return {
        init: function () {
            form = document.querySelector('#kt_taller_in_form');
            submitButton = document.querySelector('#kt_taller_in_submit');

            document.querySelector("#Telefono").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            handleValidation();
            initData();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTTaller.init();
});