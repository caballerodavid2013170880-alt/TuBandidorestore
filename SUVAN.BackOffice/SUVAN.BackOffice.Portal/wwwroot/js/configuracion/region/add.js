"use strict";
/**
 * Módulo principal para el formulario de Región.
 * @namespace KTRegion
 */
var KTRegion = function () {
    /** @type {HTMLElement} Referencia al formulario */
    var form;
    /** @type {HTMLElement} Botón de envío */
    var submitButton;
    /** @type {object} Instancia del validador FormValidation */
    var validator;
    /**
     * Inicializa las reglas de validación del formulario con FormValidation.
     * Valida el campo NombreRegion con longitud mínima de 3 y máxima de 250 caracteres.
     * @private
     */
    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    /**
                     * Campo NombreRegion: requerido y con longitud entre 3 y 250 caracteres.
                     * Mapeado al input asp-for="NombreRegion" en la vista.
                     */
                    'NombreRegion': {
                        validators: {
                            notEmpty: {
                                message: 'El nombre de la Región es requerido'
                            },
                            stringLength: {
                                min: 3,
                                max: 250,
                                message: 'El nombre debe tener entre 3 y 250 caracteres'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',  // Desactivar ícono de estado inválido
                        eleValidClass: ''     // Desactivar ícono de estado válido
                    })
                }
            }
        );
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
            validator.validate().then(function (status) {
                if (status === 'Valid') {
                    // Deshabilitar el botón para evitar múltiples envíos
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;
                    form.submit();
                }
            });
        });
    };
    // API pública
    return {
        /**
         * Punto de entrada: inicializa el formulario, la validación y el evento de envío.
         * Se espera que el DOM contenga los elementos #kt_region_in_form y #kt_region_in_submit.
         */
        init: function () {
            form = document.querySelector('#kt_region_in_form');
            submitButton = document.querySelector('#kt_region_in_submit');
            if (!form || !submitButton) return;
            handleValidation();
            handleSubmitValidation();
        }
    };
}();
// Ejecutar al cargar el DOM
KTUtil.onDOMContentLoaded(function () {
    KTRegion.init();
});