using LogicBuilder.Domain;
using System.ComponentModel.DataAnnotations;


namespace LogicBuilder.App.Bsl.Utils.Tests.Models
{
    public class EnrollmentModel : BaseModel
    {
		public int EnrollmentID { get; set; }

		public int CourseID { get; set; }

		public int StudentID { get; set; }

		[DisplayFormat(NullDisplayText = "No grade")]
		public Grade? Grade { get; set; }

        public string? GradeLetter { get; set; }

        public string? CourseTitle { get; set; }

        public string? StudentName { get; set; }
    }
}