"use strict"; var KTCargasTransitorias = function () {
    var form;
    var submitButton;
    var validator;

    // Configuración de Reglas de Validación de Metronic (FormValidation)
    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'IdRegion': {
                        validators: {
                            callback: {
                                message: 'Debes seleccionar una región',
                                callback: function (input) { return input.value !== '0' && input.value !== ""; }
                            }
                        }
                    },
                    'IdPlanta': {
                        validators: {
                            callback: {
                                message: 'Debes seleccionar una planta',
                                callback: function (input) { return input.value !== '0' && input.value !== ""; }
                            }
                        }
                    },
                    'IdZona': {
                        validators: {
                            callback: {
                                message: 'Debes seleccionar una zona',
                                callback: function (input) { return input.value !== '0' && input.value !== ""; }
                            }
                        }
                    },
                    'IdDeposito': {
                        validators: {
                            callback: {
                                message: 'Debes seleccionar un depósito',
                                callback: function (input) { return input.value !== '0' && input.value !== ""; }
                            }
                        }
                    },
                    'VehiculoEconomico': {
                        validators: {
                            notEmpty: { message: 'El número económico del vehículo es requerido' }
                        }
                    },
                    'FolioNota': {
                        validators: {
                            notEmpty: { message: 'El folio de la nota es requerido' }
                        }
                    },
                    'Importe': {
                        validators: {
                            notEmpty: { message: 'El importe es requerido' },
                            numeric: { message: 'Debe ser un valor numérico válido' }
                        }
                    },
                    'Litros': {
                        validators: {
                            notEmpty: { message: 'Los litros son requeridos' },
                            numeric: { message: 'Debe ser un valor numérico válido' }
                        }
                    },
                    'KmActual': {
                        validators: {
                            notEmpty: { message: 'El kilometraje actual es requerido' },
                            numeric: { message: 'Debe ser un valor numérico' }
                        }
                    }
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

    var handleSubmitValidation = function () {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();

            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;
                    form.submit();
                }
            });
        });
    };

    return {
        init: function () {
            form = document.querySelector('#kt_cargas_in_form');
            submitButton = document.querySelector('#kt_cargas_in_submit');

            if (!form) return;

            handleValidation();
            handleSubmitValidation();


            //logica calulo de km recorridos 
            const inputKmAnterior = form.querySelector('[name="KmAnterior"]');
            const inputKmActual = form.querySelector('[name="KmActual"]');
            const inputKmRecorridos = form.querySelector('[name="KmRecorridos"]');

            function calcularKmRecorridos() {
                const kmAnterior = parseFloat(inputKmAnterior.value) || 0;
                const kmActual = parseFloat(inputKmActual.value) || 0;

                if (kmActual >= kmAnterior) {
                    const recorridos = kmActual - kmAnterior;
                    inputKmRecorridos.value = recorridos;
                } else {
                    inputKmRecorridos.value = 0;
                }

                //revalidacion d ecampo calculado si FormValidation lo requiere 
                if (validator) {
                    validator.revalidateField('KmRecorridos');
                }
            }

            if (inputKmAnterior && inputKmActual && inputKmRecorridos) {
                inputKmAnterior.addEventListener('input', calcularKmRecorridos);
                inputKmActual.addEventListener('input', calcularKmRecorridos);
            }


            // LÓGICA DE COMBOS EN CASCADA
            // Región a Planta
            $(form.querySelector('[name="IdRegion"]')).on('change', function () {
                var regionId = $(this).val();
                var $plantaSelect = $(form.querySelector('[name="IdPlanta"]'));
                var $zonaSelect = $(form.querySelector('[name="IdZona"]'));
                var $depositoSelect = $(form.querySelector('[name="IdDeposito"]'));

                $plantaSelect.empty().append('<option value="">Selecciona una planta...</option>').prop('disabled', true);
                $zonaSelect.empty().append('<option value="0">Selecciona una zona...</option>').prop('disabled', true);
                $depositoSelect.empty().append('<option value="0">Selecciona un depósito...</option>').prop('disabled', true);

                if (regionId && regionId !== "0") {
                    $.getJSON('/Combustible/GetPlantas', { regionId: regionId }, function (data) {
                        $plantaSelect.empty().append('<option value="">Selecciona una planta...</option>');
                        $.each(data, function (index, item) {
                            var valId = item.id || item.Id || item.idPlanta;
                            var valNombre = item.nombre || item.Nombre || item.nombrePlanta;
                            $plantaSelect.append('<option value="' + valId + '">' + valNombre + '</option>');
                        });
                        $plantaSelect.prop('disabled', false); // Quita el bloqueo de Metronic
                    });
                }
                validator.revalidateField('IdRegion');
            });

            // Planta a Zona
            $(form.querySelector('[name="IdPlanta"]')).on('change', function () {
                var plantaId = $(this).val();
                var $zonaSelect = $(form.querySelector('[name="IdZona"]'));
                var $depositoSelect = $(form.querySelector('[name="IdDeposito"]'));

                $zonaSelect.empty().append('<option value="">Selecciona una zona...</option>').prop('disabled', true);
                $depositoSelect.empty().append('<option value="0">Selecciona un depósito...</option>').prop('disabled', true);

                if (plantaId && plantaId !== "0") {
                    $.getJSON('/Combustible/GetZonas', { plantaId: plantaId }, function (data) {
                        $zonaSelect.empty().append('<option value="">Selecciona una zona...</option>');
                        $.each(data, function (index, item) {
                            var valId = item.id || item.Id || item.idZona;
                            var valNombre = item.nombre || item.Nombre || item.nombreZona;
                            $zonaSelect.append('<option value="' + valId + '">' + valNombre + '</option>');
                        });
                        $zonaSelect.prop('disabled', false); // Quita el bloqueo de Metronic
                    });
                }
                validator.revalidateField('IdPlanta');
            });

            // Zona a Depósito
            $(form.querySelector('[name="IdZona"]')).on('change', function () {
                var zonaId = $(this).val();
                var $depositoSelect = $(form.querySelector('[name="IdDeposito"]'));

                $depositoSelect.empty().append('<option value="">Selecciona un depósito...</option>').prop('disabled', true);

                if (zonaId && zonaId !== "0") {
                    $.getJSON('/Combustible/GetDepositos', { zonaId: zonaId }, function (data) {
                        $depositoSelect.empty().append('<option value="">Selecciona un depósito...</option>');
                        $.each(data, function (index, item) {
                            var valId = item.id || item.Id || item.idDeposito;
                            var valNombre = item.nombre || item.Nombre || item.descripcion;
                            $depositoSelect.append('<option value="' + valId + '">' + valNombre + '</option>');
                        });
                        $depositoSelect.prop('disabled', false); // Quita el bloqueo de Metronic
                    });
                }
                validator.revalidateField('IdZona');
            });

            // Depósito (Revalidación)
            $(form.querySelector('[name="IdDeposito"]')).on('change', function () {
                validator.revalidateField('IdDeposito');
            });
        }
    };
}(); KTUtil.onDOMContentLoaded(function () {
    KTCargasTransitorias.init();
});