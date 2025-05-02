"use strict";

// Class definition
var KTRastreo = function () {
  // Elements
  let map;
  let aMarkers = [];
  let aMarkersP = [];
  let aPolylines = [];

  let aRutasShow = [],
    aCorridasShow = [];

  let vIdRuta = 0;
  let vIdVehiculo = "";
  let tbRutas, tbCorridas;

  const handleInitMapPreview = (pIdRuta = 0) => {

    // Construimos el mapa principal y le cargamos los marcadores de las unidades
    var mapOptions = {
      mapTypeId: google.maps.MapTypeId.ROADMAP,
      center: { lat: 19.4328161, lng: -99.1335195 },
      zoom: 15
    };
    const map = new google.maps.Map(document.getElementById('map'), mapOptions);
    map.setTilt(50);

    handlePopulateMap(map, pIdRuta);
  };

  const handlePopulateMap = (map, pIdRuta = 0) => {

    aRutasShow = pIdRuta == 0 ? aRutas : aRutas.filter(x => x.Idruta == pIdRuta);
    aCorridasShow = pIdRuta == 0 ? aCorridas : aCorridas.filter(x => x.IdRuta == pIdRuta);

    if (aRutasShow.length > 0) {
      var bounds = new google.maps.LatLngBounds();

      const infoContent = '<div>' +
        '<h3>Placa: @@PLACA@@</h3>' +
        '<p>Conductor: @@CONDUCTOR@@' +
        '<br /><img class="chat" src="/assets/media/icons/duotune/communication/com007.svg" id="chatMap" data-type="2" data-id="@@IDCONDUCTOR@@"></p>' +
        '</div>'

      var vInfo = [];

      // Marker para Unidades
      var infoWindow = new google.maps.InfoWindow(), marker, i = 0;
      var icon = {
        url: "/assets/media/svg/van-icon-black.svg",
        origin: new google.maps.Point(0, 0)
      };

      var ids = aCorridasShow.map(Corrida => Corrida.IdcorridaAsignacion);

      Promise.all(
        ids.map(id => {
          i = 0;
          firebaseApp.database().ref('/' + id)
            .on('value', function (snapshot) {
              const data = snapshot.val();
              if (data) {
                //  console.log(id + " => latitude: " + data.latitude + ", longitude: " + data.longitude);

                var vCorrida = aCorridasShow.filter(x => x.IdcorridaAsignacion == id)[0];

                var vLatLong = new google.maps.LatLng(data.latitude, data.longitude);

                // Si el marcador ya fue previamente creado, solo actualizamos su ubicación.
                var j = aMarkers.findIndex(x => x.get('idCorridaAsignacion') == id);

                if (j != -1) {
                  aMarkers.filter(x => x.get('idCorridaAsignacion') == id)[0].setPosition(vLatLong);
                  return true;
                }

                // Construimos la seccion de información de la unidad
                vInfo.push(infoContent.replace('@@PLACA@@', vCorrida.Placa));
                var vIDInf = vInfo.length - 1;
                vInfo[vIDInf] = vInfo[vIDInf].replace('@@CONDUCTOR@@', vCorrida.NombreConductor);
                vInfo[vIDInf] = vInfo[vIDInf].replace('@@IDCONDUCTOR@@', vCorrida.IdConductor);

                // Construimos el marcador
                marker = new google.maps.Marker({
                  position: vLatLong,
                  map: map,
                  title: vCorrida.Placa,
                  animation: google.maps.Animation.DROP,
                  icon: icon,
                  idCorridaAsignacion: id,
                  idInfo: vIDInf
                });
                aMarkers.push(marker);
                google.maps.event.addListener(marker, 'click', (function (marker, vIDInf) {
                  return function () {
                    infoWindow.setContent(vInfo[vIDInf]);
                    const abortController = new AbortController();
                    google.maps.event.addListener(infoWindow, 'domready', function () {
                      document.getElementById("chatMap").addEventListener("click", handleChatClick, { signal: abortController.signal });
                    });
                    infoWindow.open(map, marker);

                    var handleChatClick = function (e) {
                      var img = e.currentTarget;
                      KTRastreo.chat(img.dataset.type, img.dataset.id);
                    };
                  }
                })(marker, vIDInf));
              }
            });
          i++;
        })
      ).then(r => (function () {
        //console.log('Finalización de mapeo!!!');
      }));

      // Cargamos los marcadores de las paradas al Mapa
      aRutasShow.forEach(Ruta => {

        Ruta.Paradas.forEach(Parada => {
          var positionP = new google.maps.LatLng(Parada.Latitude, Parada.Longitude);
          marker = new google.maps.Marker({
            position: positionP,
            map: map,
            title: Parada.Nombre
          });
          aMarkersP.push(marker);
        });
      })

      var aColors = ["#7d3c98", "#000", "#922b21", "#dc7633", "#28b463"]
      var aPuntos = [];
      var pPoint;

      i = 0;

      aRutasShow.forEach(ruta => {

        aPuntos = [];
        var vGeoRuta = JSON.parse(ruta.GeoRuta);
        vGeoRuta.routes[0].overview_path.forEach(x => {
          pPoint = new google.maps.LatLng(x.lat, x.lng);
          bounds.extend(pPoint);
          aPuntos.push(pPoint);
        });

        var polyline = new google.maps.Polyline({
          Idruta: ruta.Idruta,
          path: aPuntos,
          geodesic: true,
          strokeColor: aColors[i],
          strokeOpacity: 1.0,
          strokeWeight: 5
        });
        polyline.setMap(map);
        aPolylines.push(polyline);
        google.maps.event.addListener(polyline, 'click', function (e) {
          this.strokeWeight = 10;
          handlePolyClick(e, this);
        });

        var handlePolyClick = function (eventArgs, polyLine) {
          // now you can access the polyLine
          //KTRastreo.chat(1, polyLine.Idruta);

          tbRutas.destroy();
          tbCorridas.destroy();

          vIdRuta = polyLine.Idruta;
          handleRutas(vIdRuta);
          handleCorridas(vIdRuta);
          handleInitMapPreview(vIdRuta);
        };

        i++;
      });

      map.fitBounds(bounds);
    }
  };

  const handleInitMap = () => {
    handleInitMapPreview();
  }

  const handleRutas = (pIdRuta = 0) => {

    aRutasShow = pIdRuta == 0 ? aRutas : aRutas.filter(x => x.Idruta == pIdRuta);

    tbRutas = new DataTable("#dtRutas", {
      columns: [
        { title: "ID", data: "Idruta" },
        { title: "Nombre Ruta", data: "Nombre" },
        {
          title: "",
          render: function (data, type, row, meta) {
            return '<img src="/assets/media/icons/duotune/communication/com007.svg" class="chat" data-type="1" data-id="' + row.Idruta + '">';
          }
        }
      ],
      data: aRutasShow,
      info: false
    });

    var vRutaSeleccionada;

    $('#dtRutas tbody').on('click', 'tr', function () {
      vRutaSeleccionada = tbRutas.row(this).data();
      vIdRuta = vRutaSeleccionada.Idruta;
      tbCorridas.destroy();

      // Depuramos los marcadores de las unidades
      aMarkers.forEach(m => {
        m.setMap(null);
      });
      aMarkers = [];

      // Depuramos los marcadores de las estaciones
      aMarkersP.forEach(m => {
        m.setMap(null);
      });
      aMarkersP = [];

      // Limpiamos las rutas
      aPolylines.forEach(p => {
        p.setMap(null);
      });
      aPolylines = [];

      handleCorridas(vIdRuta);
      handleInitMapPreview(vIdRuta);

      if ($(this).hasClass('selected')) {
        $(this).removeClass('selected');
      } else {
        tbRutas.$('tr.selected').removeClass('selected');
        $(this).addClass('selected');
      }
    });
  }

  const handleCorridas = (pIdRuta = 0) => {

    aCorridasShow = pIdRuta == 0 ? aCorridas : aCorridas.filter(x => x.IdRuta == pIdRuta);
    tbCorridas = new DataTable("#dtUnidades", {
      columns: [
        { title: "ID", data: "IdcorridaAsignacion", visible: false },
        { title: "Vehiculo", data: "Placa" },
        { title: "Operador", data: "NombreConductor" },
        { title: "Ruta", data: "IdRuta", visible: false },
        {
          title: "",
          render: function (data, type, row, meta) {
            return '<img src="/assets/media/icons/duotune/communication/com007.svg" class="chat" data-type="2" data-id="' + row.IdConductor + '">';
          }
        }
      ],
      data: aCorridasShow,
      info: false
    });
  }

  const handleRefresh = () => {
    $.getJSON(vURLUpdate, function (pData) {

      aRutas = pData.Rutas;
      aCorridas = pData.Corridas;

      tbRutas.destroy();
      tbCorridas.destroy();

      if (aRutas.length > 0) {
        vIdRuta = aRutas[0].Idruta;
        KTRastreo.populateRutas();
        KTRastreo.populateCorridas(vIdRuta);

        var vChatItems = document.querySelectorAll('.chat');
        vChatItems.forEach(function (item) {
          item.addEventListener('click', function () {
            KTRastreo.chat(item.dataset.type, item.dataset.id);
          });
        });

        // Depuramos los marcadores de las unidades
        aMarkers.forEach(m => {
          m.setMap(null);
        });
        aMarkers = [];

        // Depuramos los marcadores de las estaciones
        aMarkersP.forEach(m => {
          m.setMap(null);
        });
        aMarkersP = [];

        // Limpiamos las rutas
        aPolylines.forEach(p => {
          p.setMap(null);
        });
        aPolylines = [];
      }
      else {
        KTRastreo.populateRutas();
        KTRastreo.populateCorridas(0);
      }
      handleInitMapPreview();
    });
    //window.removeEventListener('click', this.clickHandler)        
  }

  const chat = (type, id) => {
    var data;
    var vType = parseInt(type);

    switch (vType) {
      case 0:
        data = {
          tipoConversacion: vType
        };
        break;
      case 1:
        data = {
          tipoConversacion: vType,
          rutaId: parseInt(id)
        };
        break;
      case 2:
        data = {
          tipoConversacion: vType,
          operadorId: parseInt(id)
        };
        break;
      default:
        break;
    }
    fnMensajeriaSignalR.open(data);
    //alert("Type: " + type + "\nId: " + id);
  }

  // Public functions
  return {
    // Initialization
    init: function () {
      if (aRutas.length > 0) {
        aRutasShow = aRutas;
        aCorridasShow = aCorridas;
        vIdRuta = aRutasShow[0].Idruta;
        KTRastreo.populateRutas();
        KTRastreo.populateCorridas(vIdRuta);
        var vChatItems = document.querySelectorAll('.chat');
        vChatItems.forEach(function (item) {
          //item.clickHandler = KTRastreo.chat(item.dataset.type, item.dataset.id);
          //item.addEventListener('click', item.clickHandler);
          item.addEventListener('click', function () {
            KTRastreo.chat(item.dataset.type, item.dataset.id);
          });
        });

        document.getElementById("btnSearch").addEventListener('click', function () {
          var aCadena = $("#search").val().trim().toUpperCase();

          if (aCadena.length > 0) {

            // Depuramos los marcadores de las unidades
            aMarkers.forEach(m => {
              m.setMap(null);
            });
            aMarkers = [];

            // Depuramos los marcadores de las estaciones
            aMarkersP.forEach(m => {
              m.setMap(null);
            });
            aMarkersP = [];

            // Limpiamos las rutas
            aPolylines.forEach(p => {
              p.setMap(null);
            });
            aPolylines = [];

            let myReg = new RegExp(".*" + aCadena + ".*");
            var vRuta = aRutas.find(x => x.Nombre.toUpperCase().match(myReg));
            var vCorrida = aCorridas.find(x => x.Placa.toUpperCase().match(myReg));

            if (typeof vRuta != 'undefined' || typeof vCorrida != 'undefined') {
              tbRutas.destroy();
              tbCorridas.destroy();
              if (typeof vRuta != 'undefined') {
                vIdRuta = vRuta.Idruta;
              } else {
                vIdRuta = vCorrida.IdRuta;
                vIdVehiculo = vCorrida.IdVehiculo;
              }
              handleRutas(vIdRuta);
              handleCorridas(vIdRuta);
              handleInitMapPreview(vIdRuta);
            } else {
              alert("No existen Veh�culos/Rutas con la cadena capturada (" + $("#search").val() + ").");
              $("#search").val("");
            }
          } else {
            tbRutas.destroy();
            tbCorridas.destroy();

            vIdRuta = aRutas[0].Idruta;
            handleRutas(0);
            handleCorridas(vIdRuta);
            handleInitMapPreview();
          }
        });
      }
      else {
        KTRastreo.populateRutas();
        KTRastreo.populateCorridas(0);
        handleInitMapPreview();
      }

      setInterval(function () {
        KTRastreo.refreshRastreo();
      }, 30000);
    },

    initMap: function () {
      handleInitMap();
    },

    populateRutas: function () {
      handleRutas();
    },

    populateCorridas: function (pIdRuta) {
      handleCorridas(pIdRuta);
    },

    refreshRastreo: function () {
      handleRefresh();
    },

    getMap: function () {
      return map;
    },

    chat: function (type, id) {
      chat(type, id);
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTRastreo.init();
});
