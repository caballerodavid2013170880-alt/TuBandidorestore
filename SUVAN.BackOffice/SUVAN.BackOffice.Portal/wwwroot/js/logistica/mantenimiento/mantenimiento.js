let fnConsultaUsuarios = new function () {
    "use strict";
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
        $('[data-kt-usuario-action').off("click");
        $('[data-kt-usuario-action="edit"]').on("click", _onEdit)
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
    var buttons = new $.fn.dataTable.Buttons(table, {
        buttons: [
            {
                extend: 'pdfHtml5',
                title: documentTitle,
                exportOptions: {
                    columns: ':not(:nth-child(10))' // Excluye la decima columna
                }
            }

        ]
    }).container().appendTo($('#kt_datatable_example_buttons'));

    // Hook dropdown menu click event to datatable export buttons
    const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
    exportButtons.forEach(exportButton => {
        exportButton.addEventListener('click', e => {
            e.preventDefault();

            // Get clicked export value
            const exportValue = e.target.getAttribute('data-kt-export');
            const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

            // Trigger click event on hidden datatable export buttons
            target.click();
        });
    });
}

KTUtil.onDOMContentLoaded(function () {
    fnConsultaUsuarios.init();
});

$('#selectAll').click(function () {
    if ($(this).prop('checked')) {
        $('.rowCheckbox').prop('checked', true); // Seleccionar todas las filas
    } else {
        $('.rowCheckbox').prop('checked', false); // Deseleccionar todas las filas
    }
});