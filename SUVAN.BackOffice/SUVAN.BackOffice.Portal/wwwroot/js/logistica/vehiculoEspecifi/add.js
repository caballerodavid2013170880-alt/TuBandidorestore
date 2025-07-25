"use strict";

// Class definition
var KTEspecificaciones = function () {
    // Elements
    var submitButton;
    var validator;

    const modeloSelect = document.getElementById('IdModelo');
    const marcaSelect = document.getElementById('IdMarca');
    const marcaJsonInput = document.getElementById('MarcaJson');

    let marcaConfiguration = {};
    let modeloConfiguration = {};

    const initData = () => {
        try {
            marcaConfiguration = JSON.parse(marcaJsonInput.value);
        } catch (e) {
        }
    };

    const clearSelect = (select) => {
        while (select.options.length > 0) {
            select.remove(0);
        }
    };

    const initMarcaAndModelo = () => {
        marcaSelect.addEventListener('change', function (event) {
            const marcaId = parseInt(event.target.value);
            const marca = marcaConfiguration.find(z => z.IdMarca === marcaId);

            clearSelect(modeloSelect);

            const optionSeleccione = document.createElement('option');
            optionSeleccione.value = "";
            optionSeleccione.textContent = "Selecciona un Modelo";
            modeloSelect.appendChild(optionSeleccione);

            modeloConfiguration = marca.Modelos;

            marca.Modelos.forEach(t => {
                const option = document.createElement('option');
                option.value = t.IdModelo;
                option.textContent = t.DescripcionModeloId;
                modeloSelect.appendChild(option);
            });
        });

        modeloSelect.addEventListener('change', function (event) {
            const IdModelo = parseInt(event.target.value);
            const modelo = modeloConfiguration.find(t => t.IdModelo === IdModelo);
        });
    };

    document.addEventListener("DOMContentLoaded", function () {
        const mensaje = document.getElementById("mensajeTempData")?.value;
        if (mensaje) {
            Swal.fire({
                text: mensaje,
                icon: "success",
                confirmButtonText: "Aceptar",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            });
        }
    });

    const initControls = () => {
        initMarcaAndModelo();
    };

    // Public functions
    return {
        init: function () {
            submitButton = document.querySelector('#kt_taller_in_submit');

            initData();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTEspecificaciones.init();
});