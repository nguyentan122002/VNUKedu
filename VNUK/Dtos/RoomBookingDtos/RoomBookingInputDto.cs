namespace VNUK.Dtos.RoomBookingDtos
{
    public class RoomBookingInputDto
    {
        public string RoomID { get; set; }

        public int WeekBooking {  get; set; }

        public List<string> ClassPeriodIDs { get; set; }

        public string Purpose { get; set; }

    }
}
