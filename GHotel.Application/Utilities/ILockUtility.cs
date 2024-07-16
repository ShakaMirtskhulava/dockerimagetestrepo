namespace GHotel.Application.Utilities;

public interface ILockUtility
{
    Task Lock(string key);
    void Release(string key);
}
