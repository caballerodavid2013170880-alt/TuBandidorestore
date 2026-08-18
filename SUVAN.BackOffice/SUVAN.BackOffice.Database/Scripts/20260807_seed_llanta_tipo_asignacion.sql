-- Catálogo requerido por el módulo de asignación de llantas.
-- Idempotente: no fija IDs y reactiva registros existentes por nombre.

UPDATE llanta_tipo_asignacion
SET es_activo = 1
WHERE nombre IN ('Instalación', 'Reemplazo', 'Rotación');

INSERT INTO llanta_tipo_asignacion (nombre, descripcion, es_activo)
SELECT 'Instalación', 'Se coloca una llanta en una posición libre.', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM llanta_tipo_asignacion
    WHERE nombre = 'Instalación'
);

INSERT INTO llanta_tipo_asignacion (nombre, descripcion, es_activo)
SELECT 'Reemplazo', 'Se retira una llanta y se instala otra en la misma posición.', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM llanta_tipo_asignacion
    WHERE nombre = 'Reemplazo'
);

INSERT INTO llanta_tipo_asignacion (nombre, descripcion, es_activo)
SELECT 'Rotación', 'Se mueve una llanta instalada a otra posición.', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM llanta_tipo_asignacion
    WHERE nombre = 'Rotación'
);
