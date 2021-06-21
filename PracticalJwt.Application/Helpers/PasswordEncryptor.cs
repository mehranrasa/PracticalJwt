using NETCore.Encrypt;

namespace PracticalJwt.Application.Helpers
{
    public class PasswordEncryptor
    {
        private const string key = "4t7w!z%C*F-JaNdRgUkXp2r5u8x/A?D(";

        public string Encrypt(string password)
        {
            return EncryptProvider.AESEncrypt(password, key);
        }
    }
}
