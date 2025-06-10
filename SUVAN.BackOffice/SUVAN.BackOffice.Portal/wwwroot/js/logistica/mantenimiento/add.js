var KTMantenimiento = function () {

    const initModalEvent = () => {
        const modalTaller = document.getElementById('kt_modal_mantenimiento_taller');

        if (!modalTaller) return;

        const showModalTaller = new bootstrap.Modal(modalTaller, {
            keyboard: false,
            backdrop: 'static'
        });

        const iconoTaller = document.getElementById('iconoTaller');
        const iconoMecanico = document.getElementById('iconoMecanico');
        const inputTaller = document.getElementById('tallerID');
        const inputTallerHidden = document.getElementById('tallerIDHidden');
        const inputMecanico = document.getElementById('mecanicoID');

        const tabla = $('#kt_tabla_mantenimiento');

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
                showModalTaller.hide();
            });
        };

        if (iconoTaller) {
            iconoTaller.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Talleres';

                $.ajax({
                    url: "/Mantenimiento/ObtenerTaller",
                    type: 'GET',
                    success: function (data) {
                        showModalTaller.show();
                        configurarTabla(data, [
                            { title: "ID", data: "idTaller", visible: false },
                            { title: "Nombre del Taller", data: "nombreTaller", className: 'min-w-125px text-justify' },
                            { title: "Zona", data: "nombreZona", className: 'min-w-125px text-center' },
                            { title: "Depósito", data: "nombreDeposito", className: 'min-w-125px text-center' },
                            { title: "Domicilio", data: "domicilio", className: 'min-w-125px text-justify' },
                            { title: "Teléfono", data: "telefono", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputTaller.value = rowData.nombreTaller;
                            inputTallerHidden.value = rowData.idTaller;

                            inputMecanico.value = '';
                            const inputMecanicoHidden = document.getElementById('mecanicoIDHidden');
                            if (inputMecanicoHidden) {
                                inputMecanicoHidden.value = '';
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener los talleres.');
                    }
                });
            });
        }

        if (iconoMecanico) {
            iconoMecanico.addEventListener('click', function () {
                const idTaller = inputTallerHidden.value;

                document.getElementById('modalTitulo').textContent = 'Mecánicos';

                $.ajax({
                    url: `/Mantenimiento/ObtenerMecanico?tallerId=${idTaller}`,
                    type: 'GET',
                    success: function (data) {
                        showModalTaller.show();
                        configurarTabla(data, [
                            { title: "Nombre del Mecánico", data: "nombre", className: 'min-w-125px text-center' },
                            { title: "Puesto", data: "puesto", className: 'min-w-125px text-center' },
                            { title: "Taller", data: "nombreTaller", className: 'min-w-125px text-center' },
                            { title: "Depósito", data: "nombreDeposito", className: 'min-w-125px text-center' },
                        ], function (rowData) {
                            inputMecanico.value = rowData.nombre;
                        });
                    },
                    error: function () {
                        alert('Error al obtener los mecánicos.');
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
    KTMantenimiento.init();
});
