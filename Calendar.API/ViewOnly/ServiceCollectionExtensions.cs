using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using CalendarApp.Contracts;

namespace CalendarApp.API.ViewOnly;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddViewOnlyJsonGuards(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options => 
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver
            {
                Modifiers = { RejectViewOnlyProperties }
            });
        });

        return services;
    }

    private static void RejectViewOnlyProperties(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind is not JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (var property in typeInfo.Properties)
        {
            if (property.AttributeProvider is not MemberInfo member ||
                !member.IsDefined(typeof(ViewOnlyAttribute), inherit: true))
            {
                continue;
            }

            var propertyName = property.Name;
            property.Set = (_, _) => throw new ViewOnlyPropertyException(propertyName);
        }
    }
}
