using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alumni_Portal.Models
{
    [Table("faculties", Schema = "belek_graduate")]
    public class Faculty
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}