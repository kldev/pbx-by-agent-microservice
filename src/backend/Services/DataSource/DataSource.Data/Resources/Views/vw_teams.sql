CREATE OR REPLACE VIEW vw_teams AS
SELECT
    t.id AS RecordId,
    t.gid AS Gid,
    t.name AS Label,
    s.name AS SubLabel,
    s.code AS SbuGid,
    s.id AS SbuId,
    t.type AS Type
FROM pbx_identity.teams t
INNER JOIN pbx_identity.sbu_dict s ON t.sbu_id = s.id
WHERE t.is_deleted = 0 AND t.is_active = 1;
