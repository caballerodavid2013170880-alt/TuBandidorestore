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

  let autocompleteListener;

  let durationsContainer;
  let isSearching = false; // Variable para evitar búsquedas concurrentes

  let showModalEstacion;

  const sortableList = document.getElementById('sortable-list');
  let itemsData = [];
  let kmTotalesRuta = 0;
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


      const inputPuntos = document.getElementById('PuntosJson');
      if (itemsData !== null && itemsData.length > 0) {

        inputPuntos.value = JSON.stringify(itemsData.sort((a, b) => a.order - b.order));
      }

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

  let arrayTiempo = [];
  const handleInitMapPreview = () => {
    // carga puntos de BD
    if (itemsData === null || itemsData.length === 0) {
      return;
    }
    arrayTiempo = [];
    console.log(itemsData);
    const coordenadas = itemsData.sort((a, b) => a.order - b.order).map(item => {

      // agrega una propiedad al indice 0 para mostrar un icono diferente
      item.coordinatesArray[0].icon = 'blue-pushpin';
      item.coordinatesArray[0].estacionName = item.label;
      return item.coordinatesArray
    }).flat();

    console.log(coordenadas);

    const map = new google.maps.Map(document.getElementById('map'), {
      center: coordenadas[0],
      zoom: 6
    });

    const directionsService = new google.maps.DirectionsService();
    const directionsDisplay = new google.maps.DirectionsRenderer({
      map: map,
      suppressMarkers: true
    });

    const waypoints = coordenadas.slice(1, -1).map(coord => ({
      location: new google.maps.LatLng(coord.lat, coord.lng),
      stopover: true
    }));

    const request = {
      origin: new google.maps.LatLng(coordenadas[0].lat, coordenadas[0].lng),
      destination: new google.maps.LatLng(coordenadas[coordenadas.length - 1].lat, coordenadas[coordenadas.length - 1].lng),
      waypoints: waypoints,
      travelMode: 'DRIVING'
    };

    directionsService.route(request, function (response, status) {
      if (status === 'OK') {
        directionsDisplay.setDirections(response);

        const googleMapsDirections = document.getElementById('GoogleMapsRuta');
        googleMapsDirections.value = '';
        if (response) {
          googleMapsDirections.value = JSON.stringify(response);
        }


        // Muestra el tiempo entre cada punto
        const route = response.routes[0];
        let resultadosHTML = '';
        kmTotalesRuta = 0;
        for (let i = 0; i < route.legs.length; i++) {
          const leg = route.legs[i];

          arrayTiempo.push(leg.duration.value);
          kmTotalesRuta += leg.distance.value;
        }
        // Muestra el tiempo total del viaje
        console.log(`Tiempo total de viaje: ${route.legs[route.legs.length - 1].duration.text}`);

        //rutaPreviewContainer.innerHTML = resultadosHTML;
        previewKmRuta();
        calculateTimeEstaciones();
        let counter = 0;
        let countLetter = 0;
        let letter = '';
        coordenadas.forEach((coord, index) => {
          let numberLabel = '';
          const iconColor = coord.icon || 'red-dot';
          const iconUrl = coord.icon ? '/assets/media/logos/van-pin 64x64.svg' : `http://maps.google.com/mapfiles/ms/icons/${iconColor}.png`;
          //const iconUrl = coord.icon ? `http://maps.google.com/mapfiles/ms/icons/blue-dot.png` : `http://maps.google.com/mapfiles/ms/icons/${iconColor}.png`;


          if (coord.icon) {
            counter = counter + 1;
            numberLabel = `${counter.toString()}`;
            countLetter = 0;
          } else {
            // 1a 1b 1c ...
            letter = String.fromCharCode(97 + countLetter++);

          }

          const markerEstacion = new google.maps.Marker({
            position: coord,
            map: map,

            label: {
              text: numberLabel || `${counter}${letter}`,
              color: '#ad0a1a', // Color del texto en blanco
              fontWeight: 'bold', // Texto en negrita
              fontSize: '16px'// Ajuste para colocar el label al costado derecho del icono
            },
            icon: {
              url: iconUrl,
              labelOrigin: new google.maps.Point(10, 34),
              scaledSize: new google.maps.Size(40, 40), // Tamaño personalizado del ícono
              anchor: new google.maps.Point(16, 32)
            }
          });

          if (coord.estacionName) {
            addTooltip(markerEstacion, coord.estacionName);

          }

        });

      } else {
        window.alert('Directions request failed due to ' + status);
      }
    });
  }

  // agregar tooltip a los marcadores de estaciones
  const addTooltip = (marker, estacionName) => {
    const tooltip = new google.maps.InfoWindow({
      content: `<strong>${estacionName}</strong>`
    });

    marker.addListener('click', function () {
      tooltip.open(map, marker);
    });
  }

  //calculo de tiempo de estaciones
  const calculateTimeEstaciones = () => {

    const cantidadPuntos = itemsData.length - 1;
    let tiempoIndex = 0;
    for (let i = 0; i <= cantidadPuntos - 1; i++) {

      const numeroPuntos = itemsData[i].coordinatesArray.length;
      // suma los tiempos de arraytiempo segun el numero de puntos
      let tiempoEstacion = 0;
      if (numeroPuntos >= 1) {
        for (let j = 0; j < numeroPuntos; j++) {
          tiempoEstacion += arrayTiempo[tiempoIndex];
          tiempoIndex++;
        }
        itemsData[i].tiempo = tiempoEstacion;

      }
    }

  }

  const previewKmRuta = () => {

    const kmTotalesRutaContainer = document.getElementById('km-totales');
    kmTotalesRutaContainer.innerHTML = '';
    kmTotalesRutaContainer.innerHTML = `<div class="d-flex align-items-center position-relative mb-7">
                      <div class="fw-semibold">
                        <span class="fs-1x fw-bold text-primary">${kmTotalesRuta / 1000} km</span>
                        <div class="text-gray-400">
                          Distancia total de la ruta
                        </div>
                      </div>
                    </div>`;

    const kmTotalesRutaInput = document.getElementById('Distancia');
    kmTotalesRutaInput.value = kmTotalesRuta;

  }

  const handleInitMap = () => {
    // carga puntos de BD
    const puntos = document.getElementById('PuntosJson').value;
    console.log(puntos);
    if (puntos !== "" && puntos.length > 0) {
      itemsData = JSON.parse(puntos);
      console.log(itemsData);

      handleInitMapPreview();
      renderSortableList();
    }

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

      updateOrderInModel();

    });
  }


  // Agregar elementos al HTML usando el componente

  // Supongamos que tienes un arreglo de datos para los elementos
  //const itemsData = [
  //  { id: 1, label: 'Metro Coyoacán', coordinatesArray: [{ lat: 19.3614721, lng: -99.1709613 }], order: 1 },
  //  { id: 2, label: 'Metro Constitucion', coordinatesArray: [{ lat: 19.3455754, lng: -99.0664272 }], order: 2 },
  //  { id: 3, label: 'Periférico Oriente', coordinatesArray: [{ lat: 19.3176101, lng: -99.0777824 }], order: 3 },

  //  // Agrega más datos según sea necesario
  //];

  class SortableItem {
    constructor(id, label, coordinatesArray, order, tipo) {
      this.id = id;
      this.label = label;
      this.coordinatesArray = coordinatesArray;
      this.order = order; // Variable de orden, podrías ajustarla según tus necesidades
      this.tipo = tipo;
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
              <span class="text-gray-800 fw-bold  fs-4">${this.label}</span>
             ${this.rendertipoEstacion(this.tipo)}

            </div>
            <div>
             <i class="ki-duotone ki-pencil fs-3 text-muted me-3 my-2 edit-new-point" id="edit${this.label}" onclick="KTRutas.editPoint(this)">
               <span class="path1"></span>
               <span class="path2"></span>
              </i>
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
        <div class="timeline mx-13" >
          ${this.renderTimelineItems()}
        </div>
      </div>
      <div class="separator separator-dashed my-6"></div>
    `;


      return listItem;
    }


    rendertipoEstacion(tipo) {

      if (tipo == '1') {
        return `<span class="text-gray-600 fw-bold  fs-6"> - Solo subida </span>`;
      } else if (tipo == '2') {
        return `<span class="text-gray-600 fw-bold  fs-6"> - Subida y bajada </span>`;
      } else if (tipo == '3') {
        return `<span class="text-gray-600 fw-bold  fs-6"> - Solo bajada </span>`;
      } else {
        return ``;
      }


    }

    renderTimelineItems() {
      return this.coordinatesArray.map((coordinates, index) => `
      <div class="timeline-item align-items-center mb-3">
        <div class="timeline-icon" style="margin-left: 11px">
          ${index !== 0 ? `<i class="bi bi-arrow-return-right fs-2 text-gray-400"></i>`
          : ` <i class="ki-duotone ki-geolocation fs-2 text-gray-400"><span class="path1"></span><span class="path2"></span> </i>`}
        </div>
        <div class="timeline-content m-0 d-flex">
          <span class="fs-6 text-gray-400 fw-semibold d-block">(${coordinates.lat},${coordinates.lng})</span>
         ${index !== 0 ? `<i class="ki-duotone ki-cross-circle fs-3 text-muted ms-4 delete-timeline-item" onclick="KTRutas.deleteTimelineItem(this, ${index})"> <span class="path1"></span><span class="path2"></span></i></i>` : ''}
        </div>
        
      </div>
    `).join('');
    }
  }


  // Función para actualizar el orden en el modelo de datos (itemsData)
  const updateOrderInModel = () => {
    const sortableItems = document.querySelectorAll('.sortable-item');

    // Actualizar el orden en el modelo de datos (itemsData)
    const itemsDataUpdate = Array.from(sortableItems).map((item, index) => {
      const label = item.querySelector('.text-gray-800').innerText;
      const order = parseInt(item.querySelector('.order-label').innerText);

      return { label, order };
    });


    itemsDataUpdate.forEach(updatedItem => {
      // Encuentra el elemento correspondiente en itemsData por id
      const targetItem = itemsData.find(item => item.label === updatedItem.label);

      // Si se encuentra el elemento, actualiza la propiedad order
      if (targetItem) {
        targetItem.order = updatedItem.order;
      }
    });

    renderSortableList();
    console.log(itemsData);
    // Puedes hacer algo más con los datos actualizados si es necesario
  }

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

  const editPoint = (button) => {
   const listItem = button.closest('.sortable-item');
    // Asume que 'itemsData' es la matriz de datos original
    const index = Array.from(listItem.parentElement.children).indexOf(listItem);

    console.log(itemsData[index]);
     const modalSeleccionarTipo = document.getElementById('kt_modal_editar');


    const showModal = new bootstrap.Modal(modalSeleccionarTipo, {
      keyboard: false,
      backdrop: 'static'
    });

    showModal.show();
    // limpia los valores de los radiobutton cktipoestacionEditar para seleccionar el tipo de estacion
    const selectedTipoEstacion = document.querySelector('input[name="cktipoestacionEditar"]:checked');
    if (selectedTipoEstacion) {
      selectedTipoEstacion.checked = false;
    }


    selecctionarTipoEstacion().then((nuevoTipo) => {
    
      if (nuevoTipo) {
        itemsData[index].tipo = parseInt(nuevoTipo);
        // Vuelve a renderizar la lista
        renderSortableList();
        showModal.hide();
      }
     
    });
  }

  const selecctionarTipoEstacion =() => {
  
    const buttonSeleccionar = document.getElementById('button-editar-tipo');
     
     return new Promise((resolve) => {
      buttonSeleccionar.onclick = () => {
        const selectedTipoEstacion = document.querySelector('input[name="cktipoestacionEditar"]:checked');
        if(!selectedTipoEstacion)
        { notificacion.info('Seleccione un tipo de estaci&oacute;n');}
        else  { resolve(selectedTipoEstacion.value);}
       
      };


    });
  
  }

  const initMapModal = (lastCoordinates) => {

    if (autocompleteListener) {
      google.maps.event.removeListener(autocompleteListener);
    }

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



    let autocomplete;
    const inputSearch = document.getElementById("search-place");
    inputSearch.value = '';
    autocomplete = new google.maps.places.Autocomplete(inputSearch);
    autocomplete.bindTo("bounds", mapModal);

    autocompleteListener = autocomplete.addListener("place_changed", function () {
      const place = autocomplete.getPlace();

      if (!place.geometry) {
        Swal.fire({
          text: `El lugar seleccionado no tiene coordenadas de ubicación`,
          icon: "error",
          buttonsStyling: false,
          confirmButtonText: "Aceptar",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          }
        });
        return;
      }

      // Centra el mapa y coloca el marcador en la ubicación seleccionada
      mapModal.setCenter(place.geometry.location);
      markerModal.setPosition(place.geometry.location);
      locationModal = place.geometry.location;
      //// Obtén datos de geocodificación
      //const geocoder = new google.maps.Geocoder();
      //geocoder.geocode({ location: place.geometry.location }, (results, status) => {
      //  if (status === "OK") {
      //    if (results[0]) {
      //      //getMapsProperties(results[0]);
      //    }
      //  } else {
      //    Swal.fire({
      //      text: `Error de geocodificación: ${status}`,
      //      icon: "error",
      //      buttonsStyling: false,
      //      confirmButtonText: "Aceptar",
      //      customClass: {
      //        confirmButton: "btn fw-bold btn-primary",
      //      }
      //    });
      //  }
      //});
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
    itemsData.sort((a, b) => a.order - b.order).forEach(data => {
      const sortableItem = new SortableItem(data.id, data.label, data.coordinatesArray, data.order, data.tipo);
      sortableList.appendChild(sortableItem.render());
    });

    handleInitMapPreview();
  }

  const initSortableList = () => {

    // Inicializar la lista
    renderSortableList();
  }


  const handleSeachSelectEstacion = () => {

    document.getElementById('search-agregar-estacion').addEventListener('click', function (event) {

      const selectedRadioButton = document.querySelector('input[name="cbestacion"]:checked');
      const selectedTipoEstacion = document.querySelector('input[name="cktipoestacion"]:checked');



      if (selectedRadioButton && selectedTipoEstacion) {
        const selectedItem = JSON.parse(selectedRadioButton.dataset.item);
        const selectedTipo = selectedTipoEstacion.value;

        const existLabel = itemsData.find(item => item.label === selectedItem.nombre);

        if (existLabel) {
          Swal.fire({
            text: "Ya existe en la ruta, no se puede agregar",
            icon: "warning",
            buttonsStyling: false,
            confirmButtonText: "Ok",
            customClass: {
              confirmButton: "btn fw-bold btn-primary",
            }
          });
          return;
        }

        const lastOrder = itemsData.length > 0 ? itemsData[itemsData.length - 1].order : 0;
        itemsData.push({
          id: selectedItem.idparada,
          label: selectedItem.nombre,
          tipo: selectedTipo,
          coordinatesArray: [{
            lat: selectedItem.latitud
            , lng: selectedItem.longitud
          }],
          order: lastOrder + 1,
          tiempo: 0
        });

        renderSortableList();

        showModalEstacion.hide();

      } else {
        Swal.fire({
          text: "Debe seleccionar una estacion y tipo",
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

  const handlePreviewMap = () => {
    document.getElementById('preview-ruta').addEventListener('click', function (event) {
      handleInitMapPreview();
    });
  }

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_ruta_in_form');
      submitButton = document.querySelector('#kt_ruta_in_submit');

      handleValidation();
      handlePreviewMap();
      initModalEvent();
      handleSeachSelectEstacion();

      handleSubmitValidation(); // use for form validation submit

      handleSearchEstacion();
      handleSortEstacion();
      initSortableList();

    },
    initMap: function () {
      //handleInitMap();
      handleInitMap();
    },
    editPoint: function(value) {
      editPoint(value);
    },
    addNewPoint: function (value) {
      addNewPoint(value);
    },
    deleteSortableItem: function (value) {
      deleteSortableItem(value);
    },
    deleteTimelineItem: function (button, index) {
      deleteTimelineItem(button, index);
    },
    allowDrop: function (event) {
      allowDrop(event);
    },
    handleDrop: function (event) {
      handleDrop(event);
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTRutas.init();
});
