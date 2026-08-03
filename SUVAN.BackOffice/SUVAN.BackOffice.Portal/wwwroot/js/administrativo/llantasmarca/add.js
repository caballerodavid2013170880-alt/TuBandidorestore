"use strict";

var KTLlantaMarca = function () {
    var form;
    var submitButton;
    var validator;

    var handleValidation = function () {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    "Nombre": {
                        validators: {
                            notEmpty: {
                                message: "La marca es requerida"
                            },
                            stringLength: {
                                max: 100,
                                message: "La marca no debe exceder 100 caracteres"
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
            form = document.querySelector("#kt_llantamarca_in_form");
            submitButton = document.querySelector("#kt_llantamarca_in_submit");

            if (!form || !submitButton) {
                return;
            }

            handleValidation();
            handleSubmitValidation();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTLlantaMarca.init();
});
