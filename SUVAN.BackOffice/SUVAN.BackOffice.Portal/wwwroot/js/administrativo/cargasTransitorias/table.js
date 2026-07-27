"use strict";

var KTCargasTransitoriasList = function () {
    var table = document.getElementById('kt_table_cargas_transitorias');
    var datatable;
    var txtBuscar = document.getElementById('txtBuscarGrid');

    // 1. Manejo de los Combos en Cascada (Endpoints en inglés)
    var initFiltrosCascada = function () {
        const cmbRegion = document.getElementById('cmbRegion');
        const cmbPlanta = document.getElementById('cmbPlanta');
        const cmbZona = document.getElementById('cmbZona');
        const cmbDeposito = document.getElementById('cmbDeposito');

        if (!cmbRegion) return;

        cmbRegion.addEventListener('change', async function () {
            limpiarCombo(cmbPlanta, 'Seleccione una Planta...');
            limpiarCombo(cmbZona, 'Seleccione una Zonaa...');
            limpiarCombo(cmbDeposito, 'Seleccione un Deposito...');
            destruirGrid();

            if (!this.value) return;

            const response = await fetch(`/Combustible/GetPlantas?regionId=${this.value}`);
            const plantas = await response.json();
            llenarCombo(cmbPlanta, plantas);
            cmbPlanta.disabled = false;
        });

        cmbPlanta.addEventListener('change', async function () {
            limpiarCombo(cmbZona, 'Seleccione una Zona...');
            limpiarCombo(cmbDeposito, 'Seleccione un Deposito...');
            destruirGrid();

            if (!this.value) return;

            const response = await fetch(`/Combustible/GetZonas?plantaId=${this.value}`);
            const zonas = await response.json();
            llenarCombo(cmbZona, zonas);
            cmbZona.disabled = false;
        });

        cmbZona.addEventListener('change', async function () {
            limpiarCombo(cmbDeposito, 'Seleccione un Deposito...');
            destruirGrid();

            if (!this.value) return;

            const response = await fetch(`/Combustible/GetDepositos?zonaId=${this.value}`);
            const depositos = await response.json();
            llenarCombo(cmbDeposito, depositos);
            cmbDeposito.disabled = false;
        });

        cmbDeposito.addEventListener('change', function () {
            if (this.value) {
                cargarDatosGrid(this.value);
            } else {
                destruirGrid();
            }
        });
    };

    // 2. Carga Dinámica de Cargas (Pinta sólo las 5 columnas requeridas)
    var cargarDatosGrid = async function (idDeposito) {
        destruirGrid();

        const tbody = document.getElementById('tbodyCargas');
        tbody.innerHTML = `<tr><td colspan="5" class="text-center py-10"><span class="spinner-border text-primary"></span> Cargando información de la Base de Datos...</td></tr>`;

        const response = await fetch(`/Combustible/GetCargas?idDeposito=${idDeposito}`);
        const datos = await response.json();

        if (datos.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted py-10">No se encontraron cargas transitorias en este depósito.</td></tr>`;
            return;
        }

        tbody.innerHTML = '';
        datos.forEach(c => {
            const tr = document.createElement('tr');
            tr.setAttribute('data-id-carga', c.idCarga);

            // Únicamente los 5 datos solicitados
            tr.innerHTML = `
                <td><input type="date" class="form-control form-control-sm txt-fecha w-150px" value="${c.fecha}" /></td>
                <td><input type="number" class="form-control form-control-sm txt-importe w-125px text-end" value="${c.importe}" step="0.01" /></td>
                <td><input type="number" class="form-control form-control-sm txt-litros w-125px text-end" value="${c.litros}" step="0.01" /></td>
                <td><input type="number" class="form-control form-control-sm txt-km-actual w-125px text-end" value="${c.kmActual}" /></td>
                <td><input type="number" class="form-control form-control-sm txt-costo-lt w-125px text-end" value="${c.costoXLt}" step="0.01" disabled /></td>
            `;
            tbody.appendChild(tr);
        });

        // Inicializamos DataTable con la configuración de 5 columnas exactas
        datatable = $(table).DataTable({
            "info": false,
            'order': [],
            "pageLength": 10,
            "lengthChange": false,
            'columnDefs': [
                { orderable: false, targets: [0, 1, 2, 3, 4] } // Deshabilita ordenamiento en los inputs
            ]
        });

        if (txtBuscar) txtBuscar.disabled = false;
        initEventosCalculo();
    };

    // 3. Recalcular Costo x Litro dinámicamente si editan Importe o Litros
    var initEventosCalculo = function () {
        table.querySelectorAll('.txt-importe, .txt-litros').forEach(input => {
            input.addEventListener('input', function () {
                const fila = this.closest('tr');
                const importe = parseFloat(fila.querySelector('.txt-importe').value) || 0;
                const litros = parseFloat(fila.querySelector('.txt-litros').value) || 0;

                const costoLtInput = fila.querySelector('.txt-costo-lt');
                costoLtInput.value = litros > 0 && importe > 0 ? (importe / litros).toFixed(2) : "0.00";
            });
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
        document.getElementById('tbodyCargas').innerHTML = `<tr><td colspan="5" class="text-center text-muted py-10">Seleccione un depósito para consultar las cargas transitorias.</td></tr>`;
        if (txtBuscar) {
            txtBuscar.disabled = true;
            txtBuscar.value = '';
        }
    };

    return {
        init: function () {
            if (!table) return;
            initFiltrosCascada();
            handleSearchDatatable();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTCargasTransitoriasList.init();
});






// "use strict";

// var KTCargasTransitoriasList = function () {
//     var table = document.getElementById('kt_table_cargas_transitorias');
//     var datatable;
//     var btnGuardar = document.getElementById('btnGuardarBase');
//     var txtBuscar = document.getElementById('txtBuscarGrid');

//     //inicializacion filtros cascada
//     var initFiltrosCascada = function () {
//         const cmbRegion = document.getElementById('cmbRegion');
//         const cmbPlanta = document.getElementById('cmbPlanta');
//         const cmbZona = document.getElementById('cmbZona');
//         const cmbDeposito = document.getElementById('cmbDeposito');


//         //de region a plantas
//         cmbRegion.addEventListener('change', async function () {
//             limpiarCombo(cmbPlanta, 'Seleccione Plata...');
//             limpiarCombo(cmbZona, 'Seleccione Zona...');
//             limpiarCombo(cmbDeposito, 'Seleccione Depósito...');
//             destruirGrid();
//             if (!this.value) return;

//             const response = await fetch(`/Combustible/GetPlantas?regionId=${this.value}`);
//             const plantas = await response.json();
//             llenarCombo(cmbPlanta, plantas, 'id', 'nombre'); //misma estructura de RegionModel (Id, Nombre)
//             cmbPlanta.disabled = false;
//         })

//         //de planta a zonas
//         cmbRegion.addEventListener('change', async function () {
//             limpiarCombo(cmbZona, 'Seleccione Zona...');
//             limpiarCombo(cmbDeposito, 'Seleccione Depósito...');
//             destruirGrid();

//             if (!this.value) return;

//             const response = await fetch(`/Combustible/GetZonas?plantaId=${this.value}`);
//             const plantas = await response.json();
//             llenarCombo(cmbPlanta, plantas, 'id', 'nombre'); //misma estructura de RegionModel (Id, Nombre)
//             cmbPlanta.disabled = false;
//         })

//         //de zona a depositos
//         cmbRegion.addEventListener('change', async function () {
//             limpiarCombo(cmbDeposito, 'Seleccione Depósito...');
//             destruirGrid();

//             if (!this.value) return;

//             const response = await fetch(`/Combustible/GetDepositos?zonaId=${this.value}`);
//             const depositos = await response.json();
//             llenarCombo(cmbDeposito, depositos, 'id', 'nombre'); //misma estructura de RegionModel (Id, Nombre)
//             cmbDeposito.disabled = false;
//         });

//         //deposito a carga de renglones desde db
//         cmbDeposito.addEventListener('change', function () {
//             if (this.value) {
//                 cargarDatosGrid(this.value);
//             } else {
//                 destruirGrid();
//             }
//         });
//     };

//     //consulta y pinta el grid
//     var cargarDatosGrid = async function (idDeposito) {
//         destruirGrid();

//         const tbody = document.getElementById('tbodyCargas');
//         tbody.innerHTML = `<tr><td colspan="12" class="text-center py-10"<span-border test-primary"></span> Cargando de la base de Datos...</td></tr>`;

//         const response = await fetch(`/Combustible/GetCargas?idDeposito=${idDeposito}`);
//         const datos = await response.json();

//         if (datos.length === 0) {
//             tbody.innerHTML = `<tr><td colspan="12" class="text-center py-10">No se encontraron cargas transitorias. </td></tr>`;
//             return;
//         }

//         tbody.innerHTML = '';
//         datos.forEach(c => {
//             const tr = document.createElement
//         })


//     }
// }