SET @menu_id := 127;
SET @catalogos_id := 90;

INSERT INTO menu (idmenu, menu_idpadre, titulo, fecharegistro, activo, icono, ruta)
SELECT @menu_id, @catalogos_id, 'Marcas de llantas', NOW(), 1, 'ki-outline ki-award', '/llantasmarca'
WHERE NOT EXISTS (
    SELECT 1
    FROM menu
    WHERE idmenu = @menu_id
);

UPDATE menu
SET ruta = '/llantasmarca'
WHERE idmenu = @menu_id;

UPDATE permiso
SET agregar = 1,
    modificar = 1,
    eliminar = 1,
    ejecutar = 1,
    activo = 1
WHERE menu_idmenu = @menu_id;

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
    1,
    1,
    1,
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
