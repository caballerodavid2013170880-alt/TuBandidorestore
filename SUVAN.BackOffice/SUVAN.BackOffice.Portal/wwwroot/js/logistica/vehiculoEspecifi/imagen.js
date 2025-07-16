"use strict";

var KTImagenes = (function () {

    const MAX_IMAGENES = 10;
    const CONTENEDOR_ID = "imagenesContainer";
    const TEMPLATE_ID = "imagenTemplate";

    const _contar = (container) => container.querySelectorAll(".componente-imagen").length;

    const _inicializarCard = (card, container, file = null) => {
        const wrapper = card.querySelector(".image-input-wrapper");
        const fileInput = card.querySelector('input[type="file"]');
        const btnX = card.querySelector(".btn-cancelar-componente");
        const imageInputDiv = card.querySelector(".image-input");

        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                wrapper.style.backgroundImage = `url('${e.target.result}')`;
                imageInputDiv.classList.remove("image-input-empty", "image-input-placeholder");
                btnX.classList.remove("d-none");
            };
            reader.readAsDataURL(file);
        } else {
            btnX.classList.toggle("d-none", imageInputDiv.classList.contains("image-input-empty"));
        }

        fileInput.addEventListener("change", () => {
            const newFile = fileInput.files[0];
            if (!newFile) return;

            const reader = new FileReader();
            reader.onload = (e) => {
                wrapper.style.backgroundImage = `url('${e.target.result}')`;
                imageInputDiv.classList.remove("image-input-empty", "image-input-placeholder");
                btnX.classList.remove("d-none");

                if (!_existeComponenteVacio(container) && _contar(container) < MAX_IMAGENES) {
                    _addCard(container);
                }
            };
            reader.readAsDataURL(newFile);
        });

        btnX.addEventListener("click", () => {
            container.removeChild(card);

            if (!_existeComponenteVacio(container) && _contar(container) < MAX_IMAGENES) {
                _addCard(container);
            }
        });
    };

    const _addCard = (container) => {
        if (_contar(container) >= MAX_IMAGENES) return;

        const template = document.getElementById(TEMPLATE_ID).content.firstElementChild;
        const card = template.cloneNode(true);
        container.appendChild(card);
        _inicializarCard(card, container);
    };

    const _existeComponenteVacio = (container) => {
        return !!container.querySelector(".image-input.image-input-empty");
    };

    return {
        init: function () {
            const container = document.getElementById(CONTENEDOR_ID);
            if (!container) return;

            container.querySelectorAll(".componente-imagen").forEach(card => {
                const wrapper = card.querySelector(".image-input-wrapper");
                const background = wrapper.style.backgroundImage;
                if (background && background !== 'none') {
                    _inicializarCard(card, container, null);
                } else {
                    _inicializarCard(card, container, null);
                }
            });
            if (!_existeComponenteVacio(container)) {
                _addCard(container);
            }
        }
    };
})();

KTUtil.onDOMContentLoaded(function () {
    KTImagenes.init();
});
