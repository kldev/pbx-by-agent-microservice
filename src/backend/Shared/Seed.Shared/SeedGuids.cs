namespace Seed.Shared;

/// <summary>
/// Shared Fixed GUIDs for seed data consistency across all microservices.
/// These GUIDs ensure that cross-service references are consistent.
/// </summary>
public static class SeedGuids
{

    public static class GidOption {
        public static string _1 => "1111111-1111-1111-1111-111111111111";
        public static string _2 => "2222222-2222-2222-2222-222222222222";
        public static string _3 => "3333333-3333-3333-3333-333333333333";
        public static string _4 => "4444444-4444-4444-4444-444444444444";
        public static string _5 => "5555555-5555-5555-5555-555555555555";
        internal static string _6 => "6666666-6666-6666-6666-666666666666";
        internal static string _7 => "7777777-7777-7777-7777-777777777777";
        internal static string _8 => "8888888-8888-8888-8888-888888888888";
        internal static string _9 => "9999999-9999-9999-9999-999999999999"; 
     
    }
    #region Clients (JdSales)

    /// <summary>
    /// Client GIDs from AppSales service.
    /// </summary>
    public static class Clients
    {
        // Core showcase clients
        public static string AutoParts => $"c{GidOption._1}";       
    }

    #endregion

 
    #region Teams (Identity)

    /// <summary>
    /// Team IDs and names from App Identity service.
    /// </summary>
    public static class Teams
    {
        // Poland
        public static string WolfPackGid => $"t{GidOption._1}";
        public const string WolfPackName = "WolfPack";

        public static string TopGunnersGid => $"t{GidOption._2}";
        public const string TopGunnersName = "TopGunners";
   
         // Foregin
        public static string MadMenGid => $"t{GidOption._3}";
        public const string MadMenName = "MadMen";

        public static string VictorySquadGid => $"t{GidOption._4}";
        public const string VictorySquadName = "VictorySquad";

        public static string PitchPerfectGid => $"t{GidOption._5}";
        public const string PitchPerfectName = "PitchPerfect";

    }

    #endregion

    #region Employees (Identity)

    /// <summary>
    /// System User GIDs from Identity service.
    /// </summary>
    public static class Employees
    {
        public static string Conrad => $"e{GidOption._1}";       
    }

    #endregion

}
