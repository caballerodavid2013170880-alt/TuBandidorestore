"use strict";

// Class definition
var KTChoferUnidad = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  //:: controles
  const calenadarContainer = document.querySelector("#calendar-container");
  const calenadarYearContainer = document.querySelector("#canlendar-current-year");
  const subtractControl = document.querySelector("#calendar-subtract-year");
  const sumControl = document.querySelector("#calendar-sum-year");
  const conductorSelect = document.getElementById('ConductorId');
  const unidadSelect = document.getElementById('VehiculoId');
  const rutaSelect = document.getElementById('RutaId');
  const horarioSelect = document.getElementById('HorarioId');
  const guardarAsignacion = document.getElementById('btn-guardar-asignacion');
  const modalAsignar = document.getElementById('kt_modal_asignar');
  const btnAgregar = document.getElementById('btn_agregar_asignacion');
  const listadoContainer = document.getElementById('listado-operadores-container');
  const target = document.querySelector("#modal_body_asignacion");
  const blockUI = new KTBlockUI(target);
  // Variables
  let operadorVehiculoList = [];
  let currentYear = new Date().getFullYear();
  let nonSelectableDays = [];//[0, 2, 3]; // Domingo, Martes, Miércoles
  const autoSelectDays = [];//['2023-10-12', '2023-11-13', '2023-12-14'];
  const months = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
  const daysOfWeek = ["D", "L", "M", "M", "J", "V", "S"];
  let rutaConfiguration = {};
  let corridasConfiguration = {};
  let selectedCalendarDates = [];
  let showModalAsignar = new bootstrap.Modal(modalAsignar, {
    keyboard: false,
    backdrop: 'static'
  });



  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {
          'Vigencia': {
            validators: {
              notEmpty: {
                message: 'Vigencias es requerida'
              }
            }
          }

        },
        plugins: {
          trigger: new FormValidation.plugins.Trigger(),
          bootstrap: new FormValidation.plugins.Bootstrap5({
            rowSelector: '.fv-row',
            eleInvalidClass: '',  // comment to enable invalid state icons
            eleValidClass: '' // comment to enable valid state icons
          })
        }
      }
    );



  }

  //var handleSubmitValidation = function (e) {
  //  // Handle form submit
  //  submitButton.addEventListener('click', function (e) {
  //    // Prevent button default action
  //    e.preventDefault();

  //    // Validate form
  //    //validator.validate().then(function (status) {
  //    //  if (status == 'Valid') {
  //    // Disable button to avoid multiple click
  //    submitButton.setAttribute('data-kt-indicator', 'on');
  //    submitButton.disabled = true;
  //    //form.submit();
  //    //  }
  //    //});
  //  });


  //}

  const initData = () => {
    const rutaJsonInput = document.getElementById('RutasJson');
    rutaConfiguration = JSON.parse(rutaJsonInput.value);
    console.log(rutaConfiguration);
  }

  const initCalendar = () => {

    generateCalendar(currentYear, nonSelectableDays, autoSelectDays);
  }

  const generateCalendar = (year, nonSelectableDays, autoSelectDays) => {
    calenadarContainer.innerHTML = "";
    calenadarYearContainer.textContent = year;
    const months = Array.from({ length: 12 }, (_, i) => i);
    const currentDate = new Date();
    const diaAnterior = new Date(currentDate);
    diaAnterior.setDate(diaAnterior.getDate() - 1);
    months.forEach(monthIndex => {
      const monthElement = document.createElement("div");
      monthElement.className = "month";
      monthElement.innerHTML = `<h3>${getMonthName(monthIndex)}</h3>`;
      const days = getDaysInMonth(year, monthIndex);
      const firstDayOfMonth = new Date(year, monthIndex, 1);

      // Calcular cuántos días de la semana antes del primer día del mes
      const daysBefore = firstDayOfMonth.getDay();

      // Crear días de la semana (nombres)
      const weekHeader = document.createElement("div");
      weekHeader.className = "week";
      for (let i = 0; i < 7; i++) {
        const dayElement = document.createElement("div");
        dayElement.textContent = daysOfWeek[i];
        dayElement.className = "dayname";
        weekHeader.appendChild(dayElement);
      }
      monthElement.appendChild(weekHeader);

      // Crear días del mes en una cuadrícula
      const weeksInMonth = Math.ceil((days.length + daysBefore) / 7);
      for (let weekIndex = 0; weekIndex < weeksInMonth; weekIndex++) {
        const weekElement = document.createElement("div");
        weekElement.className = "week";

        for (let dayIndex = weekIndex * 7; dayIndex < (weekIndex + 1) * 7; dayIndex++) {
          const day = days[dayIndex - daysBefore];
          const dayElement = document.createElement("div");
          dayElement.className = "day";

          if (day) {
            dayElement.textContent = day.getDate();

            if (nonSelectableDays.includes(day.getDay()) || day < diaAnterior) {
              if (autoSelectDays.includes(formatDate(day))) {
                dayElement.classList.add("selected");
              } else {

                dayElement.classList.add("inactive");
              }

              dayElement.removeEventListener("click", () => toggleDaySelection(dayElement), year);
            } else if (autoSelectDays.includes(formatDate(day))) {
              dayElement.classList.add("selected");
              dayElement.addEventListener("click", () => toggleDaySelection(dayElement, year));
            } else {
              dayElement.addEventListener("click", () => toggleDaySelection(dayElement, year));
            }
          } else {
            // Crear celdas vacías antes del primer día del mes
            dayElement.className = "empty";
          }

          weekElement.appendChild(dayElement);
        }

        monthElement.appendChild(weekElement);
      }

      calenadarContainer.appendChild(monthElement);
    });
  }
  // Función para obtener el nombre del mes
  const getMonthName = (monthIndex) => {
    return months[monthIndex];
  }

  const getMonthNumber = (monthName) => {
    return months.indexOf(monthName) + 1;
  }
  // Función para obtener los días de un mes
  const getDaysInMonth = (year, monthIndex) => {
    const firstDay = new Date(year, monthIndex, 1);
    const lastDay = new Date(year, monthIndex + 1, 0);
    const daysInMonth = [];

    for (let day = firstDay; day <= lastDay; day.setDate(day.getDate() + 1)) {
      daysInMonth.push(new Date(day));
    }

    return daysInMonth;
  }


  // Función para alternar la selección de un día
  const toggleDaySelection = (dayElement, year) => {
    dayElement.classList.toggle("active");

    selectedCalendarDates = Array.from(calenadarContainer.querySelectorAll(".active"))
      .map(day => {
        const month = getMonthNumber(day.parentNode.parentNode.firstChild.textContent);
        return `${year}-${month.toString().padStart(2, '0')}-${day.textContent.toString().padStart(2, '0')}`;
      });
    console.log("Fechas seleccionadas:", selectedCalendarDates);
  }

  const formatDate = (date) => {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  // consultar los conductores y unidades en las fechas seleccionadas
  const ConsultarConductores = async () => {

    const data = {
      RutaId: rutaSelect.value,
      CorridaId: horarioSelect.value,
      Fechas: selectedCalendarDates
    };
    console.log(data);

    blockUI.block();
    const response = await fetch('/ChoferUnidad/ConsultarChoferesUnidad', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });


    if (!response.ok) {
      console.log("error", response);
      blockUI.release();
      Swal.fire({
        text: "Ocurrio un error al obtener los conductores",
        icon: "error",
        buttonsStyling: false,
        confirmButtonText: "Ok!",
        customClass: {
          confirmButton: "btn fw-bold btn-primary",
        }
      });
    }

    const dataResponse = await response.json();
    console.log(dataResponse);
    if (dataResponse.success) {

      operadorVehiculoList = dataResponse.data.conductores;
      buildListadoOperadores();
    }
    blockUI.release();

  }

  const initModal = () => {

    modalAsignar.addEventListener('hide.bs.modal', function () {
      conductorSelect.value = "";
      unidadSelect.value = "";
      operadorVehiculoList = [];
      listadoContainer.innerHTML = "";

    });


    document.getElementById('modal-asignar').addEventListener('click', function (event) {

      if (selectedCalendarDates.length === 0 || rutaSelect.value === "" || horarioSelect.value === "") {
        notificacion.info("Debes seleccionar una ruta, horario y fechas");
        return;
      }
      showModalAsignar.show();

      ConsultarConductores();
    });
  }

  // limpiar valores de calendario y fechas seleccionadas
  const limpiaValores = () => {
    corridasConfiguration = [];
    nonSelectableDays = [];
    selectedCalendarDates = [];

    generateCalendar(currentYear, [], []);
  }

  const initRutaAndHoarios = () => {


    $("#RutaId").on('change', function (event) {

      const rutaId = event.target.value;
      const ruta = rutaConfiguration.find(r => r.RutaId == rutaId);

      // limpia valores
      horarioSelect.innerHTML = "";
      limpiaValores();

      const optionSeleccione = document.createElement('option');
      optionSeleccione.value = "";
      optionSeleccione.textContent = "Selecciona un horario";
      horarioSelect.appendChild(optionSeleccione);
      if (ruta.Corridas.length == 0) {
        notificacion.info("No hay horarios disponibles para la ruta seleccionada");
        return;
      }
      corridasConfiguration = ruta.Corridas;
      ruta.Corridas.forEach(horario => {
        const option = document.createElement('option');
        option.value = horario.CorridaId;
        option.textContent = horario.Horas;
        horarioSelect.appendChild(option);
      });

    });

    horarioSelect.addEventListener('change', function (event) {

      const corridaId = event.target.value;
      const horario = corridasConfiguration.find(r => r.CorridaId == corridaId);
      nonSelectableDays = horario.DiasInactivos;
      console.log(horario);
      generateCalendar(currentYear, horario.DiasInactivos, horario.FechasSeleccionadas);

    });
  }

  const handleSubmitValidation = () => {

    guardarAsignacion.addEventListener('click', async (event) => {

      if (selectedCalendarDates.length === 0 || operadorVehiculoList.length === 0) {
        notificacion.info("Debes seleccionar al menos un operador y su veh&iacute;culo");
        return;
      }


      guardarAsignacion.disabled = true;

      const data = {
        RutaId: rutaSelect.value,
        Conductores: operadorVehiculoList,
        CorridaId: horarioSelect.value,
        Fechas: selectedCalendarDates
      };
      console.log(data);

      blockUI.block();
      const response = await fetch('/ChoferUnidad/Agregar', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
      });

      if (!response.ok) {
        console.log("error", response);
        guardarAsignacion.disabled = false;
        blockUI.release();
        Swal.fire({
          text: "Ocurrio un error al guardar",
          icon: "error",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        });
      }

      const dataResponse = await response.json();
      console.log(dataResponse);
      if (dataResponse.success) {
        Swal.fire({
          text: `Se ha guardado correctamente.`,
          icon: "success",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        }).then(function () {
          window.location.href = "/choferunidad/agregar";
          showModalAsignar.hide();
          limpiaValores();
          rutaSelect.value = "";
          horarioSelect.value = "";
          guardarAsignacion.disabled = false;
          blockUI.release();
        });
      } else {
        blockUI.release();
        guardarAsignacion.disabled = false;
        Swal.fire({
          text: dataResponse.message,
          icon: "error",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        });
      }
    });

  }

  const buildListadoOperadores = () => {
    listadoContainer.innerHTML = "";
    operadorVehiculoList.forEach(item => {
      const row = document.createElement('div');
      row.className = "d-flex flex-row gap-4 mb-4";
      row.innerHTML = `<span class="text-gray-900">${item.conductor}</span>
                    <span class="text-gray-500">${item.vehiculo}</span>`;

      const deleteOption = document.createElement('a');
      deleteOption.className = "ms-auto btn btn-icon btn-active-light-danger w-15px h-15px";
      deleteOption.innerHTML = `<i class="ki-outline ki-trash fs-3"></i>`;
      deleteOption.addEventListener('click', function (event) {

        operadorVehiculoList = operadorVehiculoList.filter(x => x.conductorId != item.conductorId && x.vehiculoId != item.vehiculoId);
        buildListadoOperadores();

      });

      row.appendChild(deleteOption);

      listadoContainer.appendChild(row);
    });

  }

  const handleagregarOperardor = () => {
    btnAgregar.addEventListener('click', async (event) => {

      if (selectedCalendarDates.length === 0 || conductorSelect.value === "" || unidadSelect.value === "") {
        notificacion.info("Debes seleccionar operador y veh&iacute;culo");
        return;
      }
      const data = {
        conductorId: conductorSelect.value,
        vehiculoId: unidadSelect.value,
        conductor: conductorSelect.options[conductorSelect.selectedIndex].text,
        vehiculo: unidadSelect.options[unidadSelect.selectedIndex].text
      };

      // validar que no exista el operdaro o vehiculo en la lista
      const existe = operadorVehiculoList.find(x => x.conductorId == data.conductorId || x.vehiculoId == data.vehiculoId);

      if (existe) {
        notificacion.warning("El operador o veh&iacute;culo ya existe en la lista");
        return;
      }

      operadorVehiculoList.push(data);

      //conductorSelect.value = "";
      //unidadSelect.value = "";

      $("#ConductorId").val('').trigger('change');
      $("#VehiculoId").val('').trigger('change');

      buildListadoOperadores();
    });
  }


  const initControls = () => {
    subtractControl.addEventListener("click", () => {
      currentYear--;
      generateCalendar(currentYear, nonSelectableDays, autoSelectDays);

    });
    sumControl.addEventListener("click", () => {
      currentYear++;
      generateCalendar(currentYear, nonSelectableDays, autoSelectDays);
    });

    initModal();

    initRutaAndHoarios();

    handleSubmitValidation();

    handleagregarOperardor();
  }


  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_corridas_in_form');
      submitButton = document.querySelector('#kt_corridas_in_submit');


      initCalendar();
      initControls();
      initData();
      //handleValidation();

      //handleSubmitValidation(); // use for form validation submit
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTChoferUnidad.init();
});
