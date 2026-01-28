using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Gateway.Api.Transforms;

/// <summary>
/// Transform provider that adds UserHeadersTransform to all routes.
/// </summary>
public class UserHeadersTransformProvider : ITransformProvider
{
    public void ValidateRoute(TransformRouteValidationContext context)
    {
        // No validation needed
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
        // No validation needed
    }

    public void Apply(TransformBuilderContext context)
    {
        // Add the transform to all routes
        context.AddRequestTransform(transformContext =>
        {
            var transform = new UserHeadersTransform();
            return transform.ApplyAsync(transformContext);
        });
    }
}
