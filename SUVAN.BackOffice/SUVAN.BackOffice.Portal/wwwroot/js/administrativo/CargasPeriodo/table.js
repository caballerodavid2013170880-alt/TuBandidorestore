"use strict";

var KTCargasPeriodoList = function () {
    var table = document.getElementById('kt_table_cargas_periodo');
    var datatable;
    var txtBuscar = document.getElementById('txtBuscarGrid');
    var depositoActualId = null; //variable para guardar el depósito actual y evitar recargas innecesarias
    var vehiculosDatatable = null; // Instancia DataTable para el modal de vehículos

    // 1. Manejo de los Combos en Cascada (Endpoints en inglés)
    var initFiltrosCascada = function () {
        const cmbRegion = document.getElementById('cmbRegion');
        const cmbPlanta = document.getElementById('cmbPlanta');
        const cmbZona = document.getElementById('cmbZona');
        const cmbDeposito = document.getElementById('cmbDeposito');

        if (!cmbRegion) return;

        cmbRegion.addEventListener('change', async function () {
            limpiarCombo(cmbPlanta, 'Seleccione una Planta...');
            limpiarCombo(cmbZona, 'Seleccione una Zona...');
            limpiarCombo(cmbDeposito, 'Seleccione un Deposito...');
            destruirGrid();

            if (!this.value) return;

            const response = await fetch(`/CargaPeriodo/GetPlantas?regionId=${this.value}`);
            const plantas = await response.json();
            llenarCombo(cmbPlanta, plantas);
            cmbPlanta.disabled = false;
        });

        cmbPlanta.addEventListener('change', async function () {
            limpiarCombo(cmbZona, 'Seleccione una Zona...');
            limpiarCombo(cmbDeposito, 'Seleccione un Depósito...');
            destruirGrid();

            if (!this.value) return;

            const response = await fetch(`/CargaPeriodo/GetZonas?plantaId=${this.value}`);
            const zonas = await response.json();
            llenarCombo(cmbZona, zonas);
            cmbZona.disabled = false;
        });

        cmbZona.addEventListener('change', async function () {
            limpiarCombo(cmbDeposito, 'Seleccione un Deposito...');
            destruirGrid();

            if (!this.value) return;

            const response = await fetch(`/CargaPeriodo/GetDepositos?zonaId=${this.value}`);
            const depositos = await response.json();
            llenarCombo(cmbDeposito, depositos);
            cmbDeposito.disabled = false;
        });

        cmbDeposito.addEventListener('change', function () {
            if (this.value) {
                depositoActualId = this.value; // Guardar el depósito actual
                cargarDatosGrid(this.value);
                cargarEstadisticas(this.value);
            } else {
                despositoActualId = null; // Resetear el depósito actual
                destruirGrid();
            }
        });
    };

    // 2. Carga Dinámica de Cargas (Pinta sólo las 5 columnas requeridas)
    var cargarDatosGrid = async function (idDeposito) {
        destruirGrid();

        const tbody = document.getElementById('tbodyCargas');
        tbody.innerHTML = `<tr><td colspan="14" class="text-center py-10"><span class="spinner-border text-primary"></span> Cargando información...</td></tr>`;

        const response = await fetch(`/CargaPeriodo/GetCargasPeriodo?idDeposito=${idDeposito}`);
        const datos = await response.json();

        if (datos.length === 0) {
            tbody.innerHTML = `<tr><td colspan="14" class="text-center text-muted py-10">No se encontraron cargas en este depósito para el periodo.</td></tr>`;
            return;
        }

        tbody.innerHTML = '';
        datos.forEach(c => {
            const tr = document.createElement('tr');
            //datos solicitados
            tr.innerHTML = `                
                <td class="fw-bold">${c.vehiculoEconomico}</td>
                <td>${c.marca}</td>
                <td>${c.modelo}</td>
                <td>${c.placas}</td>
                <td>${c.fecha}</td>
                <td class="text-end">${c.kmAnt.toFixed(2)}</td>
                <td class="text-end">${c.kmAct.toFixed(2)}.</td>
                <td class="text-end text-primary fw-bold">${c.kmRec.toFixed(2)}</td>
                <td class="text-center">#${c.nota}</td>
                <td class="text-end">${c.litros.toFixed(2)} Lts.</td>                                
                <td class="text-end">$${c.costoXLt.toFixed(2)}</td>
                <td class="text-end">$${c.importe.toFixed(2)}</td>
                <td class="text-end">${c.rendimiento.toFixed(2)} KmXLt</td>                
                <td>${c.combustible}</td>
                
            `;
            tbody.appendChild(tr);
        });

        // Inicializamos DataTable con la configuración de 14 columnas 
        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            "pageLength": 10,
            "lengthChange": false,
            'columnDefs': [
                { orderable: false, targets: '_all' } // Deshabilita ordenamiento en los inputs
            ]
        });

        if (txtBuscar) txtBuscar.disabled = false;
        //initEventosCalculo();
    };

    // 3. Carga de Estadísticas del periodo (Panel Inferior)
    var cargarEstadisticas = async function (idDeposito) {
        try {
            const response = await fetch(`/CargaPeriodo/GetEstadisticas?idDeposito=${idDeposito}`);
            const stats = await response.json();

            document.getElementById('lblCargasTotales').innerText = stats.cargasTotales;
            document.getElementById('lblConsumoLitros').innerText = stats.consumoLitros;
            document.getElementById('lblImporteTotal').innerText = `$${stats.importeTotal}`;
            document.getElementById('lblKmsTotales').innerText = stats.kmsRecorridosTotales;
            document.getElementById('lblKmPorLitro').innerText = stats.kilometrosLitro;
            document.getElementById('lblCostoPorKm').innerText = `$${stats.costoXKmRecorrido}`;
        } catch (error) {
            console.error('Error al cargar estadísticas:', error);
        }
    };

    //4. Inicialización delevento para cargar el Modal
    var initModalVehiculos = function () {
        const btnVerVehiculos = document.getElementById('btnVerVehiculosCargas');
        if (!btnVerVehiculos) return;

        //instancia de modal de forma segura
        const modalElement = document.getElementById('kt_modal_vehiculos_cargas');
        const bsmodal = new bootstrap.Modal(modalElement);


        btnVerVehiculos.addEventListener('click', async function () {
            if (!depositoActualId) {
                Swal.fire({
                    text: "Seleccione un deposito para ver los vehiculos.",
                    icon: "warning",
                    buttonsStyling: false,
                    confirmButtonText: "Aceptar",
                    customClass: { confirmButton: "btn fw-bold btn-primary" }
                });
                return;
            }

            const tbodyVehiculos = document.getElementById('tbodyVehiculosCargas');

            //destruir Datatable anterior de forma segura antes de impiar el HTML
            if ($.fn.DataTable.isDataTable('#kt_table_vehiculos_cargas')) {
                $('#kt_table_vehiculos_cargas').DataTable().clear().destroy();
            }
            
            tbodyVehiculos.innerHTML = `<tr><td colspan="4" class="text-center py-5"><span class="spinner-border text-primary"></span> Cargando vehículos...</td></tr>`;

            //muestra modal inmediatamente mientras carga la info
            bsmodal.show();

            try {
                const response = await fetch(`/CargaPeriodo/GetVehiculosCargasPeriodo?idDeposito=${depositoActualId}`);
                const vehiculos = await response.json();

                if (!vehiculos || vehiculos.length === 0) {
                    tbodyVehiculos.innerHTML = `<tr><td colspan="4" class="text-center text-muted py-5">No se encontraron vehiculos asociados. </td></tr>`;
                    return;
                }

                tbodyVehiculos.innerHTML = '';
                vehiculos.forEach(v => {
                    const tr = document.createElement('tr');
                    tr.innerHTML = `
                        <td class="fw-bold">${v.numeconomico}</td>
                        <td>${v.marca || ''}</td>
                        <td>${v.modelo || ''}</td>
                        <td>${v.placas || ''}</td>
                    `;
                    tbodyVehiculos.appendChild(tr);
                });

                // Inicializar DataTable dentro del modal
                vehiculosDatatable = $('#kt_table_vehiculos_cargas').DataTable({
                    "info": false,
                    "pageLength": 5,
                    "lengthChange": false,
                    'ordering': true
                });

            } catch (error) {
                console.error('Error al cargar vehiculos del periodo:', error);
                tbodyVehiculos.innerHTML = `<tr><td colspan="4" class="text-center text-danger py-5">Error al cargar los datos.</td></tr>`;
            }
        });
    };

    // Auxiliares para manipulación de combos
    var handleSearchDatatable = () => {
        if (!txtBuscar) return;
        txtBuscar.addEventListener('keyup', function (e) {
            if (datatable) datatable.search(e.target.value).draw();
        });
    };

    var limpiarCombo = (combo, texto) => {
        combo.innerHTML = `<option value="">${texto}</option>`;
        combo.disabled = true;
    };

    var llenarCombo = (combo, lista) => {
        lista.forEach(item => {
            const opt = document.createElement('option');
            opt.value = item.id;    // Corresponde a la propiedad de tus DTOs/ViewModels en inglés
            opt.text = item.nombre;
            combo.appendChild(opt);
        });
    };

    var destruirGrid = () => {
        if ($.fn.DataTable.isDataTable(table)) {
            $(table).DataTable().destroy();
        }
        document.getElementById('tbodyCargas').innerHTML = `<tr><td colspan="14" class="text-center text-muted py-10">Seleccione un deposito para consultar las cargas del periodo.</td></tr>`;

        //resetear estadísticas
        document.getElementById('lblCargasTotales').innerText = '0';
        document.getElementById('lblConsumoLitros').innerText = '0';
        document.getElementById('lblImporteTotal').innerText = '$0.00';
        document.getElementById('lblKmsTotales').innerText = '0.00';
        document.getElementById('lblKmPorLitro').innerText = '0.00';
        document.getElementById('lblCostoPorKm').innerText = '$0.00';

        if (txtBuscar) {
            txtBuscar.disabled = true;
            txtBuscar.value = '';
        }
    };

    return {
        init: function () {
            if (!table) return;
            initFiltrosCascada();
            initModalVehiculos(); // Inicializar el modal
            handleSearchDatatable();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTCargasPeriodoList.init();
});