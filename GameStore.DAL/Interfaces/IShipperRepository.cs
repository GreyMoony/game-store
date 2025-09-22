namespace GameStore.DAL.Interfaces;
public interface IShipperRepository
{
    Task<IEnumerable<Dictionary<string, object>>> GetAll();
}
