CREATE OR REPLACE VIEW vw_users_all AS
SELECT
    u.id AS RecordId,
    u.gid AS Gid,
    CONCAT(u.first_name, ' ', u.last_name) AS Label,
    u.email AS SubLabel
FROM pbx_identity.users u
WHERE u.is_deleted = 0 AND u.is_active = 1;
