using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesActors>()
                .HasKey(x => new { x.ActorId, x.MovieId });

            modelBuilder.Entity<MoviesGenders>()
                .HasKey(x => new { x.GenderId, x.MovieId });

            modelBuilder.Entity<MoviesCinemas>()
                .HasKey(x => new { x.CinemaId, x.MovieId });

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {

            var rolAdminId = "9aae0b6d-d50c-4d0a-9b90-2a6873e3845d";
            var userAdminId = "5673b8cf-12de-44f6-92ad-fae4a77932ad";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();

            var username = "felipe@hotmail.com";

            var userAdmin = new IdentityUser()
            {
                Id = userAdminId,
                UserName = username,
                NormalizedUserName = username,
                Email = username,
                NormalizedEmail = username,
                PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
            };

            //modelBuilder.Entity<IdentityUser>()
            //    .HasData(userAdmin);

            //modelBuilder.Entity<IdentityRole>()
            //    .HasData(rolAdmin);

            //modelBuilder.Entity<IdentityUserClaim<string>>()
            //    .HasData(new IdentityUserClaim<string>()
            //    {
            //        Id = 1,
            //        ClaimType = ClaimTypes.Role,
            //        UserId = userAdminId,
            //        ClaimValue = "Admin"
            //    });

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<Cinema>()
               .HasData(new List<Cinema>
               {
                    //new SalaDeCine{Id = 1, Nombre = "Agora", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))},
                    new Cinema{Id = 4, Name = "Sambil", Location = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))},
                    new Cinema{Id = 5, Name = "Megacentro", Location = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))},
                    new Cinema{Id = 6, Name = "Village East Cinema", Location = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
               });

            var aventura = new Gender() { Id = 4, Name = "Aventura" };
            var animation = new Gender() { Id = 5, Name = "Animación" };
            var suspenso = new Gender() { Id = 6, Name = "Suspenso" };
            var romance = new Gender() { Id = 7, Name = "Romance" };

            modelBuilder.Entity<Gender>()
                .HasData(new List<Gender>
                {
                    aventura, animation, suspenso, romance
                });

            var jimCarrey = new Actor() { Id = 5, Name = "Jim Carrey", Birthday = new DateTime(1962, 01, 17) };
            var robertDowney = new Actor() { Id = 6, Name = "Robert Downey Jr.", Birthday = new DateTime(1965, 4, 4) };
            var chrisEvans = new Actor() { Id = 7, Name = "Chris Evans", Birthday = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Actor>()
                .HasData(new List<Actor>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Movie()
            {
                Id = 2,
                Title = "Avengers: Endgame",
                InTheaters = true,
                PremireDate = new DateTime(2019, 04, 26)
            };

            var iw = new Movie()
            {
                Id = 3,
                Title = "Avengers: Infinity Wars",
                InTheaters = false,
                PremireDate = new DateTime(2019, 04, 26)
            };

            var sonic = new Movie()
            {
                Id = 4,
                Title = "Sonic the Hedgehog",
                InTheaters = false,
                PremireDate = new DateTime(2020, 02, 28)
            };
            var emma = new Movie()
            {
                Id = 5,
                Title = "Emma",
                InTheaters = false,
                PremireDate = new DateTime(2020, 02, 21)
            };
            var wonderwoman = new Movie()
            {
                Id = 6,
                Title = "Wonder Woman 1984",
                InTheaters = false,
                PremireDate = new DateTime(2020, 08, 14)
            };

            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie>
                {
                    endgame, iw, sonic, emma, wonderwoman
                });

            modelBuilder.Entity<MoviesGenders>().HasData(
                new List<MoviesGenders>()
                {
                    new MoviesGenders(){MovieId = endgame.Id, GenderId = suspenso.Id},
                    new MoviesGenders(){MovieId = endgame.Id, GenderId = aventura.Id},
                    new MoviesGenders(){MovieId = iw.Id, GenderId = suspenso.Id},
                    new MoviesGenders(){MovieId = iw.Id, GenderId = aventura.Id},
                    new MoviesGenders(){MovieId = sonic.Id, GenderId = aventura.Id},
                    new MoviesGenders(){MovieId = emma.Id, GenderId = suspenso.Id},
                    new MoviesGenders(){MovieId = emma.Id, GenderId = romance.Id},
                    new MoviesGenders(){MovieId = wonderwoman.Id, GenderId = suspenso.Id},
                    new MoviesGenders(){MovieId = wonderwoman.Id, GenderId = aventura.Id},
                });

            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors(){MovieId = endgame.Id, ActorId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = endgame.Id, ActorId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = iw.Id, ActorId = robertDowney.Id, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieId = iw.Id, ActorId = chrisEvans.Id, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieId = sonic.Id, ActorId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1}
                });
        }

        public DbSet<Gender> Genders { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<MoviesGenders> MoviesGenders { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MoviesCinemas> MoviesCinemas { get; set; }
        public DbSet<Review> Reviews { get; set; }

    }
}
