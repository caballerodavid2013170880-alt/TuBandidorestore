$(document).ready(function () {
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href");
        var actionUrl = '';
        switch (target) {
            case "#datosgenerales":
                actionUrl = '/VehiculoDetalle/DatosGenerales'
                break;
            case "#ubicaciondocumentacion":
                actionUrl = '/VehiculoDetalle/UbicacionDocumentacion'
                break;
            case "#compracosto":
                actionUrl = '/VehiculoDetalle/CompraCosto'
                break;
            case "#garantiaestado":
                actionUrl = '/VehiculoDetalle/GarantiaEstado'
                break;
            case "#especificaciones":
                actionUrl = '/VehiculoDetalle/EspecificacionesTecnicas'
                break;
            case "#permisoslicencias":
                actionUrl = '/VehiculoDetalle/PermisosLicencias'
                break;
        }
        if (actionUrl) {
            loadPartialView(target, actionUrl);
        }
    });

    var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    loadPartialView(firstTabTarget, '/VehiculoDetalle/DatosGenerales');

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
    if ($(targetDiv).is(':empty')) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (result) {
                $(targetDiv).html(result);
            },
            error: function () {
                console.error("Error loading partial view.");
            }
        });
    }
}


function irASiguienteTab(tabId) {
    const siguienteTab = document.querySelector(`a[data-bs-toggle="tab"][href="#${tabId}"]`);
    if (siguienteTab) {
        siguienteTab.classList.remove('disabled');

        new bootstrap.Tab(siguienteTab).show();
    }
}