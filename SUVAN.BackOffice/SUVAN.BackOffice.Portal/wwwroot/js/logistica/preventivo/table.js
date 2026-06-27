"use strict";
var datatable;
var KTPreventivoTable = function () {
    var initDatatable = function () {
        var table = document.querySelector('#kt_table_preventivo');
        if (!table) return;
        datatable = $(table).DataTable({ "info": false, "order": [], "columnDefs": [{ "orderable": false, "targets": 5 }] });

        // Filtros específicos por columna (Nombre:0, Planta:1, Marca:2, Fecha:4)
        $('#filtroNombre').on('keyup', function () { datatable.column(0).search(this.value).draw(); });
        $('#filtroPlanta').on('keyup', function () { datatable.column(1).search(this.value).draw(); });
        $('#filtroMarca').on('keyup', function () { datatable.column(2).search(this.value).draw(); });
        $('#filtroFecha').on('change', function () {
            // Formatear la fecha al formato que se muestra en la tabla (dd/MM/yyyy)
            let v = this.value; if (v) { let p = v.split('-'); datatable.column(4).search(p[2] + '/' + p[1] + '/' + p[0]).draw(); }
            else { datatable.column(4).search('').draw(); }
        });
        document.querySelector('[data-kt-preventivo-table-filter="search"]').addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    };
    return { init: function () { initDatatable(); } };
}();

function abrirModalDetalle(id) {
    fetch('/MantenimientoPreventivo/GetModalDetalle?id=' + id)
        .then(response => response.text())
        .then(html => {
            document.getElementById('modalContentAjax').innerHTML = html;
            var modal = new bootstrap.Modal(document.getElementById('modalDetallePreventivo'));
            modal.show();
            $('#kt_table_modal_vehiculos').DataTable({ "info": false, "pageLength": 5 });
        });
}
KTUtil.onDOMContentLoaded(function () { KTPreventivoTable.init(); });