"use strict";

// Class Definition
var KTSigninTwoFactor = function () {
  // Elements
  var form;
  var submitButton;
  var resendButton;

  // Handle form
  var handleForm = function (e) {
    // Handle form submit
    submitButton.addEventListener('click', async function (e) {
      e.preventDefault();

      var validated = true;

      var inputs = [].slice.call(form.querySelectorAll('.codeotp'));
      inputs.map(function (input) {
        if (input.value === '' || input.value.length === 0) {
          validated = false;
        }
      });

      if (validated === true) {
        const code = document.getElementById("OTP");
        code.value = inputs.map(function (input) { return input.value }).join('');
        // Show loading indication
        submitButton.setAttribute('data-kt-indicator', 'on');

        // Disable button to avoid multiple click 
        submitButton.disabled = true;
        form.submit();
        //const email = document.getElementById("Email").value;
        //const response = await fetch('/Seguridad/VerificarCodigoUsuario', {
        //  method: 'POST',
        //  headers: {
        //    'Content-Type': 'application/json'
        //  },
        //  body: JSON.stringify({ Email: email, OTP: code.value }),
        //});
        //const data = await response.json();
        //if (data.success === false) {
        //  Swal.fire({
        //    text: `Codigo incorrecto o expirado`,
        //    icon: "warning",
        //    buttonsStyling: false,
        //    confirmButtonText: "Aceptar",
        //    customClass: {
        //      confirmButton: "btn fw-bold btn-primary",
        //    }
        //  });

        //  submitButton.setAttribute('data-kt-indicator', 'off');
        //  submitButton.disabled = false;
        //}

        //if (data.success === true) {
        //  // redirect home page 
        //  window.location.href = "/";
        //}

      }
    });
  }

  const handleReseend = function () {
    resendButton.addEventListener('click', async function (e) {
      e.preventDefault();

      const email = document.querySelector("[name=Email]").value;

      if (email) {
        resendButton.setAttribute('data-kt-indicator', 'on');
        const response = await fetch('/Seguridad/ReenviarCodigo', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ Email: email }),
        });
        const data = await response.json();
        if (data.success === true) {
          Swal.fire({
            text: `Se envio un nuevo codigo a su correo!`,
            icon: "success",
            buttonsStyling: false,
            confirmButtonText: "Aceptar",
            customClass: {
              confirmButton: "btn fw-bold btn-primary",
            }
          });
          resendButton.setAttribute('data-kt-indicator', 'off');
        }
        if (data.success === false) {
          Swal.fire({
            text: 'No se pudo enviar el codigo.',
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Aceptar",
            customClass: {
              confirmButton: "btn fw-bold btn-primary",
            }
          });
          resendButton.setAttribute('data-kt-indicator', 'off');

        }
      }
    });
  }


  var handleType = function () {
    var input1 = form.querySelector("[name=code_1]");
    var input2 = form.querySelector("[name=code_2]");
    var input3 = form.querySelector("[name=code_3]");
    var input4 = form.querySelector("[name=code_4]");

    input1.focus();

    input1.addEventListener("keyup", function () {
      if (this.value.length === 1) {
        input2.focus();
      }
    });

    input2.addEventListener("keyup", function () {
      if (this.value.length === 1) {
        input3.focus();
      }
    });

    input3.addEventListener("keyup", function () {
      if (this.value.length === 1) {
        input4.focus();
      }
    });


    input4.addEventListener("keyup", function () {
      if (this.value.length === 1) {
        input4.blur();
      }
    });

  }

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_sing_in_two_factor_form');
      submitButton = document.querySelector('#kt_sing_in_two_factor_submit');
      resendButton = document.querySelector('#reseend-code');

      handleForm();
      handleType();
      handleReseend();
    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTSigninTwoFactor.init();
});