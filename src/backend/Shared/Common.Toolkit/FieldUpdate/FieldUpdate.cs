using System.Text.Json.Serialization;

namespace Common.Toolkit.FieldUpdate;

/// <summary>
/// Wrapper dla pola umozliwiajacego partial update.
/// Gdy IsUpdate = true, pole zostanie zaktualizowane (nawet jesli Value = null).
/// Gdy IsUpdate = false, pole zostanie pominiete przy aktualizacji.
/// </summary>
/// <typeparam name="T">Typ wartosci pola</typeparam>
public class FieldUpdate<T>
{
    /// <summary>
    /// Czy pole ma byc zaktualizowane
    /// </summary>
    [JsonPropertyName("isUpdate")]
    public bool IsUpdate { get; set; }

    /// <summary>
    /// Nowa wartosc pola (uzywana tylko gdy IsUpdate = true)
    /// </summary>
    [JsonPropertyName("value")]
    public T? Value { get; set; }

    /// <summary>
    /// Domyslny konstruktor (IsUpdate = false)
    /// </summary>
    public FieldUpdate()
    {
        IsUpdate = false;
        Value = default;
    }

    /// <summary>
    /// Konstruktor z wartoscia (IsUpdate = true)
    /// </summary>
    public FieldUpdate(T? value)
    {
        IsUpdate = true;
        Value = value;
    }

    /// <summary>
    /// Konstruktor pelny
    /// </summary>
    public FieldUpdate(bool isUpdate, T? value)
    {
        IsUpdate = isUpdate;
        Value = value;
    }

    /// <summary>
    /// Tworzy FieldUpdate z flaga update i wartoscia
    /// </summary>
    public static FieldUpdate<T> Update(T? value) => new(true, value);

    /// <summary>
    /// Tworzy FieldUpdate bez aktualizacji (skip)
    /// </summary>
    public static FieldUpdate<T> Skip() => new(false, default);

    /// <summary>
    /// Niejawna konwersja z wartosci na FieldUpdate (IsUpdate = true)
    /// </summary>
    public static implicit operator FieldUpdate<T>(T? value) => new(value);
}

/// <summary>
/// Metody rozszerzajace dla FieldUpdate
/// </summary>
public static class FieldUpdateExtensions
{
    /// <summary>
    /// Aplikuje aktualizacje pola jesli IsUpdate = true
    /// </summary>
    /// <typeparam name="T">Typ wartosci</typeparam>
    /// <param name="field">FieldUpdate do sprawdzenia</param>
    /// <param name="currentValue">Aktualna wartosc pola</param>
    /// <returns>Nowa wartosc jesli IsUpdate, w przeciwnym razie aktualna</returns>
    public static T? Apply<T>(this FieldUpdate<T>? field, T? currentValue)
    {
        if (field == null || !field.IsUpdate)
            return currentValue;

        return field.Value;
    }

    /// <summary>
    /// Aplikuje aktualizacje pola z transformacja
    /// </summary>
    public static TResult? Apply<T, TResult>(this FieldUpdate<T>? field, TResult? currentValue, Func<T?, TResult?> transform)
    {
        if (field == null || !field.IsUpdate)
            return currentValue;

        return transform(field.Value);
    }

    /// <summary>
    /// Sprawdza czy pole wymaga aktualizacji
    /// </summary>
    public static bool ShouldUpdate<T>(this FieldUpdate<T>? field)
    {
        return field != null && field.IsUpdate;
    }

    /// <summary>
    /// Wykonuje akcje jesli pole wymaga aktualizacji
    /// </summary>
    public static void IfUpdate<T>(this FieldUpdate<T>? field, Action<T?> action)
    {
        if (field != null && field.IsUpdate)
            action(field.Value);
    }

    /// <summary>
    /// Wykonuje asynchroniczna akcje jesli pole wymaga aktualizacji
    /// </summary>
    public static async Task IfUpdateAsync<T>(this FieldUpdate<T>? field, Func<T?, Task> action)
    {
        if (field != null && field.IsUpdate)
            await action(field.Value);
    }
}
