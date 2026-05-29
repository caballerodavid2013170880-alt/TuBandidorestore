/**
 * Autor:Hedilberto Cruz Vasquez
 * Fecha: 02 de Diciembre del 2023
 * Descripcion: Js que permite la interaccion de la pantalla de Establecer tarifas
 */
$(function () { fnEstableTarifas.init(); });
let fnEstableTarifas = new function () {
    "use strict";
    var self = this,
        _dataUpdateTarifa = {},
        _obInitAutonumeric = {
            currencySymbol: '$ ',
            decimalCharacter: '.',
            digitGroupSeparator: ',',
            minimumValue: 0
        },
        $contenedorPiramide = $("#contenedorPiramide");

    function _init() {
        $("#RutaId").on("change", _onChangeRutaTipoTarifa);
        //$("#OperadorId").on("change", _onChangeOperador);
        $("#TipoTarifaId").on("change", _onChangeRutaTipoTarifa);
        $("#btnActualizaPiramide").on("click", _onClickActualizaPiramide);
        new AutoNumeric('#txtMontoTarifaFija', _obInitAutonumeric);
        $("#btnGuardar").on("click", _onClickGuardar)
    }

    function _onChangeRutaTipoTarifa() {
        $("#divTipoFija, #divTipoEscalonada, #divContentPrecioTarifas").hide();
        $("#divTitleMonto").text("");

        var tipoTaridaId = null;
        if (this.name == "TipoTarifaId")
            tipoTaridaId = $("#TipoTarifaId").val()

        if ($("#RutaId").val() != "") {
            $("#divContentPrecioTarifas").show();
            let _dataRequest = {
                rutaId: $("#RutaId").val(), // $("#RutaId").val(),
                empresaId: $("#OperadorId").val(),
                tarifaId: tipoTaridaId
            };
            _ajaxObtenerPrecioTarifa(_dataRequest, function (response) {
                _dataUpdateTarifa = response;
                $("#TipoTarifaId").val(response.tarifaId);

                switch (response.tarifaId) {
                    case 1:
                        $("#divTitleMonto").text("Monto de tarifa fija");
                        $("#divTipoFija").show();
                        AutoNumeric.set("#txtMontoTarifaFija", response.tarifaGeneral["montoTarifa"])
                        break;
                    case 2:
                        $("#divTitleMonto").text("Monto de tarifa escalonada");
                        $("#divTipoEscalonada").show();
                        _onRenderTarifaEscalonada(response);
                        break;
                }

            })
        }



    }
    function _onClickActualizaPiramide() {
        let _montoIncrementar = $("#txtMontoCambio").val();
        if (_montoIncrementar == '') {
            notificacion.error("Tarifa de actualización no valida", "Guardar");
        } else {
            _montoIncrementar = _montoIncrementar != "0" ? parseFloat(_montoIncrementar) : 0;

            if (_montoIncrementar == 0) {
                notificacion.error("Tarifa de actualización no valida", "Guardar");
            } else {
                $.each($(".inputparada"), function (index, item) {
                    let _montoInput = AutoNumeric.getNumericString("#" + $(item).attr("id")) != "0" ? parseFloat(AutoNumeric.getNumericString("#" + $(item).attr("id"))) : 0.0;
                    _montoInput = $("#dropTipoCambio").val() == 1 ? (_montoInput + _montoIncrementar) : (_montoInput + (_montoInput * (_montoIncrementar / 100)));
                    AutoNumeric.set("#" + $(item).attr("id"), _montoInput);
                    $(item).trigger("change");
                });
            }
        }
    }
    function _onClickGuardar() {

        var guardado = true;

        if (_dataUpdateTarifa.tarifaId == 1) {

            var montoGeneral = AutoNumeric.getNumericString("#txtMontoTarifaFija");

            if (montoGeneral == 0) {
                notificacion.error("Tarifa no valida", "Guardar");
                guardado = false;

            }
            else {
                _dataUpdateTarifa.tarifaGeneral["montoTarifa"] = AutoNumeric.getNumericString("#txtMontoTarifaFija")
            }
        } else if (_dataUpdateTarifa.tarifaId == 2) {

            $.each($("#contenedorPiramide div.row").not(".titleInicio"), function (index, item) {
                $.each($(item).find("div.col-md-1 input.inputparada"), function (indexinput, input) {
                    var monto = $(input).data("escala").montoPago;
                    if (monto == 0) {
                        guardado = false;
                    }
                        
                })
            });

            if (guardado) {
                $.each($("#contenedorPiramide div.row").not(".titleInicio"), function (index, item) {
                    _dataUpdateTarifa.tarifaEscalonada[index].escalas = [];
                    $.each($(item).find("div.col-md-1 input.inputparada"), function (indexinput, input) {
                        _dataUpdateTarifa.tarifaEscalonada[index].escalas.push($(input).data("escala"));
                    })
                });
            } else {
                notificacion.error("Tarifa no valida", "Guardar");
            }


        }

        if (guardado) {
            $("#btnGuardar").attr("data-kt-indicator", "on");
            $("#btnGuardar").attr("disabled", true);
            _ajaxActualizaPrecioTarifa(_dataUpdateTarifa, function (response) {
                $("#btnGuardar").removeAttr("data-kt-indicator");
                $("#btnGuardar").attr("disabled", false);
                notificacion.success("Información registrada con exito", "Guardar")
            })
        }
    }
    function _onRenderTarifaEscalonada(response) {
        let _dataResponse = response["tarifaEscalonada"]
        $contenedorPiramide.empty();

        if (response["paradaInicio"]) {
            $contenedorPiramide.append($("<div>",
                {
                    class: "row titleInicio",
                    append: $("<label>", {
                        class: "col-form-label text-gray-600",
                        append: response["paradaInicio"]
                    })
                }
            ))
        }

        $.each(_dataResponse, function (indexR, itemR) {
            if (itemR.escalas.length > 0) {
                let $renglon = $("<div>", { class: "row" });

                $.each(itemR.escalas.sort((a, b) => a.orden - b.orden), function (indexE, itemE) {
                    let idElment = "parada_" + itemE["paradaInicio"] + "_" + itemE["paradaFin"];
                    let $col = $("<div>", {
                        append: $("<input>", {
                            class: "form-control inputparada",
                            value: itemE["montoPago"],
                            id: idElment,
                            name: idElment,
                            bind: {
                                change: function () {
                                    let _dataEscala = $(this).data("escala");
                                    let _montoInput = AutoNumeric.getNumericString("#" + $(this).attr("id")) != "0" ? parseFloat(AutoNumeric.getNumericString("#" + $(this).attr("id"))) : 0.0;
                                    _dataEscala.montoPago = _montoInput;
                                    $(this).data("escala", _dataEscala);
                                }
                            }
                        }),
                        class: "col-md-1"
                    })
                    $col.find("input").data("escala", itemE);
                    $renglon.append($col);
                    if ((indexE + 1) == itemR.escalas.length) {
                        $renglon.append($("<div>", {
                            append: $("<label>", {
                                class: "col-form-label text-gray-600",
                                append: itemR["nombreParada"]
                            }),
                            class: "col-md-4"
                        }))
                    }
                })
                $contenedorPiramide.append($renglon);
            }
        });

        AutoNumeric.multiple('.inputparada', _obInitAutonumeric)

    }
    function _ajaxServiceCatalogo(_data, _callBackOk) {
        $.ajax({
            url: "../configuracion/ObtenerOperadoresTipoTarifa",
            type: "POST",
            data: _data,
            dataType: 'json',
            success: _callBackOk
        });
    }
    function _ajaxObtenerPrecioTarifa(_data, _callBackOk) {
        $.ajax({
            url: "../configuracion/ObtenerPrecioTarifa",
            type: "POST",
            data: _data,
            dataType: 'json',
            success: _callBackOk
        });
    }
    function _ajaxActualizaPrecioTarifa(_data, _callBackOk) {
        $.ajax({
            url: "../configuracion/ActualizaPrecioTarifa",
            type: "POST",
            data: _data,
            dataType: 'json',
            success: _callBackOk
        });
    }


    self.init = function () {
        _init();
    }
};