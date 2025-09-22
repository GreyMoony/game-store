using GameStore.Domain.Entities;

namespace GameStore.Application.DTOs.GameDtos;
public class CachedCommonGames
{
    public List<ICommonGame> Games { get; set; }

    public int TotalFilteredCount { get; set; }
}
