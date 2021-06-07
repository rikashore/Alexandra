using System.Linq;
using System.Reflection;
using Alexandra.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Alexandra.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLexServices(this IServiceCollection services)
        {
            var baseType = typeof(LexService);
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.IsAssignableTo(baseType) && !x.IsAbstract);

            foreach (var type in types)
                services.AddSingleton(type);

            return services;
        }
    }
}