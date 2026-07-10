const KTDetalleEspecificaciones = function () {
    let dt = null;

    const columnas = [
        { title: "Ancho", data: "ancho", className: "col-dimensioncapacidad text-center" },
        { title: "Largo", data: "largo", className: "col-dimensioncapacidad text-center" },
        { title: "Altura", data: "altura", className: "col-dimensioncapacidad text-center" },
        { title: "Peso Bruto", data: "pesoBruto", className: "col-dimensioncapacidad text-center" },

        { title: "Tipo de Motor", data: "tipoMotor", className: "col-motordesempeno text-center" },
        { title: "Potencia del Motor", data: "potenciaMotor", className: "col-motordesempeno text-center" },
        { title: "No. de Cilindros", data: "noCilindros", className: "col-motordesempeno text-center" },
        { title: "Capacidad de Aceite", data: "capacidadAceite", className: "col-motordesempeno text-center" },

        { title: "Transmisión", data: "transmision", className: "col-transmisiontraccion text-center" },
        { title: "Tracción", data: "traccion", className: "col-transmisiontraccion text-center" },
        { title: "Tipo de Eje", data: "tipoEje", className: "col-transmisiontraccion text-center" },
        { title: "Carga por Eje", data: "cargaPorEje", className: "col-transmisiontraccion text-center" },

        { title: "Origen", data: "origen", className: "col-datosadicionales text-center" },
        { title: "Observaciones", data: "observaciones", className: "col-datosadicionales text-center" }
    ];

    const configurarTabla = (data) => {
        if (dt) {
            dt.destroy();
            $('#kt_table_especificaciones tbody').empty();
        }

        dt = $('#kt_table_especificaciones').DataTable({
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

        mostrarColumnasPorClase("col-dimensioncapacidad");
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
        const modalElement = document.getElementById('kt_modal_especificaciones');
        const modal = new bootstrap.Modal(modalElement);

        $(document).on('click', '.btnModal', function () {
            const idMarca = $(this).data('idmarca');
            const idModelo = $(this).data('idmodelo');

            $.get(`/VehiculoDetalle/ObtenerEspecifi?idMarca=${idMarca}&idModelo=${idModelo}`, function (data) {
                configurarTabla(data);
                $('#vehiculoTabsEspecificaciones a[data-tab="dimensioncapacidad"]').tab('show');
                modal.show();
            });
        });

        $('#vehiculoTabsEspecificaciones a[data-bs-toggle="pill"]').on('shown.bs.tab', function (e) {
            const tab = $(e.target).data('tab');

            mostrarColumnasPorClase("col-" + tab);

            $('.tp').removeClass('show active');
            $('#tab-' + tab).addClass('show active');

            if (tab === "imagen") {
                const imagenes = dt?.data()?.toArray()[0]?.imagenes || [];
                $('#imagenesContainer').empty();

                imagenes.forEach(img => {
                    const componente = `
            <div class="componente-imagen" style="flex:0 0 auto;width:160px;">
                <div class="image-input image-input-outline" data-kt-image-input="true">
                    <div class="image-input-wrapper w-150px h-150px" style="background-image: url('${img.ruta}'); background-size: cover; background-position: center;"></div>
                </div>
            </div>`;
                    $('#imagenesContainer').append(componente);
                });
            }

        });

        $(document).on('click', '.componente-imagen', function () {
            const rawUrl = $(this).find('.image-input-wrapper').css('background-image');
            const url = rawUrl.replace(/^url\(["']?/, '').replace(/["']?\)$/, '');

            $('#imagenGrande').css('background-image', `url(${url})`);
            $('#imagemodal').modal('show');
        });
    };

    return {
        init: function () {
            initModalEvent();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalleEspecificaciones.init();
});

