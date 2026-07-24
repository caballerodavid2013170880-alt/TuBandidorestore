"use strict";

var KTLlantaCrear = function () {
    var form;
    var submitButton;
    var validator;
    var regionSelect;
    var plantaSelect;
    var zonaSelect;
    var depositoSelect;

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

    var handleCascada = function () {
        if (!regionSelect || !plantaSelect || !zonaSelect || !depositoSelect) {
            console.error("No se encontraron todos los controles de ubicación.");
            return;
        }

        $(regionSelect).on("change", function () {
            var idRegion = $(this).val();

            console.log("Región seleccionada:", idRegion);

            clearFromRegion();

            if (!idRegion) {
                return;
            }

            if (
                !window.SUVAN ||
                !window.SUVAN.Llantas ||
                !window.SUVAN.Llantas.getPlantasUrl
            ) {
                console.error("No se configuró la URL para obtener plantas.");
                return;
            }

            getJson(
                window.SUVAN.Llantas.getPlantasUrl,
                { idRegion: idRegion }
            )
                .then(function (items) {
                    console.log("Plantas obtenidas:", items);

                    fillSelect(
                        plantaSelect,
                        "Selecciona una planta",
                        items
                    );
                })
                .catch(function (error) {
                    console.error("Error al obtener las plantas:", error);

                    resetSelect(
                        plantaSelect,
                        "No fue posible cargar las plantas",
                        true
                    );
                });
        });

        $(plantaSelect).on("change", function () {
            var idRegion = $(regionSelect).val();
            var idPlanta = $(this).val();

            clearFromPlanta();

            if (!idRegion || !idPlanta) {
                return;
            }

            getJson(
                window.SUVAN.Llantas.getZonasUrl,
                {
                    idRegion: idRegion,
                    idPlanta: idPlanta
                }
            )
                .then(function (items) {
                    fillSelect(
                        zonaSelect,
                        "Selecciona una zona",
                        items
                    );
                })
                .catch(function (error) {
                    console.error("Error al obtener las zonas:", error);

                    resetSelect(
                        zonaSelect,
                        "No fue posible cargar las zonas",
                        true
                    );
                });
        });

        $(zonaSelect).on("change", function () {
            var idRegion = $(regionSelect).val();
            var idPlanta = $(plantaSelect).val();
            var idZona = $(this).val();

            clearFromZona();

            if (!idRegion || !idPlanta || !idZona) {
                return;
            }

            getJson(
                window.SUVAN.Llantas.getDepositosUrl,
                {
                    idRegion: idRegion,
                    idPlanta: idPlanta,
                    idZona: idZona
                }
            )
                .then(function (items) {
                    fillSelect(
                        depositoSelect,
                        "Selecciona un depósito",
                        items
                    );
                })
                .catch(function (error) {
                    console.error("Error al obtener los depósitos:", error);

                    resetSelect(
                        depositoSelect,
                        "No fue posible cargar los depósitos",
                        true
                    );
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

    var handleValidation = function () {
        var fields = {
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
            "IdRegion": {
                validators: {
                    callback: {
                        message: "Debes seleccionar una región",
                        callback: function (input) {
                            return Number(input.value) > 0;
                        }
                    }
                }
            },
            "IdPlanta": {
                validators: {
                    callback: {
                        message: "Debes seleccionar una planta",
                        callback: function (input) {
                            return Number(input.value) > 0;
                        }
                    }
                }
            },
            "IdZona": {
                validators: {
                    callback: {
                        message: "Debes seleccionar una zona",
                        callback: function (input) {
                            return Number(input.value) > 0;
                        }
                    }
                }
            },
            "IdDeposito": {
                validators: {
                    callback: {
                        message: "Debes seleccionar un depósito",
                        callback: function (input) {
                            return Number(input.value) > 0;
                        }
                    }
                }
            },
            "PresionMinimaPsi": {
                validators: {
                    callback: {
                        message: "La presión mínima no puede ser negativa",
                        callback: function (input) {
                            return isPositiveOrEmpty(input.value);
                        }
                    }
                }
            },
            "PresionMaximaPsi": {
                validators: {
                    callback: {
                        message: "La presión máxima no puede ser menor que la mínima",
                        callback: function (input) {
                            var minima = form.querySelector('[name="PresionMinimaPsi"]').value;
                            return isPositiveOrEmpty(input.value) && (input.value === "" || minima === "" || Number(input.value) >= Number(minima));
                        }
                    }
                }
            },
            "FechaFabricacion": {
                validators: {
                    callback: {
                        message: "La fecha de fabricación no puede ser posterior a la fecha actual",
                        callback: function (input) {
                            return isTodayOrPast(input.value);
                        }
                    }
                }
            },
            "ProfundidadOriginalMm": {
                validators: {
                    callback: {
                        message: "La profundidad original no puede ser negativa",
                        callback: function (input) {
                            return isPositiveOrEmpty(input.value);
                        }
                    }
                }
            },
            "VidaUtilEstimadaKm": {
                validators: {
                    callback: {
                        message: "La vida útil estimada no puede ser negativa",
                        callback: function (input) {
                            return isPositiveOrEmpty(input.value);
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
        };

        validator = FormValidation.formValidation(
            form,
            {
                fields: fields,
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: ".fv-row, .col-md-4, .col-md-12",
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

            if (!form || !submitButton) {
                return;
            }

            handleCascada();
            handleValidation();
            handleSubmit();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTLlantaCrear.init();
});
