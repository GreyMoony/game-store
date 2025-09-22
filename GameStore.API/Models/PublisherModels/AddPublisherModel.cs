using System.ComponentModel.DataAnnotations;
using GameStore.API.Attributes;

namespace GameStore.API.Models.PublisherModels;

public class AddPublisherModel
{
    [Required]
    public string CompanyName { get; set; }

    [OptionalUrl]
    public string? HomePage { get; set; }

    public string? Description { get; set; }
}