"use strict";

// Class definition
var KTPregunta = function () {
  // Elements
  var form;
  var submitButton;
  var validator;


  const initQuill = () => {
    // Define all elements for quill editor
    const elements = [
      '#kt_contenido_description',
    ];

    // Loop all elements
    elements.forEach(element => {
      // Get quill element
      let quill = document.querySelector(element);

      // Break if element not found
      if (!quill) {
        return;
      }

      // Init quill --- more info: https://quilljs.com/docs/quickstart/
      quill = new Quill(element, {
        modules: {
          toolbar: [
            ['bold', 'italic', 'underline', 'strike'],

            [{ 'header': 1 }, { 'header': 2 }],               // custom button values
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],    // superscript/subscript
            [{ 'indent': '-1' }, { 'indent': '+1' }],                         // text direction
            [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

            [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
            [{ 'font': [] }],
            [{ 'align': [] }]                                      // remove formatting button

          ]
        },
        placeholder: '',
        theme: 'snow' // or 'bubble'
      });

      quill.on('text-change', function () {
        var htmlContent = quill.root.innerHTML;
        document.getElementById('Contenido').value = htmlContent;
      });

      // Cargar contenido en Quill desde el campo oculto al cargar la página
      var contenidoGuardado = document.getElementById('Contenido').value;
      quill.root.innerHTML = contenidoGuardado;
    });
  }

  const initImage64 = () => {

    const image64 = document.getElementById('Imagen64').value;
    if (image64) {
      const imageWrapper = document.getElementById('image-contenido-wrapper');

      imageWrapper.style.backgroundImage = `url(data:image/png;base64,${image64})`;

    }

  }

  // Handle form
  var handleValidation = function (e) {
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    validator = FormValidation.formValidation(
      form,
      {
        fields: {

          'Titulo': {
            validators: {
              notEmpty: {
                message: 'Titulo requerido'
              },
              stringLength: {
                min: 4,
                max: 250,
                message: 'deben tener entre 4 y 250 caracteres',
              },
            }
          },
          'Imagen': {
            validators: {
              file: {
                extension: 'jpeg,jpg,png',
                type: 'image/jpeg,image/png',
                maxSize: 2097152, // 2048 * 1024
                message: 'El archivo seleccionado no es valido',
              },
            }
          },
          'Contenido': {
            validators: {
              notEmpty: {
                message: 'Contenido requerido'
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

  var handleSubmitAjax = function (e) {
    // Handle form submit
    submitButton.addEventListener('click', function (e) {
      // Prevent button default action
      e.preventDefault();

      // Validate form
      validator.validate().then(function (status) {
        if (status == 'Valid') {
          // Show loading indication
          submitButton.setAttribute('data-kt-indicator', 'on');

          // Disable button to avoid multiple click
          submitButton.disabled = true;

          // Check axios library docs: https://axios-http.com/docs/intro
          axios.post(submitButton.closest('form').getAttribute('action'), new FormData(form)).then(function (response) {
            if (response) {
              form.reset();

              // Show message popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
              Swal.fire({
                text: "You have successfully logged in!",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                  confirmButton: "btn btn-primary"
                }
              });

              const redirectUrl = form.getAttribute('data-kt-redirect-url');

              if (redirectUrl) {
                location.href = redirectUrl;
              }
            } else {
              // Show error popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
              Swal.fire({
                text: "Sorry, the email or password is incorrect, please try again.",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                  confirmButton: "btn btn-primary"
                }
              });
            }
          }).catch(function (error) {
            Swal.fire({
              text: "Lo sentimos, se han detectado algunos errores. Int&eacute;ntalo de nuevo.",
              icon: "error",
              buttonsStyling: false,
              confirmButtonText: "Aceptar",
              customClass: {
                confirmButton: "btn btn-primary"
              }
            });
          }).then(() => {
            // Hide loading indication
            submitButton.removeAttribute('data-kt-indicator');

            // Enable button
            submitButton.disabled = false;
          });
        }
      });
    });
  }

  var isValidUrl = function (url) {
    try {
      new URL(url);
      return true;
    } catch (e) {
      return false;
    }
  }

  // Public functions
  return {
    // Initialization
    init: function () {
      form = document.querySelector('#kt_contenido_in_form');
      submitButton = document.querySelector('#kt_contenido_in_submit');

      initQuill();
      handleValidation();
      initImage64();

      handleSubmitValidation(); // use for form validation submit

    }
  };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
  KTPregunta.init();
});
