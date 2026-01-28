-- View: vw_user_subordinates
-- Purpose: Get user subordinates for RCP supervisor filtering
-- Cross-database view reading from jd_identity.users

CREATE OR REPLACE VIEW vw_user_subordinates AS
SELECT
    u.id AS RecordId,
    u.gid AS Gid,
    CONCAT(u.first_name, ' ', u.last_name) AS Label,
    u.email AS SubLabel,
    u.supervisor_id AS SupervisorId
FROM pbx_identity.users u
WHERE u.is_deleted = 0 AND u.is_active = 1;
