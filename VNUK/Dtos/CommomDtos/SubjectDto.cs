using System.ComponentModel.DataAnnotations.Schema;
using VNUK.Models;

namespace VNUK.Dtos.CommomDtos
{
    public class SubjectDto
    {
        public string SubjectsID { get; set; }

        public string SubjectsName { get; set; }

        public int NumberOfCredits { get; set; }

        public int WeekStart { get; set; }

        public int WeekEnd { get; set; }

        public string MajorName { get; set; }
    }
}
