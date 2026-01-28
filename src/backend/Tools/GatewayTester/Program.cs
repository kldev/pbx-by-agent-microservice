using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Spectre.Console;
using GatewayTester;

var gatewayUrl = args.Length > 0 ? args[0] : "http://localhost:8080";
var httpClient = new HttpClient
{
    BaseAddress = new Uri(gatewayUrl),
    Timeout = TimeSpan.FromSeconds(5)
};

AnsiConsole.MarkupLine($"[blue]Gateway Tester[/] - {gatewayUrl}\n");

// 1. Login
AnsiConsole.MarkupLine("[yellow]1. Login[/]");

HttpResponseMessage loginResponse;
try
{
    loginResponse = await httpClient.PostAsJsonAsync("/api/auth/login", new { email = "admin@jdapp.local", password = "Agent666" });
}
catch (HttpRequestException)
{
    AnsiConsole.MarkupLine($"[red]Nie udało się połączyć z {gatewayUrl}[/]");
    return 1;
}
catch (TaskCanceledException)
{
    AnsiConsole.MarkupLine($"[red]Timeout - brak odpowiedzi z {gatewayUrl} (5s)[/]");
    return 1;
}

var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

if (!loginResponse.IsSuccessStatusCode)
{
    AnsiConsole.MarkupLine($"[red]Login failed: {loginResponse.StatusCode}[/]");
    return 1;
}

var token = loginResult.GetProperty("token").GetString()!;
AnsiConsole.MarkupLine($"[green]OK[/] - Token: {token[..50]}...\n");

httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

// Test endpoints
var tests = new (string Name, string Method, string Url)[]
{
    // Identity
    ("Identity: /me", "GET", "/api/identity/auth/me"),
    ("Identity: Deperatment list", "GET", "/api/identity/department/all"),
    ("Identity: Department by ID", "GET", $"/api/identity/department/{AppGuids.MainDepartmentGuid}"),

    // CDR Service (pagination required)
    ("CdrService: list", "GET", "/api/cdr?PageNumber=1&PageSize=10"),
    ("CdrService: only answer", "GET", "/api/cdr/answer?PageNumber=1&PageSize=10"),
    ("CdrService: only poland (paged)", "GET", "/api/cdr/?country=PL&PageNumber=1&PageSize=5"),

    // Sales Service (pagination required)
    ("SalesService: Clients", "GET", "/api/sales/clients?PageNumber=1&PageSize=10"),    
    ("SalesService: Client Search", "GET", "/api/sales/clients/search?Query=test&MaxResults=10"),
};

var table = new Table();
table.AddColumn("Endpoint");
table.AddColumn("Status");
table.AddColumn("Response");

foreach (var (name, method, url) in tests)
{
    try
    {
        HttpResponseMessage response = method == "GET"
            ? await httpClient.GetAsync(url)
            : await httpClient.PostAsync(url, null);

        var content = await response.Content.ReadAsStringAsync();
        var preview = content.Length > 80 ? content[..80] + "..." : content;

        var status = response.IsSuccessStatusCode
            ? $"[green]{(int)response.StatusCode} OK[/]"
            : $"[red]{(int)response.StatusCode} {response.ReasonPhrase}[/]";

        table.AddRow(
            $"[cyan]{name}[/]\n{method} {url}",
            status,
            preview.Replace("[", "[[").Replace("]", "]]")
        );
    }
    catch (Exception ex)
    {
        table.AddRow(
            $"[cyan]{name}[/]\n{method} {url}",
            "[red]ERROR[/]",
            ex.Message
        );
    }
}

AnsiConsole.Write(table);

AnsiConsole.MarkupLine("\n[green]Test completed![/]");
return 0;
