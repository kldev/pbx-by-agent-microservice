using System.Reflection;
using App.Shared.Web;

namespace RateService.Api.Definitions;

public static class ErrorCodeHelper
{
    private static readonly Dictionary<string, string> Messages = new();

    static ErrorCodeHelper()
    {
        LoadMessages(typeof(ErrorCodes));
    }

    private static void LoadMessages(Type type)
    {
        foreach (var nestedType in type.GetNestedTypes())
        {
            foreach (var field in nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var code = field.GetValue(null) as string;
                    var attr = field.GetCustomAttribute<ErrorMessageAttribute>();
                    if (code != null && attr != null)
                    {
                        Messages[code] = attr.Polish;
                    }
                }
            }
        }
    }

    public static string GetMessage(string code)
    {
        return Messages.TryGetValue(code, out var message) ? message : code;
    }
}
