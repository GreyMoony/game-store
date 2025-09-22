using System.Text.Json.Serialization;
using GameStore.API.Attributes;
using GameStore.Domain.Enums;

namespace GameStore.API.Models.UserModels;

public class NotificationMethodsUpdateModel
{
    [JsonConverter(typeof(CaseInsensitiveEnumListConverter<NotificationMethodType>))]
    public List<NotificationMethodType> Notifications { get; set; }
}
