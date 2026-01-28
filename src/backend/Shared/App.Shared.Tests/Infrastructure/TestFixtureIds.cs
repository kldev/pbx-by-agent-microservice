namespace App.Shared.Tests.Infrastructure;

/// <summary>
/// Fixed test IDs for stable test data.
/// Using fixed IDs ensures tests are deterministic and reproducible.
/// </summary>
public static class TestFixtureIds
{
    /// <summary>
    /// Fixed numeric IDs for entities using long/int keys
    /// </summary>
    public static class Ids
    {
        // Structures (int)
        public const int TestStructure1 = 1001;
        public const int TestStructure2 = 1002;

        // Teams (long)
        public const long TestTeam1 = 2001;
        public const long TestTeam2 = 2002;

        // Users (long)
        public const long TestUser1 = 3001;
        public const long TestUser2 = 3002;
        public const long TestUser3 = 3003;

        // Jobs (long)
        public const long TestJob1 = 4001;
        public const long TestJob2 = 4002;

        // Job Descriptions (long)
        public const long TestJobDescription1 = 5001;
        public const long TestJobDescription2 = 5002;

        // Positions (long)
        public const long TestPosition1 = 6001;
        public const long TestPosition2 = 6002;

        // Clients (long) - JdSales
        public const long TestClient1 = 7001;
        public const long TestClient2 = 7002;

        // SalesLeads (long) - JdSales
        public const long TestSalesLead1 = 8001;
        public const long TestSalesLead2 = 8002;

        // OperatingCompanies (long) - ProjectPanel
        public const long TestOperatingCompany1 = 9001;
        public const long TestOperatingCompany2 = 9002;

        // Projects (long) - ProjectPanel
        public const long TestProject1 = 10001;
        public const long TestProject2 = 10002;
        public const long TestProject3 = 10003;

        // ProjectContracts (long) - ProjectPanel
        public const long TestContract1 = 11001;
        public const long TestContract2 = 11002;

        // ProjectPositions (long) - ProjectPanel
        public const long TestPosition3 = 12001;
        public const long TestPosition4 = 12002;
        public const long TestPositionApt = 12003; // Position for APT project

        // ProjectRates (long) - ProjectPanel
        public const long TestRate1 = 13001;
        public const long TestRate2 = 13002;

        // WorkerAssignments (long) - ProjectPanel
        public const long TestAssignment1 = 14001;
        public const long TestAssignment2 = 14002;
        public const long TestAssignmentApt = 14003; // Assignment for APT project

        // WorkerLeaves (long) - ProjectPanel
        public const long TestLeave1 = 15001;
        public const long TestLeave2 = 15002;

        // WorkerTimesheets (long) - ProjectPanel
        public const long TestTimesheet1 = 16001;
        public const long TestTimesheet2 = 16002;
        public const long TestTimesheetApt = 16003; // Timesheet for APT project (should be excluded from GetByPeriod)

        // TimesheetDays (long) - ProjectPanel
        public const long TestTimesheetDay1 = 17001;
        public const long TestTimesheetDay2 = 17002;
        public const long TestTimesheetDay3 = 17003;

        // ProjectWorkers (long) - ProjectPanel
        public const long TestProjectWorker1 = 18001;
        public const long TestProjectWorker2 = 18002;
    }

    /// <summary>
    /// Fixed GIDs (string GUIDs) for entities using Gid field
    /// </summary>
    public static class Gids
    {
        public const string TestUser1 = "11111111-1111-1111-1111-111111111111";
        public const string TestUser2 = "22222222-2222-2222-2222-222222222222";
        public const string TestUser3 = "33333333-3333-3333-3333-333333333333";

        public const string TestTeam1 = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        public const string TestTeam2 = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

        public const string TestJob1 = "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee";
        public const string TestJob2 = "ffffffff-ffff-ffff-ffff-ffffffffffff";

        public const string TestJobDescription1 = "11111111-2222-3333-4444-555555555555";
        public const string TestJobDescription2 = "66666666-7777-8888-9999-000000000000";

        public const string TestPosition1 = "12121212-1212-1212-1212-121212121212";
        public const string TestPosition2 = "13131313-1313-1313-1313-131313131313";

        // Clients - JdSales
        public const string TestClient1 = "c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1";
        public const string TestClient2 = "c2c2c2c2-c2c2-c2c2-c2c2-c2c2c2c2c2c2";

        // SalesLeads - JdSales
        public const string TestSalesLead1 = "d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1";
        public const string TestSalesLead2 = "d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2";

        // OperatingCompanies - ProjectPanel
        public const string TestOperatingCompany1 = "oc-test-company-0001";
        public const string TestOperatingCompany2 = "oc-test-company-0002";

        // Projects - ProjectPanel
        public const string TestProject1 = "prj-test-project-0001";
        public const string TestProject2 = "prj-test-project-0002";
        public const string TestProject3 = "prj-test-project-0003";

        // ProjectContracts - ProjectPanel
        public const string TestContract1 = "ctr-test-contract-0001";
        public const string TestContract2 = "ctr-test-contract-0002";

        // ProjectPositions - ProjectPanel
        public const string TestProjectPosition1 = "pos-test-position-0001";
        public const string TestProjectPosition2 = "pos-test-position-0002";
        public const string TestProjectPositionApt = "pos-test-position-apt";

        // ProjectRates - ProjectPanel
        public const string TestRate1 = "rate-test-rate-0001";
        public const string TestRate2 = "rate-test-rate-0002";

        // WorkerAssignments - ProjectPanel
        public const string TestAssignment1 = "asgn-test-assignment-0001";
        public const string TestAssignment2 = "asgn-test-assignment-0002";
        public const string TestAssignmentApt = "asgn-test-assignment-apt";

        // Workers (virtual - for snapshots)
        public const string TestWorker1 = "wkr-jan-kowalski-0001";
        public const string TestWorker2 = "wkr-piotr-nowak-0002";
        public const string TestWorkerApt = "wkr-apt-worker-0003";

        // WorkerLeaves - ProjectPanel
        public const string TestLeave1 = "leave-test-leave-0001";
        public const string TestLeave2 = "leave-test-leave-0002";

        // WorkerTimesheets - ProjectPanel
        public const string TestTimesheet1 = "ts-test-timesheet-0001";
        public const string TestTimesheet2 = "ts-test-timesheet-0002";
        public const string TestTimesheetApt = "ts-test-timesheet-apt";

        // TimesheetDays - ProjectPanel
        public const string TestTimesheetDay1 = "tsd-test-day-0001";
        public const string TestTimesheetDay2 = "tsd-test-day-0002";
        public const string TestTimesheetDay3 = "tsd-test-day-0003";

        // ProjectWorkers - ProjectPanel
        public const string TestProjectWorker1 = "pw-jan-kowalski-0001";
        public const string TestProjectWorker2 = "pw-piotr-nowak-0002";
    }
}
