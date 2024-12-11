using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("RoomBooking", Schema = "dbo")]
    public class RoomBooking
    {
        [Key]
        public int BookingID { get; set; }

        public string RoomID { get; set; }

        public string BookedBy { get; set; }

        public string Purpose { get; set; }

        public DateTime CreatedDate {  get; set; } = DateTime.Now;

        public int WeekBooking {  get; set; }

        public string ClassPeriodID { get; set; }
    }
}
