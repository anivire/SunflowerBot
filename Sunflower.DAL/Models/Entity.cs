using System.ComponentModel.DataAnnotations;

namespace Sunflower.DAL.Models
{
    public abstract class Entity
    {
        [Key]
        public ulong Id { get; set; }
    }
}
