"use strict";

// Class definition
var KTEspecificaciones = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    const modeloSelect = document.getElementById('IdModelo');
    const marcaSelect = document.getElementById('IdMarca');
    const marcaJsonInput = document.getElementById('MarcaJson');

    let marcaConfiguration = {};
    let modeloConfiguration = {};

    // Handle form
    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
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
                    }
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
            marcaConfiguration = JSON.parse(marcaJsonInput.value);
        } catch (e) {
        }
    };

    const clearSelect = (select) => {
        while (select.options.length > 0) {
            select.remove(0);
        }
    };

    const initMarcaAndModelo = () => {
        marcaSelect.addEventListener('change', function (event) {
            const marcaId = parseInt(event.target.value);
            const marca = marcaConfiguration.find(z => z.IdMarca === marcaId);

            clearSelect(modeloSelect);

            const optionSeleccione = document.createElement('option');
            optionSeleccione.value = "";
            optionSeleccione.textContent = "Selecciona un Modelo";
            modeloSelect.appendChild(optionSeleccione);

            modeloConfiguration = marca.Modelos;

            marca.Modelos.forEach(t => {
                const option = document.createElement('option');
                option.value = t.IdModelo;
                option.textContent = t.DescripcionModeloId;
                modeloSelect.appendChild(option);
            });
        });

        modeloSelect.addEventListener('change', function (event) {
            const IdModelo = parseInt(event.target.value);
            const modelo = modeloConfiguration.find(t => t.IdModelo === IdModelo);
        });
    };

    //var handleSubmitValidation = function (e) {
    //    // Handle form submit
    //    submitButton.addEventListener('click', function (e) {
    //        // Prevent button default action
    //        e.preventDefault();

    //        // Validate form
    //        validator.validate().then(function (status) {
    //            if (status == 'Valid') {
    //                // Disable button to avoid multiple click
    //                submitButton.setAttribute('data-kt-indicator', 'on');
    //                submitButton.disabled = true;
    //                form.submit();
    //            }
    //        });
    //    });
    //}

    const initControls = () => {
        initMarcaAndModelo();
/*        handleSubmitValidation();*/
    };

    // Public functions
    return {
        init: function () {
            form = document.querySelector('#kt_especificaciones_in_form');
            submitButton = document.querySelector('#kt_taller_in_submit');

            handleValidation();
            initData();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTEspecificaciones.init();
});