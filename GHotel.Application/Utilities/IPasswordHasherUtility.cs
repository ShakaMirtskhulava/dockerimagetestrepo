namespace GHotel.Application.Utilities;

public interface IPasswordHasherUtility
{
    string GenerateHash(string password);
    bool VerifyHash(string passwordHash, string givenPassword);
}
