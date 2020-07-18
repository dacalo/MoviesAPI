using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Gender, GenderDTO>().ReverseMap();
            CreateMap<GenderCreateDTO, Gender>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreateDTO, Actor>().ForMember(x => x.Image, options => options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<MovieCreateDTO, Movie>()
                .ForMember(x => x.Image, options => options.Ignore())
                .ForMember(x => x.MoviesGenders, options => options.MapFrom(MapMoviesGenders))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDetailsDTO>()
                .ForMember(x => x.Genders, options => options.MapFrom(MapMoviesGenders))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));

            CreateMap<MoviePatchDTO, Movie>().ReverseMap();

            CreateMap<Cinema, CinemaDTO>()
                .ForMember(x => x.Latitude, x => x.MapFrom(y => y.Location.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(x => x.Location.X));

            CreateMap<CinemaDTO, Cinema>()
                .ForMember(x => x.Location, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));

            CreateMap<CinemaCreateDTO, Cinema>()
                .ForMember(x => x.Location, x => x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));

            CreateMap<IdentityUser, UserDTO>();

            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.UserName, x => x.MapFrom(y => y.User.UserName));

            CreateMap<ReviewDTO, Review>();
            CreateMap<ReviewCreateDTO, Review>();
        }

        private List<MoviesGenders> MapMoviesGenders(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MoviesGenders>();
            if (movieCreateDTO.GendersId == null)
                return result;
            foreach (var id in movieCreateDTO.GendersId)
            {
                result.Add(new MoviesGenders() { GenderId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            if (movieCreateDTO.Actors == null)
                return result;
            foreach (var actor in movieCreateDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.ActorId, Character = actor.Character });
            }
            return result;
        }

        private List<GenderDTO> MapMoviesGenders(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenderDTO>();
            if (movie.MoviesGenders == null)
                return result;
            foreach(var genderMovie in movie.MoviesGenders)
            {
                result.Add(new GenderDTO() { Id = genderMovie.GenderId, Name = genderMovie.Gender.Name });
            }
            return result;
        }

        private List<ActorMovieDetailDTO> MapMoviesActors(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<ActorMovieDetailDTO>();
            if (movie.MoviesActors == null)
                return result;
            foreach (var actorMovie in movie.MoviesActors)
            {
                result.Add(new ActorMovieDetailDTO { ActorId = actorMovie.ActorId, Character = actorMovie.Character, NamePerson = actorMovie.Actor.Name });
            }
            return result;
        }
    }
}
