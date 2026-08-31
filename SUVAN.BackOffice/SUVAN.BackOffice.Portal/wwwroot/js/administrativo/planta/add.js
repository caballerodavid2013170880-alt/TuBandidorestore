"use strict";
/**
 * Módulo principal para el formulario de Planta.
 * @namespace KTPlantum
 */
var KTPlantum = function () {
    /** @type {HTMLElement} Referencia al formulario */
    var form;
    /** @type {HTMLElement} Botón de envío */
    var submitButton;
    /** @type {object} Instancia del validador FormValidation */
    var validator;
    /**
     * Inicializa las reglas de validación del formulario con FormValidation.
     * Valida los campos NombrePlanta e IdRegion (solo en modo alta).
     * @private
     */
    var handleValidation = function () {
        // Detectar si es modo edición (el select de región está deshabilitado)
        var isEdicion = document.querySelector('input[name="IdPlanta"]') &&
            parseInt(document.querySelector('input[name="IdPlanta"]').value) > 0;
        var fields = {
            'NombrePlanta': {
                validators: {
                    notEmpty: {
                        message: 'El nombre de la Planta es requerido'
                    },
                    stringLength: {
                        min: 3,
                        max: 45,
                        message: 'El nombre debe tener entre 3 y 45 caracteres'
                    }
                }
            }
        };
        // En modo alta, el selector de Región es requerido
        if (!isEdicion) {
            fields['IdRegion'] = {
                validators: {
                    notEmpty: {
                        message: 'La Región es requerida'
                    },
                    callback: {
                        message: 'Seleccione una Región válida',
                        /**
                         * Valida que el valor del selector no sea la opción vacía (0).
                         * @param {object} input - Contexto del validador.
                         * @returns {{ valid: boolean }} Resultado de la validación.
                         */
                        callback: function (input) {
                            return { valid: input.value !== '0' && input.value !== '' };
                        }
                    }
                }
            };
        }
        validator = FormValidation.formValidation(form, {
            fields: fields,
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap5({
                    rowSelector: '.fv-row',
                    eleInvalidClass: '',   // Desactivar ícono de estado inválido
                    eleValidClass: ''      // Desactivar ícono de estado válido
                })
            }
        });
    };
    /**
     * Enlaza el evento click del botón de envío para ejecutar la validación
     * antes de enviar el formulario. Muestra el indicador de progreso al enviar.
     * @private
     */
    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            // Prevenir el envío por defecto hasta que la validación sea exitosa
            e.preventDefault();
            validator.validate().then(async function (status) {
                if (status === 'Valid') {
                    // Deshabilitar el botón para evitar múltiples envíos
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;

                    try {
                        //recopila los datos del formulario 
                        const formData = new FormData(form);

                        const response = await fetch(form.action, {
                            method: 'POST',
                            body: formData
                        });
                        const result = await response.json();

                        //quitar indicador de carga
                        submitButton.removeAttribute('data-kt-indicator');
                        submitButton.disabled = false;

                        if (result.success) {
                            Swal.fire({
                                text: result.message,
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            }).then(function () {
                                // Redirigir a la página de listado de regiones
                                window.location.href = '/Administrativo/Plantas';
                            });
                        } else {
                            Swal.fire({
                                text: result.message || "Ocurrió un error al guardar la región.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            });
                        }
                    } catch (error) {
                        //quitar indicador en caso de error en la red
                        submitButton.removeAttribute('data-kt-indicator');
                        submitButton.disabled = false;

                        Swal.fire({
                            text: "Error de conexión con el servidor",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary"
                            }
                        });
                    }

                }
            });
        });
    };
    // API pública
    return {
        /**
         * Punto de entrada: inicializa el formulario, la validación y el evento de envío.
         */
        init: function () {
            form = document.querySelector('#kt_planta_in_form');
            submitButton = document.querySelector('#kt_planta_in_submit');
            if (!form || !submitButton) return;
            handleValidation();
            handleSubmitValidation();
        }
    };
}();
// Ejecutar al cargar el DOM
KTUtil.onDOMContentLoaded(function () {
    KTPlantum.init();
});
