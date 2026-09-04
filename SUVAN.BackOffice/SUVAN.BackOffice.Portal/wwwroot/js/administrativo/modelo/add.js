"use strict";

// Definición de la clase
var KTModelo = function () {
    // Elementos
    var form;
    var submitButton;
    var validator;
    var ejesContainer;
    var addEjeButton;
    var ejesValidation;
    var tiposEje = (window.SUVAN && window.SUVAN.Modelo && window.SUVAN.Modelo.tiposEje) || [];

    // Función para validar el formulario
    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'IdMarca': {
                        validators: {
                            notEmpty: {
                                message: 'Marca requerida',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'IdTipoV': {
                        validators: {
                            notEmpty: {
                                message: 'Tipo de Vehículo requerido',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'AnioDesde': {
                        validators: {
                            notEmpty: {
                                message: 'Año Desde requerido'
                            },
                            regexp: {
                                regexp: /^\d{4}$/,
                                message: 'El año debe tener 4 dígitos'
                            }
                        }
                    },
                    'AnioHasta': {
                        validators: {
                            notEmpty: {
                                message: 'Año Desde requerido'
                            },
                            regexp: {
                                regexp: /^\d{4}$/,
                                message: 'El año debe tener 4 dígitos'
                            }
                        }
                    },
                    'Descripcion': {
                        validators: {
                            notEmpty: {
                                message: 'Descripción requerida'
                            },
                            stringLength: {
                                min: 7,
                                max: 60,

                                message: 'deben tener entre 7 y 60 caracteres',
                            },
                        }
                    },
                    'KmGarantia': {
                        validators: {
                            notEmpty: {
                                message: 'Kilómetros de Garantía requerido'
                            }
                        }
                    },
                    'MesGarantia': {
                        validators: {
                            notEmpty: {
                                message: 'Meses de Garantía requerida'
                            }
                        }
                    },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
    };

    // Manejo del envío del formulario
    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault(); // Evita el envío por defecto
            renumberEjes();

            // Validar el formulario
            validator.validate().then(function (status) {
                if (status == 'Valid' && validateEjes()) {
                    submitButton.setAttribute('data-kt-indicator', 'on'); // Indicador de carga
                    submitButton.disabled = true; // Evita múltiples envíos
                    form.submit(); // Envía el formulario
                }
            });
        });
    };

    var getTipoEjeValue = function (tipoEje, field) {
        var camelCaseField = field.charAt(0).toLowerCase() + field.slice(1);
        return tipoEje[field] !== undefined ? tipoEje[field] : tipoEje[camelCaseField];
    };

    var getEjeRows = function () {
        return Array.from(ejesContainer.querySelectorAll('[data-modelo-eje-row]'));
    };

    var updatePosiciones = function (select) {
        var row = select.closest('[data-modelo-eje-row]');
        var selectedOption = select.options[select.selectedIndex];
        var posiciones = selectedOption ? selectedOption.getAttribute('data-posiciones') : '';
        var posicionesLabel = row.querySelector('[data-posiciones-label]');
        var posicionesInput = row.querySelector('[data-field="numeroPosiciones"]');

        posicionesLabel.textContent = posiciones ? `${posiciones} posiciones` : '-';
        posicionesInput.value = posiciones || '';
    };

    var renumberEjes = function () {
        getEjeRows().forEach(function (row, index) {
            var numeroEje = index + 1;

            row.querySelector('[data-eje-label]').textContent = `Eje ${numeroEje}`;
            row.querySelector('[data-field="idModeloEje"]').setAttribute('name', `Ejes[${index}].IdModeloEje`);
            row.querySelector('[data-field="idTipoEje"]').setAttribute('name', `Ejes[${index}].IdTipoEje`);
            row.querySelector('[data-field="numeroEje"]').setAttribute('name', `Ejes[${index}].NumeroEje`);
            row.querySelector('[data-field="numeroEje"]').value = numeroEje;
            row.querySelector('[data-field="numeroPosiciones"]').setAttribute('name', `Ejes[${index}].NumeroPosiciones`);
        });
    };

    var validateEjes = function () {
        var rows = getEjeRows();
        ejesValidation.textContent = '';

        if (rows.length === 0) {
            ejesValidation.textContent = 'Debe configurar al menos un eje';
            return false;
        }

        var ejeSinTipo = rows.some(function (row) {
            return !row.querySelector('[data-field="idTipoEje"]').value;
        });

        if (ejeSinTipo) {
            ejesValidation.textContent = 'Todos los ejes deben tener un tipo de eje seleccionado';
            return false;
        }

        return true;
    };

    var bindEjeRow = function (row) {
        row.querySelector('[data-field="idTipoEje"]').addEventListener('change', function () {
            updatePosiciones(this);
            validateEjes();
        });

        row.querySelector('[data-eje-remove]').addEventListener('click', function () {
            row.remove();
            renumberEjes();
            validateEjes();
        });
    };

    var createEjeRow = function () {
        var row = document.createElement('div');
        row.className = 'row align-items-end modelo-eje-row mb-4';
        row.setAttribute('data-modelo-eje-row', '');

        var idModeloEje = document.createElement('input');
        idModeloEje.type = 'hidden';
        idModeloEje.value = '0';
        idModeloEje.setAttribute('data-field', 'idModeloEje');

        var numeroEje = document.createElement('input');
        numeroEje.type = 'hidden';
        numeroEje.setAttribute('data-field', 'numeroEje');

        var numeroPosiciones = document.createElement('input');
        numeroPosiciones.type = 'hidden';
        numeroPosiciones.setAttribute('data-field', 'numeroPosiciones');

        var ejeCol = document.createElement('div');
        ejeCol.className = 'col-sm-2';
        ejeCol.innerHTML = '<label class="col-form-label text-gray-600">Eje</label><div class="form-control bg-light text-muted" data-eje-label></div>';

        var tipoCol = document.createElement('div');
        tipoCol.className = 'col-sm-5';

        var tipoLabel = document.createElement('label');
        tipoLabel.className = 'col-form-label text-gray-600 required';
        tipoLabel.textContent = 'Tipo de eje';

        var tipoSelect = document.createElement('select');
        tipoSelect.className = 'form-control';
        tipoSelect.setAttribute('data-field', 'idTipoEje');

        var emptyOption = document.createElement('option');
        emptyOption.value = '';
        emptyOption.textContent = 'Selecciona un Tipo de Eje';
        tipoSelect.appendChild(emptyOption);

        tiposEje.forEach(function (tipoEje) {
            var option = document.createElement('option');
            var idTipoEje = getTipoEjeValue(tipoEje, 'IdTipoEje');
            var nombre = getTipoEjeValue(tipoEje, 'Nombre');
            var posiciones = getTipoEjeValue(tipoEje, 'NumeroPosiciones');

            option.value = idTipoEje;
            option.setAttribute('data-posiciones', posiciones);
            option.textContent = `${idTipoEje} - ${nombre}`;
            tipoSelect.appendChild(option);
        });

        tipoCol.appendChild(tipoLabel);
        tipoCol.appendChild(tipoSelect);

        var posicionesCol = document.createElement('div');
        posicionesCol.className = 'col-sm-3';
        posicionesCol.innerHTML = '<label class="col-form-label text-gray-600">Posiciones</label><div class="form-control bg-light text-muted" data-posiciones-label>-</div>';

        var removeCol = document.createElement('div');
        removeCol.className = 'col-sm-2 text-end';

        var removeButton = document.createElement('button');
        removeButton.type = 'button';
        removeButton.className = 'btn btn-light-danger btn-sm';
        removeButton.setAttribute('data-eje-remove', '');
        removeButton.textContent = 'Eliminar';
        removeCol.appendChild(removeButton);

        row.appendChild(idModeloEje);
        row.appendChild(numeroEje);
        row.appendChild(ejeCol);
        row.appendChild(tipoCol);
        row.appendChild(posicionesCol);
        row.appendChild(numeroPosiciones);
        row.appendChild(removeCol);

        ejesContainer.appendChild(row);
        bindEjeRow(row);
        renumberEjes();
    };

    var handleEjes = function () {
        getEjeRows().forEach(function (row) {
            bindEjeRow(row);
            updatePosiciones(row.querySelector('[data-field="idTipoEje"]'));
        });

        renumberEjes();

        addEjeButton.addEventListener('click', function () {
            createEjeRow();
            validateEjes();
        });
    };

    // Función pública de inicialización
    return {
        init: function () {
            form = document.querySelector('#kt_modelo_in_form');
            submitButton = document.querySelector('#kt_modelo_in_submit');
            ejesContainer = document.querySelector('#kt_modelo_ejes_container');
            addEjeButton = document.querySelector('#kt_modelo_add_eje');
            ejesValidation = document.querySelector('#kt_modelo_ejes_validation');

            document.querySelector("#MesGarantia").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#AnioDesde").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#AnioHasta").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#KmGarantia").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9.]/g, '');
                if ((this.value.match(/\./g) || []).length > 1) {
                    this.value = this.value.slice(0, -1);
                }
            });

            handleValidation();

            handleEjes();

            handleSubmitValidation();

        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTModelo.init();
});
