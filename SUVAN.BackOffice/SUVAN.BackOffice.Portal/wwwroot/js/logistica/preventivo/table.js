"use strict";
var datatable;
var KTPreventivoTable = function () {
    var initDatatable = function () {
        var table = document.querySelector('#kt_table_preventivo');
        if (!table) return;

        // Inicializamos Datatables (Quitando el orden de la columna Acciones, que ahora es la 9)
        datatable = $(table).DataTable({
            "info": false,
            "order": [],
            "columnDefs": [{ "orderable": false, "targets": 9 }]
        });

        // Filtro: Fecha Prevista (Columna 8)
        $('#filtroFecha').on('change', function () {
            let v = this.value;
            if (v) {
                let p = v.split('-');
                datatable.column(8).search(p[2] + '/' + p[1] + '/' + p[0]).draw();
            } else {
                datatable.column(8).search('').draw();
            }
        });

        // Filtro: Búsqueda General
        document.querySelector('[data-kt-preventivo-table-filter="search"]').addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });

        var clearSorts = function (exceptId) {
            $('.sort-filter').not('#' + exceptId).val('');
        };

        $('#sortNombre').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortNombre'); datatable.order([0, v]).draw(); } });
        $('#sortRegion').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortRegion'); datatable.order([1, v]).draw(); } });
        $('#sortPlanta').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortPlanta'); datatable.order([2, v]).draw(); } });
        $('#sortZona').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortZona'); datatable.order([3, v]).draw(); } });
        $('#sortDeposito').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortDeposito'); datatable.order([4, v]).draw(); } });
        $('#sortMarca').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortMarca'); datatable.order([5, v]).draw(); } });
        $('#sortModelo').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortModelo'); datatable.order([6, v]).draw(); } });
        $('#sortCosto').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortCosto'); datatable.order([7, v]).draw(); } });
        $('#sortFecha').on('change', function () { var v = $(this).val(); if (v) { clearSorts('sortFecha'); datatable.order([8, v]).draw(); } });

        // Limpiar Filtros
        $('#btnLimpiarOrden').on('click', function () {
            $('.sort-filter').val('');
            $('#filtroGeneralMantenimientos').val('');
            $('#filtroFecha').val('');
            if (datatable) {
                datatable.search('').columns().search('').order([0, 'asc']).draw();
            }
        });
    };

    window.abrirModalDetalle = function (id) {
        $('#modalContentAjax').html('<div class="d-flex justify-content-center p-10"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Cargando...</span></div></div>');
        $('#modalDetallePreventivo').modal('show');
        $('#modalContentAjax').load('/MantenimientoPreventivo/GetModalDetalle?id=' + id, function (response, status, xhr) {
            if (status == "error") {
                $('#modalContentAjax').html('<div class="alert alert-danger m-5">Error al cargar los detalles del preventivo.</div>');
            }
        });
    };
    return { init: function () { initDatatable(); } };
}();