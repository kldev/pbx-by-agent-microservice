namespace CdrService.Api.Features.CallRecords.Model;

public class CallRecordListFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Szukaj w numerach dzwoniącego/wywoływanego
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Data rozpoczęcia od
    /// </summary>
    public DateTime? StartDateFrom { get; set; }

    /// <summary>
    /// Data rozpoczęcia do
    /// </summary>
    public DateTime? StartDateTo { get; set; }

    /// <summary>
    /// Filtr po statusie połączenia
    /// </summary>
    public int? CallStatusId { get; set; }

    /// <summary>
    /// Filtr po kierunku połączenia
    /// </summary>
    public int? CallDirectionId { get; set; }

    /// <summary>
    /// Filtr po kliencie
    /// </summary>
    public string? CustomerGid { get; set; }

    /// <summary>
    /// Filtr po koncie SIP
    /// </summary>
    public string? SipAccountGid { get; set; }

    /// <summary>
    /// Minimalny koszt
    /// </summary>
    public decimal? MinCost { get; set; }

    /// <summary>
    /// Maksymalny koszt
    /// </summary>
    public decimal? MaxCost { get; set; }
}
