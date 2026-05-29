
"use strict";

// Class definition
var KTUsuario = function () {
  // Elements
  var form;
  var submitButton;
  var validator;
  let empresasList = [];
  const empresasUsaurioInput = document.getElementById('EmpresasUsuario');


  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {
          'Nombre': {
            validators: {
              notEmpty: {
                message: 'Nombre de usuario requerido'
              },
              regexp: {
                regexp: /^[a-zA-ZÀ-ÿ\u00f1\u00d1]+(\s*[a-zA-ZÀ-ÿ\u00f1\u00d1]*)*[a-zA-ZÀ-ÿ\u00f1\u00d1]+$/i,
                message: 'El Nombre de usuario solo debe conteneres alfabeticos ',
              },
              stringLength: {
                min: 4,
                max: 250,

                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Email': {
            validators: {
              regexp: {
                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: 'No es un correo electr&oacutenico valido',
              },
              notEmpty: {
                message: 'Correo electr&oacutenico es requerido'
              }
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
  }

  var handleSubmitValidation = function (e) {
    // Handle form submit
    submitButton.addEventListener('click', function (e) {
      // Prevent button default action
      e.preventDefault();


      if (empresasList !== null && empresasList.length > 0) {

        empresasUsaurioInput.value = JSON.stringify(empresasList);
      }

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Disable button to avoid multiple click
          submitButton.setAttribute('data-kt-indicator', 'on');
          submitButton.disabled = true;
          form.submit();
        }
      });
    });
  }


  const handleControls = () => {
    const agregarEmpresaButton = document.getElementById("agregar-empresa");
    agregarEmpresaButton.addEventListener("click", agregarEmpresa);
  }

  const initEmpesasGrid = () => {
    //empresasList = [{ empresaId: 1, empresaNombre: "Kratos Consultores", perfilId: 1, perfilNombre: "Administrador", esPrincipal: true },
    //{ empresaId: 2, empresaNombre: "Kratos Consultores 2", PerfilId: 2, perfilNombre: "Contenidos", esPrincipal: false }];


    const empresasUsaurio = empresasUsaurioInput.value;
    if (empresasUsaurio !== null && empresasUsaurio !== "" && empresasUsaurio.length > 0) {
      empresasList = JSON.parse(empresasUsaurio);
    }

    rednerEmpresasGrid();

  }


  const rednerEmpresasGrid = () => {
    const appElement = document.getElementById("empresasContainer");
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
      radioInput.classList.add("form-check-input", "w-45px", "h-30px")
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

    const newItem = {
      empresaId: selectedEmpresaId,
      empresaNombre: empresaSelect.options[empresaSelect.selectedIndex].text,
      perfilId: selectedPerfilId,
      perfilNombre: perfilSelect.options[perfilSelect.selectedIndex].text,
      esPrincipal: empresasList.length === 0 // si solo hay un item, es el principal
    };

    empresasList.push(newItem);
    rednerEmpresasGrid();
    empresaSelect.value = "";
    perfilSelect.value = "";
  }

  const eliminarItem = (item) => {
    const index = empresasList.indexOf(item);
    if (index !== -1) {
      empresasList.splice(index, 1);
      rednerEmpresasGrid();
    }
  }

  const actualizarPrincipal = (item) => {
    empresasList.forEach(i => {
      i.esPrincipal = false;
    });
    item.esPrincipal = true;
    rednerEmpresasGrid();
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

      //handleSubmitAjax(); // use for ajax submit
      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTUsuario.init();
});
