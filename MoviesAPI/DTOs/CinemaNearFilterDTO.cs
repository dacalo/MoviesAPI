using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CinemaNearFilterDTO
    {
        private int _distanceKms = 10;
        private int _distanceMaxKms = 50;

        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
        public int DistanceKms 
        {
            get => _distanceKms;
            set
            {
                _distanceKms = (value > _distanceMaxKms) ? _distanceMaxKms : value;
            }
        }

    }
}
