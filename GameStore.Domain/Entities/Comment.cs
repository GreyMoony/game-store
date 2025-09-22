using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;
public class Comment : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Body { get; set; }

    public Guid? ParentCommentId { get; set; }

    [Required]
    public Guid GameId { get; set; }

    public Comment ParentComment { get; set; }

    public Game Game { get; set; }

    public ICollection<Comment> ChildComments { get; set; }
}
