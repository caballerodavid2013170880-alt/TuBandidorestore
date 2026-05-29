$(document).ready(function () {
    // Assuming you have the userId stored in a JavaScript variable
    var clienteId = window.userId || $('[data-user-id]').data('userId');

    // Trigger the load when the tab is clicked
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href"); // activated tab
        var actionUrl = '';
        switch (target) {
            case "#viajes":
                actionUrl = '/Comercial/ViajesCliente'
                break;
            case "#viajesproximos":
                actionUrl = '/Comercial/ViajesProximosCliente'
                break;
            case "#monedero":
                actionUrl = '/Comercial/MonederoCliente'
                break;
            case "#calificacion":
                actionUrl = '/Comercial/CalificacionCliente'
                break;
            case "#membresias":
                actionUrl = '/Comercial/MembresiaCliente'
                break;
            // Add cases for other tabs as needed
        }
        if (actionUrl) {
            loadPartialView(target, actionUrl, clienteId);
        }
    });

    // Load the first tab content on page load
    var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    loadPartialView(firstTabTarget, '/Comercial/ViajesCliente', clienteId);

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

function loadPartialView(targetDiv, url, clienteId) {
    $.ajax({
        url: url,
        type: 'GET',
        data: { clienteId: clienteId },
        success: function (result) {
            $(targetDiv).html(result);
            $(targetDiv).find('.date-cell').each(function () {
                var dateString = $(this).text().trim()
                    .replace('p. m.', 'PM')
                    .replace('a. m.', 'AM');

                // Parse the date assuming the format 'DD/MM/YYYY HH:MM:SS AM/PM'
                var parts = dateString.split(/[\s/:]/);
                var day = parseInt(parts[0], 10);
                var month = parseInt(parts[1], 10) - 1; // JavaScript months are 0-based
                var year = parseInt(parts[2], 10);
                var hour = parseInt(parts[3], 10) + (parts[6] === 'PM' && parts[3] !== '12' ? 12 : 0);
                var minute = parseInt(parts[4], 10);
                var second = parseInt(parts[5], 10);

                var date = new Date(year, month, day, hour, minute, second);
                var formattedDate = ('0' + day).slice(-2) + '/'
                    + ('0' + (month + 1)).slice(-2) + '/'
                    + year;

                $(this).text(formattedDate);
            });
            $(targetDiv).find('.date-cell2').each(function () {

                var FechaVigencia = $(this).text().trim();

                var partes = FechaVigencia.split(' - ');
                const fechaInicio = new Date(partes[0]
                    .replace('p. m.', 'PM')
                    .replace('a. m.', 'AM'));
                const fechaFin = new Date(partes[1]
                    .replace('p. m.', 'PM')
                    .replace('a. m.', 'AM'));

                var dateStringInicio = fechaInicio.toLocaleDateString();

                var dateStringFin = fechaFin.toLocaleDateString();

                $(this).text(dateStringInicio + ' - ' + dateStringFin);
            });
            inicializarDataTable();
        },
        error: function () {
            console.error("Error loading partial view.");
        }
    });

    function inicializarDataTable() {
        if (!$.fn.DataTable.isDataTable('#kt_table_viajes_usuario')) {
            $('#kt_table_viajes_usuario').DataTable({
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
        if (!$.fn.DataTable.isDataTable('#kt_table_viajes_proximos_usuario')) {
            $('#kt_table_viajes_proximos_usuario').DataTable({
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
        if (!$.fn.DataTable.isDataTable('#kt_table_monedero_usuario')) {
            $('#kt_table_monedero_usuario').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "El usuario no cuenta con registros",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
        if (!$.fn.DataTable.isDataTable('#kt_table_calificacion_usuario')) {
            $('#kt_table_calificacion_usuario').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "El usuario no cuenta con calificaciones",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
        if (!$.fn.DataTable.isDataTable('#kt_table_membresia_usuario')) {
            $('#kt_table_membresia_usuario').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "El usuario no cuenta con registros de membresías",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
    }
}