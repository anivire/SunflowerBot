using System.ComponentModel.DataAnnotations;

namespace Sunflower.Models
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
