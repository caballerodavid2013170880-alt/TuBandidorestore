SET @menu_id := 126;
SET @catalogos_id := 90;

INSERT INTO menu (idmenu, menu_idpadre, titulo, fecharegistro, activo, icono, ruta)
SELECT @menu_id, @catalogos_id, 'Llantas', NOW(), 1, 'ki-outline ki-car-2', '/ModuloAdministrativo/Llantas'
WHERE NOT EXISTS (
    SELECT 1
    FROM menu
    WHERE idmenu = @menu_id
);

UPDATE menu
SET ruta = '/ModuloAdministrativo/Llantas'
WHERE idmenu = @menu_id;

INSERT INTO permiso (
    perfil_idperfil,
    menu_idmenu,
    agregar,
    modificar,
    eliminar,
    ejecutar,
    fecharegistro,
    activo
)
SELECT
    p.perfil_idperfil,
    @menu_id,
    0,
    0,
    0,
    1,
    NOW(),
    1
FROM permiso p
WHERE p.menu_idmenu = @catalogos_id
  AND p.activo = 1
  AND NOT EXISTS (
      SELECT 1
      FROM permiso existing
      WHERE existing.perfil_idperfil = p.perfil_idperfil
        AND existing.menu_idmenu = @menu_id
  );
