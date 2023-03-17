

namespace MileDALibrary.Models
{
    using System;
    public class UserBookSearchModel
    {
        public int? Search_Id { get; set; }

        public int? User_Id { get; set; }

        public string? Name { get; set; }

        public string? Gender { get; set; }

        public string? Phone_Num { get; set; }

        public string? From_Location { get; set; }

        public string? To_Location { get; set; }

        public string? From_Latitude { get; set; }

        public string? From_Longitude { get; set; }

        public string? To_Latitude { get; set; }

        public string? To_Longitude { get; set; }

        public string? Fare_Date { get; set; }

        public string? Fare_Type { get; set; }

        public string? Fare_Status { get; set; }

        public string? Others_Number { get; set; }

        public int? Vehicle_Id { get; set; }

        public Decimal? Kms { get; set; }

        public Decimal? Cal_Fare { get; set; }

        public string? Comments { get; set; }

        public int? Routed_Driver_Id { get; set; }

        public decimal Distance_In_Kms { get; set; }
    }
}
