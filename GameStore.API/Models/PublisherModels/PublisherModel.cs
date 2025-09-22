using System.ComponentModel.DataAnnotations;
using GameStore.API.Attributes;

namespace GameStore.API.Models.PublisherModels;

public class PublisherModel
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string CompanyName { get; set; }

    public string? Description { get; set; }

    [OptionalUrl]
    public string? HomePage { get; set; }
}
