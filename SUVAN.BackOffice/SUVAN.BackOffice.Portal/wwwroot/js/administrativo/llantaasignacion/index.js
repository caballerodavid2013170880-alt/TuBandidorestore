"use strict";

var SuvanLlantaAsignacion = function () {
    var selectorVehiculo;
    var alerta;
    var resumen;
    var loading;
    var diagrama;
    var detalle;
    var modalInstalacionElement;
    var modalInstalacion;
    var formInstalacion;
    var selectLlantaInstalacion;
    var submitInstalacion;
    var alertaInstalacion;
    var modalRetiroElement;
    var modalRetiro;
    var formRetiro;
    var selectMotivoRetiro;
    var selectEstadoRetiro;
    var submitRetiro;
    var alertaRetiro;
    var botonRetiroReemplazar;
    var modalReemplazoElement;
    var modalReemplazo;
    var formReemplazo;
    var selectLlantaReemplazo;
    var selectMotivoReemplazo;
    var selectEstadoSalienteReemplazo;
    var submitReemplazo;
    var alertaReemplazo;
    var config;
    var configuracionActual;
    var configuracionRequestId = 0;
    var posicionInstalacionActual;
    var posicionRetiroActual;
    var posicionReemplazoActual;

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

    var postForm = function (url, formData) {
        return fetch(url, {
            method: "POST",
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            },
            body: formData
        }).then(function (response) {
            if (!response.ok) {
                throw new Error("No fue posible guardar la instalación.");
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

    var showModalAlert = function (message) {
        if (!alertaInstalacion) {
            return;
        }

        alertaInstalacion.textContent = message || "No fue posible guardar la instalación.";
        alertaInstalacion.classList.remove("d-none");
    };

    var hideModalAlert = function () {
        if (alertaInstalacion) {
            alertaInstalacion.classList.add("d-none");
            alertaInstalacion.textContent = "";
        }
    };

    var showRetiroAlert = function (message) {
        if (!alertaRetiro) {
            return;
        }

        alertaRetiro.textContent = message || "No fue posible guardar el retiro.";
        alertaRetiro.classList.remove("d-none");
    };

    var hideRetiroAlert = function () {
        if (alertaRetiro) {
            alertaRetiro.classList.add("d-none");
            alertaRetiro.textContent = "";
        }
    };

    var showReemplazoAlert = function (message) {
        if (!alertaReemplazo) {
            return;
        }

        alertaReemplazo.textContent = message || "No fue posible guardar el reemplazo.";
        alertaReemplazo.classList.remove("d-none");
    };

    var hideReemplazoAlert = function () {
        if (alertaReemplazo) {
            alertaReemplazo.classList.add("d-none");
            alertaReemplazo.textContent = "";
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

    var setSelectOptions = function (select, placeholder, items) {
        if (!select) {
            return;
        }

        select.innerHTML = "";
        select.appendChild(new Option(placeholder, ""));

        items.forEach(function (item) {
            select.appendChild(new Option(getValue(item, "Nombre", "nombre"), getValue(item, "Id", "id")));
        });

        if (window.jQuery) {
            window.jQuery(select).val("").trigger("change.select2");
        }
    };

    var setSubmitInstalacionLoading = function (isLoading) {
        if (!submitInstalacion) {
            return;
        }

        submitInstalacion.disabled = isLoading;
        submitInstalacion.setAttribute("data-kt-indicator", isLoading ? "on" : "off");
    };

    var setSubmitRetiroLoading = function (isLoading) {
        if (!submitRetiro) {
            return;
        }

        submitRetiro.disabled = isLoading;
        submitRetiro.setAttribute("data-kt-indicator", isLoading ? "on" : "off");
    };

    var setSubmitReemplazoLoading = function (isLoading) {
        if (!submitReemplazo) {
            return;
        }

        submitReemplazo.disabled = isLoading;
        submitReemplazo.setAttribute("data-kt-indicator", isLoading ? "on" : "off");
    };

    var abrirModalInstalacion = async function (eje, posicion) {
        if (!modalInstalacion || !formInstalacion) {
            return;
        }

        var idVehiculo = getValue(configuracionActual, "IdVehiculo", "idVehiculo");
        var idVehiculoEje = getValue(eje, "IdVehiculoEje", "idVehiculoEje");
        var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
        var numeroPosicion = getValue(posicion, "NumeroPosicion", "numeroPosicion");
        var kilometraje = getValue(configuracionActual, "KilometrajeActual", "kilometrajeActual");
        var today = new Date().toISOString().slice(0, 10);

        posicionInstalacionActual = {
            idVehiculo: idVehiculo,
            idVehiculoEje: idVehiculoEje,
            numeroPosicion: numeroPosicion
        };

        formInstalacion.reset();
        hideModalAlert();
        setSelectOptions(selectLlantaInstalacion, "Cargando llantas disponibles...", []);

        document.getElementById("llanta-instalacion-id-vehiculo").value = idVehiculo;
        document.getElementById("llanta-instalacion-id-vehiculo-eje").value = idVehiculoEje;
        document.getElementById("llanta-instalacion-numero-posicion").value = numeroPosicion;
        document.getElementById("llanta-instalacion-posicion").textContent = "Eje " + numeroEje + " - Posición " + numeroPosicion;
        document.getElementById("llanta-instalacion-fecha").value = today;
        document.getElementById("llanta-instalacion-km").value = kilometraje !== null && kilometraje !== undefined ? Math.trunc(Number(kilometraje)) : "";

        modalInstalacion.show();

        try {
            var result = await getJson(config.llantasDisponiblesUrl);

            if (!result.success) {
                throw new Error(result.message || "No fue posible cargar las llantas disponibles.");
            }

            setSelectOptions(selectLlantaInstalacion, "Seleccione una llanta", result.data || []);

            if (!result.data || !result.data.length) {
                showModalAlert("No hay llantas disponibles para instalar.");
            }
        } catch (error) {
            setSelectOptions(selectLlantaInstalacion, "No fue posible cargar llantas", []);
            showModalAlert(error.message);
        }
    };

    var abrirModalRetiro = async function (eje, posicion) {
        if (!modalRetiro || !formRetiro) {
            return;
        }

        var asignacion = getValue(posicion, "Asignacion", "asignacion");
        if (!asignacion) {
            return;
        }

        var idVehiculo = getValue(configuracionActual, "IdVehiculo", "idVehiculo");
        var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
        var numeroPosicion = getValue(posicion, "NumeroPosicion", "numeroPosicion");
        var kilometraje = getValue(configuracionActual, "KilometrajeActual", "kilometrajeActual");
        var codigoLlanta = getValue(asignacion, "CodigoLlanta", "codigoLlanta") || "-";
        var serie = getValue(asignacion, "NumeroSerieDot", "numeroSerieDot") || "-";
        var today = new Date().toISOString().slice(0, 10);

        posicionRetiroActual = {
            idVehiculo: idVehiculo,
            idLlantaAsignacion: getValue(asignacion, "IdLlantaAsignacion", "idLlantaAsignacion"),
            eje: eje,
            posicion: posicion
        };

        formRetiro.reset();
        hideRetiroAlert();
        setSelectOptions(selectMotivoRetiro, "Cargando motivos...", []);
        setSelectOptions(selectEstadoRetiro, "Cargando estados...", []);

        document.getElementById("llanta-retiro-id-asignacion").value = posicionRetiroActual.idLlantaAsignacion;
        document.getElementById("llanta-retiro-posicion").textContent = "Eje " + numeroEje + " - Posición " + numeroPosicion;
        document.getElementById("llanta-retiro-llanta").value = codigoLlanta + " - " + serie;
        document.getElementById("llanta-retiro-fecha").value = today;
        document.getElementById("llanta-retiro-km").value = kilometraje !== null && kilometraje !== undefined ? Math.trunc(Number(kilometraje)) : "";

        modalRetiro.show();

        try {
            var result = await getJson(config.catalogosRetiroUrl);

            if (!result.success) {
                throw new Error(result.message || "No fue posible cargar los catálogos de retiro.");
            }

            var data = result.data || {};
            var motivos = getValue(data, "Motivos", "motivos") || [];
            var estadosDestino = getValue(data, "EstadosDestino", "estadosDestino") || [];

            setSelectOptions(selectMotivoRetiro, "Seleccione un motivo", motivos);
            setSelectOptions(selectEstadoRetiro, "Seleccione un estado", estadosDestino);

            if (!motivos.length) {
                showRetiroAlert("No hay motivos de retiro activos.");
            } else if (!estadosDestino.length) {
                showRetiroAlert("No hay estados destino activos para retiro.");
            }
        } catch (error) {
            setSelectOptions(selectMotivoRetiro, "No fue posible cargar motivos", []);
            setSelectOptions(selectEstadoRetiro, "No fue posible cargar estados", []);
            showRetiroAlert(error.message);
        }
    };

    var abrirModalReemplazo = async function (eje, posicion) {
        if (!modalReemplazo || !formReemplazo) {
            return;
        }

        var asignacion = getValue(posicion, "Asignacion", "asignacion");
        if (!asignacion) {
            return;
        }

        var idVehiculo = getValue(configuracionActual, "IdVehiculo", "idVehiculo");
        var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
        var numeroPosicion = getValue(posicion, "NumeroPosicion", "numeroPosicion");
        var kilometraje = getValue(configuracionActual, "KilometrajeActual", "kilometrajeActual");
        var codigoLlanta = getValue(asignacion, "CodigoLlanta", "codigoLlanta") || "-";
        var serie = getValue(asignacion, "NumeroSerieDot", "numeroSerieDot") || "-";
        var today = new Date().toISOString().slice(0, 10);

        posicionReemplazoActual = {
            idVehiculo: idVehiculo,
            idLlantaAsignacion: getValue(asignacion, "IdLlantaAsignacion", "idLlantaAsignacion")
        };

        formReemplazo.reset();
        hideReemplazoAlert();
        setSelectOptions(selectLlantaReemplazo, "Cargando llantas disponibles...", []);
        setSelectOptions(selectMotivoReemplazo, "Cargando motivos...", []);
        setSelectOptions(selectEstadoSalienteReemplazo, "Cargando estados...", []);

        document.getElementById("llanta-reemplazo-id-asignacion").value = posicionReemplazoActual.idLlantaAsignacion;
        document.getElementById("llanta-reemplazo-posicion").textContent = "Eje " + numeroEje + " - Posición " + numeroPosicion;
        document.getElementById("llanta-reemplazo-saliente").value = codigoLlanta + " - " + serie;
        document.getElementById("llanta-reemplazo-fecha").value = today;
        document.getElementById("llanta-reemplazo-km").value = kilometraje !== null && kilometraje !== undefined ? Math.trunc(Number(kilometraje)) : "";

        modalReemplazo.show();

        try {
            var results = await Promise.all([
                getJson(config.llantasDisponiblesUrl),
                getJson(config.catalogosRetiroUrl)
            ]);
            var llantasResult = results[0];
            var catalogosResult = results[1];

            if (!llantasResult.success) {
                throw new Error(llantasResult.message || "No fue posible cargar las llantas disponibles.");
            }

            if (!catalogosResult.success) {
                throw new Error(catalogosResult.message || "No fue posible cargar los catálogos de retiro.");
            }

            var dataCatalogos = catalogosResult.data || {};
            var motivos = getValue(dataCatalogos, "Motivos", "motivos") || [];
            var estadosDestino = getValue(dataCatalogos, "EstadosDestino", "estadosDestino") || [];

            setSelectOptions(selectLlantaReemplazo, "Seleccione una llanta", llantasResult.data || []);
            setSelectOptions(selectMotivoReemplazo, "Seleccione un motivo", motivos);
            setSelectOptions(selectEstadoSalienteReemplazo, "Seleccione un estado", estadosDestino);

            if (!llantasResult.data || !llantasResult.data.length) {
                showReemplazoAlert("No hay llantas disponibles para instalar como reemplazo.");
            } else if (!motivos.length) {
                showReemplazoAlert("No hay motivos de retiro activos.");
            } else if (!estadosDestino.length) {
                showReemplazoAlert("No hay estados destino activos para la llanta saliente.");
            }
        } catch (error) {
            setSelectOptions(selectLlantaReemplazo, "No fue posible cargar llantas", []);
            setSelectOptions(selectMotivoReemplazo, "No fue posible cargar motivos", []);
            setSelectOptions(selectEstadoSalienteReemplazo, "No fue posible cargar estados", []);
            showReemplazoAlert(error.message);
        }
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

            if (selectLlantaInstalacion) {
                window.jQuery(selectLlantaInstalacion).select2({
                    width: "100%",
                    dropdownParent: window.jQuery(modalInstalacionElement),
                    placeholder: selectLlantaInstalacion.getAttribute("data-placeholder") || "Seleccione"
                });
            }

            if (selectMotivoRetiro) {
                window.jQuery(selectMotivoRetiro).select2({
                    width: "100%",
                    dropdownParent: window.jQuery(modalRetiroElement),
                    placeholder: selectMotivoRetiro.getAttribute("data-placeholder") || "Seleccione"
                });
            }

            if (selectEstadoRetiro) {
                window.jQuery(selectEstadoRetiro).select2({
                    width: "100%",
                    dropdownParent: window.jQuery(modalRetiroElement),
                    placeholder: selectEstadoRetiro.getAttribute("data-placeholder") || "Seleccione"
                });
            }

            if (selectLlantaReemplazo) {
                window.jQuery(selectLlantaReemplazo).select2({
                    width: "100%",
                    dropdownParent: window.jQuery(modalReemplazoElement),
                    placeholder: selectLlantaReemplazo.getAttribute("data-placeholder") || "Seleccione"
                });
            }

            if (selectMotivoReemplazo) {
                window.jQuery(selectMotivoReemplazo).select2({
                    width: "100%",
                    dropdownParent: window.jQuery(modalReemplazoElement),
                    placeholder: selectMotivoReemplazo.getAttribute("data-placeholder") || "Seleccione"
                });
            }

            if (selectEstadoSalienteReemplazo) {
                window.jQuery(selectEstadoSalienteReemplazo).select2({
                    width: "100%",
                    dropdownParent: window.jQuery(modalReemplazoElement),
                    placeholder: selectEstadoSalienteReemplazo.getAttribute("data-placeholder") || "Seleccione"
                });
            }
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
                var asignacion = getValue(posicion, "Asignacion", "asignacion");
                var ocupada = getValue(posicion, "Ocupada", "ocupada") === true;

                diagrama.querySelectorAll(".llanta-posicion").forEach(function (item) {
                    item.classList.remove("active");
                });
                button.classList.add("active");

                renderDetallePosicion(eje, posicion);

                if (!ocupada && !asignacion) {
                    abrirModalInstalacion(eje, posicion);
                } else {
                    abrirModalRetiro(eje, posicion);
                }
            });
        }

        if (formInstalacion) {
            formInstalacion.addEventListener("submit", function (event) {
                event.preventDefault();
                hideModalAlert();

                if (!posicionInstalacionActual) {
                    showModalAlert("Selecciona una posición libre.");
                    return;
                }

                if (!selectLlantaInstalacion.value) {
                    showModalAlert("Selecciona una llanta disponible.");
                    return;
                }

                var fecha = document.getElementById("llanta-instalacion-fecha").value;
                var kilometraje = document.getElementById("llanta-instalacion-km").value;

                if (!fecha) {
                    showModalAlert("Captura la fecha de instalación.");
                    return;
                }

                if (kilometraje === "" || Number(kilometraje) < 0) {
                    showModalAlert("Captura un kilometraje válido.");
                    return;
                }

                setSubmitInstalacionLoading(true);

                postForm(config.instalarUrl, new FormData(formInstalacion))
                    .then(function (result) {
                        if (!result.success) {
                            throw new Error(result.message || "No fue posible instalar la llanta.");
                        }

                        modalInstalacion.hide();
                        return cargarConfiguracion(posicionInstalacionActual.idVehiculo);
                    })
                    .catch(function (error) {
                        showModalAlert(error.message);
                    })
                    .finally(function () {
                        setSubmitInstalacionLoading(false);
                    });
            });
        }

        if (formRetiro) {
            formRetiro.addEventListener("submit", function (event) {
                event.preventDefault();
                hideRetiroAlert();

                if (!posicionRetiroActual) {
                    showRetiroAlert("Selecciona una posición ocupada.");
                    return;
                }

                if (!selectMotivoRetiro.value) {
                    showRetiroAlert("Selecciona un motivo de retiro.");
                    return;
                }

                if (!selectEstadoRetiro.value) {
                    showRetiroAlert("Selecciona un estado destino.");
                    return;
                }

                var fecha = document.getElementById("llanta-retiro-fecha").value;
                var kilometraje = document.getElementById("llanta-retiro-km").value;

                if (!fecha) {
                    showRetiroAlert("Captura la fecha de retiro.");
                    return;
                }

                if (kilometraje === "" || Number(kilometraje) < 0) {
                    showRetiroAlert("Captura un kilometraje válido.");
                    return;
                }

                setSubmitRetiroLoading(true);

                postForm(config.retirarUrl, new FormData(formRetiro))
                    .then(function (result) {
                        if (!result.success) {
                            throw new Error(result.message || "No fue posible retirar la llanta.");
                        }

                        modalRetiro.hide();
                        return cargarConfiguracion(posicionRetiroActual.idVehiculo);
                    })
                    .catch(function (error) {
                        showRetiroAlert(error.message);
                    })
                    .finally(function () {
                        setSubmitRetiroLoading(false);
                    });
            });
        }

        if (botonRetiroReemplazar) {
            botonRetiroReemplazar.addEventListener("click", function () {
                if (!posicionRetiroActual || !posicionRetiroActual.eje || !posicionRetiroActual.posicion) {
                    showRetiroAlert("Selecciona una posición ocupada.");
                    return;
                }

                if (modalRetiro) {
                    modalRetiro.hide();
                }

                abrirModalReemplazo(posicionRetiroActual.eje, posicionRetiroActual.posicion);
            });
        }

        if (formReemplazo) {
            formReemplazo.addEventListener("submit", function (event) {
                event.preventDefault();
                hideReemplazoAlert();

                if (!posicionReemplazoActual) {
                    showReemplazoAlert("Selecciona una posición ocupada.");
                    return;
                }

                if (!selectLlantaReemplazo.value) {
                    showReemplazoAlert("Selecciona una llanta entrante.");
                    return;
                }

                if (!selectMotivoReemplazo.value) {
                    showReemplazoAlert("Selecciona un motivo de retiro.");
                    return;
                }

                if (!selectEstadoSalienteReemplazo.value) {
                    showReemplazoAlert("Selecciona el estado destino de la llanta saliente.");
                    return;
                }

                var fecha = document.getElementById("llanta-reemplazo-fecha").value;
                var kilometraje = document.getElementById("llanta-reemplazo-km").value;

                if (!fecha) {
                    showReemplazoAlert("Captura la fecha de reemplazo.");
                    return;
                }

                if (kilometraje === "" || Number(kilometraje) < 0) {
                    showReemplazoAlert("Captura un kilometraje válido.");
                    return;
                }

                setSubmitReemplazoLoading(true);

                postForm(config.reemplazarUrl, new FormData(formReemplazo))
                    .then(function (result) {
                        if (!result.success) {
                            throw new Error(result.message || "No fue posible reemplazar la llanta.");
                        }

                        modalReemplazo.hide();
                        return cargarConfiguracion(posicionReemplazoActual.idVehiculo);
                    })
                    .catch(function (error) {
                        showReemplazoAlert(error.message);
                    })
                    .finally(function () {
                        setSubmitReemplazoLoading(false);
                    });
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
        modalInstalacionElement = document.getElementById("llanta-instalacion-modal");
        formInstalacion = document.getElementById("llanta-instalacion-form");
        selectLlantaInstalacion = document.getElementById("llanta-instalacion-id-llanta");
        submitInstalacion = document.getElementById("llanta-instalacion-submit");
        alertaInstalacion = document.getElementById("llanta-instalacion-alerta");
        modalRetiroElement = document.getElementById("llanta-retiro-modal");
        formRetiro = document.getElementById("llanta-retiro-form");
        selectMotivoRetiro = document.getElementById("llanta-retiro-motivo");
        selectEstadoRetiro = document.getElementById("llanta-retiro-estado");
        submitRetiro = document.getElementById("llanta-retiro-submit");
        alertaRetiro = document.getElementById("llanta-retiro-alerta");
        botonRetiroReemplazar = document.getElementById("llanta-retiro-reemplazar");
        modalReemplazoElement = document.getElementById("llanta-reemplazo-modal");
        formReemplazo = document.getElementById("llanta-reemplazo-form");
        selectLlantaReemplazo = document.getElementById("llanta-reemplazo-entrante");
        selectMotivoReemplazo = document.getElementById("llanta-reemplazo-motivo");
        selectEstadoSalienteReemplazo = document.getElementById("llanta-reemplazo-estado-saliente");
        submitReemplazo = document.getElementById("llanta-reemplazo-submit");
        alertaReemplazo = document.getElementById("llanta-reemplazo-alerta");
        config = window.SUVAN && window.SUVAN.LlantaAsignacion ? window.SUVAN.LlantaAsignacion : {};

        if (!selectorVehiculo || !config.configuracionUrlTemplate) {
            return;
        }

        try {
            if (modalInstalacionElement && window.bootstrap) {
                modalInstalacion = new bootstrap.Modal(modalInstalacionElement);
            }

            if (modalRetiroElement && window.bootstrap) {
                modalRetiro = new bootstrap.Modal(modalRetiroElement);
            }

            if (modalReemplazoElement && window.bootstrap) {
                modalReemplazo = new bootstrap.Modal(modalReemplazoElement);
            }

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
