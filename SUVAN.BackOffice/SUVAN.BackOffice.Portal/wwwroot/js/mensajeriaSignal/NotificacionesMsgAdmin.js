"use strict";
var KTNotificacionesAdmin = function () {

  const calculateTime = (fechaCreacion) => {

    // Convertir la propiedad de fecha a un objeto Date
    let fechaPropiedad = new Date(fechaCreacion);

    // Fecha y hora actual
    let fechaActual = new Date();

    // Calcular la diferencia en milisegundos
    let diferenciaMilisegundos = fechaActual - fechaPropiedad;

    // Convertir la diferencia a minutos totales
    let diferenciaMinutosTotales = Math.floor(diferenciaMilisegundos / (1000 * 60));

    // Calcular horas y minutos
    let diferenciaHoras = Math.floor(diferenciaMinutosTotales / 60);
    let diferenciaMinutos = diferenciaMinutosTotales % 60;

    // Retornar el resultado como string
    return `${diferenciaHoras} horas y ${diferenciaMinutos} minutos`;

  }

  const marcarMensajesLeidos = async (conversacionId) => {


    const response = await fetch('/Home/MarcarMensajesLeidos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        ConversacionId: conversacionId
      })
    });

    if (response.ok) {
      const data = await response.json();
      if (data.success) {
        fetchNotification();

      }

    }
  }


  const buildNotification = (data) => {

    const chatContainer = document.getElementById('kt_drawer_chat_container');
    const countContainer = document.getElementById('kt_drawer_chat_counter');

    if (!data.success || data.data.length == 0) {
      chatContainer.innerHTML = `
         <div class="d-flex justify-content-start mb-10">
          <div class="d-flex flex-column align-items-start">
            <div class="d-flex align-items-center mb-2">
              <div class="ms-3">
                <span class="fs-5 fw-bold text-gray-900 text-hover-primary me-1">Sin notificaciones de chat nuevas el día de hoy</span>
              </div>
            </div>
          </div>
        </div>
    `;
      countContainer.innerHTML = '0';

      return;
    }

    let messages = data.data.map((item, index) => `
         <div class="d-flex justify-content-start mb-10">
          <div class="d-flex flex-column align-items-start">
            <div class="d-flex align-items-center mb-2">
              <div class="ms-3">
                <span class="fs-5 text-gray-900 text-hover-primary me-1">Tienes un mensaje de </span>
                <span class="fs-5 fw-bold text-gray-900 text-hover-primary me-1">${item.usuario}</span>
                <span class="text-muted fs-7 mb-1"> ${item.fecha}</span>
              </div>
            </div>
          </div>
        </div>
    `).join('');


    chatContainer.innerHTML = messages;

    countContainer.innerHTML = data.data.length;

    $("#kt_drawer_chat_container").find(".text-hover-primary").each(function (index, element) {

      $(element).on('click', function () {

        let usuario = $(element).text().trim();

        const inboxContainer = $("#app_suvan_drawer_inbox_messenger_body");

        const userConversacion = inboxContainer.find("a.inbox-conversacion:contains('" + usuario + "')");
        userConversacion.trigger('click');
        $("#kt_drawer_chat_close_notificacion").trigger('click');
        const convversacionId = userConversacion.attr('data-conversacion');

        marcarMensajesLeidos(convversacionId);
      });

    });
  }


  const fetchNotification = async () => {

    const response = await fetch('/Home/MostrarNuevosMensajes', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    if (response.ok) {
      const data = await response.json();
      buildNotification(data);
    }

  }

  const getNotification = () => {

    fetchNotification();

    setInterval(fetchNotification, 15000);

  }

  return {
    // Initialization
    init: function () {

      getNotification();
    }
  };
}();

KTUtil.onDOMContentLoaded((function () {
  KTNotificacionesAdmin.init();
}));