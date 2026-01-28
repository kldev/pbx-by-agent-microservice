CREATE OR REPLACE VIEW vw_sbu AS
SELECT
    id AS RecordId,
    code AS Gid,
    name AS Label,
    region AS SubLabel,
    region AS Region
FROM pbx_identity.structure_dict
WHERE is_active = 1;
