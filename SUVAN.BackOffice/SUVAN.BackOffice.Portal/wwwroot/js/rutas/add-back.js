"use strict";

// Class definition
var KTRutas = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  let map;
  let directionsService;
  let directionsRenderer;
  let markers = [];

  let durationsContainer;
  let isSearching = false; // Variable para evitar búsquedas concurrentes

  let showModalEstacion;
  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {

          'Nombre': {
            validators: {
              notEmpty: {
                message: 'Nombre de la ruta requerida'
              },
              stringLength: {
                min: 3,
                max: 250,

                message: 'deben tener entre 3 y 250 caracteres',
              },
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

  var handleSubmitValidation = function (e) {
    // Handle form submit
    submitButton.addEventListener('click', function (e) {
      // Prevent button default action
      e.preventDefault();

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Disable button to avoid multiple click
          submitButton.setAttribute('data-kt-indicator', 'on');
          submitButton.disabled = true;
          form.submit();
        }
      });
    });
  }


  var handleInitMap = function () {

    let initialLocation = { lat: 19.426108, lng: -99.16951 };
    map = new google.maps.Map(document.getElementById('map'), {
      center: initialLocation,
      zoom: 15
    });
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({ map: map });
    durationsContainer = document.getElementById('duration');

    // Agregar clic al mapa para añadir puntos
    map.addListener('click', function (event) {
      addMarker(event.latLng);
      calculateAndDisplayRoute();
    });

    // Agregar clic derecho al mapa para añadir puntos intermedios
    map.addListener('rightclick', function (event) {
      addIntermediateMarker(event.latLng);
      calculateAndDisplayRoute();
    });
  }

  const addMarker = (location) => {
    const marker = new google.maps.Marker({
      position: location,
      map: map,
      label: markers.length.toString(), // Número de marcador
      draggable: true // Permitir que el marcador sea arrastrable
    });

    // Agregar eventos para mover y remover marcadores
    marker.addListener('dragend', function () {
      calculateAndDisplayRoute();
    });

    marker.addListener('rightclick', function () {
      removeMarker(marker);
      calculateAndDisplayRoute();
    });

    markers.push(marker);
    console.log(markers);
  }

  const addIntermediateMarker = (location) => {
    const marker = new google.maps.Marker({
      position: location,
      map: map,
      label: markers.length.toString(), // Número de marcador
      draggable: true // Permitir que el marcador sea arrastrable
    });

    // Agregar eventos para mover y remover marcadores
    marker.addListener('dragend', function () {
      calculateAndDisplayRoute();
    });

    marker.addListener('rightclick', function () {
      removeMarker(marker);
      calculateAndDisplayRoute();
    });

    // Insertar el marcador intermedio después del último marcador
    const lastMarkerIndex = markers.length - 1;
    markers.splice(lastMarkerIndex, 0, marker);
  }


  const removeMarker = (marker) => {
    marker.setMap(null); // Eliminar el marcador del mapa
    const index = markers.indexOf(marker);
    if (index !== -1) {
      markers.splice(index, 1); // Eliminar el marcador del array
    }
  }

  const calculateAndDisplayRoute = () => {
    if (markers.length < 2) {
      // Necesitas al menos dos puntos para calcular una ruta
      directionsRenderer.setMap(null); // Limpiar la ruta del mapa
      durationsContainer.textContent = '';
      return;
    }

    const durations = [];
    const waypoints = markers.map(marker => ({
      location: marker.getPosition(),
      stopover: true
    }));

    const origin = waypoints.shift().location;
    const destination = waypoints.pop().location;

    const request = {
      origin: origin,
      destination: destination,
      waypoints: waypoints,
      travelMode: 'DRIVING'
    };

    directionsService.route(request, function (response, status) {
      if (status === 'OK') {
        directionsRenderer.setMap(map); // Mostrar la ruta en el mapa
        directionsRenderer.setDirections(response);
        // Calcular y mostrar las duraciones
        const legs = response.routes[0].legs;
        for (let i = 0; i < legs.length; i++) {
          durations.push(legs[i].duration.text);
        }
        renderDurations(durations);
      } else {
        window.alert('No se pudo calcular la ruta debido a: ' + status);
      }
    });
  }
  const renderDurations = (durations) => {
    // Mostrar las duraciones en el elemento con id 'durations'
    durationsContainer.textContent = 'Duraciones estimadas: ' + durations.join(', ');
  }


  const handleSearchEstacion = () => {

    document.getElementById('search-estacion').addEventListener('keyup', function (event) {
      const searchText = event.target.value.trim();

      // Verificar si se ingresaron al menos 3 caracteres y no hay búsqueda en curso
      if (searchText.length >= 3 && !isSearching) {
        isSearching = true;

        // Realizar la búsqueda fetch
        fetch(`/Estaciones/BuscarEstacion?query=${searchText}`)
          .then(response => response.json())
          .then(data => {
            // Manejar los resultados y generar HTML
            const autocompleteList = document.getElementById('autocomplete-list');
            autocompleteList.innerHTML = ''; // Limpiar resultados anteriores
            console.log(data);
            const result = data.result;
            result.forEach(item => {

              const optionContainer = document.createElement('div');
              optionContainer.classList.add('form-check', 'form-check-custom', 'form-check-solid', 'my-4');

              const radioButton = document.createElement('input');
              radioButton.classList.add('form-check-input');
              radioButton.dataset.item = JSON.stringify(item);
              radioButton.type = 'radio';
              radioButton.name = 'cbestacion';
              radioButton.id = `ckes-${item.idparada}`;

              const label = document.createElement('label');
              label.classList.add('form-check-label');
              label.setAttribute('for', `ckes-${item.idparada}`);
              label.textContent = item.nombre;

              optionContainer.appendChild(radioButton);
              optionContainer.appendChild(label);

              autocompleteList.appendChild(optionContainer);
            });
          })
          .catch(error => console.error('Error en la búsqueda:', error))
          .finally(() => {
            isSearching = false; // Marcar que la búsqueda ha terminado
          });
      }
    });
  }

  const initModalEvent = () => {
    const modalAgregar = document.getElementById('kt_modal_agregar');
    const inputSearch = document.getElementById('search-estacion');
    modalAgregar.addEventListener('hide.bs.modal', function () {
      inputSearch.value = '';
      inputSearch.focus();
      const autocompleteList = document.getElementById('autocomplete-list');
      autocompleteList.innerHTML = ''; // Limpiar resultados anteriores

    });


    showModalEstacion = new bootstrap.Modal(modalAgregar, {
      keyboard: false,
      backdrop: 'static'
    });

    document.getElementById('modal-agregar-estacion').addEventListener('click', function (event) {
      showModalEstacion.show();
    });

  }
  const handleSortEstacion = () => {

    const sortableList = document.getElementById('sortable-list');

    // Agregar eventos de arrastrar y soltar a cada elemento de la lista
    // Agregar eventos de arrastrar y soltar solo a los elementos li
    sortableList.addEventListener('dragstart', function (e) {
      if (e.target.tagName.toLowerCase() === 'li') {
        e.dataTransfer.setData('text/plain', e.target.innerHTML);
        e.target.classList.add('dragging');
      } else {
        e.preventDefault();
      }
    });

    sortableList.addEventListener('dragover', function (e) {
      e.preventDefault();
      const draggedElement = document.querySelector('.dragging');
      if (e.target.tagName.toLowerCase() === 'li') {
        const bounding = e.target.getBoundingClientRect();
        const offset = bounding.y + bounding.height / 2 > e.clientY ? 'beforebegin' : 'afterend';
        e.target.insertAdjacentElement(offset, draggedElement);
      }
    });

    sortableList.addEventListener('dragend', function () {
      const items = document.querySelectorAll('.sortable-item');
      items.forEach((item, index) => {
        item.querySelector('.order-label').textContent = index + 1;
        item.classList.remove('dragging');
      });
    });
  }


  // Agregar elementos al HTML usando el componente
  const sortableList = document.getElementById('sortable-list');

  // Supongamos que tienes un arreglo de datos para los elementos
  const itemsData = [
    { id: 1, label: 'Metro Constituyentes', coordinatesArray: [{ lat: 19.426108, lng: -99.16951 }], order: 1 },
    { id: 2, label: 'Metro Constitucion', coordinatesArray: [{ lat: 19.426108, lng: -99.16951 }], order: 2 },
    { id: 3, label: 'Santa Fe', coordinatesArray: [{ lat: 19.426108, lng: -99.16951 }], order: 3 },

    // Agrega más datos según sea necesario
  ];

  class SortableItem {
    constructor(id, label, coordinatesArray, order) {
      this.id = id;
      this.label = label;
      this.coordinatesArray = coordinatesArray;
      this.order = order; // Variable de orden, podrías ajustarla según tus necesidades
    }

    render() {
      const listItem = document.createElement('li');
      listItem.className = 'sortable-item';
      listItem.draggable = true;

      listItem.innerHTML = `
      <div class="m-0">
        <div class="d-flex align-items-sm-center mb-3">
          <div class="symbol symbol-circle symbol-40px me-4">
            <span class="symbol-label bg-primary">
              <span class="order-label text-inverse-primary fs-4">${this.order}</span>
            </span>
          </div>
          <div class="d-flex align-items-center flex-row-fluid flex-wrap">
            <div class="flex-grow-1 me-2">
              <span class="text-gray-800 fw-bold d-block fs-4">${this.label}</span>
            </div>
            <div>

              <i class="fa-solid fa-map-location-dot fs-3 text-muted me-3 my-2 add-new-point" id="add${this.label}" onclick="KTRutas.addNewPoint(this)">
               <span class="path1"></span>
               <span class="path2"></span>
               <span class="path3"></span>
              </i>
              <i class="ki-duotone ki-arrow-up-down fs-3 text-muted me-3 my-2">
                <span class="path1"></span>
                <span class="path2"></span>
                </i>
              <i class="ki-duotone ki-cross-circle fs-3 text-muted me-3 my-2 delete-sortable-item" id="delete${this.label}" onclick="KTRutas.deleteSortableItem(this)">
               <span class="path1"></span>
               <span class="path2"></span>
              </i>
            </div>
          </div>
        </div>
        <div class="timeline mx-13">
          ${this.renderTimelineItems()}
        </div>
      </div>
      <div class="separator separator-dashed my-6"></div>
    `;

      return listItem;
    }

    renderTimelineItems() {
      return this.coordinatesArray.map((coordinates, index) => `
      <div class="timeline-item align-items-center mb-3">
        <div class="timeline-icon" style="margin-left: 11px">
          <i class="bi bi-arrow-return-right fs-2 text-gray-400"></i>
        </div>
        <div class="timeline-content m-0 d-flex">
          <span class="fs-6 text-gray-400 fw-semibold d-block">(${coordinates.lat},${coordinates.lng})</span>
         ${index !== 0 ? `<i class="ki-duotone ki-cross-circle fs-3 text-muted ms-4 delete-timeline-item" onclick="KTRutas.deleteTimelineItem(this, ${index})"> <span class="path1"></span><span class="path2"></span></i></i>` : ''}
        </div>
        
      </div>
    `).join('');
    }
  }

  //


  // Función para eliminar un elemento del timeline
  function deleteTimelineItem(button, index) {
    const listItem = button.closest('.sortable-item');
    if (index > 0) {
      // Asume que 'itemsData' es la matriz de datos original
      const itemIndex = Array.from(listItem.parentElement.children).indexOf(listItem);
      itemsData[itemIndex].coordinatesArray.splice(index, 1);

      // Vuelve a renderizar la lista
      renderSortableList();
    }
  }

  // Función para agregar un nuevo punto
  const addNewPoint = (button) => {

    const listItem = button.closest('.sortable-item');
    // Asume que 'itemsData' es la matriz de datos original
    const index = Array.from(listItem.parentElement.children).indexOf(listItem);
    //const newCoordinates = '(20.000000, -100.000000)';
    //itemsData[index].coordinatesArray.push(newCoordinates);
    const lastCoordinates = itemsData[index].coordinatesArray[itemsData[index].coordinatesArray.length - 1];
    const modalSeleccionar = document.getElementById('kt_modal_seleccionar_punto');


    const showModal = new bootstrap.Modal(modalSeleccionar, {
      keyboard: false,
      backdrop: 'static'
    });

    showModal.show();

    initMapModal(lastCoordinates).then((newCoordinates) => {
      if (newCoordinates) {
        itemsData[index].coordinatesArray.push(newCoordinates);
        // Vuelve a renderizar la lista
        renderSortableList();
        showModal.hide();
      }
    });


  }




  const initMapModal = (lastCoordinates) => {

    let mapModal = {};
    let markerModal = {};
    let locationModal = {};
    let buttonSeleccionar = document.getElementById('seleccionar-punto');
    mapModal = new google.maps.Map(document.getElementById("mapModal"), {
      center: { lat: lastCoordinates.lat, lng: lastCoordinates.lng },
      zoom: 15
    });

    markerModal = new google.maps.Marker({
      position: { lat: lastCoordinates.lat, lng: lastCoordinates.lng },
      map: mapModal,
    });

    mapModal.addListener('click', function (event) {
      placeMarkerModal(event.latLng);
    });

    const placeMarkerModal = (location) => {
      if (markerModal) {
        markerModal.setMap(null);
      }
      markerModal = new google.maps.Marker({
        position: location,
        map: mapModal
      });

      locationModal = location;
      console.log("modal marker lat lng: ", location.lat(), location.lng());
      //markers.push(marker);


    }


    return new Promise((resolve) => {
      buttonSeleccionar.onclick = () => {
        const selectedCoordinates = { lat: locationModal.lat(), lng: locationModal.lng() };
        resolve(selectedCoordinates);
      };


    });
  }

  // Función para eliminar un elemento sortable
  const deleteSortableItem = (button) => {
    const listItem = button.closest('.sortable-item');
    // Asume que 'itemsData' es la matriz de datos original
    const index = Array.from(listItem.parentElement.children).indexOf(listItem);
    itemsData.splice(index, 1);

    // Vuelve a renderizar la lista
    renderSortableList();
  }

  // Función para renderizar la lista
  const renderSortableList = () => {
    sortableList.innerHTML = '';
    itemsData.forEach(data => {
      const sortableItem = new SortableItem(data.id, data.label, data.coordinatesArray, data.order);
      sortableList.appendChild(sortableItem.render());
    });
  }

  const initSortableList = () => {

    // Inicializar la lista
    renderSortableList();
  }


  const handleSeachSelectEstacion = () => {

    document.getElementById('search-agregar-estacion').addEventListener('click', function (event) {

      const selectedRadioButton = document.querySelector('input[name="cbestacion"]:checked');

      if (selectedRadioButton) {
        const selectedItem = JSON.parse(selectedRadioButton.dataset.item);

        const lastOrder = itemsData.length > 0 ? itemsData[itemsData.length - 1].order : 0;
        itemsData.push({
          id: selectedItem.idparada, label: selectedItem.nombre, coordinatesArray: [{
            lat: selectedItem.latitud
            , lng: selectedItem.longitud
          }], order: lastOrder + 1
        });

        renderSortableList();

        showModalEstacion.hide();

      } else {
        Swal.fire({
          text: "Debe seleccionar una",
          icon: "warning",
          buttonsStyling: false,
          confirmButtonText: "Ok",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        });
      }
    });
  }

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_ruta_in_form');
      submitButton = document.querySelector('#kt_ruta_in_submit');

      handleValidation();
      initModalEvent();
      handleSeachSelectEstacion();

      handleSubmitValidation(); // use for form validation submit

      handleSearchEstacion();
      handleSortEstacion();
      initSortableList();

    },
    initMap: function () {
      handleInitMap();
    },
    addNewPoint: function (value) {
      addNewPoint(value);
    },
    deleteSortableItem: function (value) {
      deleteSortableItem(value);
    },
    deleteTimelineItem: function (button, index) {
      deleteTimelineItem(button, index);
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTRutas.init();
});
