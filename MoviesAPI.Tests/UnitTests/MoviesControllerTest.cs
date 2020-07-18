using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class MoviesControllerTest : BaseTests
    {
        private string CreateDataTest()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var gender = new Gender() { Name = "genero 1" };

            var movies = new List<Movie>()
            {
                new Movie(){Title = "Película 1", PremireDate = new DateTime(2010, 1,1), InTheaters = false},
                new Movie(){Title = "No estrenada", PremireDate = DateTime.Today.AddDays(1), InTheaters = false},
                new Movie(){Title = "Película en Cines", PremireDate = DateTime.Today.AddDays(-1), InTheaters = true}
            };

            var moviesWithGender = new Movie()
            {
                Title = "Película con Género",
                PremireDate = new DateTime(2010, 1, 1),
                InTheaters = false
            };
            movies.Add(moviesWithGender);

            context.Add(gender);
            context.AddRange(movies);
            context.SaveChanges();

            var movieGender = new MoviesGenders() { GenderId = gender.Id, MovieId = moviesWithGender.Id };
            context.Add(movieGender);
            context.SaveChanges();

            return nameDB;
        }

        [TestMethod]
        public async Task FilterTitle()
        {
            var nameDB = CreateDataTest();
            var mapper = ConfigurationAutoMapper();
            var context = BuildContext(nameDB);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var titleMovie = "Película 1";

            var filterDTO = new FilterMoviesDTO()
            {
                Title = titleMovie,
                RecordsPage = 10
            };

            var response = await controller.Filters(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual(titleMovie, movies[0].Title);
        }

        [TestMethod]
        public async Task FilterMovies()
        {
            var nameDB = CreateDataTest();
            var mapper = ConfigurationAutoMapper();
            var context = BuildContext(nameDB);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                InTheaters = true
            };

            var response = await controller.Filters(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual("Película en Cines", movies[0].Title);
        }

        [TestMethod]
        public async Task FilterGender()
        {
            var nameDB = CreateDataTest();
            var mapper = ConfigurationAutoMapper();
            var context = BuildContext(nameDB);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var genderId = context.Genders.Select(x => x.Id).First();

            var filterDTO = new FilterMoviesDTO()
            {
                GenderId = genderId
            };

            var response = await controller.Filters(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Count);
            Assert.AreEqual("Película con Género", movies[0].Title);
        }

        [TestMethod]
        public async Task FilterTitleAscending()
        {
            var nameDB = CreateDataTest();
            var mapper = ConfigurationAutoMapper();
            var context = BuildContext(nameDB);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                FieldOrder = "Title",
                OrderAscending = true
            };

            var response = await controller.Filters(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(nameDB);
            var moviesDB = context2.Movies.OrderBy(x => x.Title).ToList();

            Assert.AreEqual(moviesDB.Count, movies.Count);

            for (int i = 0; i < moviesDB.Count; i++)
            {
                var movieController = movies[i];
                var movieDB = moviesDB[i];

                Assert.AreEqual(movieDB.Id, movieController.Id);
            }
        }

        [TestMethod]
        public async Task FilterTitleDescending()
        {
            var nameDB = CreateDataTest();
            var mapper = ConfigurationAutoMapper();
            var context = BuildContext(nameDB);

            var controller = new MoviesController(context, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                FieldOrder = "Title",
                OrderAscending = false
            };

            var response = await controller.Filters(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(nameDB);
            var moviesDB = context2.Movies.OrderByDescending(x => x.Title).ToList();

            Assert.AreEqual(moviesDB.Count, movies.Count);

            for (int i = 0; i < moviesDB.Count; i++)
            {
                var movieController = movies[i];
                var movieDB = moviesDB[i];

                Assert.AreEqual(movieDB.Id, movieController.Id);
            }
        }

        [TestMethod]
        public async Task FilterIncorrectField()
        {
            var nameDB = CreateDataTest();
            var mapper = ConfigurationAutoMapper();
            var context = BuildContext(nameDB);

            var mock = new Mock<ILogger<MoviesController>>();

            var controller = new MoviesController(context, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                FieldOrder = "abc",
                OrderAscending = true
            };

            var response = await controller.Filters(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(nameDB);
            var moviesDB = context2.Movies.ToList();
            Assert.AreEqual(moviesDB.Count, movies.Count);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
    }
}
