using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class GendersControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetAllGenders()
        {
            //Preparation
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            context.Genders.Add(new Gender { Name = "Género 1" });
            context.Genders.Add(new Gender { Name = "Género 2" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);

            //Test
            var controller = new GendersController(context2, mapper);
            var response = await controller.Get();

            //Check
            var genders = response.Value;
            Assert.AreEqual(2, genders.Count);
        }

        [TestMethod]
        public async Task GetGenderByIdNotExist()
        {
            //Preparation
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            //Test
            var controller = new GendersController(context, mapper);
            var response = await controller.Get(1);

            //Check
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetGenderByIdExist()
        {
            //Preparation
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            context.Genders.Add(new Gender { Name = "Género 1" });
            context.Genders.Add(new Gender { Name = "Género 2" });
            context.SaveChanges();
            var context2 = BuildContext(nameDB);

            //Test
            var controller = new GendersController(context2, mapper);
            var id = 1;
            var response = await controller.Get(id);

            //Check
            var result = response.Value;
            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        public async Task CreateGender()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            var newGender = new GenderCreateDTO { Name = "Nuevo Género" };
            var controller = new GendersController(context, mapper);

            var response = await controller.Post(newGender);

            var result = response as CreatedAtRouteResult;

            Assert.IsNotNull(result);

            var context2 = BuildContext(nameDB);
            var quantity = await context2.Genders.CountAsync();
            Assert.AreEqual(1, quantity);
        }

        [TestMethod]
        public async Task UpdateGender()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            context.Genders.Add(new Gender { Name = "Género 1" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var controller = new GendersController(context2, mapper);

            var genderCreateDTO = new GenderCreateDTO { Name = "Nuevo nombre" };
            var id = 1;
            var response = await controller.Put(id, genderCreateDTO);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(nameDB);
            var exist = await context3.Genders.AnyAsync(x => x.Name.Equals("Nuevo nombre"));
            Assert.IsTrue(exist);
        }

        [TestMethod]
        public async Task DeleteGender()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            context.Genders.Add(new Gender { Name = "Género 1" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var controller = new GendersController(context2, mapper);

            var response = await controller.Delete(1);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);


            var context3 = BuildContext(nameDB);
            var exist = await context3.Genders.AnyAsync();
            Assert.IsFalse(exist);
        }
    }
}
