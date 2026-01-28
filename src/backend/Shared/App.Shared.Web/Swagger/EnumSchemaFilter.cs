using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Shared.Web.Swagger;

/// <summary>
/// Swagger schema filter that converts enums to string representation.
/// This ensures that Swagger/OpenAPI schema shows enum values as strings
/// instead of integer values, making generated clients more readable.
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            schema.Type = "string";

            foreach (var name in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}
