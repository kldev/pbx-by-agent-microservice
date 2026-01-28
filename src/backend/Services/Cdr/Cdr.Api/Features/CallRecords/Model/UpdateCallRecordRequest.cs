namespace CdrService.Api.Features.CallRecords.Model;

public class UpdateCallRecordRequest
{
    // Czas (można aktualizować)
    public DateTime? AnswerTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public int BillableSeconds { get; set; }

    // Status
    public int CallStatusId { get; set; }
    public int TerminationCauseId { get; set; }

    // Rozliczenie
    public decimal TotalCost { get; set; }

    // Dodatkowe
    public string? UserData { get; set; }
}
