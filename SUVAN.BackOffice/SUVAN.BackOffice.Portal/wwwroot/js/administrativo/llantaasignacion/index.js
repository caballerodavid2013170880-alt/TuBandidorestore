"use strict";

var SuvanLlantaAsignacion = function () {
    var selectorVehiculo;
    var alerta;
    var resumen;
    var loading;
    var diagrama;
    var detalle;
    var config;
    var configuracionActual;
    var configuracionRequestId = 0;

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

    var escapeHtml = function (value) {
        if (value === null || value === undefined) {
            return "";
        }

        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    };

    var formatNumber = function (value) {
        return value !== null && value !== undefined ? Number(value).toLocaleString("es-MX") : "-";
    };

    var normalizeText = function (value) {
        return String(value || "")
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    };

    var getEstadoPosicion = function (posicion) {
        var asignacion = getValue(posicion, "Asignacion", "asignacion");
        var ocupada = getValue(posicion, "Ocupada", "ocupada") === true;
        var estadoLlanta = asignacion ? normalizeText(getValue(asignacion, "EstadoLlanta", "estadoLlanta")) : "";

        if (estadoLlanta.indexOf("fuera") >= 0 || estadoLlanta.indexOf("baja") >= 0) {
            return {
                key: "fuera-servicio",
                label: "Fuera de servicio"
            };
        }

        if (estadoLlanta.indexOf("advert") >= 0 || estadoLlanta.indexOf("observ") >= 0 || estadoLlanta.indexOf("repar") >= 0) {
            return {
                key: "advertencia",
                label: "Advertencia"
            };
        }

        return ocupada
            ? {
                key: "ocupada",
                label: "Ocupada"
            }
            : {
                key: "libre",
                label: "Libre"
            };
    };

    var renderLlantaButton = function (eje, posicion, ejeIndex, posicionIndex, lado) {
        var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
        var numeroPosicion = getValue(posicion, "NumeroPosicion", "numeroPosicion");
        var estado = getEstadoPosicion(posicion);
        var asignacion = getValue(posicion, "Asignacion", "asignacion");
        var codigoLlanta = asignacion ? getValue(asignacion, "CodigoLlanta", "codigoLlanta") : null;
        var ariaLabel = [
            "Eje " + numeroEje,
            "posicion " + numeroPosicion,
            lado,
            estado.label
        ].join(", ");

        return [
            '<button type="button" class="llanta-posicion llanta-wheel llanta-wheel--' + estado.key + '"',
            ' data-eje-index="' + ejeIndex + '"',
            ' data-posicion-index="' + posicionIndex + '"',
            ' aria-label="' + escapeHtml(ariaLabel) + '"',
            ' title="' + escapeHtml(ariaLabel) + '">',
            '<span class="llanta-wheel__tread" aria-hidden="true"></span>',
            '<span class="llanta-wheel__number">' + escapeHtml(numeroPosicion) + '</span>',
            codigoLlanta ? '<span class="llanta-wheel__code">' + escapeHtml(codigoLlanta) + '</span>' : '',
            '<span class="llanta-wheel__state">' + escapeHtml(estado.label) + '</span>',
            '</button>'
        ].join("");
    };

    var getJson = function (url) {
        return fetch(url, {
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        }).then(function (response) {
            if (!response.ok) {
                throw new Error("No fue posible cargar la configuración del vehículo.");
            }

            return response.json();
        });
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

        renderDetalleDefault();
    };

    var renderDetalleDefault = function () {
        if (!detalle) {
            return;
        }

        detalle.innerHTML = [
            '<div class="fw-bold text-gray-800 mb-2">Detalle de posición</div>',
            '<div class="text-muted">Sin posición seleccionada.</div>'
        ].join("");
    };

    var renderDetallePosicion = function (eje, posicion) {
        if (!detalle) {
            return;
        }

        var asignacion = getValue(posicion, "Asignacion", "asignacion");
        var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
        var numeroPosicion = getValue(posicion, "NumeroPosicion", "numeroPosicion");
        var tipoEje = getValue(eje, "NombreTipoEje", "nombreTipoEje") || getValue(eje, "DescripcionTipoEje", "descripcionTipoEje");

        if (!asignacion) {
            detalle.innerHTML = [
                '<div class="d-flex align-items-center justify-content-between mb-4">',
                '<div>',
                '<div class="fw-bold text-gray-800">Eje ' + escapeHtml(numeroEje) + ' - Posición ' + escapeHtml(numeroPosicion) + '</div>',
                '<div class="text-muted">' + escapeHtml(tipoEje || "-") + '</div>',
                '</div>',
                '<span class="badge badge-light-success">Libre</span>',
                '</div>',
                '<div class="text-muted">La posición no tiene una llanta instalada.</div>'
            ].join("");
            return;
        }

        detalle.innerHTML = [
            '<div class="d-flex align-items-center justify-content-between mb-4">',
            '<div>',
            '<div class="fw-bold text-gray-800">Eje ' + escapeHtml(numeroEje) + ' - Posición ' + escapeHtml(numeroPosicion) + '</div>',
            '<div class="text-muted">' + escapeHtml(tipoEje || "-") + '</div>',
            '</div>',
            '<span class="badge badge-light-primary">Ocupada</span>',
            '</div>',
            '<div class="separator separator-dashed my-4"></div>',
            '<div class="mb-3">',
            '<div class="fw-semibold text-muted">Código de llanta</div>',
            '<div class="fw-bold text-gray-800">' + escapeHtml(getValue(asignacion, "CodigoLlanta", "codigoLlanta") || "-") + '</div>',
            '</div>',
            '<div class="mb-3">',
            '<div class="fw-semibold text-muted">Serie DOT</div>',
            '<div class="fw-bold text-gray-800">' + escapeHtml(getValue(asignacion, "NumeroSerieDot", "numeroSerieDot") || "-") + '</div>',
            '</div>',
            '<div class="mb-3">',
            '<div class="fw-semibold text-muted">Marca / modelo</div>',
            '<div class="fw-bold text-gray-800">' + escapeHtml([getValue(asignacion, "Marca", "marca"), getValue(asignacion, "Modelo", "modelo")].filter(Boolean).join(" ") || "-") + '</div>',
            '</div>',
            '<div class="mb-3">',
            '<div class="fw-semibold text-muted">Estado</div>',
            '<div class="fw-bold text-gray-800">' + escapeHtml(getValue(asignacion, "EstadoLlanta", "estadoLlanta") || "-") + '</div>',
            '</div>',
            '<div>',
            '<div class="fw-semibold text-muted">Km instalación</div>',
            '<div class="fw-bold text-gray-800">' + escapeHtml(formatNumber(getValue(asignacion, "KmVehiculoAsignacion", "kmVehiculoAsignacion"))) + '</div>',
            '</div>'
        ].join("");
    };

    var renderDiagrama = function (ejes) {
        if (!diagrama) {
            return;
        }

        if (!ejes.length) {
            showAlert("El vehículo seleccionado no tiene una configuración de ejes.");
            diagrama.innerHTML = [
                '<i class="ki-outline ki-information-5 fs-2tx mb-4"></i>',
                '<div class="fw-semibold fs-6 text-gray-700">El vehículo no tiene ejes configurados.</div>',
                '<div class="text-muted">Configura sus ejes antes de asignar llantas.</div>'
            ].join("");
            renderDetalleDefault();
            return;
        }

        var html = [
            '<div class="llanta-vehicle-panel text-start">',
            '<div class="d-flex align-items-center justify-content-between flex-wrap gap-3 mb-5">',
            '<div>',
            '<div class="fw-bold text-gray-800 fs-5">Ejes y posiciones</div>',
            '<div class="text-muted">Vista superior del vehículo. Selecciona una llanta para ver el detalle.</div>',
            '</div>',
            '<div class="d-flex flex-wrap gap-2">',
            '<span class="badge badge-light-success">Libre</span>',
            '<span class="badge badge-light-primary">Ocupada</span>',
            '<span class="badge badge-light-warning">Advertencia</span>',
            '<span class="badge badge-light-danger">Fuera de servicio</span>',
            '</div>',
            '</div>',
            '<div class="llanta-vehicle-shell">',
            '<svg class="llanta-vehicle-silhouette" viewBox="0 0 320 640" preserveAspectRatio="none" aria-hidden="true" focusable="false">',
            '<path d="M102 20H218C247 64 266 121 272 190L286 592C286 610 272 624 254 624H66C48 624 34 610 34 592L48 190C54 121 73 64 102 20Z"></path>',
            '<path d="M96 56H224L246 160H74L96 56Z"></path>',
            '<path d="M70 518H250L244 604H76L70 518Z"></path>',
            '</svg>',
            '<div class="llanta-vehicle-front">Frente</div>',
            '<div class="llanta-vehicle-axles">'
        ];

        ejes.forEach(function (eje, ejeIndex) {
            var posiciones = getValue(eje, "Posiciones", "posiciones") || [];
            var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
            var tipoEje = getValue(eje, "NombreTipoEje", "nombreTipoEje") || getValue(eje, "DescripcionTipoEje", "descripcionTipoEje") || "Eje";
            var numeroPosiciones = getValue(eje, "NumeroPosiciones", "numeroPosiciones") || posiciones.length;
            var splitIndex = Math.ceil(posiciones.length / 2);
            var posicionesIzquierda = posiciones.slice(0, splitIndex);
            var posicionesDerecha = posiciones.slice(splitIndex);

            html.push(
                '<div class="llanta-vehicle-axle" style="--llanta-axis-order:' + ejeIndex + '">',
                '<div class="llanta-vehicle-side llanta-vehicle-side--left" aria-label="Lado izquierdo del eje ' + escapeHtml(numeroEje) + '">'
            );

            posicionesIzquierda.forEach(function (posicion, posicionIndex) {
                html.push(renderLlantaButton(eje, posicion, ejeIndex, posicionIndex, "izquierda"));
            });

            html.push(
                '</div>',
                '<div class="llanta-vehicle-axis-bar" aria-hidden="true"></div>',
                '<div class="llanta-vehicle-axis-label">',
                '<span class="llanta-vehicle-axis-title">Eje ' + escapeHtml(numeroEje) + '</span>',
                '<span class="llanta-vehicle-axis-type">' + escapeHtml(tipoEje) + '</span>',
                '<span class="llanta-vehicle-axis-count">' + escapeHtml(numeroPosiciones) + ' posiciones</span>',
                '</div>',
                '<div class="llanta-vehicle-side llanta-vehicle-side--right" aria-label="Lado derecho del eje ' + escapeHtml(numeroEje) + '">'
            );

            posicionesDerecha.forEach(function (posicion, posicionIndex) {
                html.push(renderLlantaButton(eje, posicion, ejeIndex, splitIndex + posicionIndex, "derecha"));
            });

            html.push('</div>', '</div>');
        });

        html.push('</div>', '</div>', '</div>');
        diagrama.innerHTML = html.join("");
        renderDetalleDefault();
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

        configuracionActual = data;
        renderDiagrama(ejes);
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
        var requestId = ++configuracionRequestId;

        hideAlert();
        clearResumen();

        if (!idVehiculo) {
            return;
        }

        setLoading(true);

        try {
            var url = config.configuracionUrlTemplate.replace("__ID__", idVehiculo);
            var result = await getJson(url);

            if (requestId !== configuracionRequestId) {
                return;
            }

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
        if (window.jQuery) {
            window.jQuery(selectorVehiculo).on("change", function () {
                cargarConfiguracion(window.jQuery(this).val());
            });
        } else {
            selectorVehiculo.addEventListener("change", function () {
                cargarConfiguracion(selectorVehiculo.value);
            });
        }

        if (diagrama) {
            diagrama.addEventListener("click", function (event) {
                var button = event.target.closest(".llanta-posicion");
                if (!button || !configuracionActual) {
                    return;
                }

                var ejes = getValue(configuracionActual, "Ejes", "ejes") || [];
                var eje = ejes[parseInt(button.getAttribute("data-eje-index"), 10)];
                var posiciones = getValue(eje, "Posiciones", "posiciones") || [];
                var posicion = posiciones[parseInt(button.getAttribute("data-posicion-index"), 10)];

                diagrama.querySelectorAll(".llanta-posicion").forEach(function (item) {
                    item.classList.remove("active");
                });
                button.classList.add("active");

                renderDetallePosicion(eje, posicion);
            });
        }
    };

    var init = async function () {
        selectorVehiculo = document.getElementById("llanta-asignacion-vehiculo");
        alerta = document.getElementById("llanta-asignacion-alerta");
        resumen = document.getElementById("llanta-asignacion-resumen");
        loading = document.getElementById("llanta-asignacion-loading");
        diagrama = document.getElementById("llanta-asignacion-diagrama");
        detalle = document.getElementById("llanta-asignacion-detalle");
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
