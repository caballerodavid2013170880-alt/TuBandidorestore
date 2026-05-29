"use strict";

// Class definition
var KTEstacion = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  let map;
  let marker;
  let autocomplete;

  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {

          'NombreEstacion': {
            validators: {
              notEmpty: {
                message: 'Nombre de la estaci&oacute;n requerida'
              },
              stringLength: {
                min: 3,
                max: 250,

                message: 'deben tener entre 3 y 250 caracteres',
              },
            }
          },

          'Calle': {
            validators: {
              notEmpty: {
                message: 'Nombre de la calle requerida'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Municipio': {
            validators: {
              notEmpty: {
                message: 'Municipio requerido'
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Ciudad': {
            validators: {
              notEmpty: {
                message: 'Ciudad requerida'
              },
              stringLength: {
                min: 3,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Colonia': {
            validators: {
              notEmpty: {
                message: 'Colonia requerida'
              },
              stringLength: {
                min: 3,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Numero': {
            validators: {
              notEmpty: {
                message: 'N&uacute;mero requerido'
              }
            }
          },
          'CodigoPostal': {
            validators: {
              regexp: {
                regexp: /^\d{5}$/,
                message: 'C&oacute;digo postal debe contener 5 d&iacute;gitos',
              },
            }
          },
          Latitud: {
            validators: {
              notEmpty: {
                message: 'La latitud es requerida'
              },
              regexp: {
                regexp: /^-?\d{1,2}(?:\.\d{1,20})?$/,
                message: 'Ingrese una latitud v&aacute;lida'
              }
            }
          },
          Longitud: {
            validators: {
              notEmpty: {
                message: 'La longitud es requerida'
              },
              regexp: {
                regexp: /^-?\d{1,3}(?:\.\d{1,20})?$/,
                message: 'Ingrese una longitud v&aacute;lida'
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

    const lat = document.getElementById('Latitud').value;
    const lng = document.getElementById('Longitud').value;

    if (lat !== "0.00000000" && lng !== "0.00000000") {
      initialLocation = { lat: parseFloat(lat), lng: parseFloat(lng) };
    }

    // Configurar el mapa
    map = new google.maps.Map(document.getElementById("map"), {
      center: initialLocation,
      zoom: 19,
    });

    marker = new google.maps.Marker({
      position: initialLocation,
      map: map,
    });


    const inputSearch = document.getElementById("search-place");
    autocomplete = new google.maps.places.Autocomplete(inputSearch);
    autocomplete.bindTo("bounds", map);

    autocomplete.addListener("place_changed", function () {
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
      map.setCenter(place.geometry.location);
      marker.setPosition(place.geometry.location);

      // Obtén datos de geocodificación
      const geocoder = new google.maps.Geocoder();
      geocoder.geocode({ location: place.geometry.location }, (results, status) => {
        if (status === "OK") {
          if (results[0]) {
            getMapsProperties(results[0]);
          }
        } else {
          Swal.fire({
            text: `Error de geocodificación: ${status}`,
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Aceptar",
            customClass: {
              confirmButton: "btn fw-bold btn-primary",
            }
          });
        }
      });
    });

    // Agregar un marcador al hacer clic en el mapa
    map.addListener("click", (event) => {
      placeMarker(event.latLng);
    });
  }

  var placeMarker = function (location) {
    // Elimina el marcador anterior si existe
    if (marker) {
      marker.setMap(null);
    }


    // Crear un nuevo marcador
    marker = new google.maps.Marker({
      position: location,
      map: map,
    });

    // Obtener datos de geocodificación
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode({ location: location }, (results, status) => {
      if (status === "OK") {
        if (results[0]) {
          getMapsProperties(results[0]);
        }
      } else {
        Swal.fire({
          text: `Error de geocodificación: ${status}`,
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
  var getMapsProperties = function (result) {
    // Rellenar el formulario con los datos de geocodificación
    console.log(result);
    const addressComponents = result.address_components;


    const adressData = {
      latitud: result.geometry.location.lat(),
      longitud: result.geometry.location.lng(),
      calle: '',
      numero: '',
      municipio: '',
      ciudad: '',
      colonia: '',
      estado: '',
      codigoPostal: '',
    };
    addressComponents.forEach(component => {
      if (component.types.includes('route')) {
        adressData.calle = component.long_name;
      } else if (component.types.includes('street_number')) {
        adressData.numero = component.long_name;
      } else if (component.types.includes('locality')) {
        adressData.ciudad = component.long_name;
      } else if (component.types.includes('administrative_area_level_1')) {
        adressData.estado = component.long_name;
      } else if (component.types.includes('administrative_area_level_2')) {
        adressData.municipio = component.long_name;
      } else if (component.types.includes('neighborhood') || component.types.includes('sublocality')) {
        adressData.colonia = `${adressData.colonia} ${component.long_name}`;
      } else if (component.types.includes('postal_code')) {
        adressData.codigoPostal = component.long_name;
      }
    });


    fillForm(adressData);
  }

  var fillForm = async function (data) {
    console.log(data);
    const municipio = await searchMunicipio(data.codigoPostal);
    if (municipio === undefined) {
      data.municipio = data.ciudad;
    } else {
      data.municipio = municipio;
    }

    document.getElementById('Latitud').value = data.latitud;
    document.getElementById('Longitud').value = data.longitud;
    document.getElementById('Calle').value = data.calle;
    document.getElementById('Numero').value = data.numero;
    document.getElementById('Municipio').value = data.municipio;
    document.getElementById('Ciudad').value = data.ciudad;
    document.getElementById('Colonia').value = data.colonia;
    document.getElementById('CodigoPostal').value = data.codigoPostal;
  }


  var searchMunicipio = async function (codigoPostal) {
    // Ruta del archivo JSON
    var jsonFilePath = '/json/muncipioscp.json';

    // Realiza una solicitud HTTP para obtener el contenido del archivo JSON
    return await fetch(jsonFilePath)
      .then(response => response.json())
      .then(data => {
        // Aquí puedes trabajar con los datos JSON
        //console.log(data);

        const resultado = data.find(elemento => elemento.cp === codigoPostal);

        // Si encontramos el elemento, devolvemos el municipio, de lo contrario, devolvemos null
        return resultado ? resultado.municipio : undefined;

      })
      .catch(error => console.error('Error al cargar el archivo JSON de municipios:', error));
  }

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_estacion_in_form');
      submitButton = document.querySelector('#kt_estacion_in_submit');


      handleValidation();

      handleSubmitValidation(); // use for form validation submit

    },
    initMap: function () {
      handleInitMap();
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTEstacion.init();
});
