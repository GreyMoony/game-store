using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.CommentModels;

public class CommentModel
{
    [Required]
    public string Body { get; set; }
}
