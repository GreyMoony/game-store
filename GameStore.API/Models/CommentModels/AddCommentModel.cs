using System.Text.Json.Serialization;
using GameStore.API.Attributes;
using GameStore.Application.Services.CommentServices;

namespace GameStore.API.Models.CommentModels;

public class AddCommentModel
{
    public CommentModel Comment { get; set; }

    [JsonConverter(typeof(NullableGuidConverter))]
    [ParentIdValidation]
    public Guid? ParentId { get; set; }

    [AllowedValues(CommentActions.Quote, CommentActions.Reply, CommentActions.Comment)]
    public string Action { get; set; }
}
