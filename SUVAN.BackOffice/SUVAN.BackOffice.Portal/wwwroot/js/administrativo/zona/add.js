"use strict";

var KTZona = function () {
    var form;
    var submitButton;
    var validator;
    var selRegion;
    var selPlanta;

    function modoEdicion() {
        var input = document.getElementById('IdZona') || document.querySelector('input[name="IdZona"]');
        return !!input && parseInt(input.value, 10) > 0;
    }

    function clearSelect(sel) {
        while (sel.options.length > 0) sel.remove(0);
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

    var initCascadeListeners = function () {
        if (modoEdicion()) return;

        selRegion.addEventListener('change', function () {
            var idRegion = parseInt(this.value, 10);
            resetChild(selPlanta, '-- Seleccione una Planta --');

            if (!idRegion || idRegion === 0) return;

            fetch('/Administrativo/GetPlantasPorRegion?idRegion=' + idRegion)
                .then(response => response.json())
                .then(data => {
                    if (data && data.length > 0) {
                        clearSelect(selPlanta);
                        addOption(selPlanta, '0', '-- Seleccione una Planta --');
                        data.forEach(p => addOption(selPlanta, p.id || p.Id, p.nombre || p.Nombre));
                        selPlanta.disabled = false;
                    } else {
                        resetChild(selPlanta, '-- Sin plantas registradas --');
                    }
                    if (validator) validator.revalidateField('IdRegion');
                })
                .catch(error => console.error('Error cargando plantas:', error));
        });
    };

    var handleValidation = function () {
        var esEdicion = modoEdicion();
        var selectorOk = function (input) {
            return { valid: input.value !== '0' && input.value !== '' };
        };

        var fields = {
            ZonaNombre: {
                validators: {
                    notEmpty: { message: 'Nombre de la Zona requerido' },
                    stringLength: { min: 3, max: 80, message: 'Entre 3 y 80 caracteres' }
                }
            },
            Domicilio: {
                validators: {
                    notEmpty: { message: 'Domicilio requerido' },
                    stringLength: { min: 5, max: 250, message: 'Entre 5 y 250 caracteres' }
                }
            },
            Rfc: {
                validators: {
                    notEmpty: { message: 'RFC requerido' },
                    regexp: { regexp: /^([A-Z&Ññ]{3,4})(\d{6})([A-V1-9])([A-Z\d]{1,4})$/, message: 'El RFC no es válido' }
                }
            },
            Responsable: {
                validators: {
                    notEmpty: { message: 'Nombre del Responsable requerido' },
                    stringLength: { min: 3, max: 60, message: 'Entre 3 y 60 caracteres' }
                }
            },
            FechaApertura: {
                validators: { notEmpty: { message: 'Fecha de Apertura es requerida' } }
            },
            Telefono1: {
                validators: {
                    notEmpty: { message: 'Número telefónico requerido' },
                    regexp: { regexp: /^[0-9]{10}$/, message: 'Debe tener 10 dígitos' }
                }
            }
        };

        if (!esEdicion) {
            fields.IdRegion = { validators: { callback: { message: 'Seleccione una Región', callback: selectorOk } } };
            fields.IdPlanta = { validators: { callback: { message: 'Seleccione una Planta', callback: selectorOk } } };
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
    };

    var handleControls = function () {
        var elementoFecha = $("#FechaApertura");
        var fechaApertura = elementoFecha.val();

        if (!fechaApertura || fechaApertura === "01/01/0001") {
            fechaApertura = moment().format("DD/MM/YYYY");
        }

        elementoFecha.daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: false,
            locale: {
                format: "DD/MM/YYYY",
                applyLabel: "Aceptar",
                cancelLabel: "Cancelar",
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"]
            },
            startDate: moment(fechaApertura, "DD/MM/YYYY")
        });

        elementoFecha.on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('DD/MM/YYYY'));
            if (validator) validator.revalidateField('FechaApertura');
        });
    };

    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            validator.validate().then(function (status) {
                if (status === 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;

                    // Empaquetar datos del formulario
                    var formData = new FormData(form);

                    // Asegurar que los selects en modo "disabled" (Edición) se envíen al controlador
                    var selReg = document.getElementById('selectIdRegion');
                    var selPla = document.getElementById('selectIdPlanta');
                    if (selReg && selReg.disabled) formData.set('IdRegion', selReg.value);
                    if (selPla && selPla.disabled) formData.set('IdPlanta', selPla.value);

                    // Petición AJAX nativa
                    fetch(form.action, {
                        method: 'POST',
                        body: formData
                    })
                        .then(response => response.json())
                        .then(data => {
                            submitButton.removeAttribute('data-kt-indicator');
                            submitButton.disabled = false;

                            if (data.success) {
                                Swal.fire({
                                    text: data.message,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: { confirmButton: "btn btn-primary" }
                                }).then(function () {
                                    // Redirigir al listado al terminar
                                    window.location.href = '/Administrativo/Zonas';
                                });
                            } else {
                                // Mostrar el error (Ej. "Ya existe una Zona con el mismo RFC...")
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
                            console.error("Error en la petición:", error);
                            submitButton.removeAttribute('data-kt-indicator');
                            submitButton.disabled = false;
                            Swal.fire({
                                text: "Ocurrió un error de conexión al guardar.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: { confirmButton: "btn btn-danger" }
                            });
                        });
                }
            });
        });
    };

    return {
        init: function () {
            form = document.querySelector('#kt_zona_in_form');
            submitButton = document.querySelector('#kt_zona_in_submit');
            if (!form || !submitButton) return;

            selRegion = document.getElementById('selectIdRegion');
            selPlanta = document.getElementById('selectIdPlanta');

            document.querySelector("#Telefono1").addEventListener("input", function () { this.value = this.value.replace(/[^0-9]/g, ''); });
            var tel2 = document.querySelector("#Telefono2");
            if (tel2) tel2.addEventListener("input", function () { this.value = this.value.replace(/[^0-9]/g, ''); });

            initCascadeListeners();
            handleValidation();
            handleControls();
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTZona.init();
});