using LogicBuilder.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data
{
    [Table("CourseAssignment")]
    public class CourseAssignment : BaseData
    {
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public Instructor? Instructor { get; set; }
        public Course? Course { get; set; }
    }
}
