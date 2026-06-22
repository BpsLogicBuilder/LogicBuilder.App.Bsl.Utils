using LogicBuilder.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data
{
    [Table("OfficeAssignment")]
    public class OfficeAssignment : BaseData
    {
        [Key]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }
        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string? Location { get; set; }

        
        public virtual Instructor? Instructor { get; set; }
    }
}
