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

    $('.pestana').click(function (e) {
        e.preventDefault();

        $('.pestana').removeClass('active');

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

function irATabAnterior() {
    const tabs = Array.from(document.querySelectorAll('a[data-bs-toggle="tab"]'));
    const currentTab = tabs.find(tab => tab.classList.contains('active'));

    const currentIndex = tabs.indexOf(currentTab);
    if (currentIndex > 0) {
        const anteriorTab = tabs[currentIndex - 1];
        anteriorTab.classList.remove('disabled'); 
        new bootstrap.Tab(anteriorTab).show();
    }
}


function irASiguienteTab(tabId) {
    const siguienteTab = document.querySelector(`a[data-bs-toggle="tab"][href="#${tabId}"]`);

    if (siguienteTab) {
        siguienteTab.classList.remove('disabled');

        new bootstrap.Tab(siguienteTab).show();
    }
}

function toggleNombreTarifaVehicular() {
    const valor = $('input[name="TarifaVehicular"]:checked').val();
    const $inputNombreTarifa = $('#NombreTarifaVehicular');

    if (valor === '1') {
        $inputNombreTarifa.prop('disabled', false);
    } else {
        $inputNombreTarifa.prop('disabled', true).val('');
    }
}

function togglePermisoCargaAceite() {
    const valor = $('input[name="PermisoCargaAceite"]:checked').val();
    const $inputVigenciaPermisoAceite = $('#VigenciaPermisoAceiteVisible');

    if (valor === '1') {
        $inputVigenciaPermisoAceite.prop('disabled', false);
    } else {
        $inputVigenciaPermisoAceite.prop('disabled', true).val('');
    }
}

toggleNombreTarifaVehicular();
togglePermisoCargaAceite();

$('input[name="TarifaVehicular"]').on('change', toggleNombreTarifaVehicular);
$('input[name="PermisoCargaAceite"]').on('change', togglePermisoCargaAceite);
