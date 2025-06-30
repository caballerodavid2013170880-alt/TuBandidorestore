const KTDetalle = function () {
    let dt = null;

    const columnas = [
        { title: "Vehículo", data: function (row) { return row.idVehiculo + " - " + row.placasVehiculo; }, className: "col-general text-center" },
        { title: "Marca", data: "descripcionMarca", className: "col-general text-center" },
        { title: "Modelo", data: "descripcionModelo", className: "col-general text-center" },
        { title: "Tipo de Vehículo", data: "nombreTipoV", className: "col-general text-center" },
        { title: "VIN", data: "carroceria", className: "col-general text-center" },
        { title: "Zona", data: "nombreZona", className: "col-ubicacion text-center" },
        { title: "Depósito", data: "nombreDeposito", className: "col-ubicacion text-center" },
        { title: "Copia de la Factura", data: function (row) { return row.copiaFactura && row.copiaFactura != 0 ? "Sí" : "No"; }, className: "col-ubicacion text-center" },
        { title: "Copia de la Verificacíon", data: function (row) { return row.copiaVerificacion && row.copiaVerificacion != 0 ? "Sí" : "No"; }, className: "col-ubicacion text-center" },
        { title: "Proveedor", data: "proveedor", className: "col-compracosto text-center" },
        { title: "Fecha de Compra", data: function (row) {
                if (!row.fechaCompra) return "";
                const f = new Date(row.fechaCompra);
                const dia = String(f.getDate()).padStart(2, '0');
                const mes = String(f.getMonth() + 1).padStart(2, '0');
                const anio = f.getFullYear();
                return `${dia}/${mes}/${anio}`;
            }, className: "col-compracosto text-center"
        },
        { title: "Costo del Vehículo", data: "costoVehiculo", className: "col-compracosto text-center" },
        { title: "Baja", data: "descripcionBaja", className: "col-garantia text-center" },
        { title: "Causa Baja", data: "descripcionCausaBaja", className: "col-garantia text-center" },
        { title: "Meses de Garantía", data: "mesesGarantia", className: "col-garantia text-center" },
        { title: "Fecha de Baja", data: function (row) {
                if (!row.fechaBaja) return "";
                const f = new Date(row.fechaBaja);
                const dia = String(f.getDate()).padStart(2, '0');
                const mes = String(f.getMonth() + 1).padStart(2, '0');
                const anio = f.getFullYear();
                return `${dia}/${mes}/${anio}`;
            },
            className: "col-garantia text-center"
        },
        { title: "Peso Mínimo", data: "pesoMinimo", className: "col-especificaciones text-center" },
        { title: "Peso Máximo", data: "pesoMaximo", className: "col-especificaciones text-center" },
        { title: "Volumen Mínimo", data: "volumenMinimo", className: "col-especificaciones text-center" },
        { title: "Volumen Máximo", data: "volumenMaximo", className: "col-especificaciones text-center" },
        { title: "Tipo de Licencia Requerida", data: "tipoLicenciaRequerida", className: "col-permisos text-center" },
        { title: "Tarjeta de Circulación", data: "tarjetaCirculacion", className: "col-permisos text-center" },
        {
            title: "Vigencia de Tarjeta de Circulación", data: function (row) {
                if (!row.vigenciaTarjetaCirculacion) return "";
                const f = new Date(row.vigenciaTarjetaCirculacion);
                const dia = String(f.getDate()).padStart(2, '0');
                const mes = String(f.getMonth() + 1).padStart(2, '0');
                const anio = f.getFullYear();
                return `${dia}/${mes}/${anio}`;
            }, className: "col-permisos text-center"
        }
    ];

    const configurarTabla = (data) => {
        if (dt) {
            dt.destroy();
            $('#kt_table_eye').empty();
        }

        dt = $('#kt_table_eye').DataTable({
            data: data,
            columns: columnas,
            paging: false,
            searching: false,
            ordering: false,
            lengthChange: false,
            info: false,
            createdRow: function (row, data, dataIndex) {
                $(row).addClass('text-gray-600 fw-semibold');
            },
            headerCallback: function (thead) {
                $(thead).find('th').addClass('text-center text-muted fw-bold fs-7 text-uppercase gs-0');
            }
        });

        mostrarColumnasPorClase("col-general");
    };

    const mostrarColumnasPorClase = (claseVisible) => {
        dt.columns().every(function (index) {
            const column = dt.column(index);
            const header = $(column.header());
            const classList = header.attr('class') || '';

            if (classList.includes(claseVisible)) {
                column.visible(true);
            } else {
                column.visible(false);
            }
        });
    };

    const initModalEvent = () => {
        const modalElement = document.getElementById('kt_modal_eye');
        const modal = new bootstrap.Modal(modalElement);

        $(document).on('click', '.eyeModal', function () {
            const idVehiculoDetalle = $(this).data('idvehiculodetalle');

            $.get(`/VehiculoDetalle/ObtenerDetalleVehiculo?idVehiculoDetalle=${idVehiculoDetalle}`, function (data) {
                configurarTabla(data);
                $('#vehiculoTabs a[data-tab="general"]').tab('show');
                modal.show();
            });
        });

        $('#vehiculoTabs a[data-bs-toggle="pill"]').on('shown.bs.tab', function (e) {
            const tab = $(e.target).data('tab');
            mostrarColumnasPorClase("col-" + tab);
        });
    };


    return {
        init: function () {
            initModalEvent();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalle.init();
});
