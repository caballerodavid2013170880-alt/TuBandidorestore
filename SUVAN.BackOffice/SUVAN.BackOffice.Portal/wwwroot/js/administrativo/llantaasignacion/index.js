"use strict";

var SuvanLlantaAsignacion = function () {
    var selectorVehiculo;
    var alerta;
    var resumen;
    var loading;
    var diagrama;
    var config;

    var getValue = function (source, pascalName, camelName) {
        if (!source) {
            return null;
        }

        if (source[pascalName] !== undefined && source[pascalName] !== null) {
            return source[pascalName];
        }

        return source[camelName] !== undefined ? source[camelName] : null;
    };

    var setText = function (id, value) {
        var element = document.getElementById(id);
        if (element) {
            element.textContent = value || "-";
        }
    };

    var setLoading = function (isLoading) {
        if (!loading) {
            return;
        }

        loading.classList.toggle("d-none", !isLoading);
        loading.classList.toggle("d-flex", isLoading);
    };

    var showAlert = function (message) {
        if (!alerta) {
            return;
        }

        alerta.textContent = message || "No fue posible cargar la información.";
        alerta.classList.remove("d-none");
    };

    var hideAlert = function () {
        if (alerta) {
            alerta.classList.add("d-none");
            alerta.textContent = "";
        }
    };

    var clearResumen = function () {
        if (resumen) {
            resumen.classList.add("d-none");
        }

        setText("llanta-asignacion-numero-economico", "-");
        setText("llanta-asignacion-placas", "-");
        setText("llanta-asignacion-unidad", "-");
        setText("llanta-asignacion-kilometraje", "-");

        if (diagrama) {
            diagrama.innerHTML = [
                '<i class="ki-outline ki-truck fs-2tx mb-4"></i>',
                '<div class="fw-semibold fs-6">Selecciona un vehículo para consultar sus ejes y posiciones.</div>'
            ].join("");
        }
    };

    var renderResumen = function (data) {
        var numeroEconomico = getValue(data, "NumeroEconomico", "numeroEconomico");
        var placas = getValue(data, "Placas", "placas");
        var marca = getValue(data, "Marca", "marca");
        var modelo = getValue(data, "Modelo", "modelo");
        var kilometraje = getValue(data, "KilometrajeActual", "kilometrajeActual");
        var ejes = getValue(data, "Ejes", "ejes") || [];

        setText("llanta-asignacion-numero-economico", numeroEconomico);
        setText("llanta-asignacion-placas", placas);
        setText("llanta-asignacion-unidad", [marca, modelo].filter(Boolean).join(" "));
        setText("llanta-asignacion-kilometraje", kilometraje !== null && kilometraje !== undefined ? kilometraje.toLocaleString("es-MX") : "-");

        if (resumen) {
            resumen.classList.remove("d-none");
        }

        if (diagrama) {
            diagrama.innerHTML = [
                '<i class="ki-outline ki-truck fs-2tx mb-4"></i>',
                '<div class="fw-semibold fs-6 text-gray-700">Configuración cargada</div>',
                '<div class="text-muted">Ejes configurados: ' + ejes.length + '</div>'
            ].join("");
        }
    };

    var initSelectVehiculo = function () {
        if (window.jQuery && window.jQuery.fn && window.jQuery.fn.select2) {
            window.jQuery(selectorVehiculo).select2({
                width: "100%",
                placeholder: selectorVehiculo.getAttribute("data-placeholder") || "Seleccione"
            });
        }
    };

    var cargarConfiguracion = async function (idVehiculo) {
        hideAlert();
        clearResumen();

        if (!idVehiculo) {
            return;
        }

        setLoading(true);

        try {
            var url = config.configuracionUrlTemplate.replace("__ID__", idVehiculo);
            var response = await fetch(url);
            var result = await response.json();

            if (!result.success) {
                throw new Error(result.message || "No fue posible cargar la configuración del vehículo.");
            }

            renderResumen(result.data);
        } catch (error) {
            showAlert(error.message);
        } finally {
            setLoading(false);
        }
    };

    var bindEvents = function () {
        selectorVehiculo.addEventListener("change", function () {
            cargarConfiguracion(selectorVehiculo.value);
        });
    };

    var init = async function () {
        selectorVehiculo = document.getElementById("llanta-asignacion-vehiculo");
        alerta = document.getElementById("llanta-asignacion-alerta");
        resumen = document.getElementById("llanta-asignacion-resumen");
        loading = document.getElementById("llanta-asignacion-loading");
        diagrama = document.getElementById("llanta-asignacion-diagrama");
        config = window.SUVAN && window.SUVAN.LlantaAsignacion ? window.SUVAN.LlantaAsignacion : {};

        if (!selectorVehiculo || !config.configuracionUrlTemplate) {
            return;
        }

        try {
            initSelectVehiculo();
            bindEvents();
        } catch (error) {
            showAlert(error.message);
        }
    };

    return {
        init: init
    };
}();

document.addEventListener("DOMContentLoaded", function () {
    SuvanLlantaAsignacion.init();
});
