using System.ComponentModel.DataAnnotations.Schema;
using VNUK.Models;

namespace VNUK.Dtos.Notices.NoticeInput
{
    public class NoticeOffsetInputDto
    {
        public string CourseClassID { get; set; }
        public List<string> ClassPeriodIDs { get; set; }
        public string RoomID { get; set; }
        public int WeekOffset { get; set; }
        public string Note { get; set; }

    }
}
