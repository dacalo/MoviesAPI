using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using NetTopologySuite;
using System.Linq;
using System.Security.Claims;

namespace MoviesAPI.Tests
{
    public class BaseTests
    {
        protected string defaultUserId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        protected string defaultUserEmail = "ejemplo@hotmail.com";

        protected ApplicationDbContext BuildContext(string nameDB)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nameDB).Options;

            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        protected IMapper ConfigurationAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        protected ControllerContext BuildControllerContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, defaultUserEmail),
                new Claim(ClaimTypes.Email, defaultUserEmail),
                new Claim(ClaimTypes.NameIdentifier, defaultUserId)
            }));

            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
        
        protected WebApplicationFactory<Startup> BuildWebApplicationFactory(string nameDB,
            bool ignoreSecurity = true)
        {
            var factory = new WebApplicationFactory<Startup>();

            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptorDBContext = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptorDBContext != null)
                    {
                        services.Remove(descriptorDBContext);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(nameDB));

                    if (ignoreSecurity)
                    {
                        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new FakeUserFilter());
                        });
                    }
                });
            });

            return factory;
        }
        
    }
}
