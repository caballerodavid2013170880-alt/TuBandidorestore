"use strict";

// Class definition
var KTMecanico = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    const tallerSelect = document.getElementById('IdTaller');
    const depositoSelect = document.getElementById('IdDeposito');
    const depositoJsonInput = document.getElementById('DepositoJson');

    let depositoConfiguration = {};
    let tallerConfiguration = {};


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
                                message: 'Nombre requerido'
                            }
                        }
                    },
                    'Puesto': {
                        validators: {
                            notEmpty: {
                                message: 'Puesto requerido'
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
                    'IdTaller': {
                        validators: {
                            notEmpty: {
                                message: 'Taller requerido',
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

    const initData = () => {
        try {
            depositoConfiguration = JSON.parse(depositoJsonInput.value);
        } catch (e) {
        }
    };

    const clearSelect = (select) => {
        while (select.options.length > 0) {
            select.remove(0);
        }
    };

    const initDepositoAndTaller = () => {
        depositoSelect.addEventListener('change', function (event) {
            const depositoId = parseInt(event.target.value);
            const deposito = depositoConfiguration.find(z => z.DepositoId === depositoId);

            clearSelect(tallerSelect);

            const optionSeleccione = document.createElement('option');
            optionSeleccione.value = "";
            optionSeleccione.textContent = "Selecciona un taller";
            tallerSelect.appendChild(optionSeleccione);

            tallerConfiguration = deposito.Talleres;

            deposito.Talleres.forEach(t => {
                const option = document.createElement('option');
                option.value = t.IdTaller;
                option.textContent = t.TallerNombreId;
                tallerSelect.appendChild(option);
            });
        });

        tallerSelect.addEventListener('change', function (event) {
            const tallerId = parseInt(event.target.value);
            const taller = tallerConfiguration.find(t => t.IdTaller === tallerId);
            console.log("Taller seleccionado:", taller);
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
        initDepositoAndTaller();
        handleSubmitValidation();
    };



    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_mecanico_in_form');
            submitButton = document.querySelector('#kt_mecanico_in_submit');

            handleValidation();
            initData();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTMecanico.init();
});