"use strict";
/**
 * @fileoverview Módulo del formulario de alta/edición de Departamento.
 * Cascada de selectores Región → Planta → Zona → Depósito usando datos
 * JSON embebidos en la página (patrón del módulo Taller).
 * @module KTDepto
 */
var KTDepto = function () {
    var form;
    var submitButton;
    var validator;

    var selRegion;
    var selPlanta;
    var selZona;
    var selDeposito;

    /** Datos jerárquicos parseados del hidden input CascadeJson */
    var cascadeData = [];

    /** Referencia al objeto región actualmente seleccionado */
    var currentRegion = null;
    /** Referencia al objeto planta actualmente seleccionado */
    var currentPlanta = null;
    /** Referencia al objeto zona actualmente seleccionado */
    var currentZona = null;

    // ── Helpers ───────────────────────────────────────────────────────
    function modoEdicion() {
        var input = document.getElementById('IdDepto') ||
            document.querySelector('input[name="IdDepto"]');
        return !!input && parseInt(input.value, 10) > 0;
    }

    function clearSelect(sel) {
        while (sel.options.length > 0) {
            sel.remove(0);
        }
    }

    function addOption(sel, value, text) {
        var opt = document.createElement('option');
        opt.value = value;
        opt.textContent = text;
        sel.appendChild(opt);
    }

    function resetChild(sel, placeholder) {
        clearSelect(sel);
        addOption(sel, '0', placeholder);
        sel.disabled = true;
    }

    // ── Cascada ───────────────────────────────────────────────────────

    function initCascadeListeners() {
        if (modoEdicion()) {
            console.log('[KTDepto] Modo edición – cascada desactivada.');
            return;
        }

        if (!selRegion || !selPlanta || !selZona || !selDeposito) {
            console.warn('[KTDepto] Faltan selectores en el DOM.');
            return;
        }

        // Región → Planta
        selRegion.addEventListener('change', function () {
            var idRegion = parseInt(this.value, 10);

            resetChild(selPlanta, '-- Seleccione una Planta --');
            resetChild(selZona, '-- Primero seleccione una Planta --');
            resetChild(selDeposito, '-- Primero seleccione una Zona --');

            if (!idRegion || idRegion === 0) return;

            fetch('/Configuracion/GetPlantasPorRegion?idRegion=' + idRegion)
                .then(function (response) { return response.json(); })
                .then(function (data) {
                    if (data && data.length > 0) {
                        clearSelect(selPlanta);
                        addOption(selPlanta, '0', '-- Seleccione una Planta --');
                        data.forEach(function (p) {
                            addOption(selPlanta, p.idPlanta || p.IdPlanta, p.nombre || p.Nombre);
                        });
                        selPlanta.disabled = false;
                    } else {
                        resetChild(selPlanta, '-- Sin plantas para esta Región --');
                    }
                    if (validator) { try { validator.revalidateField('IdRegion'); } catch (e) { } }
                })
                .catch(function (error) { console.error('Error obteniendo plantas:', error); });
        });

        // Planta → Zona
        selPlanta.addEventListener('change', function () {
            var idRegion = parseInt(selRegion.value, 10);
            var idPlanta = parseInt(this.value, 10);

            resetChild(selZona, '-- Seleccione una Zona --');
            resetChild(selDeposito, '-- Primero seleccione una Zona --');

            if (!idPlanta || idPlanta === 0 || !idRegion) return;

            fetch('/Configuracion/GetZonasPorPlanta?idRegion=' + idRegion + '&idPlanta=' + idPlanta)
                .then(function (response) { return response.json(); })
                .then(function (data) {
                    if (data && data.length > 0) {
                        clearSelect(selZona);
                        addOption(selZona, '0', '-- Seleccione una Zona --');
                        data.forEach(function (z) {
                            addOption(selZona, z.idZona || z.IdZona, z.nombre || z.Nombre);
                        });
                        selZona.disabled = false;
                    } else {
                        resetChild(selZona, '-- Sin zonas para esta Planta --');
                    }
                    if (validator) { try { validator.revalidateField('IdPlanta'); } catch (e) { } }
                })
                .catch(function (error) { console.error('Error obteniendo zonas:', error); });
        });

        // Zona → Depósito
        selZona.addEventListener('change', function () {
            var idRegion = parseInt(selRegion.value, 10);
            var idPlanta = parseInt(selPlanta.value, 10);
            var idZona = parseInt(this.value, 10);

            resetChild(selDeposito, '-- Seleccione un Depósito --');

            if (!idZona || idZona === 0 || !idPlanta) return;

            fetch('/Configuracion/GetDepositosPorZona?idRegion=' + idRegion + '&idPlanta=' + idPlanta + '&idZona=' + idZona)
                .then(function (response) { return response.json(); })
                .then(function (data) {
                    if (data && data.length > 0) {
                        clearSelect(selDeposito);
                        addOption(selDeposito, '0', '-- Seleccione un Depósito --');
                        data.forEach(function (d) {
                            addOption(selDeposito, d.idDeposito || d.IdDeposito, d.nombre || d.Nombre);
                        });
                        selDeposito.disabled = false;
                    } else {
                        resetChild(selDeposito, '-- Sin depósitos para esta Zona --');
                    }
                    if (validator) { try { validator.revalidateField('IdZona'); } catch (e) { } }
                })
                .catch(function (error) { console.error('Error obteniendo depósitos:', error); });
        });

        console.log('[KTDepto] Cascada AJAX inicializada correctamente.');
    }

    // ── Validación ────────────────────────────────────────────────────
    function initValidation() {
        var esEdicion = modoEdicion();

        var selectorOk = function (input) {
            return { valid: input.value !== '0' && input.value !== '' };
        };

        var fields = {
            NombreDepto: {
                validators: {
                    notEmpty: { message: 'El nombre del Departamento es requerido' },
                    stringLength: { min: 3, max: 70, message: 'Entre 3 y 70 caracteres' }
                }
            },
            Responsable: {
                validators: {
                    notEmpty: { message: 'El Responsable es requerido' },
                    stringLength: { min: 3, max: 70, message: 'Entre 3 y 70 caracteres' }
                }
            }
        };

        if (!esEdicion) {
            fields.IdRegion = { validators: { callback: { message: 'Seleccione una Región', callback: selectorOk } } };
            fields.IdPlanta = { validators: { callback: { message: 'Seleccione una Planta', callback: selectorOk } } };
            fields.IdZona = { validators: { callback: { message: 'Seleccione una Zona', callback: selectorOk } } };
            fields.IdDeposito = { validators: { callback: { message: 'Seleccione un Depósito', callback: selectorOk } } };
        }

        validator = FormValidation.formValidation(form, {
            fields: fields,
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

    // ── API pública ──────────────────────────────────────────────────

    return {
        init: function () {
            form = document.querySelector('#kt_depto_in_form');
            submitButton = document.querySelector('#kt_depto_in_submit');
            if (!form || !submitButton) return;

            selRegion = document.getElementById('selectIdRegion');
            selPlanta = document.getElementById('selectIdPlanta');
            selZona = document.getElementById('selectIdZona');
            selDeposito = document.getElementById('selectIdDeposito');

            // Se elimina initCascadeData() para procesarlo completamente por los Endpoints AJAX ya definidos
            initCascadeListeners();
            initValidation();
            initSubmit();

            console.log('[KTDepto] Módulo inicializado.');
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDepto.init();
});