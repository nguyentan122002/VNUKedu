using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VNUK.Dtos.Notices.NoticeInput
{
    public class NoticeOfLeaveInputDto
    {
        [Required]
        public string CourClassID { get; set; }

        public string Reason { get; set; }

        //public DateTime CreatedDate { get; set; } 

        [Required]
        public int WeekOff {  get; set; }

    }
}
