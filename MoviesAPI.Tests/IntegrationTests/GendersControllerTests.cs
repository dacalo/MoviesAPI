using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.IntegrationTests
{
    [TestClass]
    public class GendersControllerTests : BaseTests
    {
        private static readonly string url = "/api/genders";

        [TestMethod]
        public async Task GetAllGendersEmpty()
        {
            var nameDB = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDB);

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genders = JsonConvert
                .DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(0, genders.Count);
        }

        [TestMethod]
        public async Task GetAllGenders()
        {
            var nameDB = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDB);

            var contexto = BuildContext(nameDB);
            contexto.Genders.Add(new Gender() { Name = "Género 1" });
            contexto.Genders.Add(new Gender() { Name = "Género 2" });
            await contexto.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genders = JsonConvert
                .DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(2, genders.Count);
        }

        [TestMethod]
        public async Task DeleteGender()
        {
            var nameDB = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDB);

            var context = BuildContext(nameDB);
            context.Genders.Add(new Gender() { Name = "Género 1" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.DeleteAsync($"{url}/1");
            response.EnsureSuccessStatusCode();

            var context2 = BuildContext(nameDB);
            var exist = await context2.Genders.AnyAsync();
            Assert.IsFalse(exist);
        }

        [TestMethod]
        public async Task DeleteGenderReturn401()
        {
            var nameDb = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDb, ignoreSecurity: false);

            var client = factory.CreateClient();
            var response = await client.DeleteAsync($"{url}/1");
            Assert.AreEqual("Unauthorized", response.ReasonPhrase);
        }
    }
}
