-- Tworzenie baz danych dla mikroserwisów
CREATE DATABASE IF NOT EXISTS pbx_identity CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS pbx_rates CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS pbx_datasource CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS pbx_rcp CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS pbx_cdr CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS pbx_answerrule CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Nadanie uprawnień użytkownikowi jd_user do wszystkich baz
GRANT ALL PRIVILEGES ON pbx_identity.* TO 'db_user'@'%';
GRANT ALL PRIVILEGES ON pbx_rates.* TO 'db_user'@'%';
GRANT ALL PRIVILEGES ON pbx_datasource.* TO 'db_user'@'%';
GRANT ALL PRIVILEGES ON pbx_rcp.* TO 'db_user'@'%';
GRANT ALL PRIVILEGES ON pbx_cdr.* TO 'db_user'@'%';
GRANT ALL PRIVILEGES ON pbx_answerrule.* TO 'db_user'@'%';

FLUSH PRIVILEGES;
