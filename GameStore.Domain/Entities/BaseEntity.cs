using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;
public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}
