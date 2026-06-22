using LogicBuilder.Domain;

namespace LogicBuilder.App.Bsl.Utils.Tests.Models
{
    public class CourseAssignmentModel : BaseModel
    {
		public int InstructorID { get; set; }

		public int CourseID { get; set; }

        public string? CourseTitle { get; set; }

        public string CourseNumberAndTitle { get; set; } = "";

        public string? Department { get; set; }
    }
}