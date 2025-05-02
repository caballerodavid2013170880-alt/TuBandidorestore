
document.addEventListener('DOMContentLoaded', function () {
  // Obtén la URL actual
  const currentUrl = window.location.href;

  // Selecciona todos los enlaces del menú
  const menuLinks = document.querySelectorAll('a.menu-link');

  // Itera sobre cada enlace del menú
  menuLinks.forEach(function (link) {
    // Obtiene la URL del enlace
    const linkUrl = link.href;

    // Compara la URL actual con la URL del enlace
    if (currentUrl === linkUrl) {
      // Agrega una clase al enlace activo
      link.classList.add('active');

      // Busca el elemento padre con la clase .menu-accordion
      const menuAccordionParent = findAncestor(link, 'menu-accordion');

      // Agrega una clase al elemento padre
      if (menuAccordionParent) {
        menuAccordionParent.classList.add('here', 'show');

      }
    }
  });

  // Función para encontrar el ancestro con una clase específica
  function findAncestor(element, className) {
    while ((element = element.parentElement) && !element.classList.contains(className));
    return element;
  }
});
