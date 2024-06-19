using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTM.DataAccess.IRepository;
using MTM.DataAccess.Repository;
using MTM.Services.IService;
using MTM.Services.Service;
using static MTM.Services.Helpers.AutoMapperProfiles;

namespace MTM.Services.IOC
{
    public static class DependencyInjection
    {
        public static void InjectDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            // Mapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Category
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Category
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserRepository, UserRepository>();

        }
    }
}
