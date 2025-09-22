namespace GameStore.Application.DTOs.CommentDtos;
public class AddCommentDto
{
    public string GameKey { get; set; }

    public string Action { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public Guid? ParentId { get; set; }
}
