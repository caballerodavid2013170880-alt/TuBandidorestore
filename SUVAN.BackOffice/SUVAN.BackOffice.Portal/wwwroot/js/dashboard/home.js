"use strict";
var KTHome = function () {

  const fechas = document.getElementById('fechas');
  const periodo = document.getElementById('periodo');
  const indicador = document.getElementById('indicador');
  const rutaSelect = document.getElementById('ruta');
  const horarioSelect = document.getElementById('horario');
  const filtrar = document.getElementById('filtrar');
  let tableIngresos;
  var chart = {
    self: null,
    rendered: false
  };


  const chartIngresos = function (perido, cantidad) {
    var element = document.getElementById("kt_charts_widget_ingresos_chart");

    if (!element) {
      return;
    }



    var initChart = function (categories, data) {


      var options = {
        series: [{
          data: cantidad,
          show: !1
        }],
        chart: {
          type: "bar",
          height: 350,
          toolbar: {
            show: !1
          }
        },
        plotOptions: {
          bar: {
            borderRadius: 4,
            horizontal: !0,
            distributed: !0,
            barHeight: 23
          }
        },
        dataLabels: {
          enabled: !1
        },
        legend: {
          show: !1
        },
        colors: ["#3E97FF", "#F1416C", "#50CD89", "#FFC700", "#7239EA", "#50CDCD", "#3F4254"],
        xaxis: {
          title: {
            text: 'Ingreso / Ocupación'
          },
          categories: perido,
          labels: {
            formatter: function (e) {
              return e
            },
            style: {
              colors: KTUtil.getCssVariableValue("--bs-gray-400"),
              fontSize: "14px",
              fontWeight: "600",
              align: "left"
            }
          },
          axisBorder: {
            show: !1
          }
        },
        yaxis: {
          title: {
            text: 'Periodo'
          },
          labels: {
            style: {
              colors: KTUtil.getCssVariableValue("--bs-gray-800"),
              fontSize: "14px",
              fontWeight: "600"
            },
            offsetY: 2,
            align: "left"
          }
        },
        grid: {
          // borderColor: a,
          xaxis: {
            lines: {
              show: !0
            }
          },
          yaxis: {
            lines: {
              show: !1
            }
          },
          strokeDashArray: 4
        }
      };

      chart.self = new ApexCharts(element, options);
      chart.self.render();
      chart.rendered = true;
    }


    if (chart.rendered) {
      chart.self.destroy();
    }

    // Init chart
    initChart(perido, cantidad);

    // Update chart on theme mode change
    //KTThemeMode.on("kt.thememode.change", function () {
    //  if (chart.rendered) {
    //    chart.self.destroy();
    //  }

    //  initChart(perido, cantidad);
    //});
  }

  const initRutaAndHoarios = () => {

    rutaSelect.innerHTML = "";
    //horarioSelect.innerHTML = "";

    const optionSeleccione = document.createElement('option');
    optionSeleccione.value = "";
    optionSeleccione.textContent = "Todos";
    rutaSelect.appendChild(optionSeleccione);


    rutasArray.forEach(ruta => {

      const option = document.createElement('option');
      option.value = ruta.RutaId;
      option.textContent = ruta.Nombre;
      rutaSelect.appendChild(option);

    });



    rutaSelect.addEventListener('change', function (event) {

      const rutaId = event.target.value;
      const ruta = rutasArray.find(r => r.RutaId == rutaId);

      // limpia valores
      horarioSelect.innerHTML = "";

      const optionSeleccione = document.createElement('option');
      optionSeleccione.value = "";
      optionSeleccione.textContent = "Todos";
      horarioSelect.appendChild(optionSeleccione);
      if (ruta.Corridas.length == 0) {
        notificacion.info("No hay horarios disponibles para la ruta seleccionada");
        return;
      }
      //const corridasConfiguration = ruta.Corridas;
      ruta.Corridas.forEach(horario => {
        const option = document.createElement('option');
        option.value = horario.CorridaId;
        option.textContent = horario.Horas;
        horarioSelect.appendChild(option);
      });

    });

    //horarioSelect.addEventListener('change', function (event) {

    //  const corridaId = event.target.value;
    //  const horario = corridasConfiguration.find(r => r.CorridaId == corridaId);
    //  nonSelectableDays = horario.DiasInactivos;
    //  console.log(horario);
    //  generateCalendar(currentYear, horario.DiasInactivos, horario.FechasSeleccionadas);

    //});
  }

  const handleFilterIngresos = () => {

    filtrar.addEventListener('click', function (event) {
      //if (tableIngresos) {
      //  tableIngresos.destroy();
      //}
      getIngresosOcupacion();
    });

  }

  const initControls = () => {

    console.log(rutasArray);

    initRutaAndHoarios();
    handleFilterIngresos();
    $("#fechas").daterangepicker({
      locale: {
        format: "DD/M/yyyy",
        "applyLabel": "Aplicar",
        "cancelLabel": "Cancelar",
        "daysOfWeek": [
          "Do",
          "Lu",
          "Ma",
          "Mi",
          "Ju",
          "Vi",
          "Sa"
        ],
        "monthNames": [
          "Enero",
          "Febrero",
          "Marzo",
          "Abril",
          "Mayo",
          "Junio",
          "Julio",
          "Agosto",
          "Septiembre",
          "Octubre",
          "Noviembre",
          "Diciembre"
        ],
      }
    });
  }

  const handleTableIngresos = (data) => {

    if (tableIngresos) {
      tableIngresos.destroy();
    }
    tableIngresos = new DataTable("#kt_table_widget_ingresos", {
      columns: [
        { title: "Fecha", data: "fechaViaje" },
        { title: "Cantidad Boletos", data: "cantidadBoleto" },
        { title: "Ingresos $", data: "sumatoriaCostoFinal", className: "text-end" },
      ],
      data: data,
      info: false
    });
  }

  const getIngresosOcupacion = () => {

    filtrar.setAttribute('data-kt-indicator', 'on');
    filtrar.disabled = true;

    const data = {
      Periodo: periodo.value,
      Indicador: indicador.value,
      Fecha: fechas.value,
      RutaId: rutaSelect.value,
      HorarioId: horarioSelect.value
    };

    fetch('/home/DashboardIngresos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    })
      .then(response => response.json())
      .then(data => {

        console.log(data);
        const periodo = data.data.chart.map(x => x.periodo);
        const cantidad = data.data.chart.map(x => x.cantidadUsuarios);


        chartIngresos(periodo, cantidad);
        handleTableIngresos(data.data.table)
      })
      .catch(error => console.error('Error en la búsqueda:', error))
      .finally(() => {
        //isSearching = false;
        filtrar.setAttribute('data-kt-indicator', 'off');
        filtrar.disabled = false;
      });
  }

  const initChartIngresos = () => {



    getIngresosOcupacion();

  }

  return {
    init: function () {
      initControls();

      initChartIngresos();
    }
  }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTHome.init();
});