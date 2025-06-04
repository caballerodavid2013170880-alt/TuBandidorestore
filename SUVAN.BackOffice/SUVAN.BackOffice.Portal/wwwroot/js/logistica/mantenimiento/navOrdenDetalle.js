$(document).ready(function () {
    // Assuming you have the userId stored in a JavaScript variable
    //var clienteId = window.userId || $('[data-user-id]').data('userId');

    // Trigger the load when the tab is clicked
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href"); // activated tab
        var actionUrl = '';
        switch (target) {
            case "#ordendetalle":
                actionUrl = '/Mantenimiento/OrdenMantenimiento'
                break;
            case "#datosvehiculo":
                actionUrl = '/Mantenimiento/DatosVehiculo'
                break;
            // Add cases for other tabs as needed
        }
        if (actionUrl) {
            loadPartialView(target, actionUrl);
        }
    });

    // Load the first tab content on page load
    //var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    //loadPartialView(firstTabTarget, '/Mantenimiento/OrdenMantenimiento', clienteId);

    var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    loadPartialView(firstTabTarget, '/Mantenimiento/OrdenMantenimiento');

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

function loadPartialView(targetDiv, url) {

    $.ajax({
        url: url,
        type: 'GET',
        success: function (result) {
            $(targetDiv).html(result);
  
            inicializarDataTable();
        },
        error: function () {
            console.error("Error loading partial view.");
        }
    });
    function inicializarDataTable() {
        if (!$.fn.DataTable.isDataTable('#kt_table_detalle_orden')) {
            $('#kt_table_detalle_orden').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay niguna orden de mantenimiento registrada",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }

        if (!$.fn.DataTable.isDataTable('#kt_table_datos_vehiculo')) {
            $('#kt_table_datos_vehiculo').DataTable({
                // Tus opciones de configuración aquí
                "order": [],
                "pageLength": 10,
                "info": false,
                "lengthChange": false,
                "language": {
                    "emptyTable": "No hay datos del Vehículo registrados",
                },
                "columnDefs": [
                    { "orderable": false, "targets": "_all" }
                ]
            });
        }
    }
}