namespace DataSource.Api.Enums;

public enum DataSourceType
{
    // Geo
    Countries = 1,
    Provinces = 2,

    // Users
    UsersSales = 10,
    UsersAll = 11,

    // CRM/Sales
    Clients = 20,

    // Identity/Org structure
    Sbu = 30,
    Teams = 31,

    // Jobs - Dictionaries
    Benefits = 40,
    Certificates = 41,
    Tools = 42,
    Traits = 43,
    Occupations = 44,
    Positions = 45
}
