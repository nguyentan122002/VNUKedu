namespace VNUK.Dtos.RoomBookingDto
{
    public class RoomBookingDto
    {
        public int BookingID { get; set; }

        public string RoomID { get; set; }

        public string RoomName { get; set; }

        public string BookedBy { get; set; }

        public string Purpose { get; set; }

        public DateTime CreatedDate { get; set; } 

        public int WeekBooking { get; set; }

        public string ClassPeriodName { get; set; }

        public string DayofWeekname { get; set; }

    }
}
