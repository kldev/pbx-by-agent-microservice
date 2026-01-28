using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Rcp.Data.Entities;

namespace Rcp.Api.Features.TimeEntry;

public class RcpExcelExportService : IRcpExcelExportService
{
    private readonly IRcpDataHandler _dataHandler;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RcpExcelExportService(IRcpDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<byte[]> GenerateMonthlyReportAsync(int year, int month)
    {
        var period = await _dataHandler.GetPeriodAsync(year, month);
        var entries = period != null
            ? await _dataHandler.GetMonthlyEntriesForPeriodAsync(period.Id)
            : [];

        using var stream = new MemoryStream();
        using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            // Sheet 1: Summary
            var summarySheetPart = workbookPart.AddNewPart<WorksheetPart>();
            summarySheetPart.Worksheet = CreateSummarySheet(entries);
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(summarySheetPart),
                SheetId = 1,
                Name = "Podsumowanie"
            });

            // Sheet 2: Details
            var detailsSheetPart = workbookPart.AddNewPart<WorksheetPart>();
            detailsSheetPart.Worksheet = CreateDetailsSheet(entries);
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(detailsSheetPart),
                SheetId = 2,
                Name = "Szczegóły"
            });

            workbookPart.Workbook.Save();
        }

        return stream.ToArray();
    }

    private Worksheet CreateSummarySheet(List<RcpMonthlyEntry> entries)
    {
        var sheetData = new SheetData();

        // Header row
        sheetData.Append(CreateRow(
            "Imie i nazwisko", "Suma godzin", "Suma minut", "Godziny (decimal)", "Status"
        ));

        var totalMinutes = 0;

        foreach (var entry in entries)
        {
            var hours = entry.TotalMinutes / 60;
            var minutes = entry.TotalMinutes % 60;
            var decimalHours = Math.Round((decimal)entry.TotalMinutes / 60, 2);

            sheetData.Append(CreateRow(
                entry.EmployeeFullName ?? "Nieznany",
                hours.ToString(),
                minutes.ToString(),
                decimalHours.ToString("F2"),
                entry.Status.ToString()
            ));

            totalMinutes += entry.TotalMinutes;
        }

        // Summary row
        var totalHours = totalMinutes / 60;
        var totalMins = totalMinutes % 60;
        var totalDecimal = Math.Round((decimal)totalMinutes / 60, 2);

        sheetData.Append(CreateRow(
            "RAZEM",
            totalHours.ToString(),
            totalMins.ToString(),
            totalDecimal.ToString("F2"),
            ""
        ));

        return new Worksheet(sheetData);
    }

    private Worksheet CreateDetailsSheet(List<RcpMonthlyEntry> entries)
    {
        var sheetData = new SheetData();

        // Header row
        sheetData.Append(CreateRow(
            "Imie i nazwisko", "Data", "Dzien", "Start", "Koniec",
            "Godziny", "Minuty", "Decimal", "Notatki"
        ));

        var allDays = entries
            .SelectMany(e => e.DayEntries.Select(d => new { Entry = e, Day = d }))
            .OrderBy(x => x.Entry.EmployeeFullName)
            .ThenBy(x => x.Day.WorkDate);

        foreach (var item in allDays)
        {
            var hours = item.Day.WorkMinutes / 60;
            var minutes = item.Day.WorkMinutes % 60;
            var decimalHours = Math.Round((decimal)item.Day.WorkMinutes / 60, 2);
            var dayOfWeek = GetPolishDayName(item.Day.WorkDate.DayOfWeek);

            sheetData.Append(CreateRow(
                item.Entry.EmployeeFullName ?? "Nieznany",
                item.Day.WorkDate.ToString("yyyy-MM-dd"),
                dayOfWeek,
                item.Day.StartTime.ToString(@"hh\:mm"),
                item.Day.EndTime.ToString(@"hh\:mm"),
                hours.ToString(),
                minutes.ToString(),
                decimalHours.ToString("F2"),
                item.Day.Notes ?? ""
            ));
        }

        return new Worksheet(sheetData);
    }

    private Row CreateRow(params string[] values)
    {
        var row = new Row();
        foreach (var value in values)
        {
            row.Append(new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value)
            });
        }
        return row;
    }

    private static string GetPolishDayName(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "Pon",
        DayOfWeek.Tuesday => "Wto",
        DayOfWeek.Wednesday => "Sro",
        DayOfWeek.Thursday => "Czw",
        DayOfWeek.Friday => "Pia",
        DayOfWeek.Saturday => "Sob",
        DayOfWeek.Sunday => "Nie",
        _ => ""
    };
}
