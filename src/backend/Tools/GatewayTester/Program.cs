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
    loginResponse = await httpClient.PostAsJsonAsync("/api/auth/login", new { email = "admin@pbx.local", password = "Agent666" });
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
var tests = new (string Name, string Method, string Url, object? body)[]
{
    // Identity
    ("Identity: /me", "POST", "/api/identity/auth/me", null),
    ("Identity: structure list", "POST", "/api/identity/structure/all", new { isActive = true }),
    ("Identity: structure by ID", "POST", "/api/identity/structure/by-id", new { id = 1 }),
    ("Identity: app-users list", "POST", "/api/identity/app-users/list", new { pageNumber = 1, pageSize = 10 }),
    ("Identity: teams list", "POST", "/api/identity/teams/list", new { pageNumber = 1, pageSize = 10 }),

    // CDR Service
    ("CDR: call-records list", "POST", "/api/cdr/call-records/list", new { pageNumber = 1, pageSize = 10 }),
    ("CDR: call-directions list", "POST", "/api/cdr/call-directions/list", null),
    ("CDR: call-statuses list", "POST", "/api/cdr/call-statuses/list", null),
    ("CDR: termination-causes list", "POST", "/api/cdr/termination-causes/list", null),

    // Rate Service
    ("Rate: destination-groups", "GET", "/api/rate/destination-groups", null),
    ("Rate: tariffs list", "POST", "/api/rate/tariffs/list", new { pageNumber = 1, pageSize = 10 }),
    ("Rate: rates list", "POST", "/api/rate/rates/list", new { pageNumber = 1, pageSize = 10 }),

    // RCP Service (Time Entry)
    ("RCP: my monthly entry", "POST", "/api/rcp/my", new { year = DateTime.Now.Year, month = DateTime.Now.Month }),
    ("RCP: supervisor period", "POST", "/api/rcp/supervisor/period", new { year = DateTime.Now.Year, month = DateTime.Now.Month }),
    ("RCP: payroll period", "POST", "/api/rcp/payroll/period", new { year = DateTime.Now.Year, month = DateTime.Now.Month }),
    
    // FinCosts
    ("FinCosts: Document types", "POST", "/api/fincosts/costs/document-types", new {}),
    ("FinCosts: Currency types", "POST", "/api/fincosts/costs/currency-types", new {}),
    ("FinCosts: VAT Rate types", "POST", "/api/fincosts/costs/vat-rate-types", new {}),
    ("FinCosts: Documents (page 1)", "POST", "/api/fincosts/costs/documents/list", new { pageNumber = 1, pageSize = 10 }),
    ("FinCosts: Documents (page 2)", "POST", "/api/fincosts/costs/documents/list", new { pageNumber = 2, pageSize = 5 }),
    ("FinCosts: Documents (search)", "POST", "/api/fincosts/costs/documents/list", new { pageNumber = 1, pageSize = 10, search = "TechNova" }),
};

var table = new Table();
table.AddColumn("Endpoint", tc => { tc.Width = 40; });
table.AddColumn("Status", tc => { tc.Width = 20; });
table.AddColumn("Response", tc => { tc.Width = 120; });

foreach (var (name, method, url, body) in tests)
{
    try
    {
        HttpResponseMessage response = method == "GET"
            ? await httpClient.GetAsync(url)
            : await httpClient.PostAsJsonAsync(url, body ?? new { });

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
