var KTDetalle = function () {

    const initModalEvent = () => {
        const modalDetalle = document.getElementById('kt_modal_detalle');

        if (!modalDetalle) return;

        const showModalDetalle = new bootstrap.Modal(modalDetalle, {
            keyboard: false,
            backdrop: 'static'
        });

        const iconoTipoVehiculo = document.getElementById('iconoTipoVehiculo');
        const inputTipoVehiculo = document.getElementById('IdTipoVehiculo');
        const inputTipoVehiculoVisible = document.getElementById('IdTipoVehiculoVisible');

        const iconoMarca = document.getElementById('iconoMarca');
        const inputMarca = document.getElementById('IdMarca');
        const inputMarcaVisible = document.getElementById('IdMarcaVisible');

        const tabla = $('#kt_tabla_detalle');

        const configurarTabla = (data, columnas, onRowClick) => {
            if ($.fn.DataTable.isDataTable(tabla)) {
                tabla.DataTable().destroy();
                tabla.empty();
            }

            tabla.DataTable({
                data: data,
                columns: columnas,
                pageLength: 10,
                lengthChange: false,
                searching: false,
                ordering: false,
                info: false,
                createdRow: function (row, data) {
                    $(row).addClass('text-gray-600 fw-semibold');
                },
                headerCallback: function (thead) {
                    $(thead).find('th').addClass('text-center text-muted fw-bold fs-7 text-uppercase gs-0');
                }
            });

            tabla.find('tbody').off('click').on('click', 'tr', function () {
                const rowData = tabla.DataTable().row(this).data();
                onRowClick(rowData);
                showModalDetalle.hide();
            });
        };

        if (iconoTipoVehiculo) {
            iconoTipoVehiculo.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Tipos de Vehículo';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerTipoVehiculo",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Tipo de Vehículo", data: "tipoUnidadId", className: 'min-w-125px text-justify' },
                            { title: "Nombre", data: "nombre", className: 'min-w-125px text-justify' }
                        ], function (rowData) {
                            inputTipoVehiculo.value = rowData.tipoUnidadId;
                            inputTipoVehiculoVisible.value = rowData.nombre;
                        });
                    },
                    error: function () {
                        alert('Error al obtener los Tipos de Vehículos.');
                    }
                });
            });
        }

        if (iconoMarca) {
            iconoMarca.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Marcas';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerMarca",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Marca", data: "idMarca", className: 'min-w-125px text-justify' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-justify' }
                        ], function (rowData) {
                            inputMarca.value = rowData.idMarca;
                            inputMarcaVisible.value = rowData.descripcion;
                        });
                    },
                    error: function () {
                        alert('Error al obtener las Marcas.');
                    }
                });
            });
        }
    }

    return {
        init: function () {
            initModalEvent();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalle.init();
});
