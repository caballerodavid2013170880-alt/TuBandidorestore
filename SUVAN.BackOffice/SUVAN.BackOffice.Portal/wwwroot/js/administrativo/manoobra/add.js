"use strict";
var KTManoObraAdd = function () {
    var form;
    var submitButton;
    var btnAgregarActividad;
    var tbodyActividades;
    var initForm = function () {
        if (!form) return;
        btnAgregarActividad = document.getElementById('btnAgregarActividad');
        tbodyActividades = document.getElementById('tbodyActividades');
        if (btnAgregarActividad) {
            btnAgregarActividad.addEventListener('click', function () {
                var emptyRow = document.getElementById('trEmpty');
                if (emptyRow) {
                    emptyRow.remove();
                }
                var index = tbodyActividades.querySelectorAll('tr.actividad-row').length;

                var tr = document.createElement('tr');
                tr.className = 'actividad-row';
                tr.setAttribute('data-index', index);

                tr.innerHTML = `
                    <td>
                        <input type="hidden" name="Detalles[${index}].IdMoDetalle" value="0" class="id-detalle" />
                        <input type="hidden" name="Detalles[${index}].IdManoObra" value="0" />
                        <input type="text" name="Detalles[${index}].DescripcionActividad" class="form-control" placeholder="Ingrese descripción de la actividad" required />
                    </td>
                    <td class="text-center">
                        <div class="form-check form-switch form-check-custom form-check-solid d-flex justify-content-center">
                            <input type="hidden" name="Detalles[${index}].EsObligatorio" class="hidden-obligatorio" value="false" />
                            <input class="form-check-input h-20px w-30px chk-obligatorio" type="checkbox" />
                        </div>
                    </td>
                    <td class="text-end">
                        <button type="button" class="btn btn-icon btn-active-light-danger w-30px h-30px btnEliminarActividad">
                            <i class="ki-outline ki-trash fs-3"></i>
                        </button>
                    </td>
                `;

                tbodyActividades.appendChild(tr);
            });
        }
        $(tbodyActividades).on('click', '.btnEliminarActividad', function () {
            $(this).closest('tr').remove();
            reindexRows();

            if (tbodyActividades.querySelectorAll('tr.actividad-row').length === 0) {
                tbodyActividades.innerHTML = `
                    <tr id="trEmpty">
                        <td colspan="3" class="text-center text-muted">No hay actividades registradas. Haga clic en "Agregar Actividad" para comenzar.</td>
                    </tr>
                `;
            }
        });

        $(tbodyActividades).on('change', '.chk-obligatorio', function () {
            $(this).siblings('.hidden-obligatorio').val(this.checked ? "true" : "false");
        });

        var reindexRows = function () {
            var rows = tbodyActividades.querySelectorAll('tr.actividad-row');
            rows.forEach((row, newIndex) => {
                row.setAttribute('data-index', newIndex);
                var inputs = row.querySelectorAll('input');
                inputs.forEach(input => {
                    var name = input.getAttribute('name');
                    if (name) {
                        var updatedName = name.replace(/\[\d+\]/, `[${newIndex}]`);
                        input.setAttribute('name', updatedName);
                    }
                });
            });
        };
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            var descripcion = form.querySelector('[name="DescripcionManoobra"]').value;
            var costo = form.querySelector('[name="CostoUnitario"]').value;
            if (!descripcion || !costo) {
                Swal.fire({
                    text: "Por favor, complete los datos generales obligatorios.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Entendido",
                    customClass: { confirmButton: "btn btn-primary" }
                });
                return;
            }
            var emptyDesc = false;
            var detallesInputs = tbodyActividades.querySelectorAll('input[type="text"]');
            detallesInputs.forEach(function (input) {
                if (!input.value.trim()) {
                    emptyDesc = true;
                }
            });
            if (emptyDesc) {
                Swal.fire({
                    text: "Todas las actividades deben tener una descripción.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Entendido",
                    customClass: { confirmButton: "btn btn-primary" }
                });
                return;
            }
            submitButton.setAttribute('data-kt-indicator', 'on');
            submitButton.disabled = true;
            var payload = {
                IdManoObra: parseInt(form.querySelector('[name="IdManoObra"]').value) || 0,
                DescripcionManoobra: descripcion,
                CostoUnitario: parseFloat(costo) || 0,
                Detalles: []
            };

            tbodyActividades.querySelectorAll('tr.actividad-row').forEach(function (row) {
                var idDetalle = row.querySelector('.id-detalle').value;
                var idMo = row.querySelector('input[name*=".IdManoObra"]').value;
                var descAct = row.querySelector('input[type="text"]').value;
                var isObligatorio = row.querySelector('input[type="checkbox"]').checked;

                payload.Detalles.push({
                    IdMoDetalle: parseInt(idDetalle) || 0,
                    IdManoObra: parseInt(idMo) || 0,
                    DescripcionActividad: descAct,
                    EsObligatorio: isObligatorio
                });
            });

            var token = form.querySelector('input[name="__RequestVerificationToken"]').value;
            fetch(window.SUVAN.Urls.AjaxSave, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(payload)
            })
                .then(response => response.json())
                .then(data => {
                    submitButton.removeAttribute('data-kt-indicator');
                    submitButton.disabled = false;
                    if (data.success) {
                        Swal.fire({
                            text: data.message,
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: { confirmButton: "btn btn-primary" }
                        }).then(function () {
                            window.location = '/ManoObra/Index';
                        });
                    } else {
                        Swal.fire({
                            text: data.message,
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Entendido",
                            customClass: { confirmButton: "btn btn-primary" }
                        });
                    }
                })
                .catch(error => {
                    submitButton.removeAttribute('data-kt-indicator');
                    submitButton.disabled = false;
                    Swal.fire({
                        text: "Ha ocurrido un error inesperado.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Entendido",
                        customClass: { confirmButton: "btn btn-primary" }
                    });
                });
        });
    };
    return {
        init: function () {
            form = document.querySelector('#form_agregar_manoobra');
            if (form) {
                submitButton = document.getElementById('btn_guardar');
                initForm();
            }
        }
    };
}();
KTUtil.onDOMContentLoaded(function () {
    KTManoObraAdd.init();
});
