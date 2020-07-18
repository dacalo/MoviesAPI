using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class ReviewsControllerTests : BaseTests
    {
        [TestMethod]
        public async Task UserCantReviews()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            CreateMovies(nameDB);

            var movieId = context.Movies.Select(x => x.Id).First();
            var review1 = new Review()
            {
                MovieId = movieId,
                UserId = defaultUserId,
                Mark = 5
            };

            context.Add(review1);
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            var controller = new ReviewsController(context2, mapper);
            controller.ControllerContext = BuildControllerContext();

            var reviewCreateDTO = new ReviewCreateDTO { Mark = 5 };
            var response = await controller.Post(movieId, reviewCreateDTO);

            var value = response as IStatusCodeActionResult;
            Assert.AreEqual(400, value.StatusCode.Value);
        }

        [TestMethod]
        public async Task CreateReview()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            CreateMovies(nameDB);

            var movieId = context.Movies.Select(x => x.Id).First();
            var context2 = BuildContext(nameDB);

            var mapper = ConfigurationAutoMapper();
            var controller = new ReviewsController(context2, mapper);
            controller.ControllerContext = BuildControllerContext();

            var reviewCreateDTO = new ReviewCreateDTO() { Mark = 5 };
            var response = await controller.Post(movieId, reviewCreateDTO);

            var value = response as NoContentResult;
            Assert.IsNotNull(value);

            var context3 = BuildContext(nameDB);
            var reviewDB = context3.Reviews.First();
            Assert.AreEqual(defaultUserId, reviewDB.UserId);
        }

        private void CreateMovies(string nameDB)
        {
            var context = BuildContext(nameDB);

            context.Movies.Add(new Movie() { Title = "pelicula 1" });

            context.SaveChanges();
        }
    }
}
