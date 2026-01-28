using System.Reflection;
using App.Shared.Web;

namespace CdrService.Api.Definitions;

public static class ErrorCodeHelper
{
    private static readonly Dictionary<string, string> Messages = new();

    static ErrorCodeHelper()
    {
        LoadMessages(typeof(ErrorCodes));
    }

    public static string GetMessage(string code)
    {
        return Messages.TryGetValue(code, out var message) ? message : code;
    }

    private static void LoadMessages(Type type)
    {
        foreach (var nestedType in type.GetNestedTypes())
        {
            foreach (var field in nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var attribute = field.GetCustomAttribute<ErrorMessageAttribute>();
                    if (attribute != null)
                    {
                        var code = (string?)field.GetRawConstantValue();
                        if (code != null)
                        {
                            Messages[code] = attribute.Polish;
                        }
                    }
                }
            }
        }
    }
}
