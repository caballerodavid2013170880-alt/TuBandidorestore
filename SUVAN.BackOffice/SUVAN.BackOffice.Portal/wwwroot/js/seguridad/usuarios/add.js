"use strict";

// Class definition
var KTUsuario = function () {
    // Elements
    var form;
    var submitButton;
    var validator;
    let empresasList = [];
    let jerarquiasList = [];

    const empresasUsaurioInput = document.getElementById('EmpresasUsuario');
    const jerarquiasUsuarioInput = document.getElementById('JerarquiasUsuario');
    const adminIdInput = document.getElementById('AdminId');

    // Selectores jerárquicos
    let selRegion, selPlanta, selZona, selDeposito, selDepto;

    const initHierarchyElements = () => {
        selRegion = document.getElementById("selectIdRegion");
        selPlanta = document.getElementById("selectIdPlanta");
        selZona = document.getElementById("selectIdZona");
        selDeposito = document.getElementById("selectIdDeposito");
        selDepto = document.getElementById("selectIdDepto");
    }

    // Obtenemos el ID de la empresa marcada como principal
    const getPrincipalEmpresaId = () => {
        const principalItem = empresasList.find(i => i.esPrincipal);
        if (principalItem) {
            return principalItem.empresaId;
        }
        if (empresasList.length > 0) {
            return empresasList[0].empresaId;
        }
        return 0;
    }

    // Carga la jerarquía completa para la empresa principal seleccionada
    const cargarJerarquiaEmpresaPrincipal = (empresaId) => {
        initHierarchyElements();
        if (!selRegion) return;

        resetSelect(selRegion, "-- Seleccione una Región --");
        resetSelect(selPlanta, "-- Primero seleccione una Región --");
        resetSelect(selZona, "-- Primero seleccione una Planta --");
        resetSelect(selDeposito, "-- Primero seleccione una Zona --");
        resetSelect(selDepto, "-- Primero seleccione un Depósito --");

        if (!empresaId || empresaId === 0) {
            resetSelect(selRegion, "-- Primero seleccione una Empresa Principal --");
            jerarquiasList = [];
            if (jerarquiasUsuarioInput) {
                jerarquiasUsuarioInput.value = "[]";
            }
            renderJerarquiasGrid();
            return;
        }

        const adminId = adminIdInput ? parseInt(adminIdInput.value) || 0 : 0;

        // 1. Cargar Regiones para la empresa seleccionada
        fetch(`/Usuarios/GetRegionesPorEmpresa?idEmpresa=${empresaId}`)
            .then(res => res.json())
            .then(regiones => {
                fillSelect(selRegion, regiones, "-- Seleccione una Región --");
                selRegion.disabled = false;

                // 2. Si existe un usuario en edición, buscar las jerarquías registradas para esta empresa
                if (adminId > 0) {
                    fetch(`/Usuarios/GetJerarquiasUsuario?adminId=${adminId}&idEmpresa=${empresaId}`)
                        .then(res => res.json())
                        .then(resJerarquia => {
                            if (resJerarquia.success && resJerarquia.items && resJerarquia.items.length > 0) {
                                jerarquiasList = resJerarquia.items;
                                if (jerarquiasUsuarioInput) {
                                    jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
                                }
                                renderJerarquiasGrid();

                                const principal = jerarquiasList.find(j => j.esPrincipal) || jerarquiasList[0];
                                if (principal && principal.regionId) {
                                    selRegion.value = principal.regionId;
                                    cargarCascadaPlanta(empresaId, principal.regionId, principal.plantaId, principal.zonaId, principal.depositoId, principal.deptoId);
                                }
                            } else {
                                jerarquiasList = [];
                                if (jerarquiasUsuarioInput) {
                                    jerarquiasUsuarioInput.value = "[]";
                                }
                                renderJerarquiasGrid();
                            }
                        });
                }
            });
    }

    // Carga una asignación jerárquica específica en los selectores limpiándolos previamente
    const cargarJerarquiaEnSelectores = (empresaId, item) => {
        initHierarchyElements();
        if (!selRegion) return;

        resetSelect(selRegion, "-- Seleccione una Región --");
        resetSelect(selPlanta, "-- Primero seleccione una Región --");
        resetSelect(selZona, "-- Primero seleccione una Planta --");
        resetSelect(selDeposito, "-- Primero seleccione una Zona --");
        resetSelect(selDepto, "-- Primero seleccione un Depósito --");

        if (!empresaId || !item) return;

        fetch(`/Usuarios/GetRegionesPorEmpresa?idEmpresa=${empresaId}`)
            .then(res => res.json())
            .then(regiones => {
                fillSelect(selRegion, regiones, "-- Seleccione una Región --");
                selRegion.disabled = false;

                if (item.regionId) {
                    selRegion.value = item.regionId;
                    cargarCascadaPlanta(empresaId, item.regionId, item.plantaId, item.zonaId, item.depositoId, item.deptoId);
                }
            });
    }

    const cargarCascadaPlanta = (empresaId, idRegion, targetPlantaId, targetZonaId, targetDepositoId, targetDeptoId) => {
        if (!idRegion) return;
        fetch(`/Usuarios/GetPlantasPorRegion?idEmpresa=${empresaId}&idRegion=${idRegion}`)
            .then(res => res.json())
            .then(plantas => {
                fillSelect(selPlanta, plantas, "-- Seleccione una Planta --");
                selPlanta.disabled = false;
                if (targetPlantaId) {
                    selPlanta.value = targetPlantaId;
                    cargarCascadaZona(empresaId, idRegion, targetPlantaId, targetZonaId, targetDepositoId, targetDeptoId);
                }
            });
    }

    const cargarCascadaZona = (empresaId, idRegion, idPlanta, targetZonaId, targetDepositoId, targetDeptoId) => {
        if (!idPlanta) return;
        fetch(`/Usuarios/GetZonasPorPlanta?idEmpresa=${empresaId}&idRegion=${idRegion}&idPlanta=${idPlanta}`)
            .then(res => res.json())
            .then(zonas => {
                fillSelect(selZona, zonas, "-- Seleccione una Zona --");
                selZona.disabled = false;
                if (targetZonaId) {
                    selZona.value = targetZonaId;
                    cargarCascadaDeposito(empresaId, idRegion, idPlanta, targetZonaId, targetDepositoId, targetDeptoId);
                }
            });
    }

    const cargarCascadaDeposito = (empresaId, idRegion, idPlanta, idZona, targetDepositoId, targetDeptoId) => {
        if (!idZona) return;
        fetch(`/Usuarios/GetDepositosPorZona?idEmpresa=${empresaId}&idRegion=${idRegion}&idPlanta=${idPlanta}&idZona=${idZona}`)
            .then(res => res.json())
            .then(depositos => {
                fillSelect(selDeposito, depositos, "-- Seleccione un Depósito --");
                selDeposito.disabled = false;
                if (targetDepositoId) {
                    selDeposito.value = targetDepositoId;
                    cargarCascadaDepto(empresaId, idRegion, idPlanta, idZona, targetDepositoId, targetDeptoId);
                }
            });
    }

    const cargarCascadaDepto = (empresaId, idRegion, idPlanta, idZona, idDeposito, targetDeptoId) => {
        if (!idDeposito) return;
        fetch(`/Usuarios/GetDeptosPorDeposito?idEmpresa=${empresaId}&idRegion=${idRegion}&idPlanta=${idPlanta}&idZona=${idZona}&idDeposito=${idDeposito}`)
            .then(res => res.json())
            .then(deptos => {
                fillSelect(selDepto, deptos, "-- Seleccione un Departamento --");
                selDepto.disabled = false;
                if (targetDeptoId) {
                    selDepto.value = targetDeptoId;
                }
            });
    }

    // Handle form
    var handleValidation = function (e) {
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'Nombre': {
                        validators: {
                            notEmpty: {
                                message: 'Nombre de usuario requerido'
                            },
                            stringLength: {
                                min: 4,
                                max: 250,
                                message: 'Deben tener entre 4 y 250 caracteres',
                            },
                        }
                    },
                    'Email': {
                        validators: {
                            regexp: {
                                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                message: 'No es un correo electrónico valido',
                            },
                            notEmpty: {
                                message: 'Correo electrónico es requerido'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
    }

    const syncCurrentSelectorsToJerarquias = () => {
        initHierarchyElements();
        if (!selDeposito) return;

        const regId = selRegion ? parseInt(selRegion.value) || null : null;
        const planId = selPlanta ? parseInt(selPlanta.value) || null : null;
        const zonId = selZona ? parseInt(selZona.value) || null : null;
        const depId = parseInt(selDeposito.value) || null;
        const deptId = selDepto ? parseInt(selDepto.value) || null : null;
        const deptNombre = (selDepto && selDepto.selectedIndex >= 0 && deptId) ? selDepto.options[selDepto.selectedIndex].text : "";

        if (depId && depId > 0) {
            let target = jerarquiasList.find(j => j.depositoId === depId);
            if (!target) {
                target = jerarquiasList.find(j => j.esPrincipal) || (jerarquiasList.length > 0 ? jerarquiasList[0] : null);
            }

            if (target) {
                target.regionId = regId || target.regionId;
                if (selRegion && selRegion.selectedIndex >= 0 && regId) target.regionNombre = selRegion.options[selRegion.selectedIndex].text;
                target.plantaId = planId || target.plantaId;
                if (selPlanta && selPlanta.selectedIndex >= 0 && planId) target.plantaNombre = selPlanta.options[selPlanta.selectedIndex].text;
                target.zonaId = zonId || target.zonaId;
                if (selZona && selZona.selectedIndex >= 0 && zonId) target.zonaNombre = selZona.options[selZona.selectedIndex].text;
                target.depositoId = depId;
                if (selDeposito && selDeposito.selectedIndex >= 0) target.depositoNombre = selDeposito.options[selDeposito.selectedIndex].text;
                target.deptoId = deptId;
                target.deptoNombre = deptNombre;
            } else {
                const empId = getPrincipalEmpresaId();
                jerarquiasList.push({
                    idUsuarioJerarquia: 0,
                    empresaId: empId,
                    regionId: regId,
                    regionNombre: (selRegion && selRegion.selectedIndex >= 0) ? selRegion.options[selRegion.selectedIndex].text : "",
                    plantaId: planId,
                    plantaNombre: (selPlanta && selPlanta.selectedIndex >= 0) ? selPlanta.options[selPlanta.selectedIndex].text : "",
                    zonaId: zonId,
                    zonaNombre: (selZona && selZona.selectedIndex >= 0) ? selZona.options[selZona.selectedIndex].text : "",
                    depositoId: depId,
                    depositoNombre: (selDeposito && selDeposito.selectedIndex >= 0) ? selDeposito.options[selDeposito.selectedIndex].text : "",
                    deptoId: deptId,
                    deptoNombre: deptNombre,
                    esPrincipal: true
                });
            }
        }

        // Asegurar que exactamente una jerarquía tenga esPrincipal = true
        if (jerarquiasList.length > 0) {
            const hasPrincipal = jerarquiasList.some(j => j.esPrincipal);
            if (!hasPrincipal) {
                jerarquiasList[0].esPrincipal = true;
            }
        }

        if (jerarquiasUsuarioInput) {
            jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
        }
    };


    var handleSubmitValidation = function (e) {
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();

            syncCurrentSelectorsToJerarquias();

            if (empresasList !== null && empresasList.length > 0) {
                empresasUsaurioInput.value = JSON.stringify(empresasList);
            }

            if (jerarquiasList !== null && jerarquiasList.length > 0) {
                if (jerarquiasUsuarioInput) {
                    jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
                }
            }

            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    submitButton.setAttribute('data-kt-indicator', 'on');
                    submitButton.disabled = true;


            // Habilitar temporalmente los desplegables jerarquia para asegurar su envío en el POST
            if (selRegion) selRegion.disabled = false;
            if (selPlanta) selPlanta.disabled = false;
            if (selZona) selZona.disabled = false;
            if (selDeposito) selDeposito.disabled = false;
            if (selDepto) selDepto.disabled = false;


                    form.submit();
                }
            });
        });
    }

    const handleControls = () => {
        const agregarEmpresaButton = document.getElementById("agregar-empresa");
        if (agregarEmpresaButton) {
            agregarEmpresaButton.addEventListener("click", agregarEmpresa);
        }

        const btnAgregarJerarquia = document.getElementById("btn-agregar-jerarquia");
        if (btnAgregarJerarquia) {
            btnAgregarJerarquia.addEventListener("click", agregarJerarquia);
        }

        initHierarchyElements();

        // Eventos manuales de cambio en los desplegables de jerarquía
        if (selRegion) {
            selRegion.addEventListener("change", function () {
                const idEmpresa = getPrincipalEmpresaId();
                const idRegion = this.value;
                resetSelect(selPlanta, "-- Seleccione una Planta --");
                resetSelect(selZona, "-- Primero seleccione una Planta --");
                resetSelect(selDeposito, "-- Primero seleccione una Zona --");
                resetSelect(selDepto, "-- Primero seleccione un Depósito --");

                if (idEmpresa > 0 && idRegion && idRegion !== "0") {
                    fetch(`/Usuarios/GetPlantasPorRegion?idEmpresa=${idEmpresa}&idRegion=${idRegion}`)
                        .then(res => res.json())
                        .then(data => {
                            fillSelect(selPlanta, data, "-- Seleccione una Planta --");
                            selPlanta.disabled = false;
                        });
                }
            });
        }

        if (selPlanta) {
            selPlanta.addEventListener("change", function () {
                const idEmpresa = getPrincipalEmpresaId();
                const idRegion = selRegion ? selRegion.value : 0;
                const idPlanta = this.value;
                resetSelect(selZona, "-- Seleccione una Zona --");
                resetSelect(selDeposito, "-- Primero seleccione una Zona --");
                resetSelect(selDepto, "-- Primero seleccione un Depósito --");

                if (idEmpresa > 0 && idPlanta && idPlanta !== "0") {
                    fetch(`/Usuarios/GetZonasPorPlanta?idEmpresa=${idEmpresa}&idRegion=${idRegion}&idPlanta=${idPlanta}`)
                        .then(res => res.json())
                        .then(data => {
                            fillSelect(selZona, data, "-- Seleccione una Zona --");
                            selZona.disabled = false;
                        });
                }
            });
        }

        if (selZona) {
            selZona.addEventListener("change", function () {
                const idEmpresa = getPrincipalEmpresaId();
                const idRegion = selRegion ? selRegion.value : 0;
                const idPlanta = selPlanta ? selPlanta.value : 0;
                const idZona = this.value;
                resetSelect(selDeposito, "-- Seleccione un Depósito --");
                resetSelect(selDepto, "-- Primero seleccione un Depósito --");

                if (idEmpresa > 0 && idZona && idZona !== "0") {
                    fetch(`/Usuarios/GetDepositosPorZona?idEmpresa=${idEmpresa}&idRegion=${idRegion}&idPlanta=${idPlanta}&idZona=${idZona}`)
                        .then(res => res.json())
                        .then(data => {
                            fillSelect(selDeposito, data, "-- Seleccione un Depósito --");
                            selDeposito.disabled = false;
                        });
                }
            });
        }

        if (selDeposito) {
            selDeposito.addEventListener("change", function () {
                const idEmpresa = getPrincipalEmpresaId();
                const idRegion = selRegion ? selRegion.value : 0;
                const idPlanta = selPlanta ? selPlanta.value : 0;
                const idZona = selZona ? selZona.value : 0;
                const idDeposito = parseInt(this.value) || 0;
                resetSelect(selDepto, "-- Seleccione un Departamento --");

                if (idEmpresa > 0 && idDeposito && idDeposito !== "0") {
                    fetch(`/Usuarios/GetDeptosPorDeposito?idEmpresa=${idEmpresa}&idRegion=${idRegion}&idPlanta=${idPlanta}&idZona=${idZona}&idDeposito=${idDeposito}`)
                        .then(res => res.json())
                        .then(data => {
                            fillSelect(selDepto, data, "-- Seleccione un Departamento --");
                            selDepto.disabled = false;
                        });
                }
            });
        }

        if (selDepto) {
            selDepto.addEventListener("change", function () {
                const deptId = parseInt(this.value) || null;
                const deptNombre = (this.selectedIndex >= 0 && deptId) ? this.options[this.selectedIndex].text : "";

                // Sincronizar de inmediato con el elemento activo en jerarquiasList
                let targetItem = null;
                const curDepId = selDeposito ? parseInt(selDeposito.value) || 0 : 0;
                if (curDepId > 0) {
                    targetItem = jerarquiasList.find(j => j.depositoId === curDepId);
                }
                if (!targetItem) {
                    targetItem = jerarquiasList.find(j => j.esPrincipal) || (jerarquiasList.length > 0 ? jerarquiasList[0] : null);
                }

                if (targetItem) {
                    targetItem.deptoId = deptId;
                    targetItem.deptoNombre = deptNombre;
                    if (jerarquiasUsuarioInput) {
                        jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
                    }
                    renderJerarquiasGrid();
                }
            });
        }
    }

    const initEmpesasGrid = () => {
        const empresasUsaurio = empresasUsaurioInput ? empresasUsaurioInput.value : "";
        if (empresasUsaurio !== null && empresasUsaurio !== "" && empresasUsaurio.length > 0) {
            empresasList = JSON.parse(empresasUsaurio);
        }

        const jerarquiasUsaurioVal = jerarquiasUsuarioInput ? jerarquiasUsuarioInput.value : "";
        if (jerarquiasUsaurioVal !== null && jerarquiasUsaurioVal !== "" && jerarquiasUsaurioVal.length > 0) {
            jerarquiasList = JSON.parse(jerarquiasUsaurioVal);
        }

        rednerEmpresasGrid();
        renderJerarquiasGrid();

        // Cargar la jerarquía de la empresa principal activa al iniciar
        const principalEmpresaId = getPrincipalEmpresaId();
        if (principalEmpresaId > 0) {
            cargarJerarquiaEmpresaPrincipal(principalEmpresaId);
        }
    }

    const rednerEmpresasGrid = () => {
        const appElement = document.getElementById("empresasContainer");
        if (!appElement) return;
        appElement.innerHTML = "";
        empresasList.forEach(item => {
            const itemDiv = document.createElement("div");
            itemDiv.classList.add("d-flex", "align-items-center", "border", "border-dashed", "border-gray-300", "rounded", "px-7", "py-3", "mb-5");

            const empresaNombre = document.createElement("span");
            empresaNombre.classList.add("fs-5", "text-dark", "text-hover-primary", "fw-semibold", "w-375px", "min-w-200px")
            empresaNombre.textContent = `Empresa: ${item.empresaNombre}`;

            const perfilNombre = document.createElement("span");
            perfilNombre.classList.add("badge", "badge-light-primary", "me-6");
            perfilNombre.textContent = `${item.perfilNombre}`;

            const deleteButton = document.createElement("a");
            deleteButton.classList.add("btn", "btn-icon", "btn-active-light-primary", "w-30px", "h-30px");
            deleteButton.innerHTML = `<i class="ki-outline ki-trash fs-3"></i>`;
            deleteButton.addEventListener("click", () => {
                eliminarItem(item);
            });

            const itemDivCheck = document.createElement("div");
            itemDivCheck.classList.add("form-check", "form-check-solid", "form-switch", "form-check-custom", "fv-row", "me-6");

            const radioInput = document.createElement("input");
            radioInput.classList.add("form-check-input", "w-45px", "h-30px");
            radioInput.type = "radio";
            radioInput.name = "principalRadio";
            radioInput.checked = item.esPrincipal;
            radioInput.addEventListener("change", () => {
                actualizarPrincipal(item);
            });

            itemDivCheck.appendChild(radioInput);

            itemDiv.appendChild(empresaNombre);
            itemDiv.appendChild(perfilNombre);
            itemDiv.appendChild(itemDivCheck);
            itemDiv.appendChild(deleteButton);

            appElement.appendChild(itemDiv);
        });
    }

    function agregarEmpresa() {
        const empresaSelect = document.getElementById("EmpresaId");
        const perfilSelect = document.getElementById("PerfilId");

        const selectedEmpresaId = parseInt(empresaSelect.value);
        const selectedPerfilId = parseInt(perfilSelect.value);

        if (!selectedEmpresaId || !selectedPerfilId) {
            Swal.fire({
                text: "Debe seleccionar una empresa y un perfil",
                icon: "warning",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary",
                }
            });
            return;
        }

        const existingItem = empresasList.find(item => item.empresaId === selectedEmpresaId);
        if (existingItem) {
            Swal.fire({
                text: "La empresa ya existe en la lista",
                icon: "warning",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary",
                }
            });
            return;
        }

        const esLaPrimera = empresasList.length === 0;
        const newItem = {
            empresaId: selectedEmpresaId,
            empresaNombre: empresaSelect.options[empresaSelect.selectedIndex].text,
            perfilId: selectedPerfilId,
            perfilNombre: perfilSelect.options[perfilSelect.selectedIndex].text,
            esPrincipal: esLaPrimera
        };

        empresasList.push(newItem);
        rednerEmpresasGrid();
        empresaSelect.value = "";
        perfilSelect.value = "";

        if (esLaPrimera) {
            cargarJerarquiaEmpresaPrincipal(selectedEmpresaId);
        }
    }

    const eliminarItem = (item) => {
        const index = empresasList.indexOf(item);
        if (index !== -1) {
            const eraPrincipal = item.esPrincipal;
            empresasList.splice(index, 1);
            if (eraPrincipal && empresasList.length > 0) {
                empresasList[0].esPrincipal = true;
            }
            rednerEmpresasGrid();
            const nuevoPrincipalId = getPrincipalEmpresaId();
            cargarJerarquiaEmpresaPrincipal(nuevoPrincipalId);
        }
    }

    const actualizarPrincipal = (item) => {
        empresasList.forEach(i => {
            i.esPrincipal = false;
        });
        item.esPrincipal = true;
        rednerEmpresasGrid();

        // Al modificar el radio button principal (updatePrincipal):
        // Se limpian los selectores y se carga la jerarquía correspondiente a la empresa seleccionada
        cargarJerarquiaEmpresaPrincipal(item.empresaId);
    }

    // =========================================================================
    // GESTIÓN DE MÚLTIPLES DEPÓSITOS/ALMACENES EN LA JERARQUÍA
    // =========================================================================

    function agregarJerarquia() {
        initHierarchyElements();
        const empId = getPrincipalEmpresaId();
        if (!empId || empId === 0) {
            Swal.fire({
                text: "Debe asignar y seleccionar una Empresa Principal primero",
                icon: "warning",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: { confirmButton: "btn fw-bold btn-primary" }
            });
            return;
        }

        const regId = parseInt(selRegion.value);
        const planId = parseInt(selPlanta.value);
        const zonId = parseInt(selZona.value);
        const depId = parseInt(selDeposito.value);
        const deptId = parseInt(selDepto.value);

        if (!regId || !planId || !zonId || !depId) {
            Swal.fire({
                text: "Debe seleccionar al menos Región, Planta, Zona y Depósito para agregar a la jerarquía",
                icon: "warning",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: { confirmButton: "btn fw-bold btn-primary" }
            });
            return;
        }

        // Verificar si ya existe este depósito asignado en la lista (o combinación deposito + depto)
        const existeDep = jerarquiasList.find(j => j.depositoId === depId && (j.deptoId === (deptId || null) || !deptId));
        if (existeDep) {
            Swal.fire({
                text: "Esta combinación de depósito / jerarquía ya se encuentra agregada en la lista del usuario",
                icon: "warning",
                buttonsStyling: false,
                confirmButtonText: "Ok",
                customClass: { confirmButton: "btn fw-bold btn-primary" }
            });
            return;
        }

        const newItem = {
            idUsuarioJerarquia: 0,
            empresaId: empId,
            regionId: regId,
            regionNombre: selRegion.options[selRegion.selectedIndex].text,
            plantaId: planId,
            plantaNombre: selPlanta.options[selPlanta.selectedIndex].text,
            zonaId: zonId,
            zonaNombre: selZona.options[selZona.selectedIndex].text,
            depositoId: depId,
            depositoNombre: selDeposito.options[selDeposito.selectedIndex].text,
            deptoId: deptId || null,
            deptoNombre: (deptId && selDepto.selectedIndex >= 0) ? selDepto.options[selDepto.selectedIndex].text : "",
            esPrincipal: jerarquiasList.length === 0
        };

        jerarquiasList.push(newItem);
        if (jerarquiasUsuarioInput) {
            jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
        }
        renderJerarquiasGrid();
    }

    const renderJerarquiasGrid = () => {
        const container = document.getElementById("jerarquiasContainer");
        if (!container) return;
        container.innerHTML = "";

        if (jerarquiasList.length === 0) {
            container.innerHTML = `<div class="text-muted fs-7 italic">No hay depósitos asignados a la jerarquía aún.</div>`;
            return;
        }

        jerarquiasList.forEach(item => {
            const itemDiv = document.createElement("div");
            itemDiv.classList.add("d-flex", "align-items-center", "justify-content-between", "border", "border-gray-300", "rounded", "px-5", "py-3", "mb-3", "bg-white");

            const infoDiv = document.createElement("div");
            infoDiv.classList.add("d-flex", "flex-column", "flex-grow-1");
            infoDiv.style.cursor = "pointer";
            infoDiv.title = "Haga clic para seleccionar y cargar esta jerarquía en los selectores";
            infoDiv.addEventListener("click", () => {
                actualizarJerarquiaPrincipal(item);
            });

            const mainText = document.createElement("span");
            mainText.classList.add("fs-6", "fw-bold", "text-gray-800");
            mainText.textContent = `Depósito: ${item.depositoNombre}`;

            const pathText = document.createElement("span");
            pathText.classList.add("fs-7", "text-gray-600");
            let path = `${item.regionNombre} > ${item.plantaNombre} > ${item.zonaNombre}`;
            if (item.deptoNombre) path += ` > Depto: ${item.deptoNombre}`;
            pathText.textContent = path;

            infoDiv.appendChild(mainText);
            infoDiv.appendChild(pathText);

            const rightDiv = document.createElement("div");
            rightDiv.classList.add("d-flex", "align-items-center", "ms-3");

            const itemDivCheck = document.createElement("div");
            itemDivCheck.classList.add("form-check", "form-check-solid", "form-switch", "form-check-custom", "me-4");

            const radioInput = document.createElement("input");
            radioInput.classList.add("form-check-input", "w-45px", "h-25px");
            radioInput.type = "radio";
            radioInput.name = "principalJerarquiaRadio";
            radioInput.checked = item.esPrincipal;
            radioInput.title = "Marcar como Depósito Principal y cargar en selectores";
            radioInput.addEventListener("change", () => {
                actualizarJerarquiaPrincipal(item);
            });

            const labelTag = document.createElement("label");
            labelTag.classList.add("form-check-label", "fs-8", "text-muted", "ms-2");
            labelTag.textContent = item.esPrincipal ? "Principal" : "Secundario";

            itemDivCheck.appendChild(radioInput);
            itemDivCheck.appendChild(labelTag);

            const deleteButton = document.createElement("a");
            deleteButton.classList.add("btn", "btn-icon", "btn-active-light-danger", "w-30px", "h-30px");
            deleteButton.innerHTML = `<i class="ki-outline ki-trash fs-4"></i>`;
            deleteButton.addEventListener("click", (e) => {
                e.stopPropagation();
                eliminarJerarquia(item);
            });

            rightDiv.appendChild(itemDivCheck);
            rightDiv.appendChild(deleteButton);

            itemDiv.appendChild(infoDiv);
            itemDiv.appendChild(rightDiv);

            container.appendChild(itemDiv);
        });
    }

    const eliminarJerarquia = (item) => {
        const index = jerarquiasList.indexOf(item);
        if (index !== -1) {
            const eraPrincipal = item.esPrincipal;
            jerarquiasList.splice(index, 1);
            if (jerarquiasUsuarioInput) {
                jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
            }
            if (eraPrincipal && jerarquiasList.length > 0) {
                jerarquiasList[0].esPrincipal = true;
                actualizarJerarquiaPrincipal(jerarquiasList[0]);
            } else if (jerarquiasList.length === 0) {
                initHierarchyElements();
                resetSelect(selRegion, "-- Seleccione una Región --");
                resetSelect(selPlanta, "-- Primero seleccione una Región --");
                resetSelect(selZona, "-- Primero seleccione una Planta --");
                resetSelect(selDeposito, "-- Primero seleccione una Zona --");
                resetSelect(selDepto, "-- Primero seleccione un Depósito --");
                const empresaId = getPrincipalEmpresaId();
                if (empresaId > 0 && selRegion) {
                    fetch(`/Usuarios/GetRegionesPorEmpresa?idEmpresa=${empresaId}`)
                        .then(res => res.json())
                        .then(regiones => {
                            fillSelect(selRegion, regiones, "-- Seleccione una Región --");
                            selRegion.disabled = false;
                        });
                }
                renderJerarquiasGrid();
            } else {
                renderJerarquiasGrid();
            }
        }
    }

    const actualizarJerarquiaPrincipal = (item) => {
        jerarquiasList.forEach(j => {
            j.esPrincipal = false;
        });
        item.esPrincipal = true;
        if (jerarquiasUsuarioInput) {
            jerarquiasUsuarioInput.value = JSON.stringify(jerarquiasList);
        }
        renderJerarquiasGrid();

        // Limpia los selectores y carga la información correspondiente a cada nivel de la jerarquía seleccionada
        const empresaId = getPrincipalEmpresaId();
        cargarJerarquiaEnSelectores(empresaId, item);
    }

    const resetSelect = (selectElem, placeholder) => {
        if (!selectElem) return;
        selectElem.innerHTML = `<option value="">${placeholder}</option>`;
        selectElem.disabled = true;
    }

    const fillSelect = (selectElem, data, placeholder) => {
        if (!selectElem) return;
        let options = `<option value="">${placeholder}</option>`;
        if (data && Array.isArray(data)) {
            data.forEach(item => {
                options += `<option value="${item.id}">${item.nombre}</option>`;
            });
        }
        selectElem.innerHTML = options;
    }

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_user_in_form');
            submitButton = document.querySelector('#kt_user_in_submit');

            handleValidation();
            initEmpesasGrid();
            handleControls();

            handleSubmitValidation();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTUsuario.init();
});
