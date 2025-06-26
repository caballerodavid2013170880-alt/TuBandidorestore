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

            new AutoNumeric('#CostoVehiculo', _FloatAutonumeric);
            new AutoNumeric('#KilometrajeAcumulado', _FloatAutonumeric);
            new AutoNumeric('#KilometrajeGarantia', _FloatAutonumeric);
            new AutoNumeric('#VolumenMinimo', _FloatAutonumeric);
            new AutoNumeric('#VolumenMaximo', _FloatAutonumeric);
        }
    };

    const flatpickrOptions = {
        dateFormat: "d/m/Y",
        defaultDate: "today"
    };
    $("#FechaCompra").flatpickr(flatpickrOptions);
    $("#FechaBaja").flatpickr(flatpickrOptions);
    $("#VigenciaTarjetaCirculacion").flatpickr(flatpickrOptions);
    $("#VigenciaPermisoAceite").flatpickr(flatpickrOptions);



    var form;
    var submitButton;
    var validator;


    // Handle form
    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        window.validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'IdTipoVehiculo': {
                        validators: {
                            notEmpty: {
                                message: 'Selecciona un Tipo de Vehículo'
                            }
                        }
                    },
                    'IdMarca': {
                        validators: {
                            notEmpty: {
                                message: 'Selecciona una Marca'
                            }
                        }
                    },
                    'IdModelo': {
                        validators: {
                            notEmpty: {
                                message: 'Selecciona un Modelo'
                            }
                        }
                    },
                    'IdTipoEje': {
                        validators: {
                            notEmpty: {
                                message: 'Selecciona un Tipo de Eje'
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
                                message: 'Selecciona si tiene Rótulo',
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
                                message: 'Selecciona si es Rentado',

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
                                message: 'Selecciona si es de Relevo',

                                callback: function (input) {
                                    const value = input.value;
                                    return value === "0" || value === "1";
                                }
                            }
                        }
                    },
                    'IdZona': {
                        validators: {
                            notEmpty: {
                                message: 'Selecciona una Zona'
                            }
                        }
                    },
                    'IdDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Selecciona un Depósito'
                            }
                        }
                    },
                    'CopiaFactura': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'CopiaPlaca': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'CopiaVerificacion': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'CopiaTarjetaCir': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (value, validator, $field) {
                                    return value !== "";
                                }
                            }
                        }
                    },
                    'CopiaPolizaSeguro': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Opción',
                                callback: function (value, validator, $field) {
                                    return value !== "";
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

    var handleSubmitValidation = function (e) {
        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            // Prevent button default action
            e.preventDefault();

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
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalleValidaciones.init();
});