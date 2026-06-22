using LogicBuilder.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace LogicBuilder.App.Bsl.Utils.Tests.Models
{
    public class DepartmentModel : BaseModel
    {
		public int DepartmentID { get; set; }

		[StringLength(50, MinimumLength = 3)]
		public string Name { get; set; } = "";

		[DataType(DataType.Currency)]
		public decimal Budget { get; set; }

        public string BudgetString { get; set; } = "";

        [DataType(DataType.Date)]
		[DisplayFormat(DataFormatString  = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Start Date")]
		public System.DateTime StartDate { get; set; }

        public string StartDateString { get; set; } = "";

        public int? InstructorID { get; set; }

		public byte[]? RowVersion { get; set; }

        public string? AdministratorName { get; set; }

		public ICollection<CourseModel>? Courses { get; set; }
    }
}