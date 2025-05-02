/**
 * Autor:Hedilberto Cruz Vasquez
 * Fecha: 18 de Diciembre del 2023
 * Descripcion: Js que permite la funcionalidad de consulta promociones
 */
let fnConsultaPromociones = new function () {
    "use strict";
    var self = this,
        $tablePromociones = $("#kt_table_promociones"),
        _dataTable = {};

    function _init() {
        _initDataTable();
    }
    function _initDataTable() {
        _dataTable = $tablePromociones.InitDataTable();
        _onRenderDataTable();
        _dataTable.on("draw", _onRenderDataTable);
        $('[data-kt-search-element="suggestions"]').on("keyup", function (e) {
            _dataTable.search(e.target.value).draw();
        });
    }
    function _onRenderDataTable() {
        $('[data-kt-promocion-action').off("click");
        $('[data-kt-promocion-action="edit"]').on("click", _onEdit)
        $('[data-kt-promocion-action="delete"]').on("click", _onDelete)
    }
    function _onEdit(){
        let $tdElement = $(this).parents("td");
        window.location = "../Comercial/GeneraPromocion?numeroPromocion=" + $tdElement.data("itemid");
    }
    function _onDelete() {
        let $tdElement = $(this).parents("td");
        Swal.fire({
            title: "Eliminación",
            text: "Esta seguro de eliminar la promoción",
            icon: "warning",
            showCancelButton: true,
            buttonsStyling: false,
            confirmButtonText: "Si, eliminar!",
            cancelButtonText: "No, cancelar",
            customClass: {
                confirmButton: "btn fw-bold btn-danger",
                cancelButton: "btn fw-bold btn-active-light-primary"
            },
            preConfirm: async () => {
                const response = await fetch('/Comercial/RemuevePromocion', {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        PromocionId: $tdElement.data("itemid")
                    })
                }).then((data) => {
                    console.log(data)
                    Swal.fire({
                        title: "Eliminación",
                        text: "El registro fue eliminado con exito",
                        icon: "success"
                    });
                }).catch(() => {

                })
            }
        }).then((result) => {
            if (result.isConfirmed) {
                _dataTable.row($tdElement.parents("tr")).remove().draw();
            }
        });
    }

    self.init = function () {
        _init();
    }
};
KTUtil.onDOMContentLoaded(function () {
    fnConsultaPromociones.init();
});