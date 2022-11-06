using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class BookingDetails
    {
        public int? User_id { get; set; }

        public int? User_track_id { get; set; }

        public string? From_location { get; set; }

        public string? To_location { get; set; }

        public string? From_latitude { get; set; }

        public string? From_longitude { get; set; }

        public string? To_latitude { get; set; }

        public string? To_longitude { get; set; }

        public string? Fare_date { get; set; }

        public string? Fare_type { get; set; }

        public string? Others_number { get; set; }

        public int? Vehicle_id { get; set; }

        public decimal? Kms { get; set; }

        public decimal? Cal_fare { get; set; }

        public int? Routed_driver_id { get; set; }

        public string? Fare_status { get; set; }

    }
}
