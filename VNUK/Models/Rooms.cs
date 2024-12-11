using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Rooms", Schema = "dbo")]
    public class Rooms
    {
        [Key]
        public string RoomID { get; set; }

        public string RoomName { get; set; }   

        public int NumberOfSeats { get; set; }

        public string RoomType { get; set; }


    }
}
