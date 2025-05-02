$(document).ready(function () {
    // Assuming you have the userId stored in a JavaScript variable
    var operadorId = window.userId || $('[data-user-id]').data('userId');

    // Trigger the load when the tab is clicked
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href"); // activated tab
        var actionUrl2 = '';
        var actionUrl = '/Comercial/Viajess';
        switch (target) {
            case "#viajes":
                actionUrl = '/Comercial/Viajess'
                volverVistaPrincipal();
                break;
            case "#viajesproximos":
                actionUrl = '/Comercial/ViajesProximos'
                cambiarTextoOpciones();
                break;
            case "#viajescancelados":
                actionUrl = '/Comercial/ViajesCancelados'
                volverVistaPrincipal();  
                break;
            case "#viajesusuario":
                actionUrl2 = '/Comercial/ViajesUsuario'
                volverVistaPrincipal2();
                break;
            case "#viajesproximosusuario":
                actionUrl2 = '/Comercial/ViajesProximosUsuario'
                cambiarTextoOpciones2();
                break;
            case "#viajescanceladosusuario":
                actionUrl2 = '/Comercial/ViajesCanceladosUsuario'
                volverVistaPrincipal2();
                break;
            // Add cases for other tabs as needed
        }
        if (actionUrl2) {
            loadPartialView2(target, actionUrl2, operadorId);
        } else if (actionUrl) {
            loadPartialView(target, actionUrl, operadorId);
        }
    });

    // Load the first tab content on page load
    var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    loadPartialView(firstTabTarget, '/Comercial/Viajess', operadorId);
    //var firstTabTarget2 = $('a[data-bs-toggle="tab"]').first().attr("href");
    //loadPartialView2(firstTabTarget2, '/Comercial/ViajesUsuario', operadorId);

    $('.nav-link').click(function (e) {
        e.preventDefault(); // Prevent default anchor click behavior

        // Remove 'active' from all tabs
        $('.nav-link').removeClass('active');
        // Add 'active' to the clicked tab
        $(this).addClass('active');

        // Get the target tab's selector
        var currentAttrValue = $(this).attr('href');

        // Remove 'show active' from all tab contents
        $('.tab-pane').removeClass('show active');
        // Add 'show active' to the corresponding tab content
        $(currentAttrValue).addClass('show active');
    })

});

function loadPartialView(targetDiv, url, operadorId) {
    $.ajax({
        url: url,
        type: 'GET',
        data: { operadorId: operadorId },
        success: function (result) {
            $(targetDiv).html(result);
            inicializarDataTable();
            document.getElementById('PeriodoId').selectedIndex = 0;
        },
        error: function () {
            console.error("Error loading partial view.");
        }
    });

    function inicializarDataTable() {
        if (!$.fn.DataTable.isDataTable('#kt_table_Viajes')) {
            let peticionEnProceso = false;
            _onRenderDataTable(peticionEnProceso);
            $('#kt_table_Viajes').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay viajes registrados",
                },
            });
        }
        if (!$.fn.DataTable.isDataTable('#kt_table_ViajesProximos')) {
            let peticionEnProceso = false;
            _onRenderDataTable(peticionEnProceso);
            $('#kt_table_ViajesProximos').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay viajes próximos registrados",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
        if (!$.fn.DataTable.isDataTable('#kt_table_ViajesCancelados')) {
            let peticionEnProceso = false;
            _onRenderDataTable(peticionEnProceso);
            $('#kt_table_ViajesCancelados').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay Viajes cancelados.",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
    }

    function _onRenderDataTable(peticionEnProceso) {
        $('[data-kt-viajes-action="edit"').off("click");
        $('[data-kt-viajes-action="edit"]').on("click", _onEdit(peticionEnProceso))
    }
    function _onEdit(peticionEnProceso) {
        const modalAgregar = document.getElementById('kt_modal_detalle_viaje');
        const inputSearch = document.getElementById('search-estacion');


        showModalEstacion = new bootstrap.Modal(modalAgregar, {
            keyboard: false,
            backdrop: 'static'
        });

        $('[data-kt-viajes-action="edit"]').on("click", function () {
            if (!peticionEnProceso) {
                peticionEnProceso = true;
                $(this).prop('disabled', true);
                var corridaId = $(this).data('id');
                $.ajax({
                    url: "/Comercial/DetalleViaje",
                    type: 'GET',
                    data: { CorridaId: corridaId },
                    success: function (data) {
                        $('#modal-body').html(data);
                        showModalEstacion.show();
                        $(this).prop('disabled', false);
                        peticionEnProceso = false;
                    },
                    error: function () {
                        alert('Error occurred while fetching data.');
                    }
                });
            }
        });
    }

}
function cambiarTextoOpciones() {
    var selectElement = document.getElementById('PeriodoId');
    var options = selectElement.options;

    var nuevosTextos = [
        'Seleccione..',
        'Hoy',
        'La semana siguiente',
        'El mes siguiente',
        'Los siguientes 7 días',
        'Los siguientes 30 días',
        'Personalizado'
    ];

    for (var i = 0; i < options.length; i++) {
        // Cambiar el texto de cada opción
        options[i].text = nuevosTextos[i];
    }
}
function volverVistaPrincipal() {
    var selectElement = document.getElementById('PeriodoId');
    var opcionesOriginales = [
        'Seleccione..',
        'Hoy',
        'La semana anterior',
        'El mes anterior',
        'Los últimos 7 días',
        'Los últimos 30 días',
        'Personalizado'
    ];

    var options = selectElement.options;
    for (var i = 0; i < options.length; i++) {
        options[i].text = opcionesOriginales[i];
    }
}
function cambiarTextoOpciones2() {
    var selectElement = document.getElementById('PeriodoId2');
    var options = selectElement.options;

    var nuevosTextos = [
        'Seleccione..',
        'Hoy',
        'La semana siguiente',
        'El mes siguiente',
        'Los siguientes 7 días',
        'Los siguientes 30 días',
        'Personalizado'
    ];

    for (var i = 0; i < options.length; i++) {
        // Cambiar el texto de cada opción
        options[i].text = nuevosTextos[i];
    }
}
function volverVistaPrincipal2() {
    var selectElement = document.getElementById('PeriodoId2');
    var opcionesOriginales = [
        'Seleccione..',
        'Hoy',
        'La semana anterior',
        'El mes anterior',
        'Los últimos 7 días',
        'Los últimos 30 días',
        'Personalizado'
    ];

    var options = selectElement.options;
    for (var i = 0; i < options.length; i++) {
        options[i].text = opcionesOriginales[i];
    }
}
function loadPartialView2(targetDiv, url, operadorId) {
    $.ajax({
        url: url,
        type: 'GET',
        data: { operadorId: operadorId },
        success: function (result) {
            $(targetDiv).html(result);;
            inicializarDataTable2();
            document.getElementById('PeriodoId2').selectedIndex = 0;
        },
        error: function () {
            console.error("Error loading partial view.");
        }
    });

    function inicializarDataTable2() {
        if (!$.fn.DataTable.isDataTable('#kt_table_ViajesUsuarios')) {
            let peticionEnProceso = false;
            _onRenderDataTable2(peticionEnProceso);
            $('#kt_table_ViajesUsuarios').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay viajes registrados",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
        if (!$.fn.DataTable.isDataTable('#kt_table_ViajesProximosUsuarios')) {
            let peticionEnProceso = false;
            _onRenderDataTable2(peticionEnProceso);
            $('#kt_table_ViajesProximosUsuarios').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay viajes próximos registrados",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
        if (!$.fn.DataTable.isDataTable('#kt_table_ViajesCanceladosUsuarios')) {
            let peticionEnProceso = false;
            _onRenderDataTable2(peticionEnProceso);
            $('#kt_table_ViajesCanceladosUsuarios').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay Viajes cancelados.",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
    }

    function _onRenderDataTable2(peticionEnProceso) {
        $('[data-kt-viajesusuario-action="edit"').off("click");
        $('[data-kt-viajesusuario-action="edit"]').on("click", _onEdit2(peticionEnProceso))
    }
    function _onEdit2(peticionEnProceso) {
        const modalAgregar = document.getElementById('kt_modal_detalle_viaje');
        const inputSearch = document.getElementById('search-estacion');


        showModalEstacion = new bootstrap.Modal(modalAgregar, {
            keyboard: false,
            backdrop: 'static'
        });

        $('[data-kt-viajesusuario-action="edit"]').on("click", function () {
            if (!peticionEnProceso) {
                peticionEnProceso = true;
                $(this).prop('disabled', true);
                var corridaId = $(this).data('id');
                $.ajax({
                    url: "/Comercial/DetalleViajeUsuario",
                    type: 'GET',
                    data: { CorridaId: corridaId },
                    success: function (data) {
                        $('#modal-body').html(data);
                        showModalEstacion.show();
                        $(this).prop('disabled', false);
                        peticionEnProceso = false;
                    },
                    error: function () {
                        alert('Error occurred while fetching data.');
                    }
                });
            }
        });
    }

}