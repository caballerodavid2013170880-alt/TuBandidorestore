"use strict";
var KTPreventivoAdd = function () {
    var initCascades = function () {
        var resetChild = function (sel, placeholder) {
            if (!sel) return;
            sel.innerHTML = '<option value="0">-- ' + placeholder + ' --</option>';
            sel.disabled = true;
        };

        var bindCascade = (srcId, targetId, url, targetText, resetCallback = null) => {
            var src = document.getElementById(srcId);
            var target = document.getElementById(targetId);
            if (!src || !target) return;

            src.addEventListener('change', function () {
                var id = parseInt(this.value, 10);
                resetChild(target, targetText);
                if (resetCallback) resetCallback(); // Ejecuta la limpieza de niveles inferiores

                if (id > 0) {
                    fetch(url + '?id=' + id).then(r => r.json()).then(data => {
                        data.forEach(item => { target.innerHTML += `<option value="${item.id}">${item.nombre}</option>`; });
                        target.disabled = false;
                    });
                }
            });
        };

        // Cadena de limpieza: Si cambia Planta, limpia Zona y Deposito
        var resetFromPlanta = () => { resetChild(document.getElementById('IdZona'), 'Seleccione Zona'); resetChild(document.getElementById('IdDeposito'), 'Seleccione Depósito'); };
        var resetFromZona = () => { resetChild(document.getElementById('IdDeposito'), 'Seleccione Depósito'); };

        bindCascade('IdRegion', 'IdPlanta', window.SUVAN.Urls.GetPlantas, 'Seleccione Planta', resetFromPlanta);
        bindCascade('IdPlanta', 'IdZona', window.SUVAN.Urls.GetZonas, 'Seleccione Zona', resetFromZona);
        bindCascade('IdZona', 'IdDeposito', window.SUVAN.Urls.GetDepositos, 'Seleccione Depósito');

        bindCascade('IdMarca', 'IdModelo', window.SUVAN.Urls.GetModelos, 'Seleccione Modelo');
    };

    var initForm = function () {
        document.getElementById('form_agregar_preventivo').addEventListener('submit', function (e) {
            e.preventDefault();
            var btn = document.getElementById('btn_guardar');
            btn.disabled = true; btn.innerText = 'Guardando...';

            var formData = new FormData(this);
            fetch(window.SUVAN.Urls.AjaxSave, { method: 'POST', body: formData })
                .then(r => r.json()).then(data => {
                    if (data.success) {
                        Swal.fire({
                            title: '¡Guardado!', text: '¿Desea generar los preventivos para los vehículos coincidentes?',
                            icon: 'success', showCancelButton: true, confirmButtonText: 'Sí, Generar', cancelButtonText: 'No, Volver al Listado',
                            customClass: { confirmButton: "btn btn-primary", cancelButton: "btn btn-light" }
                        }).then((res) => {
                            if (res.isConfirmed) {
                                fetch(window.SUVAN.Urls.AjaxGenerar, {
                                    method: 'POST', headers: { 'Content-Type': 'application/json' },
                                    body: JSON.stringify({
                                        Idpreventivo: data.idPreventivo,
                                        IdManoObra: document.getElementById('IdManoObra').value,
                                        FechaPrev: document.getElementById('FechaPrev').value
                                    })
                                }).then(r2 => r2.json()).then(d2 => {
                                    Swal.fire('Completado', d2.message, d2.success ? 'success' : 'error').then(() => window.location.href = '/MantenimientoPreventivo/Index');
                                });
                            } else {
                                window.location.href = '/MantenimientoPreventivo/Index';
                            }
                        });
                    } else {
                        Swal.fire('Error', data.message, 'error');
                        btn.disabled = false; btn.innerText = 'Guardar Registro';
                    }
                });
        });
    };

    return { init: function () { initCascades(); initForm(); } };
}();
KTUtil.onDOMContentLoaded(function () { KTPreventivoAdd.init(); });