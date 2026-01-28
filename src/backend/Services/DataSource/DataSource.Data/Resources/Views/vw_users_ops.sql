CREATE OR REPLACE VIEW vw_users_sales AS
SELECT
    u.id AS RecordId,
    u.gid AS Gid,
    CONCAT(u.first_name, ' ', u.last_name) AS Label,
    u.email AS SubLabel
FROM pbx_identity.users u
INNER JOIN pbx_identity.app_user_role_assignments ura ON u.id = ura.user_id
INNER JOIN pbx_identity.app_user_roles r ON ura.role_id = r.id
WHERE r.name = 'Ops' AND u.is_deleted = 0;
