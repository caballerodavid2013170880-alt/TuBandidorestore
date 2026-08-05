"use strict";

var KTLlantaCrear = function () {
    var form;
    var submitButton;
    var validator;
    var regionSelect;
    var plantaSelect;
    var zonaSelect;
    var depositoSelect;
    var marcaSelect;
    var modeloSelect;
    var modeloRequestId = 0;
    var detalleRequestId = 0;

    var getCatalogId = function (item) {
        return item.id ?? item.Id;
    };

    var getCatalogName = function (item) {
        return item.nombre ?? item.Nombre;
    };

    var resetSelect = function (select, placeholder, disabled) {
        if (!select) {
            return;
        }

        select.innerHTML = "";
        select.appendChild(new Option(placeholder, ""));
        select.disabled = disabled;
        $(select).val("").trigger("change.select2");
    };

    var fillSelect = function (select, placeholder, items) {
        resetSelect(select, placeholder, false);

        items.forEach(function (item) {
            select.appendChild(new Option(getCatalogName(item), getCatalogId(item)));
        });

        $(select).trigger("change.select2");
    };

    var getJson = function (url, params) {
        var query = new URLSearchParams(params);
        return fetch(url + "?" + query.toString(), {
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        }).then(function (response) {
            if (!response.ok) {
                throw new Error("No fue posible cargar el catálogo.");
            }

            return response.json();
        });
    };

    var revalidate = function (field) {
        if (validator) {
            validator.revalidateField(field);
        }
    };

    var clearFromRegion = function () {
        resetSelect(plantaSelect, "Selecciona una planta", true);
        resetSelect(zonaSelect, "Selecciona una zona", true);
        resetSelect(depositoSelect, "Selecciona un depósito", true);
    };

    var clearFromPlanta = function () {
        resetSelect(zonaSelect, "Selecciona una zona", true);
        resetSelect(depositoSelect, "Selecciona un depósito", true);
    };

    var clearFromZona = function () {
        resetSelect(depositoSelect, "Selecciona un depósito", true);
    };

    var showModeloDetalle = function (detalle) {
        var container = document.querySelector("#modeloDetalleContainer");
        if (!container) {
            return;
        }

        if (!detalle) {
            container.classList.add("d-none");
            document.querySelector("#ModeloDetalleMarca").value = "";
            document.querySelector("#ModeloDetalleNombre").value = "";
            document.querySelector("#ModeloDetalleMedida").value = "";
            document.querySelector("#ModeloDetallePresionMinima").value = "";
            document.querySelector("#ModeloDetallePresionMaxima").value = "";
            document.querySelector("#ModeloDetalleProfundidad").value = "";
            document.querySelector("#ModeloDetalleVidaUtil").value = "";
            return;
        }

        document.querySelector("#ModeloDetalleMarca").value = detalle.marca ?? detalle.Marca ?? "";
        document.querySelector("#ModeloDetalleNombre").value = detalle.modelo ?? detalle.Modelo ?? "";
        document.querySelector("#ModeloDetalleMedida").value = detalle.medida ?? detalle.Medida ?? "";
        document.querySelector("#ModeloDetallePresionMinima").value = detalle.presionMinimaPsi ?? detalle.PresionMinimaPsi ?? "";
        document.querySelector("#ModeloDetallePresionMaxima").value = detalle.presionMaximaPsi ?? detalle.PresionMaximaPsi ?? "";
        document.querySelector("#ModeloDetalleProfundidad").value = detalle.profundidadOriginalMm ?? detalle.ProfundidadOriginalMm ?? "";
        document.querySelector("#ModeloDetalleVidaUtil").value = detalle.vidaUtilEstimadaKm ?? detalle.VidaUtilEstimadaKm ?? "";
        container.classList.remove("d-none");
    };

    var handleUbicacion = function () {
        if (!regionSelect || !plantaSelect || !zonaSelect || !depositoSelect) {
            return;
        }

        $(regionSelect).on("change", function () {
            var idRegion = $(this).val();
            clearFromRegion();
            revalidate("IdRegion");

            if (!idRegion) {
                return;
            }

            getJson(window.SUVAN.Llantas.getPlantasUrl, { idRegion: idRegion })
                .then(function (items) {
                    fillSelect(plantaSelect, "Selecciona una planta", items);
                })
                .catch(function () {
                    resetSelect(plantaSelect, "No fue posible cargar las plantas", true);
                });
        });

        $(plantaSelect).on("change", function () {
            var idRegion = $(regionSelect).val();
            var idPlanta = $(this).val();
            clearFromPlanta();
            revalidate("IdPlanta");

            if (!idRegion || !idPlanta) {
                return;
            }

            getJson(window.SUVAN.Llantas.getZonasUrl, {
                idRegion: idRegion,
                idPlanta: idPlanta
            })
                .then(function (items) {
                    fillSelect(zonaSelect, "Selecciona una zona", items);
                })
                .catch(function () {
                    resetSelect(zonaSelect, "No fue posible cargar las zonas", true);
                });
        });

        $(zonaSelect).on("change", function () {
            var idRegion = $(regionSelect).val();
            var idPlanta = $(plantaSelect).val();
            var idZona = $(this).val();
            clearFromZona();
            revalidate("IdZona");

            if (!idRegion || !idPlanta || !idZona) {
                return;
            }

            getJson(window.SUVAN.Llantas.getDepositosUrl, {
                idRegion: idRegion,
                idPlanta: idPlanta,
                idZona: idZona
            })
                .then(function (items) {
                    fillSelect(depositoSelect, "Selecciona un depósito", items);
                })
                .catch(function () {
                    resetSelect(depositoSelect, "No fue posible cargar los depósitos", true);
                });
        });

        $(depositoSelect).on("change", function () {
            revalidate("IdDeposito");
        });
    };

    var handleMarcaModelo = function () {
        if (!marcaSelect || !modeloSelect) {
            return;
        }

        $(marcaSelect).on("change", function () {
            var idMarcaLlanta = $(this).val();
            var requestId = ++modeloRequestId;

            resetSelect(modeloSelect, "Seleccione un modelo", true);
            showModeloDetalle(null);
            revalidate("IdMarcaLlanta");
            revalidate("IdModeloLlanta");

            if (!idMarcaLlanta) {
                return;
            }

            getJson(window.SUVAN.Llantas.getModelosPorMarcaUrl, { idMarcaLlanta: idMarcaLlanta })
                .then(function (items) {
                    if (requestId !== modeloRequestId) {
                        return;
                    }

                    fillSelect(modeloSelect, "Seleccione un modelo", items);
                })
                .catch(function () {
                    resetSelect(modeloSelect, "No fue posible cargar los modelos", true);
                });
        });

        $(modeloSelect).on("change", function () {
            var idModeloLlanta = $(this).val();
            var requestId = ++detalleRequestId;

            showModeloDetalle(null);
            revalidate("IdModeloLlanta");

            if (!idModeloLlanta) {
                return;
            }

            getJson(window.SUVAN.Llantas.getDetalleModeloUrl, { idModeloLlanta: idModeloLlanta })
                .then(function (detalle) {
                    if (requestId !== detalleRequestId) {
                        return;
                    }

                    showModeloDetalle(detalle);
                })
                .catch(function () {
                    showModeloDetalle(null);
                });
        });
    };

    var isPositiveOrEmpty = function (value) {
        return value === "" || Number(value) >= 0;
    };

    var isTodayOrPast = function (value) {
        if (value === "") {
            return true;
        }

        var selected = new Date(value + "T00:00:00");
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        return selected <= today;
    };

    var isFabricacionBeforeAdquisicion = function () {
        var fechaFabricacion = form.querySelector('[name="FechaFabricacion"]').value;
        var fechaAdquisicion = form.querySelector('[name="FechaAdquisicion"]').value;

        if (!fechaFabricacion || !fechaAdquisicion) {
            return true;
        }

        return new Date(fechaFabricacion + "T00:00:00") <= new Date(fechaAdquisicion + "T00:00:00");
    };

    var selectorOk = function (input) {
        return Number(input.value) > 0;
    };

    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    "CodigoLlanta": {
                        validators: {
                            notEmpty: { message: "El código de llanta es obligatorio" },
                            stringLength: { max: 30, message: "El código de llanta no debe exceder 30 caracteres" }
                        }
                    },
                    "NumeroSerieDot": {
                        validators: {
                            notEmpty: { message: "El número de serie / DOT es obligatorio" },
                            stringLength: { max: 30, message: "El número de serie / DOT no debe exceder 30 caracteres" }
                        }
                    },
                    "IdMarcaLlanta": {
                        validators: {
                            callback: { message: "Debes seleccionar una marca", callback: selectorOk }
                        }
                    },
                    "IdModeloLlanta": {
                        validators: {
                            callback: { message: "Debes seleccionar un modelo", callback: selectorOk }
                        }
                    },
                    "IdEstadoLlanta": {
                        validators: {
                            callback: { message: "Debes seleccionar un estado", callback: selectorOk }
                        }
                    },
                    "IdRegion": {
                        validators: {
                            callback: { message: "Debes seleccionar una región", callback: selectorOk }
                        }
                    },
                    "IdPlanta": {
                        validators: {
                            callback: { message: "Debes seleccionar una planta", callback: selectorOk }
                        }
                    },
                    "IdZona": {
                        validators: {
                            callback: { message: "Debes seleccionar una zona", callback: selectorOk }
                        }
                    },
                    "IdDeposito": {
                        validators: {
                            callback: { message: "Debes seleccionar un depósito", callback: selectorOk }
                        }
                    },
                    "FechaFabricacion": {
                        validators: {
                            callback: {
                                message: "La fecha de fabricación no puede ser posterior a la fecha actual ni a la fecha de adquisición",
                                callback: function (input) {
                                    return isTodayOrPast(input.value) && isFabricacionBeforeAdquisicion();
                                }
                            }
                        }
                    },
                    "CostoAdquisicion": {
                        validators: {
                            notEmpty: { message: "El costo de adquisición es obligatorio" },
                            callback: {
                                message: "El costo de adquisición no puede ser negativo",
                                callback: function (input) {
                                    return isPositiveOrEmpty(input.value);
                                }
                            }
                        }
                    },
                    "FechaAdquisicion": {
                        validators: {
                            notEmpty: { message: "La fecha de adquisición es obligatoria" },
                            callback: {
                                message: "La fecha de adquisición no puede ser posterior a la fecha actual",
                                callback: function (input) {
                                    return isTodayOrPast(input.value);
                                }
                            }
                        }
                    },
                    "Observaciones": {
                        validators: {
                            stringLength: { max: 500, message: "Las observaciones no deben exceder 500 caracteres" }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: ".fv-row, .col-md-3, .col-md-4, .col-md-12",
                        eleInvalidClass: "",
                        eleValidClass: ""
                    })
                }
            }
        );
    };

    var handleSubmit = function () {
        submitButton.addEventListener("click", function (e) {
            e.preventDefault();

            validator.validate().then(function (status) {
                if (status === "Valid") {
                    submitButton.setAttribute("data-kt-indicator", "on");
                    submitButton.disabled = true;
                    form.submit();
                }
            });
        });
    };

    return {
        init: function () {
            form = document.querySelector("#kt_form_llanta");
            submitButton = document.querySelector("#kt_llanta_submit");
            regionSelect = document.querySelector("#IdRegion");
            plantaSelect = document.querySelector("#IdPlanta");
            zonaSelect = document.querySelector("#IdZona");
            depositoSelect = document.querySelector("#IdDeposito");
            marcaSelect = document.querySelector("#IdMarcaLlanta");
            modeloSelect = document.querySelector("#IdModeloLlanta");

            if (!form || !submitButton) {
                return;
            }

            handleUbicacion();
            handleMarcaModelo();
            handleValidation();
            handleSubmit();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTLlantaCrear.init();
});
