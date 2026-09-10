"use strict";

//const { Swal } = require("../../../assets/plugins/global/plugins.bundle");

// Class definition
var KTTaller = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    const regionSelect = document.getElementById('selectIdRegion');
    const plantaSelect = document.getElementById('selectIdPlanta');
    const zonaSelect = document.getElementById('selectIdZona');
    const depositoSelect = document.getElementById('selectIdDeposito');

    // Handle form
    var handleValidation = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'NombreTaller': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Taller requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 100,
                                message: 'deben tener entre 7 y 100 caracteres'
                            }
                        }
                    },
                    'IdRegion': {
                        validators: {
                            notEmpty: {
                                message: 'Region requerida'
                            }
                        }
                    },
                    'IdPlanta': {
                        validators: {
                            notEmpty: {
                                message: 'Planta requerida'
                            }
                        }
                    },
                    'IdZona': {
                        validators: {
                            notEmpty: {
                                message: 'Zona requerida'
                            }
                        }
                    },
                    'IdDeposito': {
                        validators: {
                            notEmpty: {
                                message: 'Depósito requerido'
                            }
                        }
                    },
                    'Contacto': {
                        validators: {
                            notEmpty: {
                                message: 'Contacto requerido'
                            }
                        }
                    },
                    'Telefono': {
                        validators: {
                            notEmpty: {
                                message: 'Número telefónico requerido'
                            },
                            regexp: {
                                regexp: /^[0-9]{10}$/,
                                message: 'El número debe tener 10 dígitos'
                            }
                        }
                    },
                    'Email': {
                        validators: {
                            regexp: {
                                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                message: 'No es un correo electr&oacutenico valido',
                            },
                            notEmpty: {
                                message: 'Correo requerido'
                            }
                        }
                    },
                    'Domicilio': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre del Taller requerido'
                            },
                            stringLength: {
                                min: 7,
                                max: 255,
                                message: 'deben tener entre 7 y 50 caracteres',
                            },
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',  // comment to enable invalid state icons
                        eleValidClass: '' // comment to enable valid state icons
                    })
                }
            }
        );
    };

    const clearSelect = (select, defaultMessage) => {
        if (!select) return;
        select.innerHTML = `<option value="">${defaultMessage}</option>`;
        select.disabled = true;
    };

    const initCascada = () => {
        if (regionSelect) {
            regionSelect.addEventListener('change', function () {
                const regionId = this.value;

                clearSelect(plantaSelect, "--Primero selecciona una Region--");
                clearSelect(zonaSelect, "--Primero selecciona una Planta--");
                clearSelect(depositoSelect, "--Primero selecciona una Zona--");

                if (!regionId || regionId === "0") return;

                fetch(`/Taller/GetPlantas?regionId=${regionId}`)
                    .then(response => response.json())
                    .then(data => {

                        plantaSelect.innerHTML = '<option value="">--Selecciona una Planta--</option>';
                        data.forEach(item => {
                            const option = document.createElement('option');
                            option.value = item.id;
                            option.textContent = item.nombre;
                            plantaSelect.appendChild(option);
                        });
                        plantaSelect.disabled = false;
                        if (validator) validator.revalidateField('IdPlanta');
                    })
                    .catch(error => console.error('Error al cargar plantas:', error));
            });
        }
        //Evento planta -> carga zonas por AJAX
        if (plantaSelect) {
            plantaSelect.addEventListener('change', function () {
                const plantaId = this.value;
                clearSelect(zonaSelect, "--Primero selecciona una Planta--");
                clearSelect(depositoSelect, "--Primero selecciona una Zona--");

                if (!plantaId || plantaId === "0") return;

                fetch(`/Taller/GetZonas?plantaId=${plantaId}`)
                    .then(response => response.json())
                    .then(data => {
                        zonaSelect.innerHTML = '<option value="">--Selecciona una Zona--</option>';
                        data.forEach(item => {
                            const option = document.createElement('option');
                            option.value = item.id;
                            option.textContent = item.nombre;
                            zonaSelect.appendChild(option);
                        });
                        zonaSelect.disabled = false;
                        if (validator) validator.revalidateField('IdZona');
                    })
                    .catch(error => console.error('Error al cargar zonas:', error));
            });
        }
        if (zonaSelect) {
            zonaSelect.addEventListener('change', function () {
                const zonaId = this.value;

                clearSelect(depositoSelect, "--Primero selecciona una Zona--");

                if (!zonaId || zonaId === "0") return;
                fetch(`/Taller/GetDepositos?zonaId=${zonaId}`)
                    .then(response => response.json())
                    .then(data => {
                        depositoSelect.innerHTML = '<option value="">--Selecciona un Depósito--</option>';
                        data.forEach(item => {
                            const option = document.createElement('option');
                            option.value = item.id;
                            option.textContent = item.nombre;
                            depositoSelect.appendChild(option);
                        });
                        depositoSelect.disabled = false;
                        if (validator) validator.revalidateField('IdDeposito');
                    })
                    .catch(error => console.error('Error al cargar depósitos:', error));
            });
        }
    };

    var handleSubmitValidation = function () {
        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            // Prevent button default action
            e.preventDefault();

            // Validate form
            validator.validate().then(async function (status) {
                if (status !== 'Valid') {
                    // Disable button to avoid multiple click
                    return;
                }

                //deshabilitar el botón mientras procesa
                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;

                try {
                    const formData = new FormData(form);
                    const response = await fetch(form.action, {
                        method: 'POST',
                        body: formData
                    });

                    const data = await response.json();

                    

                    if (data.success) {
                        await Swal.fire({
                            text: data.message || "Taller guardado correctamente.",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn fw-bold btn-success"//btn fw-bold btn-primary
                            }
                        })

                        window.location.href = '/Taller/Index';

                    } else {
                        Swal.fire({
                            text: result.message || "Ocurrió un error al guardar el taller.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn fw-bold btn-primary"
                            }
                        });
                    }
                } catch (error) {
                    
                    Swal.fire({
                        text: "Error de conexión con el servidor",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Aceptar",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary"
                        }
                    });
                } finally {
                    submitButton.removeAttribute('data-kt-indicator');
                    submitButton.disabled = false;
                }
            });
        });
    };

    const initControls = () => {
        initCascada();
        handleSubmitValidation();
    };

    // Public functions
    return {
        init: function () {
            form = document.querySelector('#kt_taller_in_form');
            submitButton = document.querySelector('#kt_taller_in_submit');

            const phoneInput = document.querySelector("#Telefono");
            if (phoneInput) {
                phoneInput.addEventListener("input", function () {
                    this.value = this.value.replace(/[^0-9]/g, '');
                });
            }

            handleValidation();
            //initData();
            initControls();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTTaller.init();
});