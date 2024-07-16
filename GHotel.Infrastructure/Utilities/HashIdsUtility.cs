using GHotel.Application.Utilities;
using HashidsNet;

namespace GHotel.Infrastructure.Utilities;

public class HashIdsUtility : IIdHasherUtility
{
    private readonly IHashids _hashids;

    public HashIdsUtility(IHashids hashids)
    {
        _hashids = hashids;
    }

    public string Encode(int id)
    {
        return _hashids.Encode(id);
    }
    public int Decode(string hash)
    {
        var numbers = _hashids.Decode(hash);
        return numbers.Length > 0 ? numbers[0] : 0;
    }
}
