using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.IntegrationTests
{
    [TestClass]
    public class ReviewsControllerTests : BaseTests
    {
        private static readonly string url = "/api/movies/1/reviews";

        [TestMethod]
        public async Task GetReviewsReturn404NonExistMovie()
        {
            var nameDB = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDB);

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.AreEqual(404, (int)response.StatusCode);
        }

        [TestMethod]
        public async Task GetReviewsReturnListEmpty()
        {
            var nameDB = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDB);
            var context = BuildContext(nameDB);
            context.Movies.Add(new Movie() { Title = "Película 1" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var reviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(0, reviews.Count);
        }
    }
}
