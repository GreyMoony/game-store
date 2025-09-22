using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;
public class Platform : BaseEntity
{
    [Required]
    public string Type { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation property
    public ICollection<GamePlatform> GamePlatforms { get; set; }
}
