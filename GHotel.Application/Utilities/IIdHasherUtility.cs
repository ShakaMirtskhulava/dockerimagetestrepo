namespace GHotel.Application.Utilities;

public interface IIdHasherUtility
{
    string Encode(int id);
    int Decode(string hash);
}
