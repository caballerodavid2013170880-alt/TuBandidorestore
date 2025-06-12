var KTMantenimiento = function () {

    const initModalEvent = () => {
        const modalTaller = document.getElementById('kt_modal_mantenimiento_taller');

        if (!modalTaller) return;

        const showModalTaller = new bootstrap.Modal(modalTaller, {
            keyboard: false,
            backdrop: 'static'
        });

        const iconoTaller = document.getElementById('iconoTaller');
        const inputTaller = document.getElementById('tallerID');
        const inputTallerHidden = document.getElementById('tallerIDHidden');

        const iconoMecanico = document.getElementById('iconoMecanico');
        const inputMecanico = document.getElementById('mecanicoID');

        const iconoCausa = document.getElementById('iconoCausaMantenimiento');
        const inputCausa = document.getElementById('causaMantenimientoID');

        const botonReparacion = document.getElementById('btn_agregar_reparacion')

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

        function agregarFilaReparacion(descripcion, unidades) {
            const template = document.getElementById("template-fila-reparacion");
            const tbody = document.getElementById("tablaReparaciones");

            const nuevaFila = template.content.cloneNode(true);
            const inputDescripcion = nuevaFila.querySelector(".reparacion-descripcion");
            const inputCantidad = nuevaFila.querySelector(".cantidad");
            const inputValor = nuevaFila.querySelector(".valor");
            const btnEliminar = nuevaFila.querySelector(".btn-eliminar-fila");

            inputDescripcion.value = `${descripcion} - Unidades: ${unidades}`;
            inputDescripcion.dataset.unidades = unidades;

            inputCantidad.addEventListener("input", function () {
                const cantidad = parseFloat(inputCantidad.value) || 0;
                const unidad = parseFloat(inputDescripcion.dataset.unidades) || 0;
                inputValor.value = (cantidad * unidad).toFixed(2);
            });

            btnEliminar.addEventListener("click", function () {
                btnEliminar.closest("tr").remove();
            });

            tbody.appendChild(nuevaFila);
        }

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

        if (iconoCausa) {
            iconoCausa.addEventListener('click', function () {

                document.getElementById('modalTitulo').textContent = 'Causa Mantenimiento';

                $.ajax({
                    url: `/Mantenimiento/ObtenerCausaMantenimiento`,
                    type: 'GET',
                    success: function (data) {
                        showModalTaller.show();
                        configurarTabla(data, [
                            { title: "Número de Causa", data: "idCausamantenimiento", className: 'min-w-125px text-center' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-center' },
                        ], function (rowData) {
                            inputCausa.value = rowData.descripcion;
                        });
                    },
                    error: function () {
                        alert('Error al obtener los mecánicos.');
                    }
                });
            });
        }



        if (botonReparacion) {
            botonReparacion.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Reparaciones';

                $.ajax({
                    url: `/Mantenimiento/ObtenerTipoReparacion`,
                    type: 'GET',
                    success: function (data) {
                        showModalTaller.show();

                        configurarTabla(data, [
                            { title: "Número de Reparación", data: "idTipoReparacion", className: 'min-w-125px text-start' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-start' },
                            { title: "Unidad", data: "valor", className: 'min-w-125px text-center' },
                        ], function (rowData) {
                            agregarFilaReparacion(rowData.descripcion, rowData.valor);
                        });
                    },
                    error: function () {
                        alert('Error al obtener las reparaciones.');
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
