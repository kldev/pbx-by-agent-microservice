CREATE OR REPLACE VIEW vw_countries AS
SELECT
    id AS RecordId,
    gid AS Gid,
    name AS Label,
    code AS SubLabel,
    is_favorite AS IsFavorite
FROM dict.geo_countries;
