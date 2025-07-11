var suvanCatalogs = function () {

    let catalogBaseName;
    let catalogField;
    let catalogDescription;
    const $tableCatalog = $('#table-catalog-demo');

    const initModalTable = () => {
        const $modal = $('#modalcatalogSelect');
        const $tableCatalog = $('#table-catalog-demo');
        let $currentInput;

        $(document).on('click', '.show-modal-catalog', function () {
            var enabled = true;
            const $this = $(this);
            if ($this.data('suvan-enable') != undefined) {
                enabled = $this.data('suvan-enable');
            }
            if (enabled) {
                catalogBaseName = '';
                catalogBaseName = $this.data('suvan-catalog');
                catalogFilter = $this.data('suvan-filter');
                catalogCallback = $this.attr('id');
                $currentInput = $this.closest('div.input-group').find('input.form-control');
                filtro = $this.data('suvan-catalog-filtro');
                catalogSubTipo = filtro ? $('#' + filtro).val() : undefined;
                catalogFillData = $this.data('suvan-catalog-filldata');
                catalogField = $this.data('suvan-catalog-field');
                catalogDescription = $this.data('suvan-catalog-description');
                $tableCatalog.find("tbody").empty();
                if (catalogSubTipo == undefined) {
                    getCatalogTable();
                }
                else {
                    getCatalogTableSubTipo();
                }
                $modal.modal('show');
            }
        });

        $('#btn-select-catalog').on('click', function (e) {
            var datosseleccionados = $tableCatalog.DataTable().rows({ selected: true }).data();
            if (datosseleccionados.length <= 0) {
                notificacion.warning('Seleccione un registro');
                return;
            }
            const catalogId = datosseleccionados[0].id;
            const catalogDes = datosseleccionados[0].descripcion;
            if (!catalogId) {
                notificacion.warning('Seleccione un registro');
                return;
            }

            if (catalogField) {
                const dataFieldValue = datosseleccionados[0][catalogField];
                $currentInput.val(dataFieldValue);
                $currentInput.data('catalog-id', dataFieldValue);
            } else {
                let currentInputVal;
                if (catalogDescription) {
                    const descriptions = catalogDescription.split('|').map(d => d.trim()).filter(d => d);
                    const values = descriptions
                        .map(desc => datosseleccionados[0][desc])
                        .filter(val => val !== undefined && val !== null && val !== '')
                        .join(' - ');
                    $currentInput.val(values);
                }
                else {
                    currentInputVal = `${catalogId}${catalogDes !== "undefined" ? ' - ' + catalogDes : ''}`;
                    $currentInput.val(currentInputVal);
                }
                const aspFor = $currentInput.data('catalog-for');
                if (aspFor) {
                    $(`[name="${aspFor}"]`).val(catalogId).trigger('change');
                }
                if (catalogBaseName === "vehiculo") {
                    const datos = datosseleccionados[0];

                    $('#IdVehiculo').val(datos.id);
                    $('#Carroceria').val(datos.vin);
                    $('#NumeroMotor').val(datos.numeromotor);
                    $('#EconomicoAnterior').val(datos.numeroeconomico);

                    $('#IdVehiculoVisible').val(datos.id + ' - ' + datos.placas);
                }
                $currentInput.data('catalog-id', catalogId);
                $currentInput.data('catalog-des', catalogDes);
            }
            $currentInput.trigger('change');
            $modal.modal('hide');
        });
    }

    const getCatalogTable = () => {
        $.ajax({
            url: `/Catalogs/GetCatalogosSearch?id=${catalogBaseName}`,
            method: 'GET',
            success: function (data) {
                if (!data.success) {
                    notificacion.error(data.message);
                    return;
                }
                catalogList = data.data;
                initTableCatalogList();
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

    const getCatalogTableSubTipo = () => {
        $.ajax({
            url: `/Catalogs/GetCatalogosSearch?id=${catalogBaseName}&nClaveFiltro=${catalogSubTipo}`,
            method: 'GET',
            success: function (data) {
                if (!data.success) {
                    notificacion.error(data.message);
                    return;
                }
                catalogList = data.data;
                initTableCatalogList();
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

    const initTableCatalogList = () => {
        if ($.fn.DataTable.isDataTable($tableCatalog)) {
            // Si ya está inicializado, destruir la instancia existente
            $($tableCatalog).DataTable().clear().destroy();
        }
        $tableCatalog.find("thead").empty();
        $tableCatalog.find("tbody").empty();
        var columnas = [];
        if (catalogList.length > 0) {
            const keys = Object.keys(catalogList[0]);
            var primero = true;
            keys.forEach(k => {
                const capitalizedKey = k.charAt(0).toUpperCase() + k.slice(1);
                let columnConfig = {
                    title: capitalizedKey,
                    data: k,
                    render: function (data, type, row, meta) {
                        if (k.startsWith("Fecha")) {
                            return utilerias.ControllertoFecha(data);
                        } else {
                            return data;
                        }
                    },
                    visible: !capitalizedKey.startsWith("Hidden")
                };
                if (k.startsWith("Fecha")) {
                    columnConfig.orderable = false; // Deshabilitar la ordenación para columnas con 'Fecha'
                }
                if (primero) {
                    columnConfig.render = function (data, type, row, meta) {
                        const jsonString = JSON.stringify(row);
                        return '<input id="input_' + row.id + '" type="hidden" name="selectItem" data-catalog-info=`"' + jsonString + '"` data-catalog-id="' + row.id
                            + '" data-catalog-des="' + row.descripcion + '">' + data;
                    };
                    columnConfig.visible = false;
                    primero = false;
                }
                columnas.push(columnConfig);
            });
            // busca en columnas si hay una que tiene la palabra Fecha y regresame su index
            $tableCatalog.DataTable({
                columns: columnas,
                data: catalogList,
                lengthChange: false,
                "language": {
                    "paginate": {
                        "previous": "<i class='mdi mdi-chevron-left'>",
                        "next": "<i class='mdi mdi-chevron-right'>"
                    }
                },
                "drawCallback": function () {
                    $('.dataTables_paginate > .pagination').addClass('pagination-rounded');
                },
                select: 'single'
            });
        }
    };

    return {
        init: function () {
            initModalTable();
        }
    }
}();

$(document).ready(function () {
    suvanCatalogs.init();
});
