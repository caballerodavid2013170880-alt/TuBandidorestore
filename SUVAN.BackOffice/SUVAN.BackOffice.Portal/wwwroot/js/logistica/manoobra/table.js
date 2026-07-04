"use strict";
var KTManoObraIndex = function () {
    var datatable;
    var initDatatable = function () {
        var table = document.querySelector('#kt_table_manoobra');
        if (!table) return;
        datatable = $(table).DataTable({
            "info": true,
            "order": [],
            "columnDefs": [
                { "orderable": false, "targets": 2 }
            ]
        });
        var sortMap = {
            'sortNombre': 0,
            'sortCosto': 1
        };
        $('.sort-filter').on('change', function () {
            var dir = $(this).val();
            var id = $(this).attr('id');
            $('.sort-filter').not(this).val('');
            if (dir) {
                datatable.order([sortMap[id], dir]).draw();
            } else {
                datatable.order([]).draw();
            }
        });
        $('#filtroGeneralManoObra').on('keyup', function () {
            datatable.search(this.value).draw();
        });
        $('#btnLimpiarOrden').on('click', function () {
            $('.sort-filter').val('');
            $('#filtroGeneralManoObra').val('');
            datatable.search('').columns().search('').order([]).draw();
        });
    };
    return {
        init: function () {
            initDatatable();
        }
    };
}();
window.abrirModalDetalle = function (id) {
    fetch('/ManoObra/GetModalDetalle?id=' + id)
        .then(response => response.text())
        .then(html => {
            document.getElementById('modalContentAjax').innerHTML = html;
            var modal = new bootstrap.Modal(document.getElementById('modalDetalleManoObra'));
            modal.show();
            if ($.fn.DataTable.isDataTable('#kt_table_modal_actividades')) {
                $('#kt_table_modal_actividades').DataTable().destroy();
            }
            $('#kt_table_modal_actividades').DataTable({ "info": false, "pageLength": 5 });
        })
        .catch(error => console.error("Error al cargar el modal: ", error));
};
KTUtil.onDOMContentLoaded(function () {
    KTManoObraIndex.init();
});