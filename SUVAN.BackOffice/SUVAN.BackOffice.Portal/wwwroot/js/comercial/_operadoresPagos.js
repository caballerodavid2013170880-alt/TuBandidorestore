"use strict";

var KTPagosOp = function () {
    // Define shared variables
    var table = document.getElementById('kt_reporte_table');
    let form;
    let validator;
    const listadoContainer = document.getElementById('listado-container');
    const btnEmitirLiq = document.getElementById('btnEmitir');
    const btnGenerar = document.getElementById('btnGenerar');
    const modalEmitir = document.getElementById('kt_modal_liquidacion');
    let showModalEmitirLiq = new bootstrap.Modal(modalEmitir, {
        keyboard: false,
        backdrop: 'static'
    });
    //calendarios
    const flatpickrOptions = {
        dateFormat: "d/m/Y",
        "maxDate": new Date() 
    };
    const datepickerDesde = $("#FechaInicio").flatpickr(flatpickrOptions);
    const datepickerHasta = $("#FechaFin").flatpickr(flatpickrOptions);
    const target = document.querySelector("#kt_modal_liquidacion");

    const handleEmitirLiquidacion = () => {
        btnEmitirLiq.addEventListener('click', function () {
            showModalEmitirLiq.show();
        });
    }

    const handleGenerar = () => {

        btnGenerar.addEventListener('click', async function (e) {

            e.preventDefault();

            // Validate form
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    // Disable button to avoid multiple click
                    btnGenerar.setAttribute('data-kt-indicator', 'on');
                    btnGenerar.disabled = true;

                    const model = {
                        operadorId: parseInt(document.getElementById("operadorId").value),
                        fechaInicio: $('#FechaInicio').val(),
                        fechaFin: $('#FechaFin').val()
                    };

                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: "/Comercial/GeneraRecibo",
                        data: model,
                        complete: function (data) {

                            btnGenerar.disabled = false;

                            var respose = data.responseJSON;
                            if (respose.success) {

                                if (respose.liquidacionID > 0) {

                                    Swal.fire({
                                        text: `Se genero recibo correctamente.`,
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Aceptar",
                                        customClass: {
                                            confirmButton: "btn fw-bold btn-primary",
                                        }
                                    });

                                    var req = new XMLHttpRequest();
                                    req.open("GET", "/Comercial/EmiteReciboPDF?liquidacionID=" + respose.liquidacionID, true);
                                    req.responseType = "blob";

                                    req.onload = function (event) {
                                        var blob = req.response;
                                        var link = document.createElement('a');
                                        link.href = window.URL.createObjectURL(blob);
                                        link.download = "ReciboLiquidacion_" + respose.liquidacionID + ".pdf";
                                        link.click();

                                        loadPartialView("#pagos", "/Comercial/PagosOperador", document.getElementById("operadorId").value)
                                    };

                                    req.send();

                                }
                                else {
                                    Swal.fire({
                                        text: `No existen registros para generar la liquidación.`,
                                        icon: "error",
                                        buttonsStyling: false,
                                        confirmButtonText: "Aceptar",
                                        customClass: {
                                            confirmButton: "btn fw-bold btn-primary",
                                        }
                                    });
                                }
                                showModalEmitirLiq.hide();
                                
                            } else {
                                btnGenerar.setAttribute('data-kt-indicator', 'off');
                                btnGenerar.disabled = false;

                                Swal.fire({
                                    text: respose.message,
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: {
                                        confirmButton: "btn fw-bold btn-primary",
                                    }
                                });
                            }
                        }
                    });
                }
            })
        });
    }

    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'FechaInicio': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de inicio es obligatoria.',
                            }
                        }
                    },
                    'FechaFin': {
                        validators: {
                            notEmpty: {
                                message: 'Fecha de fin debe ser capturada.',
                            },
                            callback: {
                                message: "La fecha fin debe ser igual o posterior a la fecha inicio",
                                callback: function (input) {
                                    var desde = datepickerDesde.selectedDates[0];
                                    var hasta = datepickerHasta.selectedDates[0];

                                    if (desde && hasta) {
                                        return hasta >= desde;
                                    }

                                    return true;
                                }
                            }
                        }
                    }

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

    const intiControls = () => {
        handleEmitirLiquidacion();
        handleGenerar();
    }

    return {
        // Public functions  
        init: function () {
            //initTable();
            form = document.getElementById("kt_GeneraLiq_form");
            handleValidation();
            intiControls();
        }
    }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTPagosOp.init();
});