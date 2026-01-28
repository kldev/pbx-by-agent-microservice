namespace App.Bps.Enum.Rcp;

public enum RcpTimeEntryStatus
{
    /// <summary>Wpisywanie - edytowalne przez opiekuna</summary>
    Draft = 1,

    /// <summary>Wysłane do zatwierdzenia - nieedytowalne</summary>
    Submitted = 2,

    /// <summary>Zatwierdzone przez przełożonego</summary>
    Approved = 3,

    /// <summary>Zwrócone do poprawy</summary>
    Correction = 4,

    /// <summary>Przekazane do rozliczenia (kadry)</summary>
    Settlement = 5
}