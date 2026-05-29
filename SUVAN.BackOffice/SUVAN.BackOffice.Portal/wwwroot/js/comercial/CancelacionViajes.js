$(document).ready(function () {
  // Delegación de evento 'click' para el botón 'Cancelar Viaje' que se carga dinámicamente
  $(document).on('click', '.btn-cancelar-viaje', function () {
    // Guardar una referencia al botón que fue clickeado
    var botonClickeado = $(this);

    // Muestra el modal de confirmación
    $('#confirmacionCancelarModal').modal('show');

    // Cuando se confirma la cancelación, realiza una acción
    $('#confirmarCancelar').off().on('click', function () {
      var idRuta = $('#idRuta').val();
      var corridaAsignacionId = $('#corridaAsignacionId').val();
      // Realizar la solicitud AJAX al controlador
      $.ajax({
        url: '/Comercial/CancelarViaje', // Asegúrate de reemplazar con la URL correcta
        method: 'POST',
        data: {
          idRuta: idRuta,
          corridaAsignacionId: corridaAsignacionId
        },
        success: function (response) {
          // Aquí manejas una respuesta exitosa del servidor
          // Puedes mostrar un mensaje o actualizar el estado de la página
          console.log(response);
          if (response.success === true) {
            showAlertModal(response.message);
            $('#kt_modal_detalle_viaje').modal('hide');
            reloadPartialViewProximosViajes(idRuta, corridaAsignacionId);
          }
          else {
            showAlertModal(response.message);
          }

        },
        error: function (xhr, status, error) {
          // Aquí manejas errores
          showAlertModal('Ocurrió un error al cancelar el viaje.');
        }
      });

      // Cierra el modal de confirmación
      $('#confirmacionCancelarModal').modal('hide');
    });
  });
});

function reloadPartialViewProximosViajes(idRuta, corridaAsignacionId) {
  let operadorId = window.userId || $('[data-user-id]').data('userId');
  //recarga la vista de corrida
  if (idRuta > 0 && corridaAsignacionId == 0) {
    const target = '#viajesproximos';
    const actionUrl = '/Comercial/ViajesProximos'
    cambiarTextoOpciones();
    loadPartialView(target, actionUrl, operadorId);
  } else {
    //recarga la vista de usuarios
    const target2 = '#viajesproximosusuario';
    const actionUrl2 = '/Comercial/ViajesProximosUsuario'
    cambiarTextoOpciones2();
    loadPartialView2(target2, actionUrl2, operadorId);
  }

}

function showAlertModal(message) {
  // Configurar el mensaje en el modal
  $('#alertModalBody').text(message);

  // Mostrar el modal de alerta
  $('#alertModal').modal('show');
}

