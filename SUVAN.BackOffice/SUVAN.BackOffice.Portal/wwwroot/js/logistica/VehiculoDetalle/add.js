"use strict";

// Definición de la clase
var KTVehiculoDetalle = function () {
    var form;
    var submitButton;
    var validator;

    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    
                    'Id_vehiculo': {
                        validators: {
                            notEmpty: { message: 'El número de vehículo es requerido' }
                        }
                    },
                    'Id_tipovehi': {
                        validators: {
                            notEmpty: { message: 'El número de tipo de vehículo es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'Id_marca': {
                        validators: {
                            notEmpty: { message: 'El número de la marca es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'Id_zona': {
                        validators: {
                            notEmpty: { message: 'El número de la zona es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'Id_deposito': {
                        validators: {
                            notEmpty: { message: 'El número del depósito es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },

                    'IdEspeci': {
                        validators: {
                            notEmpty: { message: 'El número de espesificacion es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'IdModelo': {
                        validators: {
                            notEmpty: { message: 'El número de modelo es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'IdPermisoAceite': {
                        validators: {
                            notEmpty: { message: 'El número de permiso de aceite es requerido' },
                            stringLength: { max: 30, message: 'Máximo 30 caracteres' }
                        }
                    },
                    'IdCognos': {
                        validators: {
                            notEmpty: { message: 'El número de Cognos es requerido' },
                            stringLength: { max: 15, message: 'Máximo 15 caracteres' }
                        }
                    },
                    'IdTipoEje': {
                        validators: {
                            notEmpty: { message: 'El número de tipo de eje es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },

                   
                    'Negocio': {
                        validators: {
                            notEmpty: { message: 'El negocio es requerido' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'Area': {
                        validators: {
                            notEmpty: { message: 'El área es requerida' },
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    },
                    'Anio': {
                        validators: {
                            notEmpty: { message: 'El año es requerido' },
                            numeric: { message: 'Debe ser un número válido' },
                            between: {
                                min: 1980,
                                max: new Date().getFullYear(),
                                message: 'Debe estar entre 1980 y el año actual'
                            }
                        }
                    },

                    
                    'Color': {
                        validators: {
                            notEmpty: { message: 'El color es requerido' },
                            stringLength: { max: 20, message: 'Máximo 20 caracteres' }
                        }
                    },
                    'Rotulo': {
                        validators: {
                            between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                        }
                    },
                    'PlacaPe': {
                        validators: {
                            notEmpty: { message: 'La placa es requerida' },
                            stringLength: { min: 9, max: 9, message: 'Debe tener exactamente 9 caracteres' }
                        }
                    },
                    'Serie': {
                        validators: {
                            notEmpty: { message: 'La serie es requerida' },
                            stringLength: { max: 30, message: 'Máximo 30 caracteres' }
                        }
                    },
                    'Motor': {
                        validators: {
                            notEmpty: { message: 'El motor es requerido' },
                            stringLength: { max: 30, message: 'Máximo 30 caracteres' }
                        }
                    },
                    'Carroc': {
                        validators: {
                            notEmpty: { message: 'El tipo de carrocería es requerido' },
                            stringLength: { max: 20, message: 'Máximo 20 caracteres' }
                        }
                    },
                    'TarCirc': {
                        validators: {
                            notEmpty: { message: 'La tarjeta de circulación es requerida' },
                            stringLength: { max: 20, message: 'Máximo 20 caracteres' }
                        }
                    },

                   
                    'Gasoline': {
                        validators: {
                            numeric: { message: 'Debe ser un número válido' }
                        }
                    }
                },


                'Encierro': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'CopFac': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'CopTcir': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'CopPla': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'CopVer': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'CopPol': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },

                
                'NoCirc': {
                    validators: {
                        notEmpty: { message: 'El número de circulación es requerido' },
                        stringLength: { max: 2, message: 'Debe tener máximo 2 caracteres' }
                    }
                },
                'DnoCirc': {
                    validators: {
                        notEmpty: { message: 'El número de documento de circulación es requerido' },
                        stringLength: { max: 2, message: 'Debe tener máximo 2 caracteres' }
                    }
                },
                'Proveed': {
                    validators: {
                        notEmpty: { message: 'El código de proveedor es requerido' },
                        stringLength: { max: 5, message: 'Debe tener máximo 5 caracteres' }
                    }
                },

              
                'FCompra': {
                    validators: {
                        notEmpty: { message: 'La fecha de compra es requerida' },
                        date: { format: 'YYYY-MM-DD', message: 'Debe ser una fecha válida' },
                        callback: {
                            message: 'No puede ser futura',
                            callback: function (input) {
                                return new Date(input.value) <= new Date();
                            }
                        }
                    }
                },
                'FBaja': {
                    validators: {
                        notEmpty: { message: 'La fecha de baja es requerida' },
                        date: { format: 'YYYY-MM-DD', message: 'Debe ser una fecha válida' },
                        callback: {
                            message: 'Debe ser posterior a la fecha de compra',
                            callback: function (input) {
                                var baja = new Date(input.value);
                                var compra = new Date(document.querySelector('[name="FCompra"]').value);
                                return baja >= compra;
                            }
                        }
                    }
                },

               
                'Factura': {
                    validators: {
                        notEmpty: { message: 'El número de factura es requerido' },
                        stringLength: { max: 15, message: 'Máximo 15 caracteres' }
                    }
                },
                'Costo': {
                    validators: {
                        notEmpty: { message: 'El costo es requerido' },
                        numeric: { message: 'Debe ser un valor positivo' },
                        greaterThan: { min: 0, message: 'Debe ser mayor que cero' }
                    }
                },

                
                'Tariave': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'Ntariave': {
                    validators: {
                        notEmpty: { message: 'El nombre de la tarifa es requerido' },
                        stringLength: { max: 25, message: 'Máximo 25 caracteres' }
                    }
                },
                'KmAcum': {
                    validators: {
                        notEmpty: { message: 'El kilometraje acumulado es requerido' },
                        numeric: { message: 'Debe ser un número válido' },
                        greaterThan: { min: 0, message: 'Debe ser mayor o igual a 0' }
                    }
                },

                
                'StVehic': {
                    validators: {
                        notEmpty: { message: 'El estado del vehículo es requerido' },
                        stringLength: { max: 1, message: 'Debe ser un solo carácter' }
                    
                }
            },
                'ColInt': {
                    validators: {
                        notEmpty: { message: 'El color interior es requerido' },
                        stringLength: { max: 20, message: 'Máximo 20 caracteres' }
                    }
                },
                'ColEst': {
                    validators: {
                        notEmpty: { message: 'El estado del color es requerido' },
                        numeric: { message: 'Debe ser un número válido' }
                    }
                },
                'ColRuta': {
                    validators: {
                        notEmpty: { message: 'La ruta del color es requerida' },
                        numeric: { message: 'Debe ser un número válido' }
                    }
                },

                
                'RegFed': {
                    validators: {
                        numeric: { message: 'Debe ser un número válido' }
                    }
                },
                'EdregPl': {
                    validators: {
                        numeric: { message: 'Debe ser un número válido' }
                    }
                },

                'Caja': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'NecRem': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'Relevo': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },
                'Rentado': {
                    validators: {
                        between: { min: 0, max: 1, message: 'Solo puede ser 0 o 1' }
                    }
                },

               
                'KmGaran': {
                    validators: {
                        numeric: { message: 'Debe ser un número válido' },
                        greaterThan: { min: 0, message: 'Debe ser mayor o igual a 0' }
                    }
                },
                'MesGara': {
                    validators: {
                        numeric: { message: 'Debe ser un número válido' }
                    }
                },
                'VRecupe': {
                    validators: {
                        numeric: { message: 'Debe ser un número válido' }
                    }
                },

                
                'VigenciaPermisoAceite': {
                    validators: {
                        date: { format: 'YYYY-MM-DD', message: 'Debe ser una fecha válida' }
                    }
                },
                'VigenciaTarjetaCircula': {
                    validators: {
                        date: { format: 'YYYY-MM-DD', message: 'Debe ser una fecha válida' }
                    }
                },

               
                'TipoLicenciaRequerida': {
                    validators: {
                        stringLength: { max: 3, message: 'Máximo 3 caracteres' }
                    }
                },
                'Usuario': {
                    validators: {
                        notEmpty: { message: 'El usuario es requerido' },
                        stringLength: { max: 20, message: 'Máximo 20 caracteres' }
                    }
                }
            ,
            plugins: {
            trigger: new FormValidation.plugins.Trigger(),
            bootstrap: new FormValidation.plugins.Bootstrap5({
                rowSelector: '.fv-row'
            })
        }
        
    

            }
        );
    };

    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault(); // Evita el envío por defecto

            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on'); // Indicador de carga
                    submitButton.disabled = true; // Evita múltiples envíos
                    form.submit(); // Envía el formulario
                }
            });
        });
    };

    return {
        init: function () {
            form = document.querySelector('#kt_vehiculo_detalle_form');
            submitButton = document.querySelector('#kt_vehiculo_detalle_submit');

            handleValidation(); // Configurar validaciones
            handleSubmitValidation(); // Manejar envío del formulario
        }
    };
}();

// Ejecutar cuando el DOM esté listo
KTUtil.onDOMContentLoaded(function () {
    KTVehiculoDetalle.init();
});