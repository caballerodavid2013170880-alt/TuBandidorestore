let fnConsultaUsuarios = new function () {
    "use strict";

    const flatpickrOptions = {
        dateFormat: "d/m/Y",
    };
    const datepickerDesde = $("#desdeview").flatpickr(flatpickrOptions);
    const datepickerHasta = $("#hastaview").flatpickr(flatpickrOptions);

    var self = this,
        $tableUsuarios = $("#kt_table_mantenimiento"),
        _dataTable = {};

    function _init() {
        _initDataTable();
        exportButtons($tableUsuarios);
    }

    function _initDataTable() {
        _dataTable = $tableUsuarios.InitDataTable();
        _onRenderDataTable();
        _dataTable.on("draw", _onRenderDataTable);
        $('[data-kt-mantenimiento-table-filter="search"]').on("keyup", function (e) {
            _dataTable.search(e.target.value).draw();
        });
    }

    function _onRenderDataTable() {
        $('[data-kt-usuario-action]').off("click");
        $('[data-kt-usuario-action="edit"]').on("click", _onEdit);
    }

    function _onEdit() {
        window.location = "../Mantenimiento/OrdenDetalle";
    }

    self.init = function () {
        _init();
    }
};

// Hook export buttons
const exportButtons = (table) => {
    const documentTitle = 'Reporte Orden Mantenimiento';
    const dt = table.DataTable();

    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            const desde = $('#desdeview').val();
            const hasta = $('#hastaview').val();

            if (!desde || !hasta) return true;

            const dateColIndex = 6;
            const fechaTexto = data[dateColIndex];
            const [dia, mes, anio] = fechaTexto.split('/');
            const fecha = new Date(`${anio}-${mes}-${dia}`);

            const [dD, dM, dA] = desde.split('/');
            const desdeDate = new Date(`${dA}-${dM}-${dD}`);
            const [hD, hM, hA] = hasta.split('/');
            const hastaDate = new Date(`${hA}-${hM}-${hD}`);

            return fecha >= desdeDate && fecha <= hastaDate;
        }
    );

    var buttons = new $.fn.dataTable.Buttons(table, {
        buttons: [
            {
                extend: 'pdfHtml5',
                title: documentTitle,
                exportOptions: {
                    columns: ':not(:nth-child(10))'
                },
                action: function (e, dt, button, config) {
                    dt.draw();

                    setTimeout(() => {
                        $.fn.dataTable.ext.buttons.pdfHtml5.action.call(this, e, dt, button, config);

                         $.fn.dataTable.ext.search.pop();
                         dt.draw();
                    }, 100);
                }
            }
        ]
    }).container().appendTo($('#kt_datatable_example_buttons'));

    // Hook export menu del dropdown
    const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
    exportButtons.forEach(exportButton => {
        exportButton.addEventListener('click', e => {
            e.preventDefault();
            const exportValue = e.target.getAttribute('data-kt-export');
            const target = document.querySelector('.dt-buttons .buttons-' + exportValue);
            target.click();
        });
    });
};

KTUtil.onDOMContentLoaded(function () {
    fnConsultaUsuarios.init();
});