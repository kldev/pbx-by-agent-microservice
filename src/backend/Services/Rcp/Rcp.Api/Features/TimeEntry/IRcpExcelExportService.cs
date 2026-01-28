namespace Rcp.Api.Features.TimeEntry;

public interface IRcpExcelExportService
{
    Task<byte[]> GenerateMonthlyReportAsync(int year, int month);
}
