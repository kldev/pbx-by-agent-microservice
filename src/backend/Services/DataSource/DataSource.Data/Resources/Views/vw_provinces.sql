CREATE OR REPLACE VIEW vw_provinces AS
SELECT
    p.id AS RecordId,
    p.gid AS Gid,
    p.name AS Label,
    c.name AS SubLabel
FROM dict.geo_country_provinces p
INNER JOIN dict.geo_countries c ON p.country_id = c.id;
