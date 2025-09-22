using System.ComponentModel.DataAnnotations;
using GameStore.Application.Services.UserServices;

namespace GameStore.API.Models.CommentModels;

public class BanModel
{
    [Required]
    public string User { get; set; }

    [Required]
    [AllowedValues([
        BanDurations.Hour,
        BanDurations.Day,
        BanDurations.Week,
        BanDurations.Month,
        BanDurations.Permanent])]
    public string Duration { get; set; }
}
