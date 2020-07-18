using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class CinemasControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetCinemaLessThan5Kilometers()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            using (var context = LocalDataBaseInitializer.GetDbContextLocalDb(false))
            {
                var cinemas = new List<Cinema>()
                {
                    new Cinema{ Name = "Agora", Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233)) }
                };

                context.AddRange(cinemas);
                await context.SaveChangesAsync();
            }

            var filtro = new CinemaNearFilterDTO()
            {
                DistanceKms = 5,
                Latitude = 18.481139,
                Longitude = -69.938950
            };

            using (var context = LocalDataBaseInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurationAutoMapper();
                var controller = new CinemasController(context, mapper, geometryFactory);
                var response = await controller.Nearby(filtro);
                var value = response.Value;
                Assert.AreEqual(2, value.Count);
            }

        }
    }
}
