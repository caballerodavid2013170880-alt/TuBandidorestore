$(document).ready(function () {
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
        }
        if (actionUrl) {
            loadPartialView(target, actionUrl);
        }
    });

    var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    loadPartialView(firstTabTarget, '/Mantenimiento/OrdenMantenimiento');

    $('.nav-link').click(function (e) {
        e.preventDefault();

        $('.nav-link').removeClass('active');

        $(this).addClass('active');

        var currentAttrValue = $(this).attr('href');

        $('.tab-pane').removeClass('show active');

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