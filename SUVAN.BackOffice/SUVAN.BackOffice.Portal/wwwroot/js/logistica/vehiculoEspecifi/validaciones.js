var KTEspecificacionesValidaciones = function () {

    _InitAutonumeric = {
        decimalPlaces: 0,
        digitGroupSeparator: '',
    }

    _FloatAutonumeric = {
        decimalCharacter: '.',
        decimalPlaces: 2,
        digitGroupSeparator: '',
    }

    const initAutoNumeric = () => {
        if (typeof AutoNumeric !== 'undefined') {
            const fields = [
                { selector: '#Ancho', config: _FloatAutonumeric },
                { selector: '#Largo', config: _FloatAutonumeric },
                { selector: '#Altura', config: _FloatAutonumeric },
                { selector: '#PesoBruto', config: _FloatAutonumeric },
                { selector: '#ToneladasCarga', config: _FloatAutonumeric },
                { selector: '#MetrosCubCarga', config: _FloatAutonumeric },
                { selector: '#Pallets', config: _InitAutonumeric },
                { selector: '#NoCilindros', config: _InitAutonumeric },
                { selector: '#CapacidadAceite', config: _FloatAutonumeric },
                { selector: '#CapacidadCombu', config: _FloatAutonumeric },
                { selector: '#RenEsp', config: _FloatAutonumeric },
                { selector: '#TipoCombustible', config: _InitAutonumeric },
                { selector: '#TipoEje', config: _InitAutonumeric },
                { selector: '#CargaMax', config: _FloatAutonumeric },
                { selector: '#TotalLlantas', config: _InitAutonumeric },
                { selector: '#LlantasRepuesto', config: _InitAutonumeric },
                { selector: '#DimensionLlantas', config: _InitAutonumeric }
            ];

            fields.forEach(f => {
                const el = document.querySelector(f.selector);
                if (el) new AutoNumeric(el, f.config);
            });
        }
    };


    /*     Elements*/
    var form;
    var submitButton;
    var validator;

    const camposPorSeccion = {
        dimensioncapacidad: [
            'Ancho', 'Largo', 'Altura', 'PesoBruto', 'ToneladasCarga', 'MetrosCubCarga', 'Pallets'
        ],
        motordesempeno: [
            'TipoMotor', 'PotenciaMotor', 'NoCilindros', 'CapacidadAceite', 'CapacidadCombu', 'RenEsp', 'TipoCombustible'
        ],
        transmisiontraccion: [
            'Transmision', 'Traccion', 'TipoEje', 'CargaPorEje', 'CargaMax', 'TotalLlantas', 'LlantasRepuesto', 'DimensionLlantas', 'PulCub'
        ],
        datosadicionales: [
            'Origen'
        ]
    };

    /*     Handle form*/
    var handleValidation = function (e) {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'Ancho': {
                        validators: {
                            callback: {
                                message: 'Ancho requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'Largo': {
                        validators: {
                            callback: {
                                message: 'Largo requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'Altura': {
                        validators: {
                            callback: {
                                message: 'Altura requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'PesoBruto': {
                        validators: {
                            callback: {
                                message: 'Peso Bruto requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'ToneladasCarga': {
                        validators: {
                            callback: {
                                message: 'Capacidad de carga requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'MetrosCubCarga': {
                        validators: {
                            callback: {
                                message: 'Volumen máximo de Carga requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'Pallets': {
                        validators: {
                            callback: {
                                message: 'Pallets requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'TipoMotor': {
                        validators: {
                            notEmpty: {
                                message: 'Tipo de Motor requerido'
                            },
                            stringLength: {
                                min: 3,
                                max: 20,

                                message: 'deben tener entre 3 y 20 caracteres',
                            },
                        }
                    },
                    'PotenciaMotor': {
                        validators: {
                            notEmpty: {
                                message: 'Potencia del Motor requerido'
                            },
                            stringLength: {
                                min: 3,
                                max: 8,

                                message: 'deben tener entre 3 y 20 caracteres',
                            },
                        }
                    },
                    'NoCilindros': {
                        validators: {
                            callback: {
                                message: 'No. de Cilindros requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'CapacidadAceite': {
                        validators: {
                            callback: {
                                message: 'Capacidad de Aceite requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'CapacidadCombu': {
                        validators: {
                            callback: {
                                message: 'Capacidad de Combustible requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'RenEsp': {
                        validators: {
                            callback: {
                                message: 'Rendimiento Especial requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'TipoCombustible': {
                        validators: {
                            callback: {
                                message: 'Tipo de Combustible requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'Transmision': {
                        validators: {
                            notEmpty: {
                                message: 'Transmisión requerido'
                            },
                            stringLength: {
                                min: 1,
                                max: 1,

                                message: 'deben tener 1 caracter',
                            },
                        }
                    },
                    'Traccion': {
                        validators: {
                            notEmpty: {
                                message: 'Traccion requerido'
                            },
                            stringLength: {
                                min: 1,
                                max: 5,

                                message: 'deben tener entre 1 y 5 caracteres',
                            },
                        }
                    },
                    'TipoEje': {
                        validators: {
                            callback: {
                                message: 'Tipo de Eje requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'CargaPorEje': {
                        validators: {
                            callback: {
                                message: 'Carga por Eje requerido',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'CargaMax': {
                        validators: {
                            callback: {
                                message: 'Carga Máxima requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'TotalLlantas': {
                        validators: {
                            callback: {
                                message: 'Número Total de LLantas requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'LlantasRepuesto': {
                        validators: {
                            callback: {
                                message: 'Llantas de Repuesto requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'DimensionLlantas': {
                        validators: {
                            callback: {
                                message: 'Tamaño de Llantas requerida',
                                callback: function (input) {
                                    const value = parseInt(input.value, 10);
                                    return value > 0;
                                }
                            }
                        }
                    },
                    'PulCub': {
                        validators: {
                            notEmpty: {
                                message: 'Pulido de Cubiertas requerida'
                            },
                            stringLength: {
                                min: 1,
                                max: 6,

                                message: 'deben tener entre 1 y 6 caracteres',
                            },
                        }
                    },
                    'Origen': {
                        validators: {
                            notEmpty: {
                                message: 'Origen requerido'
                            },
                            stringLength: {
                                min: 1,
                                max: 1,

                                message: 'deben tener 1 caracter',
                            },
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
                    campos.map(nombreCampo => validator.validateField(nombreCampo))
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

    var handleSubmitValidation = function (e) {
        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            // Prevent button default action
            e.preventDefault();

            // Validate form
            validator.validate().then(function (status) {
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
            form = document.querySelector('#kt_especificaciones_in_form');
            submitButton = document.querySelector('#kt_especificaciones_in_submit');

            initAutoNumeric();
            handleValidation();
            handleNextTabValidation();
            handleSubmitValidation();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTEspecificacionesValidaciones.init();
});