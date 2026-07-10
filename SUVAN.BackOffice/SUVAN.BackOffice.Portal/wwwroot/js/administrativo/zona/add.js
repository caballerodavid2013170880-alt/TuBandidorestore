"use strict";

// Class definition
var KTZona = function () {
    // Elements
    var form;
    var submitButton;
    var validator;



    //Funcion para cascada
    var handleCascadaCombos = function () {
        //Para cuando se cambia la Región
        $('#IdRegion').on('change', function () {
            var regionId = $(this).val();
            var $plantaSelect = $('#IdPlanta');

            var plantaSeleccionadaId = $('#IdPlantaHidden').val();

            //limpia combos y muestra estado de carga
            $plantaSelect.empty().append('<option value="">Cargando plantas...</option>');

            if (regionId && regionId !== "0" && regionId !== "") {
                //Petición AJAX al controlador de zona
                $.getJSON('/Zona/ObtenerPlantas', { regionId: regionId }, function (data) {
                    //console.log("Buscando PlantaID: ",plantaSeleccionadaId); //para ver en consola que datos estan llegando
                    //console.log("Datos recibidos del server: ", data);
                    $plantaSelect.empty().append('<option value="">Seleccione una planta</option>');

                    $.each(data, function (i, item) {
                        $plantaSelect.append($('<option>', {
                            value: item.id,
                            text: item.nombre
                        }));
                    });

                    if (plantaSeleccionadaId && plantaSeleccionadaId !== "0") {

                        var idASeleccionar = plantaSeleccionadaId.toString();

                        $plantaSelect.prop('disabled', false);

                        $plantaSelect.val(idASeleccionar);

                        //console.log("3. Valor despues de intentar asignar: ", $plantaSelect.val());

                        if ($('#ZonaId').val() > 0) {
                            $plantaSelect.prop('disabled', true);
                        }
                    }


                }).fail(function () {
                    //console.log("Error en petición:", textStatus, errorThrown);
                    $plantaSelect.empty().append('<option value="">Error al cargar plantas</option>');
                });
            } else {
                $plantaSelect.empty().append('<option value="">Seleccione una región primero</option>');
                //$plantaSelect.trigger('change.select2');
            }
        });
    };


    // Handle form
    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'IdRegion': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Región'
                            },
                        }
                    },

                    'IdPlanta': {
                        validators: {
                            notEmpty: {
                                message: 'Seleccione una Planta'
                            },
                        }
                    },
                    'ZonaNombre': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre de la Zona requerido'
                            },
                            stringLength: {
                                min: 5,
                                max: 225,

                                message: 'deben tener entre 5 y 225 caracteres',
                            },
                        }
                    },
                    'Domicilio': {
                        validators: {
                            notEmpty: {
                                message: 'Domicilio requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 80,

                                message: 'deben tener entre 7 y 80 caracteres',
                            },
                        }
                    },
                    'Rfc': {
                        validators: {
                            notEmpty: {
                                message: 'RFC requerido'
                            },
                            regexp: {
                                regexp: /^([A-Z&Ññ]{3,4})(\d{6})([A-V1-9])([A-Z\d]{1,4})$/,
                                message: 'El RFC no es válido ',
                            }
                        }
                    },
                    'Responsable': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Responsable requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 60,

                                message: 'deben tener entre 7 y 60 caracteres',
                            },
                        }
                    },
                    'FechaApertura': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de Apertura es requerida'
                            }
                        }
                    },
                    'Telefono1': {
                        validators: {
                            notEmpty: {
                                message: 'Número telefónico requerido'
                            },
                            regexp: {
                                regexp: /^[0-9]{10}$/,
                                message: 'El número debe tener 10 dígitos'
                            }
                        }
                    },
                    'Telefono2': {
                        validators: {
                            notEmpty: {
                                message: 'Número telefónico requerido'
                            },
                            regexp: {
                                regexp: /^[0-9]{10}$/,
                                message: 'El número debe tener 10 dígitos'
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
                }),

            excluded: new FormValidation.plugins.Excluded({
                excluded: function (name, ele, nodes) {
                    return ele.disabled === true || ele.type === 'hidden';
                }
            }),
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
                monthNames: [
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                ]
            },
            startDate: moment(fechaApertura, "DD/MM/YYYY")
        });
        elementoFecha.on('apply.daterangepicker', function (ev, picker) {
            var fechaSeleccionada = picker.startDate.format('DD/MM/YYYY');
            console.log("Boton aceptar presionado");
            $(this).val(fechaSeleccionada);


            if (validator) {
                validator.revalidateField('FechaApertura');
            }
        });

        elementoFecha.on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
        });
    };

    //     //envio del formulario
    // var handleSubmitValidation = function () {
    //     submitButton.addEventListener('click', function (e) {
    //         e.preventDefault();
    //         if (validator) {
    //             validator.validate().then(function (status) {
    //                 if (status == 'Valid') {
    //                     submitButton.setAttribute('data-kt-indicator', 'on');
    //                     submitButton.disabled = true;
    //                     form.submit();
    //                 }
    //             });
    //         }
    //     });
    // };

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_zona_in_form');
            submitButton = document.querySelector('#kt_zona_in_submit');

            document.querySelector("#Telefono1").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            document.querySelector("#Telefono2").addEventListener("input", function () {
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            handleCascadaCombos();
            handleValidation();
            handleControls();

            handleSubmitValidation(); // use for form validation submit

        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTZona.init();
});