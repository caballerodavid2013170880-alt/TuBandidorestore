"use strict";
/**
 * @module KTDeposito
 */
var KTDeposito = function () {
    var form;
    var submitButton;
    var validator;

    var selRegion;
    var selPlanta;
    var selZona;

    function modoEdicion() {
        var input = document.querySelector('input[name="IdDeposito"]');
        return !!input && parseInt(input.value, 10) > 0;
    }

    function clearSelect(sel) {
        while (sel.options.length > 0) { sel.remove(0); }
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

    function initCascadeListeners() {
        if (modoEdicion()) return;

        if (!selRegion || !selPlanta || !selZona) return;

        // Región → Planta
        selRegion.addEventListener('change', function () {
            var idRegion = parseInt(this.value, 10);
            resetChild(selPlanta, '-- Seleccione una Planta --');
            resetChild(selZona, '-- Primero seleccione una Planta --');

            if (!idRegion || idRegion === 0) return;

            fetch('/Administrativo/GetPlantasPorRegion?idRegion=' + idRegion)
                .then(r => r.json())
                .then(data => {
                    if (data && data.length > 0) {
                        clearSelect(selPlanta);
                        addOption(selPlanta, '0', '-- Seleccione una Planta --');
                        data.forEach(p => addOption(selPlanta, p.id || p.Id, p.nombre || p.Nombre));
                        selPlanta.disabled = false;
                    }
                    if (validator) validator.revalidateField('IdRegion');
                });
        });

        // Planta → Zona
        selPlanta.addEventListener('change', function () {
            var idRegion = parseInt(selRegion.value, 10);
            var idPlanta = parseInt(this.value, 10);
            resetChild(selZona, '-- Seleccione una Zona --');

            if (!idPlanta || idPlanta === 0 || !idRegion) return;

            fetch('/Administrativo/GetZonasPorPlanta?idRegion=' + idRegion + '&idPlanta=' + idPlanta)
                .then(r => r.json())
                .then(data => {
                    if (data && data.length > 0) {
                        clearSelect(selZona);
                        addOption(selZona, '0', '-- Seleccione una Zona --');
                        data.forEach(z => addOption(selZona, z.id || z.Id, z.nombre || z.Nombre));
                        selZona.disabled = false;
                    }
                    if (validator) validator.revalidateField('IdPlanta');
                });
        });

        // Evento simple zona
        selZona.addEventListener('change', function () {
            if (validator) validator.revalidateField('IdZona');
        });
    }

    var handleValidation = function () {
        var esEdicion = modoEdicion();
        var selectorOk = function (input) {
            return { valid: input.value !== '0' && input.value !== '' };
        };

        var fields = {
            NombreDeposito: { validators: { notEmpty: { message: 'El nombre es requerido' } } },
            Direc: { validators: { notEmpty: { message: 'La dirección es requerida' } } },
            Ciudad: { validators: { notEmpty: { message: 'La ciudad es requerida' } } },
            Respon: { validators: { notEmpty: { message: 'El responsable es requerido' } } },
            Tel: { validators: { notEmpty: { message: 'El teléfono es requerido' } } },
            LocFor: { validators: { notEmpty: { message: 'El campo LocFor es requerido' }, regexp: { regexp: /^[LF]$|^[lf]$/, message: 'Solo se permite L o F' } } },
            RPerson: { validators: { notEmpty: { message: 'RPerson es requerido' } } },
            NomCorto: { validators: { notEmpty: { message: 'El nombre corto es requerido' } } },
            Rfc: { validators: { notEmpty: { message: 'RFC requerido' }, regexp: { regexp: /^([A-Z&Ññ]{3,4})(\d{6})([A-V1-9])([A-Z\d]{1,4})$/, message: 'RFC Inválido' } } },
            Cp: { validators: { notEmpty: { message: 'El CP es requerido' }, regexp: { regexp: /^\d{4,5}$/, message: 'Código postal inválido' } } }
        };

        if (!esEdicion) {
            fields.IdRegion = { validators: { callback: { message: 'Seleccione una Región', callback: selectorOk } } };
            fields.IdPlanta = { validators: { callback: { message: 'Seleccione una Planta', callback: selectorOk } } };
            fields.IdZona = { validators: { callback: { message: 'Seleccione una Zona', callback: selectorOk } } };
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

    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            validator.validate().then(function (status) {
                if (status === 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;

                    // Serializar el formulario
                    var formData = new FormData(form);

                    // Envío por AJAX
                    fetch(form.action, {
                        method: 'POST',
                        body: formData
                    })
                        .then(response => response.json())
                        .then(data => {
                            submitButton.removeAttribute('data-kt-indicator');
                            submitButton.disabled = false;

                            if (data.success) {
                                // Alerta de éxito y redirección
                                Swal.fire({
                                    text: data.message,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: { confirmButton: "btn btn-primary" }
                                }).then(function (result) {
                                    if (result.isConfirmed) {
                                        window.location.href = '/Administrativo/Depositos';
                                    }
                                });
                            } else {
                                // Alerta de error desde el servidor (ModelState, Excepciones, etc)
                                Swal.fire({
                                    text: data.message,
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: { confirmButton: "btn btn-danger" }
                                });
                            }
                        })
                        .catch(error => {
                            submitButton.removeAttribute('data-kt-indicator');
                            submitButton.disabled = false;
                            Swal.fire({
                                text: "Ocurrió un error al procesar la solicitud.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: { confirmButton: "btn btn-danger" }
                            });
                        });
                }
            });
        });
    }

    return {
        init: function () {
            form = document.querySelector('#kt_deposito_in_form');
            submitButton = document.querySelector('#kt_deposito_in_submit');
            if (!form || !submitButton) return;

            selRegion = document.getElementById('selectIdRegion');
            selPlanta = document.getElementById('selectIdPlanta');
            selZona = document.getElementById('selectIdZona');

            initCascadeListeners();
            handleValidation();
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDeposito.init();
});