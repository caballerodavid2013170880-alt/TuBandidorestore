$(document).ready(function () {
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href");
        var actionUrl = '';
        switch (target) {
            case "#dimensioncapacidad":
                actionUrl = '/VehiculoEspecificaciones/DimensionCapacidad'
                break;
            case "#motordesempeno":
                actionUrl = '/VehiculoEspecificaciones/MotorDesempeno'
                break;
            case "#transmisiontraccion":
                actionUrl = '/VehiculoEspecificaciones/TransmisionTraccion'
                break;
            case "#datosadicionales":
                actionUrl = '/VehiculoEspecificaciones/DatosAdicionales'
                break;
        }
        if (actionUrl) {
            loadPartialView(target, actionUrl);
        }
    });

    var firstTabTarget = $('a[data-bs-toggle="tab"]').first().attr("href");
    loadPartialView(firstTabTarget, '/VehiculoEspecificaciones/DimensionCapacidad');

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
