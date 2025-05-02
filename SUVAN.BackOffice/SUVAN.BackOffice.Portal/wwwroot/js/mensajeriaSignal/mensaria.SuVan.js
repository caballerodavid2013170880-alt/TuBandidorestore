/**
 * Autor:Hedilberto Cruz Vasquez
 * Fecha: 16 de Enero del 2024
 * Descripcion: Envio de mensajes mediante Signal R para comentarios en Tiempo real
 */
let fnMensajeriaSignalR = new function () {
    "use strict";
    const ENUMTYPECHAT = {
        GLOBAL: 0,
        ROUTE: 1,
        INDIDUAL: 2
    };
    const URLCONCENTRADORHUB = apiURL + "Hub/Mensajeria";
    const ACCESSTOKENFACTORY = accessTokenFactory; //Lllave de acceso
    var self = this,
        conectionHub = null,
        settingsConversationDefault = {
            conversacionId: null,
            tipoConversacion: ENUMTYPECHAT.GLOBAL,
            rutaId: null,
            operadorId: null,
            conexionId: null,
            mensaje: ""
        },
        conversation = null,
        $elementDrawer = $("#app_suvan_drawer"),
        $btnSendMessage = $("#btnSendMessage"),
        $btnCerrarConversacion = $("#btnCerrarConversacion"),
        drawerinstance = {},
        divLoading = null,
        templateNeConversation = document.querySelector('[data-kt-element="template-newConversation"]'),
        elementContainerMessage = document.querySelector('#app_suvan_drawer [data-kt-element="messages"]'),
        templareCardTitle = document.querySelector('#app_suvan_drawer #app_suvan_drawer_messenger_header'),
        templateMessageOfIn = document.querySelector('#app_suvan_drawer [data-kt-element="template-in"]'),
        templateMessageOfOut = document.querySelector('#app_suvan_drawer [data-kt-element="template-out"]'),
        inputMessage = document.querySelector('#app_suvan_drawer_messenger_footer [data-kt-element="input"]');

    function _open() {
        _getConversation();
    }
    function _init() {
        drawerinstance = KTDrawer.getInstance(document.querySelector("#app_suvan_drawer"));
        drawerinstance.on("kt.drawer.hide", _onCloseConversation);
        $btnSendMessage.on("click", _sendMessage);
        $btnCerrarConversacion.on("click", _onClickCloseConversation);
        $("#kt_drawer_chat_close").on("click", function () {
            KTDrawer.getInstance(document.querySelector("#app_suvan_drawer")).hide();
            $btnCerrarConversacion.addClass("d-none");
        });
        $("#kt_drawer_inbox_close").on("click", function () {
            KTDrawer.getInstance(document.querySelector("#app_suvan_drawer_inbox")).hide();

        });
    }
    function _initConectionHub(dataConversation, callBackThen) {
        callBackThen = callBackThen ? callBackThen : function () { };
        conectionHub = new signalR.HubConnectionBuilder().withUrl(URLCONCENTRADORHUB, { accessTokenFactory: () => ACCESSTOKENFACTORY }).build();
        conectionHub.on("ReceiveMessage", _renderReceiveMessage);
        conectionHub.on("ConversationCreated", _renderConversationCreated);
        conectionHub.on("MessageSentToOperator", _renderSentMesssageToOperator);
        conectionHub.on("RenderIbox", _renderIbox);
        conectionHub.start().then(function (dataResponse) {
            templareCardTitle.querySelector("a").innerText = dataConversation.nombreConversacion;
            templareCardTitle.querySelector('[data-kt-element="status"]').innerText = "Online";
            // Se actualiza el connectionId de la conversacion con el nuevo que se genero en el Hub
            conectionHub.invoke("updateConexionId", dataConversation.conversacionId, conectionHub.connection.connectionId)
                .catch(function (err) {
                    return console.error(err.toString());
                });
            callBackThen();
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
    function _getConversation() {
        _ajaxServiceGetConversation(conversation, function (dataConversation) {
            $btnCerrarConversacion.removeClass("d-none");
            conversation = dataConversation;
            _hideLoading();
            if (dataConversation.conversacionId != null) {
                Swal.fire({
                    title: "Conversacion  existente",
                    text: "Existe una conversacion en curso¿Desea generar una nueva?,",
                    icon: "info",
                    showCancelButton: true,
                    buttonsStyling: false,
                    confirmButtonText: "Si",
                    cancelButtonText: "No",
                    customClass: {
                        confirmButton: "btn fw-bold btn-success",
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    },
                    preConfirm: async () => {
                        const response = await fetch('/Configuracion/CerrarConversacion?conversacionId=' + dataConversation.conversacionId + "&estatus=0", {
                            method: 'PUT',
                            headers: {
                                'Content-Type': 'application/json'
                            }
                        }).then((dataResponse) => {
                            if (dataResponse.ok) {
                                Swal.fire({
                                    title: "Información",
                                    text: "La conversacion previamente abierta fue cerrada con exito",
                                    icon: "success"
                                });
                                conversation.conversacionId = undefined;
                                conversation.mensajes = [];
                                $btnCerrarConversacion.addClass("d-none");
                            } else {
                                Swal.fire({
                                    title: "Información",
                                    text: "Se genero un error en el cambio de estatus",
                                    icon: "error"
                                });
                                KTDrawer.getInstance(document.querySelector("#app_suvan_drawer")).hide();
                            }
                        })

                    }
                }).then((result) => {
                    _initConectionHub(conversation, result.isConfirmed ? _loadIboxAfterCloseConversation : undefined);
                    _drawMessages(conversation);
                    drawerinstance.show();
                });
            } else {
                $btnCerrarConversacion.addClass("d-none");
                _initConectionHub(conversation);
                _drawMessages(conversation);
                drawerinstance.show();
            }

        });
    }
    function _onCloseConversation(evt) {
        if (conectionHub != null) {
            conectionHub.connection.stop().then(function () {
                conversation = null;
                $btnCerrarConversacion.addClass("d-none");
            }).catch(function (err) {
                return console.error(err.toString());
            });
        }

    }
    function _sendMessage() {
        if (inputMessage.value != "" && inputMessage.value.trim().length > 0) {
            let elementMessageOuput = templateMessageOfOut.cloneNode(true);
            elementMessageOuput.classList.remove("d-none")
            elementMessageOuput.classList.add("mensajeEnviandose");
            elementMessageOuput.querySelector("a.nombreEstatus").innerText = "Enviando..";
            elementMessageOuput.querySelector("span.fechaEnvio").remove();
            elementMessageOuput.querySelector('[data-kt-element="message-text"]').innerText = inputMessage.value;
            elementContainerMessage.appendChild(elementMessageOuput)
            elementContainerMessage.scrollTop = elementContainerMessage.scrollHeight;
            conversation.mensaje = inputMessage.value;
            conversation.conexionId = conectionHub.connection.connectionId;
            console.log(conversation.conexionId);
            conectionHub.invoke("SendMessageOperator", {
                conversacionId: conversation.conversacionId,
                tipoConversacion: conversation.tipoConversacion,
                nombreConversacion: conversation.nombreConversacion,
                empresaId: conversation.empresaId,
                rutaId: conversation.rutaId,
                operadorId: conversation.operadorId,
                usuarioIdCreacion: conversation.usuarioIdCreacion,
                conexionId: conversation.conexionId,
                mensaje: conversation.mensaje
            }).catch(function (err) {
                return console.error(err.toString());
            });
            inputMessage.value = "";
        }

    }
    function _loadIboxAfterCloseConversation() {
        conectionHub.invoke("GetListOfConversations", conectionHub.connection.connectionId, conversation.usuarioIdCreacion).catch(function (err) {
            return console.error(err.toString());
        });
    }
    function _renderSentMesssageToOperator(dataResult) {
        console.log(dataResult);
        document.querySelector('.mensajeEnviandose').remove();
        let elementMessageOuput = templateMessageOfOut.cloneNode(true);
        elementMessageOuput.classList.remove("d-none")
        elementMessageOuput.querySelector("a.nombreEstatus").remove();
        elementMessageOuput.querySelector("span.fechaEnvio").innerText = new Date().toLocaleTimeString('en-US', { hour12: false, hour: "2-digit", minute: "2-digit" });
        elementMessageOuput.querySelector('[data-kt-element="message-text"]').innerText = dataResult.mensaje;
        elementContainerMessage.appendChild(elementMessageOuput)
        elementContainerMessage.scrollTop = elementContainerMessage.scrollHeight;
    }
    function _renderConversationCreated(dataResponse) {
        conversation.conversacionId = dataResponse.conversacionId;
        $btnCerrarConversacion.removeClass("d-none");
    }
    function _renderIbox(dataResponse) {
        let divContainer = document.querySelector('#app_suvan_drawer_inbox_messenger_body [data-kt-element="messages"]');
        $(divContainer).empty();
        $.each(dataResponse, function (index, dataConversacion) {
            let elementNewConversation = templateNeConversation.cloneNode(true);
            elementNewConversation.classList.remove("d-none");

            let templateClassEstilo = "symbol-label  bg-light-{{ESTILO}} text-{{ESTILO}} fs-6 fw-bolder";
            let elementEstilo = elementNewConversation.querySelector("span.spanEstilo");
            let elementLink = elementNewConversation.querySelector("a.inbox-conversacion");
            $(elementEstilo).addClass(templateClassEstilo.replaceAll('{{ESTILO}}', dataConversacion.estilo));
            $(elementEstilo).text(dataConversacion.abreviatura)

            elementLink.innerText = dataConversacion.nombreConversacion;
            $(elementLink).data("conversacion", dataConversacion.conversacionId);
            elementNewConversation.querySelector("div.nombreTipo").innerText = dataConversacion.nombreTipo;
            elementNewConversation.querySelector("span.diferenciaTiempo").innerText = dataConversacion.diferenciaTiempo;
            elementNewConversation.querySelector("span.totalMensajes").innerText = dataConversacion.totalMensajes;
            divContainer.appendChild(elementNewConversation);
        });
        $("a.inbox-conversacion").on("click", _onClickGetConversation);
    }
    function _renderReceiveMessage(dataMessage) {
        let mensajeActual = new Date(dataMessage.fechaCreacion).toLocaleDateString('en-US') == new Date().toLocaleDateString('en-US');
        let fechaHoraEnvio = new Date(dataMessage.fechaCreacion).toLocaleTimeString('en-US', { hour12: false, hour: "2-digit", minute: "2-digit" });

        let elementMessageInput = templateMessageOfIn.cloneNode(true);
        elementMessageInput.classList.remove("d-none")
        elementMessageInput.querySelector('[data-kt-element="message-text"]').innerText = dataMessage.comentario;
        elementMessageInput.querySelector("a.nombreEstatus").remove();
        elementMessageInput.querySelector("span.fechaEnvio").innerText = mensajeActual ? fechaHoraEnvio : new Date(dataMessage.fechaCreacion).toLocaleString();
        elementContainerMessage.appendChild(elementMessageInput);
        elementContainerMessage.scrollTop = elementContainerMessage.scrollHeight;
    }
    function _loadConversation(id) {
        _ajaxServiceGetConversation({
            conversacionId: id
        }, function (dataConversation) {
            conversation = dataConversation;
            _initConectionHub(conversation);
            _drawMessages(conversation);
            _hideLoading();
            $btnCerrarConversacion.removeClass("d-none");
            drawerinstance.show();
        });
    }
    function _drawMessages(dataConversation) {
        $(elementContainerMessage).empty();
        if (dataConversation && dataConversation.mensajes && dataConversation.mensajes.length > 0) {
            $.each(dataConversation.mensajes, function (index, elemento) {
                let mensajeActual = new Date(elemento.fechaCreacion).toLocaleDateString('en-US') == new Date().toLocaleDateString('en-US');
                let fechaHoraEnvio = new Date(elemento.fechaCreacion).toLocaleTimeString('en-US', { hour12: false, hour: "2-digit", minute: "2-digit" });

                if (elemento.usuarioId == dataConversation.usuarioIdCreacion) {
                    let elementMessageOuput = templateMessageOfOut.cloneNode(true);
                    elementMessageOuput.classList.remove("d-none")
                    elementMessageOuput.querySelector('[data-kt-element="message-text"]').innerText = elemento.comentario;
                    elementMessageOuput.querySelector("a.nombreEstatus").remove();
                    elementMessageOuput.querySelector("span.fechaEnvio").innerText = mensajeActual ? fechaHoraEnvio : new Date(elemento.fechaCreacion).toLocaleString();

                    elementContainerMessage.appendChild(elementMessageOuput)
                } else {
                    let elementMessageInput = templateMessageOfIn.cloneNode(true);
                    elementMessageInput.classList.remove("d-none")
                    elementMessageInput.querySelector('[data-kt-element="message-text"]').innerText = elemento.comentario;
                    elementMessageInput.querySelector("a.nombreEstatus").remove();
                    elementMessageInput.querySelector("span.fechaEnvio").innerText = mensajeActual ? fechaHoraEnvio : new Date(elemento.fechaCreacion).toLocaleString();
                    elementContainerMessage.appendChild(elementMessageInput)
                }
            });
        }
        elementContainerMessage.scrollTop = elementContainerMessage.scrollHeight;
        inputMessage.value = "";
    }
    function _onClickGetConversation(evt) {
        KTDrawer.getInstance(document.querySelector("#app_suvan_drawer")).hide();
        _loadConversation($(this).data("conversacion"))
    }
    function _onClickCloseConversation(evt) {
        _ajaxServiceCerrarConversacion(conversation.conversacionId, function (dataResponse) {
            if (dataResponse.data == 1) {
                _loadIboxAfterCloseConversation();
                setTimeout(function () {
                    _hideLoading();
                    Swal.fire({
                        title: "Información",
                        text: "La conversacion previamente abierta fue cerrada con exito",
                        icon: "success"
                    });
                    KTDrawer.getInstance(document.querySelector("#app_suvan_drawer")).hide();
                },1000)
            } else {
                _hideLoading();
                Swal.fire({
                    title: "Información",
                    text: "Se genero un error en el cambio de estatus",
                    icon: "error"
                });
            }
        });
    }
    function _showLoading() {
        divLoading = document.createElement("div");
        document.body.append(divLoading);
        divLoading.classList.add("page-loader");
        divLoading.innerHTML = `<span class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span</span>`;
        KTApp.showPageLoading();
    }
    function _hideLoading() {
        KTApp.hidePageLoading();
        divLoading.remove();
    }
    function _ajaxServiceCerrarConversacion(idConversacion, _callBackOk) {
        $.ajax({
            url: '../Configuracion/CerrarConversacion?conversacionId=' + idConversacion + "&estatus=0",
            type: "PUT",
            dataType: 'json',
            beforeSend: function () {
                _showLoading();
            },
            success: _callBackOk
        });
    }
    function _ajaxServiceGetConversation(_data, _callBackOk) {
        $.ajax({
            url: "../Configuracion/ObtenerInformacionChat",
            type: "POST",
            data: _data,
            dataType: 'json',
            beforeSend: function () {
                _showLoading();
            },
            success: _callBackOk
        });
    }

    self.open = function (dataConversation) {
        conversation = $.extend(false, settingsConversationDefault, dataConversation);
        _open();
    }
    self.init = function () {
        _init();
    }
    self.initConversations = function () {
        $("a.inbox-conversacion").on("click", _onClickGetConversation);
    }
};
KTUtil.onDOMContentLoaded((function () {
    fnMensajeriaSignalR.init();
    fnMensajeriaSignalR.initConversations();
}));
