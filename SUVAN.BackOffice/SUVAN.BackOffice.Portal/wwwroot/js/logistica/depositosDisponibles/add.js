"use strict";

// Class definition
var KTDeposito = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    const tallerSelect = document.getElementById('TallerId');
    const zonaSelect = document.getElementById('ZonaId');
    const zonaJsonInput = document.getElementById('ZonaJson');

    let zonaConfiguration = {};
    let tallerConfiguration = {};

    // Handle form
    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'NombreDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Deposito requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 20,

                                message: 'deben tener entre 7 y 20 caracteres',
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

    const initZonaAndTaller = () => {
        zonaSelect.addEventListener('change', function (event) {
            const zonaId = parseInt(event.target.value);
            const zona = zonaConfiguration.find(z => z.ZonaId === zonaId);

            clearSelect(tallerSelect);

            const optionSeleccione = document.createElement('option');
            optionSeleccione.value = "";
            optionSeleccione.textContent = "Selecciona un taller";
            tallerSelect.appendChild(optionSeleccione);

            tallerConfiguration = zona.Talleres;

            zona.Talleres.forEach(t => {
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
        initZonaAndTaller();
        handleSubmitValidation();
    };

    // Public functions
    return {
        init: function () {
            form = document.querySelector('#kt_deposito_in_form');
            submitButton = document.querySelector('#kt_deposito_in_submit');

            handleValidation();
            initData();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTDeposito.init();
});
