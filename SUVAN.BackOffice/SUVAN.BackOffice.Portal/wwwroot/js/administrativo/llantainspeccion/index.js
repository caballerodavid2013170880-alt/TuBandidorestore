"use strict";

var SuvanLlantaInspeccion = function () {
    var selectorVehiculo;
    var selectorTipo;
    var inputFecha;
    var inputKilometraje;
    var alerta;
    var exito;
    var resumen;
    var loading;
    var diagrama;
    var tabla;
    var captura;
    var detalles;
    var botonSeleccionarTodas;
    var botonLimpiar;
    var botonGuardar;
    var form;
    var config;
    var configuracionActual;
    var requestId = 0;
    var seleccionadas = [];

    var getValue = function (source, pascalName, camelName) {
        if (!source) {
            return null;
        }

        if (source[pascalName] !== undefined && source[pascalName] !== null) {
            return source[pascalName];
        }

        return source[camelName] !== undefined ? source[camelName] : null;
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
        if (value === null || value === undefined || value === "") {
            return "-";
        }

        var number = Number(value);
        return Number.isNaN(number) ? "-" : number.toLocaleString("es-MX");
    };

    var formatDecimal = function (value) {
        if (value === null || value === undefined || value === "") {
            return "-";
        }

        var number = Number(value);
        return Number.isNaN(number) ? "-" : number.toLocaleString("es-MX", { maximumFractionDigits: 2 });
    };

    var setText = function (id, value) {
        var element = document.getElementById(id);
        if (element) {
            element.textContent = value || "-";
        }
    };

    var showAlert = function (message) {
        if (!alerta) {
            return;
        }

        alerta.textContent = message || "No fue posible procesar la solicitud.";
        alerta.classList.remove("d-none");
        if (exito) {
            exito.classList.add("d-none");
            exito.textContent = "";
        }
    };

    var hideAlert = function () {
        if (alerta) {
            alerta.classList.add("d-none");
            alerta.textContent = "";
        }
    };

    var showSuccess = function (message) {
        if (!exito) {
            return;
        }

        exito.textContent = message || "Operación realizada correctamente.";
        exito.classList.remove("d-none");
        hideAlert();
    };

    var setLoading = function (isLoading) {
        if (!loading) {
            return;
        }

        loading.classList.toggle("d-none", !isLoading);
        loading.classList.toggle("d-flex", isLoading);
    };

    var getJson = function (url) {
        return fetch(url, {
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        }).then(function (response) {
            if (!response.ok) {
                throw new Error("No fue posible cargar la información.");
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
                throw new Error("No fue posible guardar la inspección.");
            }

            return response.json();
        });
    };

    var getAsignacion = function (posicion) {
        return getValue(posicion, "Asignacion", "asignacion");
    };

    var getAllInstaladas = function () {
        var ejes = configuracionActual ? getValue(configuracionActual, "Ejes", "ejes") || [] : [];
        var result = [];

        ejes.forEach(function (eje) {
            var posiciones = getValue(eje, "Posiciones", "posiciones") || [];
            posiciones.forEach(function (posicion) {
                var asignacion = getAsignacion(posicion);

                if (asignacion) {
                    result.push({
                        eje: eje,
                        posicion: posicion,
                        asignacion: asignacion
                    });
                }
            });
        });

        return result;
    };

    var isSelected = function (idAsignacion) {
        return seleccionadas.some(function (item) {
            return String(item.idLlantaAsignacion) === String(idAsignacion);
        });
    };

    var findSelectedIndex = function (idAsignacion) {
        return seleccionadas.findIndex(function (item) {
            return String(item.idLlantaAsignacion) === String(idAsignacion);
        });
    };

    var buildSelectedItem = function (eje, posicion, asignacion) {
        return {
            idLlantaAsignacion: getValue(asignacion, "IdLlantaAsignacion", "idLlantaAsignacion"),
            idLlanta: getValue(asignacion, "IdLlanta", "idLlanta"),
            codigoLlanta: getValue(asignacion, "CodigoLlanta", "codigoLlanta"),
            numeroSerieDot: getValue(asignacion, "NumeroSerieDot", "numeroSerieDot"),
            marca: getValue(asignacion, "Marca", "marca"),
            modelo: getValue(asignacion, "Modelo", "modelo"),
            medida: getValue(asignacion, "Medida", "medida"),
            estadoLlanta: getValue(asignacion, "EstadoLlanta", "estadoLlanta"),
            numeroEje: getValue(eje, "NumeroEje", "numeroEje"),
            numeroPosicion: getValue(posicion, "NumeroPosicion", "numeroPosicion"),
            presionMinimaPsi: getValue(asignacion, "PresionMinimaPsi", "presionMinimaPsi"),
            presionMaximaPsi: getValue(asignacion, "PresionMaximaPsi", "presionMaximaPsi"),
            profundidadOriginalMm: getValue(asignacion, "ProfundidadOriginalMm", "profundidadOriginalMm"),
            profundidadAlertaMm: getValue(asignacion, "ProfundidadAlertaMm", "profundidadAlertaMm"),
            profundidadMinimaMm: getValue(asignacion, "ProfundidadMinimaMm", "profundidadMinimaMm"),
            vidaUtilEstimadaKm: getValue(asignacion, "VidaUtilEstimadaKm", "vidaUtilEstimadaKm")
        };
    };

    var toggleSeleccion = function (eje, posicion, asignacion) {
        var idAsignacion = getValue(asignacion, "IdLlantaAsignacion", "idLlantaAsignacion");
        var index = findSelectedIndex(idAsignacion);

        if (index >= 0) {
            seleccionadas.splice(index, 1);
        } else {
            seleccionadas.push(buildSelectedItem(eje, posicion, asignacion));
        }

        renderSelectionState();
    };

    var setTodas = function () {
        seleccionadas = getAllInstaladas().map(function (item) {
            return buildSelectedItem(item.eje, item.posicion, item.asignacion);
        });
        renderSelectionState();
    };

    var limpiarSeleccion = function () {
        seleccionadas = [];
        renderSelectionState();
    };

    var renderLlantaButton = function (eje, posicion, ejeIndex, posicionIndex, lado) {
        var asignacion = getAsignacion(posicion);
        var numeroEje = getValue(eje, "NumeroEje", "numeroEje");
        var numeroPosicion = getValue(posicion, "NumeroPosicion", "numeroPosicion");
        var ocupada = getValue(posicion, "Ocupada", "ocupada") === true;
        var codigoLlanta = asignacion ? getValue(asignacion, "CodigoLlanta", "codigoLlanta") : null;
        var idAsignacion = asignacion ? getValue(asignacion, "IdLlantaAsignacion", "idLlantaAsignacion") : "";
        var selected = asignacion && isSelected(idAsignacion);
        var estadoKey = ocupada ? "ocupada" : "libre";
        var estadoLabel = ocupada ? "Instalada" : "Libre";
        var ariaLabel = [
            "Eje " + numeroEje,
            "posicion " + numeroPosicion,
            lado,
            estadoLabel
        ].join(", ");

        return [
            '<button type="button" class="llanta-posicion llanta-wheel llanta-wheel--' + estadoKey + (selected ? ' active' : '') + '"',
            ' data-eje-index="' + ejeIndex + '"',
            ' data-posicion-index="' + posicionIndex + '"',
            asignacion ? ' data-id-asignacion="' + escapeHtml(idAsignacion) + '"' : '',
            asignacion ? '' : ' disabled',
            ' aria-label="' + escapeHtml(ariaLabel) + '"',
            ' title="' + escapeHtml(ariaLabel) + '">',
            '<span class="llanta-wheel__tread" aria-hidden="true"></span>',
            '<span class="llanta-wheel__number">' + escapeHtml(numeroPosicion) + '</span>',
            codigoLlanta ? '<span class="llanta-wheel__code">' + escapeHtml(codigoLlanta) + '</span>' : '',
            '<span class="llanta-wheel__state">' + escapeHtml(selected ? "Seleccionada" : estadoLabel) + '</span>',
            '</button>'
        ].join("");
    };

    var renderDiagrama = function () {
        if (!diagrama) {
            return;
        }

        var ejes = configuracionActual ? getValue(configuracionActual, "Ejes", "ejes") || [] : [];

        if (!ejes.length) {
            diagrama.innerHTML = [
                '<i class="ki-outline ki-information-5 fs-2tx mb-4"></i>',
                '<div class="fw-semibold fs-6 text-gray-700">El vehículo no tiene ejes configurados.</div>'
            ].join("");
            return;
        }

        var html = [
            '<div class="llanta-vehicle-panel text-start">',
            '<div class="llanta-vehicle-header mb-3">',
            '<div class="fw-bold text-gray-800 fs-5">Ejes y posiciones</div>',
            '<div class="text-muted">Selecciona las llantas instaladas que serán inspeccionadas.</div>',
            '<div class="llanta-vehicle-legend d-flex flex-wrap gap-2 mt-2">',
            '<span class="badge badge-light-primary">Instalada</span>',
            '<span class="badge badge-light-success">Libre</span>',
            '<span class="badge badge-light-info">Seleccionada</span>',
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

            html.push(
                '<div class="llanta-vehicle-axle" style="--llanta-axis-order:' + ejeIndex + '">',
                '<div class="llanta-vehicle-side llanta-vehicle-side--left" aria-label="Lado izquierdo del eje ' + escapeHtml(numeroEje) + '">'
            );

            posiciones.slice(0, splitIndex).forEach(function (posicion, posicionIndex) {
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

            posiciones.slice(splitIndex).forEach(function (posicion, posicionIndex) {
                html.push(renderLlantaButton(eje, posicion, ejeIndex, splitIndex + posicionIndex, "derecha"));
            });

            html.push('</div>', '</div>');
        });

        html.push('</div>', '</div>', '</div>');
        diagrama.innerHTML = html.join("");
    };

    var renderTabla = function () {
        if (!tabla) {
            return;
        }

        var instaladas = getAllInstaladas();

        if (!configuracionActual) {
            tabla.innerHTML = '<div class="border border-dashed border-gray-300 rounded p-5 text-center text-muted">Selecciona un vehículo.</div>';
            return;
        }

        if (!instaladas.length) {
            tabla.innerHTML = '<div class="border border-dashed border-gray-300 rounded p-5 text-center text-muted">El vehículo no tiene llantas instaladas.</div>';
            return;
        }

        var html = [
            '<div class="table-responsive">',
            '<table class="table align-middle table-row-dashed fs-6 gy-4 mb-0">',
            '<thead><tr class="text-start text-muted fw-bold fs-7 text-uppercase gs-0">',
            '<th class="w-45px"></th>',
            '<th>Llanta</th>',
            '<th>Eje</th>',
            '<th>Posición</th>',
            '<th>Modelo</th>',
            '<th>Parámetros</th>',
            '</tr></thead><tbody>'
        ];

        instaladas.forEach(function (item) {
            var asignacion = item.asignacion;
            var idAsignacion = getValue(asignacion, "IdLlantaAsignacion", "idLlantaAsignacion");
            var selected = isSelected(idAsignacion);

            html.push(
                '<tr class="llanta-inspeccion-row' + (selected ? ' active' : '') + '" data-id-asignacion="' + escapeHtml(idAsignacion) + '">',
                '<td><input class="form-check-input llanta-inspeccion-check" type="checkbox" ' + (selected ? 'checked' : '') + ' /></td>',
                '<td><div class="fw-bold text-gray-800">' + escapeHtml(getValue(asignacion, "CodigoLlanta", "codigoLlanta")) + '</div><div class="text-muted">' + escapeHtml(getValue(asignacion, "NumeroSerieDot", "numeroSerieDot")) + '</div></td>',
                '<td>Eje ' + escapeHtml(getValue(item.eje, "NumeroEje", "numeroEje")) + '</td>',
                '<td>' + escapeHtml(getValue(item.posicion, "NumeroPosicion", "numeroPosicion")) + '</td>',
                '<td>' + escapeHtml([getValue(asignacion, "Marca", "marca"), getValue(asignacion, "Modelo", "modelo"), getValue(asignacion, "Medida", "medida")].filter(Boolean).join(" ")) + '</td>',
                '<td><span class="text-muted">PSI</span> ' + formatDecimal(getValue(asignacion, "PresionMinimaPsi", "presionMinimaPsi")) + ' - ' + formatDecimal(getValue(asignacion, "PresionMaximaPsi", "presionMaximaPsi")) + '<br/><span class="text-muted">Prof.</span> ' + formatDecimal(getValue(asignacion, "ProfundidadMinimaMm", "profundidadMinimaMm")) + ' / ' + formatDecimal(getValue(asignacion, "ProfundidadAlertaMm", "profundidadAlertaMm")) + ' mm</td>',
                '</tr>'
            );
        });

        html.push('</tbody></table></div>');
        tabla.innerHTML = html.join("");
    };

    var optionList = function (items, selectedValue) {
        var html = ['<option value="">Seleccione</option>'];

        (items || []).forEach(function (item) {
            var id = getValue(item, "Id", "id");
            var nombre = getValue(item, "Nombre", "nombre");
            html.push('<option value="' + escapeHtml(id) + '"' + (String(id) === String(selectedValue || "") ? ' selected' : '') + '>' + escapeHtml(nombre) + '</option>');
        });

        return html.join("");
    };

    var getPresionBadge = function (item, value) {
        if (value === null || value === undefined || value === "") {
            return { className: "badge-light", label: "Sin presión" };
        }

        var presion = Number(value);
        var minima = Number(item.presionMinimaPsi);
        var maxima = Number(item.presionMaximaPsi);

        if (Number.isNaN(presion) || Number.isNaN(minima) || Number.isNaN(maxima)) {
            return { className: "badge-light", label: "Sin parámetros" };
        }

        if (presion < minima) {
            return { className: "badge-light-danger", label: "Presión baja" };
        }

        if (presion > maxima) {
            return { className: "badge-light-warning", label: "Presión alta" };
        }

        return { className: "badge-light-success", label: "Presión normal" };
    };

    var getProfundidadBadge = function (item, value) {
        if (value === null || value === undefined || value === "") {
            return { className: "badge-light", label: "Sin profundidad" };
        }

        var profundidad = Number(value);
        var alertaMm = Number(item.profundidadAlertaMm);
        var minima = Number(item.profundidadMinimaMm);

        if (Number.isNaN(profundidad) || Number.isNaN(alertaMm) || Number.isNaN(minima)) {
            return { className: "badge-light", label: "Sin parámetros" };
        }

        if (profundidad <= minima) {
            return { className: "badge-light-danger", label: "Por cambiar" };
        }

        if (profundidad <= alertaMm) {
            return { className: "badge-light-warning", label: "Vida media" };
        }

        return { className: "badge-light-success", label: "Buena" };
    };

    var renderBadge = function (badge) {
        return '<span class="badge ' + badge.className + '">' + escapeHtml(badge.label) + '</span>';
    };

    var renderDetalles = function () {
        if (!detalles || !captura) {
            return;
        }

        captura.classList.toggle("d-none", !seleccionadas.length);

        if (!seleccionadas.length) {
            detalles.innerHTML = "";
            return;
        }

        var html = [];

        seleccionadas.forEach(function (item, index) {
            html.push(
                '<div class="llanta-inspeccion-detalle border rounded p-5 mb-5" data-index="' + index + '">',
                '<input type="hidden" name="Detalles[' + index + '].IdLlantaAsignacion" value="' + escapeHtml(item.idLlantaAsignacion) + '" />',
                '<div class="d-flex flex-wrap align-items-start justify-content-between gap-3 mb-4">',
                '<div>',
                '<div class="fw-bold text-gray-900 fs-5">' + escapeHtml(item.codigoLlanta) + '</div>',
                '<div class="text-muted">Eje ' + escapeHtml(item.numeroEje) + ' - Posición ' + escapeHtml(item.numeroPosicion) + ' · ' + escapeHtml([item.marca, item.modelo, item.medida].filter(Boolean).join(" ")) + '</div>',
                '</div>',
                '<button type="button" class="btn btn-icon btn-light btn-sm llanta-inspeccion-quitar" data-id-asignacion="' + escapeHtml(item.idLlantaAsignacion) + '" title="Quitar">',
                '<i class="ki-outline ki-cross fs-2"></i>',
                '</button>',
                '</div>',
                '<div class="llanta-inspeccion-parametros d-flex flex-wrap gap-2 mb-5">',
                '<span class="badge badge-light">PSI ' + formatDecimal(item.presionMinimaPsi) + ' - ' + formatDecimal(item.presionMaximaPsi) + '</span>',
                '<span class="badge badge-light">Profundidad original ' + formatDecimal(item.profundidadOriginalMm) + ' mm</span>',
                '<span class="badge badge-light">Alerta ' + formatDecimal(item.profundidadAlertaMm) + ' mm</span>',
                '<span class="badge badge-light">Mínima ' + formatDecimal(item.profundidadMinimaMm) + ' mm</span>',
                '</div>',
                '<div class="row g-5">',
                '<div class="col-12 col-md-3">',
                '<label class="required form-label">Profundidad mm</label>',
                '<input name="Detalles[' + index + '].ProfundidadMm" type="number" min="0" step="0.01" class="form-control form-control-solid llanta-inspeccion-profundidad" data-index="' + index + '" />',
                '<div class="mt-2 llanta-inspeccion-semaforo-profundidad">' + renderBadge(getProfundidadBadge(item, null)) + '</div>',
                '</div>',
                '<div class="col-12 col-md-3">',
                '<label class="required form-label">Presión PSI</label>',
                '<input name="Detalles[' + index + '].PresionPsi" type="number" min="0" step="0.01" class="form-control form-control-solid llanta-inspeccion-presion" data-index="' + index + '" />',
                '<div class="mt-2 llanta-inspeccion-semaforo-presion">' + renderBadge(getPresionBadge(item, null)) + '</div>',
                '</div>',
                '<div class="col-12 col-md-3">',
                '<label class="required form-label">Estado</label>',
                '<select name="Detalles[' + index + '].IdEstadoInspeccion" class="form-select form-select-solid">' + optionList(config.estadosInspeccion) + '</select>',
                '</div>',
                '<div class="col-12 col-md-3">',
                '<label class="required form-label">Conclusión</label>',
                '<select name="Detalles[' + index + '].IdConclusionInspeccion" class="form-select form-select-solid">' + optionList(config.conclusionesInspeccion) + '</select>',
                '</div>',
                '<div class="col-12">',
                '<label class="form-label">Observaciones</label>',
                '<textarea name="Detalles[' + index + '].Observaciones" class="form-control form-control-solid" rows="2" maxlength="1000"></textarea>',
                '</div>',
                '</div>',
                '</div>'
            );
        });

        detalles.innerHTML = html.join("");
    };

    var renderSelectionState = function () {
        var tieneConfiguracion = !!configuracionActual;
        var instaladas = getAllInstaladas();

        if (botonSeleccionarTodas) {
            botonSeleccionarTodas.disabled = !tieneConfiguracion || !instaladas.length;
        }

        if (botonLimpiar) {
            botonLimpiar.disabled = !seleccionadas.length;
        }

        renderDiagrama();
        renderTabla();
        renderDetalles();
    };

    var renderResumen = function (data) {
        var numeroEconomico = getValue(data, "NumeroEconomico", "numeroEconomico");
        var placas = getValue(data, "Placas", "placas");
        var marca = getValue(data, "Marca", "marca");
        var modelo = getValue(data, "Modelo", "modelo");
        var kilometraje = getValue(data, "KilometrajeActual", "kilometrajeActual");

        setText("llanta-inspeccion-numero-economico", numeroEconomico);
        setText("llanta-inspeccion-placas", placas);
        setText("llanta-inspeccion-unidad", [marca, modelo].filter(Boolean).join(" "));
        setText("llanta-inspeccion-kilometraje-actual", formatNumber(kilometraje));

        if (inputKilometraje && (inputKilometraje.value === "" || inputKilometraje.value === "0") && kilometraje !== null && kilometraje !== undefined) {
            inputKilometraje.value = Math.trunc(Number(kilometraje));
        }

        if (resumen) {
            resumen.classList.remove("d-none");
        }
    };

    var resetConfiguracion = function () {
        configuracionActual = null;
        seleccionadas = [];

        if (resumen) {
            resumen.classList.add("d-none");
        }

        if (diagrama) {
            diagrama.innerHTML = [
                '<i class="ki-outline ki-truck fs-2tx mb-4"></i>',
                '<div class="fw-semibold fs-6">Selecciona un vehículo para consultar sus llantas instaladas.</div>'
            ].join("");
        }

        renderSelectionState();
    };

    var cargarConfiguracion = async function () {
        var idVehiculo = selectorVehiculo ? selectorVehiculo.value : "";
        hideAlert();

        if (!idVehiculo) {
            resetConfiguracion();
            return;
        }

        var currentRequest = ++requestId;
        setLoading(true);
        resetConfiguracion();

        try {
            var url = config.configuracionUrlTemplate.replace("__ID__", encodeURIComponent(idVehiculo));
            var result = await getJson(url);

            if (currentRequest !== requestId) {
                return;
            }

            if (!result.success) {
                throw new Error(result.message || "No fue posible cargar la configuración del vehículo.");
            }

            configuracionActual = result.data || {};
            renderResumen(configuracionActual);
            renderSelectionState();
        } catch (error) {
            showAlert(error.message);
        } finally {
            if (currentRequest === requestId) {
                setLoading(false);
            }
        }
    };

    var getItemFromButton = function (button) {
        if (!button || !configuracionActual) {
            return null;
        }

        var ejeIndex = Number(button.getAttribute("data-eje-index"));
        var posicionIndex = Number(button.getAttribute("data-posicion-index"));
        var ejes = getValue(configuracionActual, "Ejes", "ejes") || [];
        var eje = ejes[ejeIndex];
        var posiciones = eje ? getValue(eje, "Posiciones", "posiciones") || [] : [];
        var posicion = posiciones[posicionIndex];
        var asignacion = getAsignacion(posicion);

        return asignacion ? { eje: eje, posicion: posicion, asignacion: asignacion } : null;
    };

    var getItemFromAsignacionId = function (idAsignacion) {
        return getAllInstaladas().find(function (item) {
            return String(getValue(item.asignacion, "IdLlantaAsignacion", "idLlantaAsignacion")) === String(idAsignacion);
        });
    };

    var setSubmitLoading = function (isLoading) {
        if (!botonGuardar) {
            return;
        }

        botonGuardar.disabled = isLoading;
        botonGuardar.setAttribute("data-kt-indicator", isLoading ? "on" : "off");
    };

    var guardar = async function (event) {
        event.preventDefault();
        hideAlert();

        if (!selectorVehiculo.value) {
            showAlert("Selecciona un vehículo.");
            return;
        }

        if (!selectorTipo.value) {
            showAlert("Selecciona el tipo de inspección.");
            return;
        }

        if (!inputFecha.value) {
            showAlert("Captura la fecha de inspección.");
            return;
        }

        if (!inputKilometraje.value) {
            showAlert("Captura el kilometraje.");
            return;
        }

        if (!seleccionadas.length) {
            showAlert("Selecciona al menos una llanta instalada.");
            return;
        }

        setSubmitLoading(true);

        try {
            var result = await postForm(config.guardarUrl, new FormData(form));

            if (!result.success) {
                throw new Error(result.message || "No fue posible guardar la inspección.");
            }

            showSuccess(result.message || "Inspección registrada correctamente.");
            limpiarSeleccion();
        } catch (error) {
            showAlert(error.message);
        } finally {
            setSubmitLoading(false);
        }
    };

    var initSelect2 = function () {
        if (!window.jQuery || !window.jQuery.fn || !window.jQuery.fn.select2) {
            return;
        }

        [selectorVehiculo, selectorTipo].forEach(function (select) {
            if (select) {
                window.jQuery(select).select2({
                    width: "100%",
                    placeholder: select.getAttribute("data-placeholder") || "Seleccione"
                });
            }
        });
    };

    var bindEvents = function () {
        if (selectorVehiculo) {
            var onVehiculoChange = function () {
                cargarConfiguracion();
            };

            if (window.jQuery) {
                window.jQuery(selectorVehiculo).on("change", onVehiculoChange);
                window.jQuery(selectorVehiculo).on("select2:select", onVehiculoChange);
                window.jQuery(selectorVehiculo).on("select2:clear", onVehiculoChange);
            } else {
                selectorVehiculo.addEventListener("change", onVehiculoChange);
            }
        }

        if (botonSeleccionarTodas) {
            botonSeleccionarTodas.addEventListener("click", setTodas);
        }

        if (botonLimpiar) {
            botonLimpiar.addEventListener("click", limpiarSeleccion);
        }

        if (diagrama) {
            diagrama.addEventListener("click", function (event) {
                var button = event.target.closest(".llanta-posicion");
                var item = getItemFromButton(button);

                if (item) {
                    toggleSeleccion(item.eje, item.posicion, item.asignacion);
                }
            });
        }

        if (tabla) {
            tabla.addEventListener("click", function (event) {
                var row = event.target.closest(".llanta-inspeccion-row");

                if (!row) {
                    return;
                }

                var item = getItemFromAsignacionId(row.getAttribute("data-id-asignacion"));

                if (item) {
                    toggleSeleccion(item.eje, item.posicion, item.asignacion);
                }
            });
        }

        if (detalles) {
            detalles.addEventListener("click", function (event) {
                var quitar = event.target.closest(".llanta-inspeccion-quitar");

                if (!quitar) {
                    return;
                }

                var index = findSelectedIndex(quitar.getAttribute("data-id-asignacion"));

                if (index >= 0) {
                    seleccionadas.splice(index, 1);
                    renderSelectionState();
                }
            });

            detalles.addEventListener("input", function (event) {
                var input = event.target;
                var detalle = input.closest(".llanta-inspeccion-detalle");

                if (!detalle) {
                    return;
                }

                var index = Number(detalle.getAttribute("data-index"));
                var item = seleccionadas[index];

                if (!item) {
                    return;
                }

                if (input.classList.contains("llanta-inspeccion-presion")) {
                    var target = detalle.querySelector(".llanta-inspeccion-semaforo-presion");
                    if (target) {
                        target.innerHTML = renderBadge(getPresionBadge(item, input.value));
                    }
                }

                if (input.classList.contains("llanta-inspeccion-profundidad")) {
                    var targetProfundidad = detalle.querySelector(".llanta-inspeccion-semaforo-profundidad");
                    if (targetProfundidad) {
                        targetProfundidad.innerHTML = renderBadge(getProfundidadBadge(item, input.value));
                    }
                }
            });
        }

        if (form) {
            form.addEventListener("submit", guardar);
        }
    };

    return {
        init: function () {
            selectorVehiculo = document.getElementById("llanta-inspeccion-vehiculo");
            selectorTipo = document.getElementById("llanta-inspeccion-tipo");
            inputFecha = document.getElementById("llanta-inspeccion-fecha");
            inputKilometraje = document.getElementById("llanta-inspeccion-km");
            alerta = document.getElementById("llanta-inspeccion-alerta");
            exito = document.getElementById("llanta-inspeccion-exito");
            resumen = document.getElementById("llanta-inspeccion-resumen");
            loading = document.getElementById("llanta-inspeccion-loading");
            diagrama = document.getElementById("llanta-inspeccion-diagrama");
            tabla = document.getElementById("llanta-inspeccion-tabla");
            captura = document.getElementById("llanta-inspeccion-captura");
            detalles = document.getElementById("llanta-inspeccion-detalles");
            botonSeleccionarTodas = document.getElementById("llanta-inspeccion-seleccionar-todas");
            botonLimpiar = document.getElementById("llanta-inspeccion-limpiar");
            botonGuardar = document.getElementById("llanta-inspeccion-guardar");
            form = document.getElementById("llanta-inspeccion-form");
            config = window.SUVAN && window.SUVAN.LlantaInspeccion ? window.SUVAN.LlantaInspeccion : {};

            initSelect2();
            bindEvents();
            renderSelectionState();
        }
    };
}();

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", function () {
        SuvanLlantaInspeccion.init();
    });
} else {
    SuvanLlantaInspeccion.init();
}
