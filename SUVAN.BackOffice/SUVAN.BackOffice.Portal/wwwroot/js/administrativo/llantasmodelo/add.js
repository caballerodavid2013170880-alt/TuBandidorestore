"use strict";

var KTLlantaModelo = function () {
    var form;
    var submitButton;
    var validator;

    var numericRange = function (message) {
        return {
            min: 0,
            max: 999.99,
            message: message
        };
    };

    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    "IdMarcaLlanta": {
                        validators: {
                            notEmpty: {
                                message: "La marca es requerida"
                            }
                        }
                    },
                    "Nombre": {
                        validators: {
                            notEmpty: {
                                message: "El modelo es requerido"
                            },
                            stringLength: {
                                max: 150,
                                message: "El modelo no debe exceder 150 caracteres"
                            }
                        }
                    },
                    "Medida": {
                        validators: {
                            stringLength: {
                                max: 50,
                                message: "La medida no debe exceder 50 caracteres"
                            }
                        }
                    },
                    "PresionMinimaPsi": {
                        validators: {
                            between: numericRange("La presión mínima debe estar entre 0 y 999.99")
                        }
                    },
                    "PresionMaximaPsi": {
                        validators: {
                            between: numericRange("La presión máxima debe estar entre 0 y 999.99"),
                            callback: {
                                message: "La presión máxima debe ser mayor o igual a la presión mínima",
                                callback: function (input) {
                                    var minima = parseFloat(form.querySelector('[name="PresionMinimaPsi"]').value);
                                    var maxima = parseFloat(input.value);

                                    return Number.isNaN(minima) || Number.isNaN(maxima) || maxima >= minima;
                                }
                            }
                        }
                    },
                    "ProfundidadOriginalMm": {
                        validators: {
                            between: numericRange("La profundidad original debe estar entre 0 y 999.99")
                        }
                    },
                    "VidaUtilEstimadaKm": {
                        validators: {
                            integer: {
                                message: "La vida útil debe ser un número entero"
                            },
                            greaterThan: {
                                min: 0,
                                message: "La vida útil debe ser mayor o igual a cero"
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: ".fv-row",
                        eleInvalidClass: "",
                        eleValidClass: ""
                    })
                }
            }
        );
    };

    var handleSelects = function () {
        var marca = form.querySelector("#IdMarcaLlanta");

        if (!marca) {
            return;
        }

        $(marca).on("change", function () {
            validator.revalidateField("IdMarcaLlanta");
        });
    };

    var handlePressureValidation = function () {
        var minima = form.querySelector('[name="PresionMinimaPsi"]');

        if (!minima) {
            return;
        }

        minima.addEventListener("input", function () {
            validator.revalidateField("PresionMaximaPsi");
        });
    };

    var handleSubmitValidation = function () {
        submitButton.addEventListener("click", function (e) {
            e.preventDefault();

            validator.validate().then(function (status) {
                if (status !== "Valid") {
                    return;
                }

                submitButton.setAttribute("data-kt-indicator", "on");
                submitButton.disabled = true;
                form.submit();
            });
        });
    };

    return {
        init: function () {
            form = document.querySelector("#kt_llantamodelo_in_form");
            submitButton = document.querySelector("#kt_llantamodelo_in_submit");

            if (!form || !submitButton) {
                return;
            }

            handleValidation();
            handleSelects();
            handlePressureValidation();
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTLlantaModelo.init();
});
