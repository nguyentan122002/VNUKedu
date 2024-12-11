using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Major", Schema = "dbo")]
    public class Majors
    {
        [Key]
        public string MajorID { get; set; }

        public string MajorName { get; set; }
    }
}
