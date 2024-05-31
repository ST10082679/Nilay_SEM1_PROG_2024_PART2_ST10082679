
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nilay_SEM1_PROG_2024_PART2_ST10082679.Helpers
{
    public class PasswordHasher
    {
        private int nSalt = 32; //Byte number for the salt
        private int nHash = 32; //Byte number for the Hash
        private int nIterations = 1000; // Iterations number for the hashing algorithm 
        //--------------------------------------------------------------------------------------//
        //Random salt generated
        public string GenerateSalt()
        {
            var saltBytes = new byte[this.nSalt];

            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetNonZeroBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }
        //--------------------------------------------------------------------------------------//
        //This will hash the password with the given salt 
        public string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, this.nIterations))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(this.nHash));
            }
        }
        //--------------------------------------------------------------------------------------//
        //this will verify the passwrod by rehashing it and comparing it to the stored hash password 
        public bool Verifypassword(string password, string hashedPassword, string salt)
        {
            string newHashed = HashPassword(password, salt);
            return newHashed.Equals(hashedPassword);
        }
    }
}
//---------------------------------End of FIle-----------------------------------------------------//
