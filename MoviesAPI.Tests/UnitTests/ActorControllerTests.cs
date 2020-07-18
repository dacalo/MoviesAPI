using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Tests.UnitTests
{
    [TestClass]
    public class ActorControllerTests : BaseTests
    {
        [TestMethod]
        public async Task GetActorsPagination()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            context.Actors.Add(new Entities.Actor { Name = "Actor1 " });
            context.Actors.Add(new Entities.Actor { Name = "Actor2 " });
            context.Actors.Add(new Entities.Actor { Name = "Actor3 " });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var controller = new ActorsController(context2, mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var page1 = await controller.Get(new PaginationDTO { Page = 1, RecordsPage = 2 });
            var result1 = page1.Value;
            Assert.AreEqual(2, result1.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var page2 = await controller.Get(new PaginationDTO { Page = 2, RecordsPage = 2 });
            var result2 = page2.Value;
            Assert.AreEqual(1, result2.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var page3 = await controller.Get(new PaginationDTO { Page = 3, RecordsPage = 3 });
            var result3 = page3.Value;
            Assert.AreEqual(0, result3.Count);

        }

        [TestMethod]
        public async Task CreateActorWithOutPhoto()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            var actor = new ActorCreateDTO { Name = "Felipe", Birthday = DateTime.Now };

            var mock = new Mock<IFilesStore>();
            mock.Setup(x => x.SaveFile(null, null, null, null)).Returns(Task.FromResult("url"));

            var controller = new ActorsController(context, mapper, mock.Object);
            var response = await controller.Post(actor);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(nameDB);
            var list = await context2.Actors.ToListAsync();
            Assert.AreEqual(1, list.Count);
            Assert.IsNull(list[0].Image);

            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CreateActorWithPhoto()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            var content = Encoding.UTF8.GetBytes("Image Test");
            var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "image.jpg");

            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";

            var actor = new ActorCreateDTO
            {
                Name = "New Actor",
                Birthday = DateTime.Now,
                Image = file
            };

            var mock = new Mock<IFilesStore>();
            mock.Setup(x => x.SaveFile(content, ".jpg", "actors", file.ContentType))
                .Returns(Task.FromResult("url"));

            var controller = new ActorsController(context, mapper, mock.Object);
            var response = await controller.Post(actor);
            var result = response as CreatedAtRouteResult;
            Assert.AreEqual(201, result.StatusCode);

            var context2 = BuildContext(nameDB);
            var list = await context2.Actors.ToListAsync();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("url", list[0].Image);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task PatchReturn404WithoutActorNotExist()
        {
            var nameDB = Guid.NewGuid().ToString();
            var contexto = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            var controller = new ActorsController(contexto, mapper, null);
            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            var response = await controller.Patch(1, patchDoc);
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task PatchUpdateOnlyOneField()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);
            var mapper = ConfigurationAutoMapper();

            var birthday = DateTime.Now;
            var actor = new Actor { Name = "Felipe", Birthday= birthday };
            context.Add(actor);
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var controller = new ActorsController(context2, mapper, null);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;

            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            patchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace", "/Name", null, "Claudia"));
            var response = await controller.Patch(1, patchDoc);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result.StatusCode);

            var context3 = BuildContext(nameDB);
            var actorDB = await context3.Actors.FirstAsync();
            Assert.AreEqual("Felipe", actorDB.Name);
            Assert.AreEqual(birthday, actorDB.Birthday);
        }
    }
}
