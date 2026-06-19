/**
 * Orquestador de lógica dinámica de selectores en cascada y validaciones físicas de campos FormValidation.
 */
"use strict";

var KTPreventivo = function () {
    var form;
    var submitButton;
    var validator;

    var selPlanta;
    var selDeposito;
    var selMarca;
    var selModelo;

    // Función auxiliar para vaciar el selector
    function clearSelect(sel) {
        while (sel.options.length > 0) { sel.remove(0); }
    }

    // Función auxiliar para agregar opciones al selector
    function addOption(sel, value, text) {
        var opt = document.createElement('option');
        opt.value = value;
        opt.textContent = text;
        sel.appendChild(opt);
    }

    // Reinicia el selector hijo a su estado inhabilitado por defecto
    function resetChild(sel, placeholder) {
        if (!sel) return;
        clearSelect(sel);
        addOption(sel, '0', placeholder);
        sel.disabled = true;
    }

    function initCascadeListeners() {
        // Cascada: Planta -> Depósito
        if (selPlanta && selDeposito) {
            selPlanta.addEventListener('change', function () {
                var idPlanta = parseInt(this.value, 10);

                resetChild(selDeposito, '-- Seleccione un Depósito --');

                if (!idPlanta || idPlanta === 0) {
                    if (validator) {
                        try { validator.revalidateField('IdDeposito'); } catch (e) { }
                    }
                    return;
                }

                selDeposito.options[0].text = "Cargando depósitos...";

                // La ruta base debe coincidir exactamente con el atributo [Route] del controlador
                fetch('/MantenimientoPreventivo/GetDepositosPorPlanta?idPlanta=' + idPlanta)
                    .then(function (response) {
                        if (!response.ok) throw new Error('Error de red obteniendo depósitos. Status: ' + response.status);
                        return response.json();
                    })
                    .then(function (data) {
                        if (data && data.length > 0) {
                            clearSelect(selDeposito);
                            addOption(selDeposito, '0', '-- Seleccione un Depósito --');
                            data.forEach(function (d) {
                                addOption(selDeposito, d.idDeposito || d.IdDeposito, d.nombre || d.Nombre);
                            });
                            selDeposito.disabled = false;
                        } else {
                            resetChild(selDeposito, '-- Sin depósitos para esta Planta --');
                        }

                        if (validator) {
                            try { validator.revalidateField('IdDeposito'); } catch (e) { }
                        }
                    })
                    .catch(function (error) {
                        console.error('Error en cascada Depósitos:', error);
                        resetChild(selDeposito, '-- Error al cargar --');
                    });
            });
        }

        // Cascada: Marca -> Modelo
        if (selMarca && selModelo) {
            selMarca.addEventListener('change', function () {
                var idMarca = parseInt(this.value, 10);

                resetChild(selModelo, '-- Seleccione un Modelo --');

                if (!idMarca || idMarca === 0) {
                    if (validator) {
                        try { validator.revalidateField('IdModelo'); } catch (e) { }
                    }
                    return;
                }

                selModelo.options[0].text = "Cargando modelos...";

                fetch('/MantenimientoPreventivo/GetModelosPorMarca?idMarca=' + idMarca)
                    .then(function (response) {
                        if (!response.ok) throw new Error('Error de red obteniendo modelos. Status: ' + response.status);
                        return response.json();
                    })
                    .then(function (data) {
                        if (data && data.length > 0) {
                            clearSelect(selModelo);
                            addOption(selModelo, '0', '-- Seleccione un Modelo --');
                            data.forEach(function (m) {
                                addOption(selModelo, m.idModelo || m.IdModelo, m.nombre || m.Nombre);
                            });
                            selModelo.disabled = false;
                        } else {
                            resetChild(selModelo, '-- Sin modelos para esta Marca --');
                        }

                        if (validator) {
                            try { validator.revalidateField('IdModelo'); } catch (e) { }
                        }
                    })
                    .catch(function (error) {
                        console.error('Error en cascada Modelos:', error);
                        resetChild(selModelo, '-- Error al cargar --');
                    });
            });
        }
    }

    function initValidation() {
        var selectorOk = function (input) { return { valid: input.value !== '0' && input.value !== '' }; };

        validator = FormValidation.formValidation(form, {
            fields: {
                NombrePreventivo: {
                    validators: {
                        notEmpty: { message: 'El nombre es requerido' },
                        stringLength: { min: 3, max: 70, message: 'Entre 3 y 70 caracteres' }
                    }
                },
                Meses: {
                    validators: {
                        notEmpty: { message: 'Los meses son requeridos' }
                    }
                },
                IdPlanta: { validators: { callback: { message: 'Seleccione una Planta', callback: selectorOk } } },
                IdDeposito: { validators: { callback: { message: 'Seleccione un Depósito', callback: selectorOk } } },
                IdMarca: { validators: { callback: { message: 'Seleccione una Marca', callback: selectorOk } } },
                IdModelo: { validators: { callback: { message: 'Seleccione un Modelo', callback: selectorOk } } },
                IdManoObra: { validators: { callback: { message: 'Seleccione Mano de Obra', callback: selectorOk } } }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row',
                    eleInvalidClass: '',
                    eleValidClass: ''
                })
            }
        });
    }

    function initSubmit() {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            validator.validate().then(function (status) {
                if (status === 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;
                    form.submit();
                }
            });
        });
    }

    return {
        init: function () {
            form = document.querySelector('#kt_preventivo_in_form');
            submitButton = document.querySelector('#kt_preventivo_in_submit');

            if (!form || !submitButton) return;

            selPlanta = document.getElementById('selectIdPlanta');
            selDeposito = document.getElementById('selectIdDeposito');
            selMarca = document.getElementById('selectIdMarca');
            selModelo = document.getElementById('selectIdModelo');

            initCascadeListeners();
            initValidation();
            initSubmit();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTPreventivo.init();
});