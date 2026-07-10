var KTDetalleValidaciones = function () {

    _InitAutonumeric = {
        decimalPlaces: 0,
        digitGroupSeparator: '',
    }

    _AnioAutonumeric = {
        decimalPlaces: 0,
        digitGroupSeparator: '',
        maximumValue: '9999',
    }

    _PesosAutonumeric = {
        decimalCharacter: '.',
        decimalPlaces: 2,
        digitGroupSeparator: '',
        currencySymbol: '$',
        currencySymbolPlacement: 'p'
    }

    _FloatAutonumeric = {
        decimalCharacter: '.',
        decimalPlaces: 2,
        digitGroupSeparator: '',
    }

    const initAutoNumeric = () => {
        if (typeof AutoNumeric !== 'undefined') {
            new AutoNumeric('#AnioVehiculo', _AnioAutonumeric);
            new AutoNumeric('#Gasolina', _InitAutonumeric);
            new AutoNumeric('#MesesGarantia', _InitAutonumeric);
            new AutoNumeric('#PesoMinimo', _InitAutonumeric);
            new AutoNumeric('#PesoMaximo', _InitAutonumeric);

            new AutoNumeric('#CostoVehiculo', _PesosAutonumeric);
            new AutoNumeric('#KilometrajeAcumulado', _FloatAutonumeric);
            new AutoNumeric('#KilometrajeGarantia', _FloatAutonumeric);
            new AutoNumeric('#VolumenMinimo', _FloatAutonumeric);
            new AutoNumeric('#VolumenMaximo', _FloatAutonumeric);
        }
    };

    var form;
    var submitButton;
    var validator;

    const camposPorSeccion = {
        datosgenerales: [
            'IdTipoVehiculo', 'IdMarca', 'IdModelo', 'IdTipoEje', 'AnioVehiculo',
            'ColorVehiculo', 'ColorInterior', 'NumeroSerie', 'NumeroMotor', 'Carroceria', 'Gasolina',
            'TieneRotulo', 'Rentado', 'VehiculoRelevo', 'EconomicoAnterior'
        ],
        ubicaciondocumentacion: [
            'IdZona', 'IdDeposito', 'CopiaFactura', 'CopiaPlaca', 'CopiaVerificacion',
            'CopiaTarjetaCir', 'CopiaPolizaSeguro'
        ],

        compracosto: [
            'Proveedor', 'FechaCompra', 'NumeroFactura', 'CostoVehiculo', 'TarifaVehicular', 'KilometrajeAcumulado'
        ],

        garantiaestado: [
            'KilometrajeGarantia', 'MesesGarantia', 'FechaBaja'
        ],

        especificaciones: [
            'PesoMinimo', 'PesoMaximo', 'VolumenMinimo', 'VolumenMaximo', 'TieneCaja', 'NecesitaRemolque'
        ],
    };

    const costoVehiculoAutoNumeric = ['CostoVehiculo'];

    function limpiarCamposMoneda(campos) {
        campos.forEach(campoId => {
            const elemento = document.getElementById(campoId);
            if (elemento) {
                const autoNumericInstance = AutoNumeric.getAutoNumericElement(elemento);
                if (autoNumericInstance) {
                    elemento.value = autoNumericInstance.getNumber();
                }
            }
        });
    }


    // Handle form
    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        window.validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'IdTipoVehiculo': {
                        validators: {
                            callback: {
                                message: 'Selecciona un Tipo de Vehículo',
                                callback: function (input) {
                                    return input.value && input.value.trim() !== '' && input.value !== '0';
                                }
                            }
                        }
                    },
                    'IdMarca': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Marca',
                                callback: function (input) {
                                    return input.value && input.value.trim() !== '' && input.value !== '0';
                                }
                            }
                        }
                    },
                    'IdModelo': {
                        validators: {
                            callback: {
                                message: 'Selecciona un Modelo',
                                callback: function (input) {
                                    return input.value && input.value.trim() !== '' && input.value !== '0';
                                }
                            }
                        }
                    },
                    'IdTipoEje': {
                        validators: {
                            callback: {
                                message: 'Selecciona un Tipo de Eje',
                                callback: function (input) {
                                    return input.value && input.value.trim() !== '' && input.value !== '0';
                                }
                            }
                        }
                    },
                    'AnioVehiculo': {
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
                    'ColorVehiculo': {
                        validators: {
                            notEmpty: {
                                message: 'Color del Vehículo requerido'
                            },
                            stringLength: {
                                min: 4,
                                max: 20,

                                message: 'deben tener entre 4 y 20 caracteres',
                            },
                        }
                    },
                    'ColorInterior': {
                        validators: {
                            notEmpty: {
                                message: 'Color Interior requerido'
                            },
                            stringLength: {
                                min: 4,
                                max: 20,

                                message: 'deben tener entre 4 y 20 caracteres',
                            },
                        }
                    },
                    'NumeroSerie': {
                        validators: {
                            notEmpty: {
                                message: 'Número de Serie requerido'
                            },
                            regexp: {
                                regexp: /^[A-Z0-9]+$/,
                                message: 'Ingrese un formato de Serie v&aacute;lido'
                            }
                        }
                    },
                    'NumeroMotor': {
                        validators: {
                            notEmpty: {
                                message: 'Número de Motor requerido'
                            },
                            regexp: {
                                regexp: /^[A-Z0-9]+$/,
                                message: 'Ingrese un formato de Motor v&aacute;lido'
                            }
                        }
                    },
                    'Carroceria': {
                        validators: {
                            notEmpty: {
                                message: 'VIN requerido'
                            },
                            regexp: {
                                regexp: /^[A-HJ-NPR-Z0-9]{17}$/,
                                message: 'Ingrese un formato de VIN v&aacute;lido',
                            }
                        },
                    },
                    'Gasolina': {
                        validators: {
                            callback: {
                                message: 'Litros de Gasolina requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'TieneRotulo': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',
                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'Rentado': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'VehiculoRelevo': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'IdZona': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Zona',
                                callback: function (input) {
                                    return input.value && input.value.trim() !== '' && input.value !== '0';
                                }
                            }
                        }
                    },

                    'IdDeposito': {
                        validators: {
                            callback: {
                                message: 'Selecciona un Depósito',
                                callback: function (input) {
                                    return input.value && input.value.trim() !== '' && input.value !== '0';
                                }
                            }
                        }
                    },
                    'CopiaFactura': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'CopiaPlaca': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'CopiaVerificacion': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'CopiaTarjetaCir': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'CopiaPolizaSeguro': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'Proveedor': {
                        validators: {
                            notEmpty: {
                                message: 'Proveedor requerido'
                            },
                            stringLength: {
                                max: 5,

                                message: 'deben tener entre 1 y 5 caracteres',
                            },
                        }
                    },
                    'FechaCompra': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de Compra requerida'
                            }
                        }
                    },
                    'NumeroFactura': {
                        validators: {
                            notEmpty: {
                                message: 'Número de Factura requerida'
                            }
                        }
                    },
                    'CostoVehiculo': {
                        validators: {
                            callback: {
                                message: 'Costo de Vehículo requerido',
                                callback: function (input) {
                                    const rawValue = input.value.replace(/[^0-9]/g, '');
                                    const value = parseInt(rawValue, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'TarifaVehicular': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'KilometrajeAcumulado': {
                        validators: {
                            callback: {
                                message: 'Kilometraje Acumulado requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'KilometrajeGarantia': {
                        validators: {
                            callback: {
                                message: 'Kilometraje de Garantía requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'MesesGarantia': {
                        validators: {
                            callback: {
                                message: 'Meses de Garantía requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'FechaBaja': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de Baja requerida'
                            }
                        }
                    },
                    'EconomicoAnterior': {
                        validators: {
                            notEmpty: {
                                message: 'N&uacute;mero Econ&oacute;mico Anterior requerido'
                            }
                        }
                    },
                    'PesoMinimo': {
                        validators: {
                            callback: {
                                message: 'Peso Mínimo requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'PesoMaximo': {
                        validators: {
                            callback: {
                                message: 'Peso Máximo requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'VolumenMinimo': {
                        validators: {
                            callback: {
                                message: 'Volumen Mínimo requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'VolumenMaximo': {
                        validators: {
                            callback: {
                                message: 'Volumen Máximo requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'TieneCaja': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'NecesitaRemolque': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'TipoLicenciaRequerida': {
                        validators: {
                            notEmpty: {
                                message: 'Tipo de Licencia requerida'
                            }
                        }
                    },
                    'TarjetaCirculacion': {
                        validators: {
                            notEmpty: {
                                message: 'Tarjeta de Circulación requerida'
                            },
                            regexp: {
                                regexp: /^[A-Z0-9\-]{6,15}$/i,
                                message: 'Ingrese un formato de VIN v&aacute;lido'
                            }
                        }
                    },
                    'VigenciaTarjetaCirculacion': {
                        validators: {
                            notEmpty: {
                                message: 'Vigencia de Circulación requerida'
                            }
                        }
                    },
                    'PermisoCargaAceite': {
                        validators: {
                            callback: {
                                message: 'Selecciona una Opción',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
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

    const handleNextTabValidation = () => {
        document.querySelectorAll('.btn-next-tab').forEach(boton => {
            boton.addEventListener('click', async function (e) {
                e.preventDefault();

                const seccionActual = this.dataset.current;
                const seccionSiguiente = this.dataset.next;

                const campos = camposPorSeccion[seccionActual];

                const resultados = await Promise.all(
                    campos.map(nombreCampo => window.validator.validateField(nombreCampo))
                );

                const esValido = resultados.every(r => r === 'Valid');

                if (esValido) {
                    const siguienteTab = document.querySelector(`a[data-bs-toggle="tab"][href="#${seccionSiguiente}"]`);
                    if (siguienteTab) {
                        siguienteTab.classList.remove('disabled');
                        new bootstrap.Tab(siguienteTab).show();
                    }
                }
            });
        });
    };

    function flatpickrFecha(campo) {
        const visible = document.getElementById(campo + 'Visible');
        const hidden = document.getElementById(campo);
        if (!visible || !hidden) return;

        flatpickr(visible, {
            dateFormat: 'd/m/Y',
            allowInput: true,
            defaultDate: null,
            onChange: (dates) => {
                if (dates.length) {
                    const d = dates[0];
                    hidden.value = d.toISOString().slice(0, 10) + 'T00:00:00';
                } else {
                    hidden.value = '';
                }
            },
        });

        if (hidden.value) {
            const d = new Date(hidden.value);
            if (!isNaN(d)) {
                visible.value = d.toLocaleDateString('es-ES');
            }
        }
    }

    ['FechaCompra', 'FechaBaja', 'VigenciaTarjetaCirculacion', 'VigenciaPermisoAceite'].forEach(flatpickrFecha);

    var handleSubmitValidation = function (e) {
        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            // Prevent button default action
            e.preventDefault();

            limpiarCamposMoneda(costoVehiculoAutoNumeric);

            // Validate form
            window.validator.validate().then(function (status) {
                if (status == 'Valid') {
                    // Disable button to avoid multiple click
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;
                    form.submit();
                }
            });
        });
    }

    return {
        init: function () {
            form = document.querySelector('#kt_detalle_in_form');
            submitButton = document.querySelector('#kt_detalle_in_submit');

            initAutoNumeric();
            handleValidation();
            handleNextTabValidation();
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalleValidaciones.init();
});