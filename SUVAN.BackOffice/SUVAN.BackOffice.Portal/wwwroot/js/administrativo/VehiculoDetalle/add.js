var KTDetalle = function () {

    const initModalEvent = () => {
        const modalDetalle = document.getElementById('kt_modal_detalle');

        if (!modalDetalle) return;

        const showModalDetalle = new bootstrap.Modal(modalDetalle, {
            keyboard: false,
            backdrop: 'static'
        });

        const iconoTipoVehiculo = document.getElementById('iconoTipoVehiculo');
        const inputTipoVehiculo = document.getElementById('IdTipoVehiculo');
        const inputTipoVehiculoVisible = document.getElementById('IdTipoVehiculoVisible');

        const iconoMarca = document.getElementById('iconoMarca');
        const inputMarca = document.getElementById('IdMarca');
        const inputMarcaVisible = document.getElementById('IdMarcaVisible');

        const iconoModelo = document.getElementById('iconoModelo');
        const inputModelo = document.getElementById('IdModelo');
        const inputModeloVisible = document.getElementById('IdModeloVisible');

        const iconoZona = document.getElementById('iconoZona');
        const inputZona = document.getElementById('IdZona');
        const inputZonaVisible = document.getElementById('IdZonaVisible');

        const iconoDeposito = document.getElementById('iconoDeposito');
        const inputDeposito = document.getElementById('IdDeposito');
        const inputDepositoVisible = document.getElementById('IdDepositoVisible');

        const iconoEje = document.getElementById('iconoEje');
        const inputEje = document.getElementById('IdTipoEje');
        const inputEjeVisible = document.getElementById('IdTipoEjeVisible');

        const iconoBaja = document.getElementById('iconoBaja');
        const inputBaja = document.getElementById('IdBaja');
        const inputBajaVisible = document.getElementById('IdBajaVisible');

        const iconoCausa = document.getElementById('iconoCausaBaja');
        const inputCausa = document.getElementById('IdCausaBaja');
        const inputCausaVisible = document.getElementById('IdCausaBajaVisible');

        const iconoVehiculo = document.getElementById('iconoVehiculo');
        const inputVehiculo = document.getElementById('IdVehiculo');
        const inputVIN = document.getElementById('Carroceria');
        const inputVINVisile = document.getElementById('CarroceriaVisible');
        const inputMotor = document.getElementById('NumeroMotor');
        const inputMotorVisible = document.getElementById('NumeroMotorVisible');
        const inputEconomico = document.getElementById('EconomicoAnterior');
        const inputEconomicoVisible = document.getElementById('NumeroEconomicoVisible');
        const inputVehiculoVisible = document.getElementById('IdVehiculoVisible');

        const tabla = $('#kt_tabla_detalle');

        const configurarTabla = (data, columnas, onRowClick) => {
            if ($.fn.DataTable.isDataTable(tabla)) {
                tabla.DataTable().destroy();
                tabla.empty();
            }

            tabla.DataTable({
                data: data,
                columns: columnas,
                pageLength: 10,
                lengthChange: false,
                searching: false,
                ordering: false,
                info: false,
                createdRow: function (row, data) {
                    $(row).addClass('text-gray-600 fw-semibold');
                },
                headerCallback: function (thead) {
                    $(thead).find('th').addClass('text-center text-muted fw-bold fs-7 text-uppercase gs-0');
                }
            });

            tabla.find('tbody').off('click').on('click', 'tr', function () {
                const rowData = tabla.DataTable().row(this).data();
                onRowClick(rowData);
                showModalDetalle.hide();
            });
        };

        if (iconoVehiculo) {
            iconoVehiculo.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Vehiculos';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerVehiculo",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Vehiculo", data: "idVehiculo", className: 'min-w-125px text-center' },
                            { title: "Placas", data: "placas", className: 'min-w-125px text-center' },
                            { title: "vin", data: "vin", className: 'min-w-125px text-center' },
                            { title: "Número de Motor", data: "numeromotor", className: 'min-w-125px text-center' },
                            { title: "Número Económico", data: "numeroeconomico", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputVehiculo.value = rowData.idVehiculo;
                            inputVehiculoVisible.value = rowData.idVehiculo + ' - ' + rowData.placas;
                            inputVIN.value = rowData.vin;
                            inputVINVisile.value = rowData.vin;
                            inputEconomico.value = rowData.numeroeconomico;
                            inputMotor.value = rowData.numeromotor;
                            inputMotorVisible.value = rowData.numeromotor;
                            inputEconomicoVisible.value = rowData.numeroeconomico

                            if (window.validator) {
                                window.validator.revalidateField('idVehiculo');
                                window.validator.revalidateField('Carroceria');
                                window.validator.revalidateField('NumeroMotor');
                                window.validator.revalidateField('EconomicoAnterior');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener los Tipos de Vehículos.');
                    }
                });
            });
        }

        if (iconoTipoVehiculo) {
            iconoTipoVehiculo.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Tipos de Vehículo';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerTipoVehiculo",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Tipo de Vehículo", data: "tipoUnidadId", className: 'min-w-125px text-center' },
                            { title: "Nombre", data: "nombre", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputTipoVehiculo.value = rowData.tipoUnidadId;
                            inputTipoVehiculoVisible.value = rowData.nombre;

                            if (window.validator) {
                                window.validator.revalidateField('IdTipoVehiculo');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener los Tipos de Vehículos.');
                    }
                });
            });
        }

        if (iconoMarca) {
            iconoMarca.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Marcas';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerMarca",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Marca", data: "idMarca", className: 'min-w-125px text-center' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputMarca.value = rowData.idMarca;
                            inputMarcaVisible.value = rowData.descripcion;

                            inputModelo.value = '';

                            if (inputModeloVisible) {
                                inputModeloVisible.value = '';
                            }

                            if (window.validator) {
                                window.validator.revalidateField('IdMarca');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener las Marcas.');
                    }
                });
            });
        }

        if (iconoModelo) {
            iconoModelo.addEventListener('click', function () {
                const idMarca = inputMarca.value;

                document.getElementById('modalTitulo').textContent = 'Modelos';

                $.ajax({
                    url: `/VehiculoDetalle/ObtenerModelo?idMarca=${idMarca}`,
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Modelo", data: "idModelo", className: 'min-w-125px text-center' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputModelo.value = rowData.idModelo;
                            inputModeloVisible.value = rowData.descripcion;

                            if (window.validator) {
                                window.validator.revalidateField('IdModelo');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener los Tipos de Eje.');
                    }
                });
            });
        }

        if (iconoEje) {
            iconoEje.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Tipos de Eje';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerTipoEje",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Eje", data: "idTipoEje", className: 'min-w-125px text-center' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputEje.value = rowData.idTipoEje;
                            inputEjeVisible.value = rowData.descripcion;

                            if (window.validator) {
                                window.validator.revalidateField('IdTipoEje');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener los Tipos de Eje.');
                    }
                });
            });
        }

        if (iconoBaja) {
            iconoBaja.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Baja Vehículo';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerBajaVehi",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Baja", data: "idBaja", className: 'min-w-125px text-center' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputBaja.value = rowData.idBaja;
                            inputBajaVisible.value = rowData.descripcion;

                            if (window.validator) {
                                window.validator.revalidateField('IdBaja');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener la Baja');
                    }
                });
            });
        }

        if (iconoCausa) {
            iconoCausa.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Causa Baja';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerCausaBaja",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Causa", data: "idCausaBaja", className: 'min-w-125px text-center' },
                            { title: "Descripción", data: "descripcion", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputCausa.value = rowData.idCausaBaja;
                            inputCausaVisible.value = rowData.descripcion;

                            if (window.validator) {
                                window.validator.revalidateField('IdCausaBaja');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener la Causa Baja');
                    }
                });
            });
        }

        if (iconoZona) {
            iconoZona.addEventListener('click', function () {
                document.getElementById('modalTitulo').textContent = 'Zonas';

                $.ajax({
                    url: "/VehiculoDetalle/ObtenerZona",
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Zona", data: "zonaId", className: 'min-w-125px text-center' },
                            { title: "Nombre", data: "zonaNombre", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputZona.value = rowData.zonaId;
                            inputZonaVisible.value = rowData.zonaNombre;

                            inputDeposito.value = '';

                            if (inputDepositoVisible) {
                                inputDepositoVisible.value = '';
                            }

                            if (window.validator) {
                                window.validator.revalidateField('IdZona');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener las Zonas.');
                    }
                });
            });
        }

        if (iconoDeposito) {
            iconoDeposito.addEventListener('click', function () {
                const idZona = inputZona.value;

                document.getElementById('modalTitulo').textContent = 'Depósitos';

                $.ajax({
                    url: `/VehiculoDetalle/ObtenerDeposito?idZona=${idZona}`,
                    type: 'GET',
                    success: function (data) {
                        showModalDetalle.show();
                        configurarTabla(data, [
                            { title: "Número de Depósito", data: "depositoId", className: 'min-w-125px text-center' },
                            { title: "Nombre", data: "nombreDeposito", className: 'min-w-125px text-center' }
                        ], function (rowData) {
                            inputDeposito.value = rowData.depositoId;
                            inputDepositoVisible.value = rowData.nombreDeposito;

                            if (window.validator) {
                                window.validator.revalidateField('IdDeposito');
                            }
                        });
                    },
                    error: function () {
                        alert('Error al obtener los Tipos de Eje.');
                    }
                });
            });
        }
    }

    return {
        init: function () {
            initModalEvent();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTDetalle.init();
});
