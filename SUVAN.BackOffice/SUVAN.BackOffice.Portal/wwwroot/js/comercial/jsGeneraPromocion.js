/**
 * Autor:Hedilberto Cruz Vasquez
 * Fecha: 19 de Diciembre del 2023
 * Descripcion: Js que permite la alta de promociones
 */
let fnGeneraPromociones = new function () {
  "use strict";
  const EnumTipoPromocion = {
    EMPRESA: 1,
    RUTA: 2,
    CORRIDA: 4,
    HORARIO: 3
  };
  let self = this,
    pluggingsValidate = {
      trigger: new FormValidation.plugins.Trigger(),
      bootstrap: new FormValidation.plugins.Bootstrap5({
        rowSelector: '.fv-row',
        eleInvalidClass: '',
        eleValidClass: ''
      })
    },
    _obInitAutonumeric = {
      currencySymbol: '$ ',
      decimalCharacter: '.',
      digitGroupSeparator: ',',
      minimumValue: 0
    },
    formValidator = null,
    $kt_table_horarios = $("#kt_table_horarios"),
    $kt_add_promocion_in_submit = $("#kt_add_promocion_in_submit"),
    $Promocion_TipoPromocionId = $("#Promocion_TipoPromocionId"),
    $Promocion_RutasEmpresa = $("#Promocion_RutasEmpresa"),
    $kt_promocion_in_form = $("#kt_promocion_in_form"),
    $btnAgregarHorario = $("#btnAgregarHorario"),
    _dataTable = {};

  function _init() {
    _initDataTable();
    _initValidate();
    _initControls();
    if ($("#Promocion_PromocionId").val() != "0") {
      let _tipoPromocion = $Promocion_TipoPromocionId.val() != "" ? parseInt($Promocion_TipoPromocionId.val()) : 0;
      if ($("#Promocion_TipoDescuentoId").val() == "1") {
        AutoNumeric.getAutoNumericElement("#Promocion_MontoDescuento").update({ currencySymbol: '$ ', maximumValue: '10000000000000', currencySymbolPlacement: 'p' })
      } else if ($("#Promocion_TipoDescuentoId").val() == "2") {
        AutoNumeric.getAutoNumericElement("#Promocion_MontoDescuento").update({ currencySymbol: ' %', maximumValue: 100, currencySymbolPlacement: 's' })
      }
      switch (_tipoPromocion) {
        case EnumTipoPromocion.RUTA:
          $("#divSeparator").find("span").text("Aplica a Ruta");
          $(".tipoPromocion_ruta").show();
          break;
        case EnumTipoPromocion.CORRIDA:
          $("#divSeparator").find("span").text("Aplica a Corrida");
          $(".tipoPromocion_corrida").show();
          $.each($("#Promocion_RutasEmpresa").val(), function (index, item) {
            let element = document.getElementById("divGroupRuta_" + item)
            if (element == null) {

              notificacion.info("No existen corridas registradas", item);
            }
            else {
              $(element).show();
            }
          });
          break;
        case EnumTipoPromocion.HORARIO:
          $("#divSeparator").find("span").text("Aplica a Horario");
          $(".tipoPromocion_horario").show();
          let _data = JSON.parse($("#hdHorarios").val())
          $.each(_data, function (index, item) {
            _dataTable.row.add({
              No: _dataTable.data().length + 1,
              Horario: item.horaInicio + " - " + item.horaFin,
              Inicia: moment(moment(new Date()).format("yyyy-DD-MM") + " " + item.horaInicio)._d,
              Termina: moment(moment(new Date()).format("yyyy-DD-MM") + " " + item.horaFin)._d,
            }).draw();
          });

          break;
      }
      $("#kt_promocion_in_form input,select,button").prop("disabled", true)
    }

  }
  function _initValidate() {
    formValidator = FormValidation.formValidation(
      $kt_promocion_in_form[0],
      {
        fields: {
          'Promocion.Nombre': {
            validators: {
              notEmpty: {
                message: 'El nombre es requerido'
              }
            }
          },
          'txtVigencia': {
            validators: {
              notEmpty: {
                message: 'Indique la vigencia'
              }
            }
          },
          'Promocion.TipoPromocionId': {
            validators: {
              notEmpty: {
                message: 'Tipo promoción'
              }
            }
          },
          'Promocion.TipoDescuentoId': {
            validators: {
              notEmpty: {
                message: 'Tipo de descuento'
              }
            }
          },
          'Promocion.MontoDescuento': {
            validators: {
              notEmpty: {
                message: 'Cantidad del descuento'
              }
            }
          }
        },
        plugins: pluggingsValidate
      }
    );
  }
  function _initControls() {
    new AutoNumeric('#Promocion_MontoDescuento', _obInitAutonumeric);
    let _fechaInicio = moment(new Date(), "dd-MM-yyyy");
    let _fechaFin = moment(new Date(), "dd-MM-yyyy").add(1, "days");
    if ($("#Promocion_PromocionId").val() == "0") {
      $("#Promocion_FechaInicio").val(_fechaInicio.format('YYYY-MM-DD'))
      $("#Promocion_FechaFin").val(_fechaFin.format('YYYY-MM-DD'))
    }
    moment(new Date()).format("yyyy-DD-MM")
    $("#txtVigencia").daterangepicker({
      "startDate": $("#Promocion_PromocionId").val() != "0" ? $("#Promocion_FechaInicio").val() : _fechaInicio.format("DD/MM/yyyy"),
      "endDate": $("#Promocion_PromocionId").val() != "0" ? $("#Promocion_FechaFin").val() : _fechaFin.format("DD/MM/yyyy"),
      autoApply: true,
      "isInvalidDate": function (date) {
        // Obtén la fecha actual
        const currentDate = moment().startOf('day');
        // Devuelve verdadero para deshabilitar las fechas menores a la fecha actual
        return date.isBefore(currentDate);
      },
      locale: {
        format: 'DD/MM/YYYY'
      }
    }, function (start, end, label) {
      console.log(start)
      console.log(end)
      $("#Promocion_FechaInicio").val(start.format('YYYY-MM-DD'))
      $("#Promocion_FechaFin").val(end.format('YYYY-MM-DD'))
    });
    $("#txtHoraInicio").flatpickr({
      enableTime: true,
      noCalendar: true,
      dateFormat: "H:i K",
      onClose: function (selectedDates, dateStr, instance) {
        let inputInicio = document.getElementById("txtHoraInicio");
        let inputFin = document.getElementById("txtHoraFin");
        if (inputFin._flatpickr.selectedDates.length > 0) {
          let dateStart = selectedDates[0];
          let dateEnd = inputFin._flatpickr.selectedDates[0]
          if (dateStart > dateEnd) {
            inputInicio._flatpickr.setDate(dateEnd)
            notificacion.info("Ingrese una hora menor a " + $(inputFin).val())
          }
        }
      }
    });
    $("#txtHoraFin").flatpickr({
      enableTime: true,
      noCalendar: true,
      dateFormat: "H:i K",
      onClose: function (selectedDates, dateStr, instance) {
        let inputInicio = document.getElementById("txtHoraInicio");
        let inputFin = document.getElementById("txtHoraFin");
        if (inputInicio._flatpickr.selectedDates.length > 0) {
          let dateEnd = selectedDates[0];
          let dateStart = inputInicio._flatpickr.selectedDates[0]
          if (dateEnd < dateStart) {
            inputFin._flatpickr.setDate(dateStart);
            notificacion.info("Ingrese una hora mayor a " + $(inputInicio).val())
          }
        }

      }
    });
    $("#kt_add_promocion_in_submit").on("click", function (e) {
      e.preventDefault();
      if (formValidator) {
        formValidator.validate().then(function (status) {
          if (_validateForm(status)) {
            $kt_add_promocion_in_submit.attr("data-kt-indicator", "on");
            $kt_add_promocion_in_submit.attr("disabled", true);
            let dataRequest = _setDataForm()


            _ajaxServiceSave(dataRequest, function (response) {
              $kt_add_promocion_in_submit.removeAttr("data-kt-indicator");
              $kt_add_promocion_in_submit.attr("disabled", false);
              if (response.promocionId) {
                notificacion.success("Información registrada con exito", "Guardar")
                window.location.href = "../Comercial/CodigosPromocion"

              } else {
                notificacion.error("Se genero un error en la operación", "Guardar")
              }

            })
          }
        });
      }

    });
    $Promocion_RutasEmpresa.on({
      "select2:selecting": function (e) {
        if (parseInt($Promocion_TipoPromocionId.val()) == EnumTipoPromocion.CORRIDA) {
          let element = document.getElementById("divGroupRuta_" + e.params.args.data.id)
          if (element == null) {
            e.preventDefault();
            notificacion.info("No existen corridas registradas", e.params.args.data.text);
          }
          else {
            $(element).show();
          }
        }

      },
      "select2:unselect": function (e) {
        if (parseInt($Promocion_TipoPromocionId.val()) == EnumTipoPromocion.CORRIDA) {
          let element = document.getElementById("divGroupRuta_" + e.params.data.id)
          if (element) {
            $(element).hide();
            $(element).find("input[type='checkbox']").prop("checked", false);
          }
        }

      }
    });
    $Promocion_TipoPromocionId.on("change", function () {
      let _tipoPromocion = $(this).val() != "" ? parseInt($(this).val()) : 0;
      $(".tipoPromocion").hide();
      $Promocion_RutasEmpresa.val(null).trigger('change');
      $(".divGroupRuta").hide();
      $(".tipoPromocion_corrida input[type='checkbox']").prop("checked", false);
      _dataTable.clear().draw();
      document.getElementById("txtHoraInicio")._flatpickr.clear();
      document.getElementById("txtHoraFin")._flatpickr.clear();
      switch (_tipoPromocion) {
        case EnumTipoPromocion.RUTA:
          $("#divSeparator").find("span").text("Aplica a Ruta");
          $(".tipoPromocion_ruta").show();
          break;
        case EnumTipoPromocion.CORRIDA:
          $("#divSeparator").find("span").text("Aplica a Corrida");
          $(".tipoPromocion_corrida").show();
          break;
        case EnumTipoPromocion.HORARIO:
          $("#divSeparator").find("span").text("Aplica a Horario");
          $(".tipoPromocion_horario").show();
          break;
      }

    });
    $btnAgregarHorario.on("click", function () {

      if (document.getElementById("txtHoraInicio")._flatpickr.selectedDates.length > 0 && document.getElementById("txtHoraFin")._flatpickr.selectedDates.length > 0) {
        let _rowTable = {
          No: _dataTable.data().length + 1,
          Horario: $("#txtHoraInicio").val() + " - " + $("#txtHoraFin").val(),
          Inicia: document.getElementById("txtHoraInicio")._flatpickr.selectedDates[0],
          Termina: document.getElementById("txtHoraFin")._flatpickr.selectedDates[0]
        }
        _dataTable.row.add(_rowTable).draw();
        document.getElementById("txtHoraInicio")._flatpickr.clear();
        document.getElementById("txtHoraFin")._flatpickr.clear();
      } else {
        notificacion.info("Ingrese la hora de inicio y fin", "Campos requeridos");
      }
    });
    $("#Promocion_TipoDescuentoId").on("change", function () {
      AutoNumeric.getAutoNumericElement("#Promocion_MontoDescuento").clear()
      if ($(this).val() == "1") {
        AutoNumeric.getAutoNumericElement("#Promocion_MontoDescuento").update({ currencySymbol: '$ ', maximumValue: '10000000000000', currencySymbolPlacement: 'p' })
      } else if ($(this).val() == "2") {
        AutoNumeric.getAutoNumericElement("#Promocion_MontoDescuento").update({ currencySymbol: ' %', maximumValue: 100, currencySymbolPlacement: 's' })
      }
    });
  }
  function _validateForm(status) {
    let msg = "Ingrese los campos requeridos";
    let result = false;

    if (status == "Valid") {
      result = true;
      msg = "";
      if ($Promocion_TipoPromocionId.val() == "2") {

        result = $("#Promocion_RutasEmpresa").val().length > 0;
        msg = !result ? "Seleccione al menos una ruta" : "";
      } else if ($Promocion_TipoPromocionId.val() == "3") {

        result = _dataTable.data().length > 0;
        msg = !result ? "Ingrese al menos un horario" : "";

      } else if ($Promocion_TipoPromocionId.val() == "4") {
        result = $(".tipoPromocion_corrida input:checked").length > 0;
        msg = !result ? "Seleccione al menos una corrida, de acuerdo a la ruta de la lista" : "";
      }
    }
    if (!result)
      notificacion.info(msg);
    return result
  }
  function _setDataForm() {
    let dataRequest = $kt_promocion_in_form.serializeArray();
    let montoDescuencto = dataRequest.find(function (x) { return x.name == 'Promocion.MontoDescuento' });
    montoDescuencto.value = AutoNumeric.getAutoNumericElement("#Promocion_MontoDescuento").get();
    if ($Promocion_TipoPromocionId.val() == "3") {

      $.each(_dataTable.data(), function (index, item) {
        dataRequest.push({ name: "Promocion.Horarios[" + index + "].HoraInicio", value: moment(item.Inicia).format("hh:mm A") });
        dataRequest.push({ name: "Promocion.Horarios[" + index + "].HoraFin", value: moment(item.Termina).format("hh:mm A") });
      });

    } else if ($Promocion_TipoPromocionId.val() == "4") {
      $.each($(".tipoPromocion_corrida input:checked"), function (index, item) {
        dataRequest.push({ name: "Promocion.CorridasRutas", value: $(item).val() })
      })
    }

    return dataRequest;
  }
  function _initDataTable() {
    _dataTable = $kt_table_horarios.InitDataTable({
      'columnDefs': [{
        "targets": 0,
        "width": "10%",
        className: "text-center",
        "data": "No",
      },
      {
        "targets": 1,
        "width": "70%",
        className: "text-center",
        "data": "Horario",
      },
      {
        "targets": 2,
        "width": "20%",
        className: "text-center",
        "visible": $("#Promocion_PromocionId").val() == "0",
        "data": "No",
        "render": function (data, type, row, meta) {
          return `<a href="javaScript:void(0)" class="btn btn-icon btn-active-light-primary w-30px h-30px"  data-kt-promocion-action="delete"> <i class="ki-outline ki-trash fs-3"></i></a>`;
        }
      },
      ]
    });
    _onRenderDataTable();
    _dataTable.on("draw", _onRenderDataTable);
    $('[data-kt-search-element="suggestions"]').on("keyup", function (e) {
      _dataTable.search(e.target.value).draw();
    });
  }
  function _onRenderDataTable() {
    $('[data-kt-promocion-action').off("click");
    $('[data-kt-promocion-action="edit"]').on("click", _onEdit)
    $('[data-kt-promocion-action="delete"]').on("click", _onDelete)
  }
  function _onEdit() {
    let $tdElement = $(this).parents("td");
  }
  function _onDelete() {
    let $tdElement = $(this).parents("td");
    Swal.fire({
      title: "Eliminación",
      text: "Esta seguro de eliminar el horario",
      icon: "warning",
      showCancelButton: true,
      buttonsStyling: false,
      confirmButtonText: "Si, eliminar!",
      cancelButtonText: "No, cancelar",
      customClass: {
        confirmButton: "btn fw-bold btn-danger",
        cancelButton: "btn fw-bold btn-active-light-primary"
      }
    }).then((result) => {
      if (result.isConfirmed) {
        _dataTable.row($tdElement.parents("tr")).remove().draw();
      }
    });
  }
  function _ajaxServiceSave(_data, _callBackOk) {
    $.ajax({
      url: "../Comercial/GeneraPromocion",
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
KTUtil.onDOMContentLoaded(function () {
  fnGeneraPromociones.init();
});