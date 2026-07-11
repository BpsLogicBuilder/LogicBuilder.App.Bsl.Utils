using LogicBuilder.App.Bsl.Utils;
using LogicBuilder.App.Bsl.Utils.Interfaces;

#pragma warning disable IDE0130 //Microsoft recommended namespace for service registrations
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130
{
    public static class AddBslUtilsServiceRegistrations
    {
        public static IServiceCollection AddBslUtilsServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IDeleteOperations, DeleteOperations>()
                .AddTransient<IProjectionOperations, ProjectionOperations>()
                .AddTransient<IQueryOperations, QueryOperations>()
                .AddTransient<IRequestHelper, RequestHelper>();
        }
    }
}
