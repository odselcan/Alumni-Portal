using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alumni_Portal.Models
{
    [Table("departments", Schema = "belek_graduate")]
    public class Department
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("faculty_id")]
        public int FacultyId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }
    }
}