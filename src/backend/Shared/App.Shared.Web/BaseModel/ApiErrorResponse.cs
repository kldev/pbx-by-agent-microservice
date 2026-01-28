namespace App.Shared.Web.BaseModel;

public class ApiErrorResponse
{
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
    public List<string>? ValidationErrors { get; set; }
}
