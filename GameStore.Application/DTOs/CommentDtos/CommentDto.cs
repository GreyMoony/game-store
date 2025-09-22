namespace GameStore.Application.DTOs.CommentDtos;
public class CommentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public IEnumerable<CommentDto> ChildComments { get; set; }
}
