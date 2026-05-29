var KTViajesList = function () {
    // Define shared variables
    var table = document.getElementById('kt_table_ViajesCanceladosUsuarios');
    var datatable;
    var originalData = []; // Definir originalData en el nivel superior

    // Private functions
    var initViajesTable = function () {
        // Set date data order
        const tableRows = table.querySelectorAll('tbody tr');
        let peticionEnProceso = false;

        // Store original data
        tableRows.forEach(row => {
            const cells = row.querySelectorAll('td');
            originalData.push([
                cells[0].innerText,
                cells[1].innerText,
                cells[2].innerText,
                cells[3].innerText,
                cells[4].innerText,
                cells[5].innerHTML // assuming the last cell contains HTML for actions
            ]);
        });

        _onRenderDataTable(peticionEnProceso);

        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "order": [],
            'deferRender': true,
            "info": false,
            "pageLength": 10,
            "lengthChange": false,
            'columnDefs': [
                { orderable: false, targets: "_all" } // Disable ordering on all columns                
            ]
        });

        // Re-init functions on every table re-draw -- more info: https://datatables.net/reference/event/draw
        datatable.on('draw', function () {
            _onRenderDataTable(peticionEnProceso);
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-viajes-table-filter="search2"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    const initModalEvent = () => {
        const modalAgregar = document.getElementById('kt_modal_detalle_viaje');
        const inputSearch = document.getElementById('search-estacion');

        showModalEstacion = new bootstrap.Modal(modalAgregar, {
            keyboard: false,
            backdrop: 'static'
        });

        $('[data-kt-viajesusuario-action="edit"]').on("click", function (event) {
            var corridaId = $(this).data('id');
            $.ajax({
                url: "/Comercial/DetalleViajeUsuario",
                type: 'GET',
                data: { CorridaId: corridaId },
                success: function (data) {
                    $('#modal-body').html(data);
                    showModalEstacion.show();
                },
                error: function () {
                    alert('Error occurred while fetching data.');
                }
            });
        });

    }

    return {
        // Public functions  
        init: function () {
            if (!table) {
                return;
            }

            initViajesTable();
            handleSearchDatatable();
            initFechasSeleccionar();
            $('#divFecha2').hide();
            $('#divFecha2').addClass('d-none');
            $("#PeriodoId2").on("change", _onChangePeriodo);
            datatable.on('draw.dt', function () {
                $('#kt_table_ViajesCanceladosUsuarios tbody tr').each(function () {
                    if (datatable.rows().count() > 0) {
                        $(this).find('td:last-child').addClass('text-end');
                    }
                });
            });
        },
        getOriginalData: function () {
            return originalData; // Agregar el método getOriginalData
        }
    }
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTViajesList.init();
});

function _onRenderDataTable(peticionEnProceso) {
    $('[data-kt-viajesusuario-action="edit"]').off("click");
    $('[data-kt-viajesusuario-action="edit"]').on("click", _onEdit(peticionEnProceso));
}

function _onEdit(peticionEnProceso) {
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

function _onChangePeriodo() {
    const divFecha2 = document.getElementById('divFecha2');
    if ($(this).val() != "") {
        var fechaInicio = '';
        var fechaFin = '';
        var fechaActual = new Date();
        switch (parseInt($(this).val())) {
            case 0:
                $('#divFecha2').hide();
                $('#divFecha2').addClass('d-none');
                fechaFin = new Date(fechaActual);
                fechaFin.setHours(0, 0, 0, 0);
                fechaInicio = fechaActual;
                fechaInicio.setHours(23, 59, 59, 999);
                fechaInicio = formatearFecha(fechaInicio);
                fechaFin = formatearFecha(fechaFin);
                filtrarResultadosFecha(fechaInicio, fechaFin);
                break;
            case 1:
                $('#divFecha2').hide();
                $('#divFecha2').addClass('d-none');
                var diasRestar = 7;
                var nuevaFecha = new Date(fechaActual);
                nuevaFecha.setDate(fechaActual.getDate() - diasRestar);
                fechaFin = new Date(nuevaFecha);
                fechaFin.setHours(0, 0, 0, 0);
                fechaInicio = formatearFecha(nuevaFecha);
                fechaFin = formatearFecha(fechaFin);
                filtrarResultadosFecha(fechaInicio, fechaFin);
                break;
            case 2:
                $('#divFecha2').hide();
                $('#divFecha2').addClass('d-none');
                var nuevaFecha = new Date(fechaActual);
                nuevaFecha.setMonth(fechaActual.getMonth() - 1);
                nuevaFecha.setHours(23, 59, 59, 999);
                fechaInicio = formatearFecha(nuevaFecha);
                filtrarResultadosFecha(fechaInicio, fechaFin);
                break;
            case 3:
                $('#divFecha2').hide();
                $('#divFecha2').addClass('d-none');
                var diasRestar = 7;
                var nuevaFecha = new Date(fechaActual);
                nuevaFecha.setDate(fechaActual.getDate() - diasRestar);
                fechaFin = formatearFecha(nuevaFecha);
                fechaInicio = formatearFecha(fechaActual);
                filtrarResultadosFecha(fechaInicio, fechaFin);
                break;
            case 4:
                $('#divFecha2').hide();
                $('#divFecha2').addClass('d-none');
                var nuevaFecha = new Date(fechaActual);
                nuevaFecha.setMonth(fechaActual.getMonth() - 1);
                nuevaFecha.setHours(23, 59, 59, 999);
                fechaInicio = formatearFecha(nuevaFecha);
                fechaFin = formatearFecha(nuevaFecha);
                fechaInicio = formatearFecha(fechaActual);
                filtrarResultadosFecha(fechaInicio, fechaFin);
                break;
            case 5:
                $('#divFecha2').show();
                $('#divFecha2').removeClass('d-none');
                break;
            case 6:
                $('#divFecha2').hide();
                $('#divFecha2').addClass('d-none');
                // Restaurar los datos originales en el DataTable
                var table = $("#kt_table_ViajesCanceladosUsuarios").DataTable();
                table.clear().rows.add(KTViajesList.getOriginalData()).draw();
                break;
        }

    } else {

    }
}

function filtrarResultadosFecha(fechaInicio, fechaFin) {
    var miTabla = $("#kt_table_ViajesCanceladosUsuarios").DataTable();
    miTabla.clear().draw();
    var data = datos;
    var resultadosFiltrados = [];
    for (var i = 0; i < data.length; i++) {
        var fecha = data[i].fechaviaje;
        if (fecha >= fechaFin && fecha <= fechaInicio) {
            resultadosFiltrados.push({
                viajeId: data[i].viajeId,
                viaje: data[i].viaje,
                nombre: data[i].nombre,
                codigo: data[i].codigo,
                pasajeros: data[i].pasajeros,
                fechaviaje: data[i].fechaviaje
            });
        }
    }

    for (var j = 0; j < resultadosFiltrados.length; j++) {
        var botonEditar = '<td class="text-end">' +
            '<a class="btn btn-icon btn-active-light-primary w-30px h-30px" id="kt_viajes_detalles" data-id="' + resultadosFiltrados[j].viajeId + '" data-kt-viajesusuario-action="edit">' +
            '<i class="ki-outline ki-abstract-14 fs-3"></i></a></td> '
        miTabla.row.add([
            resultadosFiltrados[j].viaje,
            resultadosFiltrados[j].nombre,
            resultadosFiltrados[j].codigo,
            resultadosFiltrados[j].pasajeros,
            formatearFechaDT(resultadosFiltrados[j].fechaviaje),
            botonEditar
        ]).draw();
    }
}

function formatearFecha(fecha) {
    var año = fecha.getFullYear();
    var mes = ('0' + (fecha.getMonth() + 1)).slice(-2); // Se suma 1 porque enero es 0
    var dia = ('0' + fecha.getDate()).slice(-2);
    var horas = ('0' + fecha.getHours()).slice(-2);
    var minutos = ('0' + fecha.getMinutes()).slice(-2);
    var segundos = ('0' + fecha.getSeconds()).slice(-2);

    var fechaFormateada = año + '-' + mes + '-' + dia + 'T' + horas + ':' + minutos + ':' + segundos;
    return fechaFormateada;
}

function formatearFechaDT(fechaStr) {
    if (!fechaStr) {
        return '';
    }

    var fecha = new Date(fechaStr);

    // Obtener los componentes de la fecha
    var dia = fecha.getDate().toString().padStart(2, '0');
    var mes = (fecha.getMonth() + 1).toString().padStart(2, '0'); // Los meses son 0-indexados
    var anio = fecha.getFullYear();
    var horas = fecha.getHours().toString().padStart(2, '0');
    var minutos = fecha.getMinutes().toString().padStart(2, '0');

    // Formatear la fecha como "dd/MM/yyyy HH:mm"
    return `${dia}/${mes}/${anio} ${horas}:${minutos}`;
}

function initFechasSeleccionar() {
    $('#txtFecha2').daterangepicker({
        startDate: moment().utcOffset(0),
        endDate: moment().utcOffset(0),
        "locale": {
            "format": "YYYY-MM-DD",
            "firstDay": 1,
            "separator": " - ",
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "daysOfWeek": [
                "Dom",
                "Lun",
                "Mar",
                "Mié",
                "Jue",
                "Vie",
                "Sáb"
            ],
            "monthNames": [
                "Ene",
                "Feb",
                "Mar",
                "Abr",
                "May",
                "Jun",
                "Jul",
                "Ago",
                "Sep",
                "Oct",
                "Nov",
                "Dic"
            ]
        }
    });
    $('#txtFecha2').on('apply.daterangepicker', function (ev, picker) {
        var fechaFin = picker.startDate.startOf('day').format('YYYY-MM-DD[T]HH:mm:ss');
        var fechaInicio = picker.endDate.endOf('day').format('YYYY-MM-DD[T]HH:mm:ss');

        filtrarResultadosFecha(fechaInicio, fechaFin);
    });
}
