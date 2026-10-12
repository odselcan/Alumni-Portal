using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alumni_Portal.Models
{
    [Table("career_positions", Schema = "belek_graduate")]
    public class CareerPosition
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("faculty")]
        public string Faculty { get; set; } = string.Empty;
    }
}