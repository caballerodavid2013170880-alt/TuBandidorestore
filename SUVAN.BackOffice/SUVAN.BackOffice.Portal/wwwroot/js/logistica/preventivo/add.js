/**
 * Orquestador de lógica dinámica de selectores en cascada y validaciones físicas de campos FormValidation.
 */
"use strict";

var KTPreventivo = function () {
    var form;
    var submitButton;
    var btnGenerar; // botón pGenerar preventivos
    var validator;
    var selPlanta, selDeposito, selMarca, selModelo, selManoObra;

    function clearSelect(sel) { while (sel.options.length > 0) { sel.remove(0); } }
    function addOption(sel, value, text) {
        var opt = document.createElement('option');
        opt.value = value;
        opt.textContent = text;
        sel.appendChild(opt);
    }
    function resetChild(sel, placeholder) {
        if (!sel) return;
        clearSelect(sel);
        addOption(sel, '0', placeholder);
        sel.disabled = true;
    }

    function initCascadeListeners() {
        if (selPlanta && selDeposito) {
            selPlanta.addEventListener('change', function () {
                var idPlanta = parseInt(this.value, 10);
                resetChild(selDeposito, '-- Seleccione un Depósito --');

                if (!idPlanta || idPlanta === 0) {
                    if (validator) { try { validator.revalidateField('IdDeposito'); } catch (e) { } }
                    return;
                }

                selDeposito.options[0].text = "Cargando depósitos...";

                // USO DE LA VARIABLE GLOBAL CREADA EN RAZOR
                fetch(window.SUVAN.Urls.GetDepositos + '?idPlanta=' + idPlanta)
                    .then(function (response) {
                        if (!response.ok) throw new Error('Error status: ' + response.status);
                        return response.json();
                    })
                    .then(function (data) {
                        if (data && data.length > 0) {
                            clearSelect(selDeposito);
                            addOption(selDeposito, '0', '-- Seleccione un Depósito --');
                            data.forEach(function (d) { addOption(selDeposito, d.idDeposito || d.IdDeposito, d.nombre || d.Nombre); });
                            selDeposito.disabled = false;
                        } else {
                            resetChild(selDeposito, '-- Sin depósitos para esta Planta --');
                        }
                        if (validator) { try { validator.revalidateField('IdDeposito'); } catch (e) { } }
                    })
                    .catch(function (error) {
                        console.error('Error:', error);
                        resetChild(selDeposito, '-- Error al cargar --');
                    });
            });
        }

        if (selMarca && selModelo) {
            selMarca.addEventListener('change', function () {
                var idMarca = parseInt(this.value, 10);
                resetChild(selModelo, '-- Seleccione un Modelo --');

                if (!idMarca || idMarca === 0) {
                    if (validator) { try { validator.revalidateField('IdModelo'); } catch (e) { } }
                    return;
                }

                selModelo.options[0].text = "Cargando modelos...";

                // USO DE LA VARIABLE GLOBAL CREADA EN RAZOR
                fetch(window.SUVAN.Urls.GetModelos + '?idMarca=' + idMarca)
                    .then(function (response) {
                        if (!response.ok) throw new Error('Error status: ' + response.status);
                        return response.json();
                    })
                    .then(function (data) {
                        if (data && data.length > 0) {
                            clearSelect(selModelo);
                            addOption(selModelo, '0', '-- Seleccione un Modelo --');
                            data.forEach(function (m) { addOption(selModelo, m.idModelo || m.IdModelo, m.nombre || m.Nombre); });
                            selModelo.disabled = false;
                        } else {
                            resetChild(selModelo, '-- Sin modelos para esta Marca --');
                        }
                        if (validator) { try { validator.revalidateField('IdModelo'); } catch (e) { } }
                    })
                    .catch(function (error) {
                        console.error('Error:', error);
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

    function initGenerarPreventivos() {
        if (!btnGenerar) return;

        btnGenerar.addEventListener('click', function (e) {
            e.preventDefault();

            var idPreventivo = parseInt(document.querySelector('input[name="Idpreventivo"]').value, 10);
            var idManoObra = parseInt(selManoObra.value, 10);

            if (!idManoObra || idManoObra === 0) {
                Swal.fire({
                    text: "Por favor, seleccione una Mano de Obra antes de generar los preventivos.",
                    icon: "warning",
                    buttonsStyling: false,
                    confirmButtonText: "Entendido",
                    customClass: { confirmButton: "btn btn-primary" }
                });
                return;
            }

            Swal.fire({
                text: "Se generarán los preventivos de este plan preventivo, ¿Desea continuar con la creación?",
                icon: "question",
                showCancelButton: true,
                buttonsStyling: false,
                confirmButtonText: "Generar preventivos",
                cancelButtonText: "Cancelar",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary",
                    cancelButton: "btn fw-bold btn-active-light-primary"
                }
            }).then(function (result) {
                if (result.value) {
                    // Animación de carga en el botón
                    btnGenerar.setAttribute('data-kt-indicator', 'on');
                    btnGenerar.disabled = true;

                    fetch(window.SUVAN.Urls.GenerarPreventivos, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            IdPreventivo: idPreventivo,
                            IdManoObra: idManoObra
                        })
                    })
                        .then(function (response) { return response.json(); })
                        .then(function (data) {
                            btnGenerar.removeAttribute('data-kt-indicator');
                            btnGenerar.disabled = false;

                            if (data.success) {
                                Swal.fire({
                                    text: data.message,
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: { confirmButton: "btn btn-primary" }
                                });
                            } else {
                                Swal.fire({
                                    text: data.message,
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: { confirmButton: "btn btn-danger" }
                                });
                            }
                        })
                        .catch(function (error) {
                            btnGenerar.removeAttribute('data-kt-indicator');
                            btnGenerar.disabled = false;
                            Swal.fire({
                                text: "Error de red al intentar generar los preventivos.",
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
            form = document.querySelector('#kt_preventivo_in_form');
            submitButton = document.querySelector('#kt_preventivo_in_submit');
            btnGenerar = document.querySelector('#btn_generar_preventivos'); // Inicializar botón

            if (!form || !submitButton) return;

            selPlanta = document.getElementById('selectIdPlanta');
            selDeposito = document.getElementById('selectIdDeposito');
            selMarca = document.getElementById('selectIdMarca');
            selModelo = document.getElementById('selectIdModelo');
            selManoObra = document.getElementById('selectIdManoObra');

            initCascadeListeners();
            initValidation();
            initSubmit();
            initGenerarPreventivos();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTPreventivo.init();
});