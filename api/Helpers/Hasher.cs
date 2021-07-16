using System.Security.Cryptography;
using System.Text;

namespace api.Helpers
{
    public class Hasher
    {

        private HMACSHA512 hmac { get; set; }

        public Hasher(byte[] passwordSalt){
            hmac = new HMACSHA512(passwordSalt);  
        }

        public Hasher(){
            hmac = new HMACSHA512();  
        }
  
        public byte[] ComputePasswordHash(string password){
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public byte[] GetPasswordSalt(){
            return hmac.Key;
        }
    }
}