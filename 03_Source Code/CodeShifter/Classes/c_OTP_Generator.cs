using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/****************************************************************         
    *                                                           *   
    * Author:  J.Anish Dev                                      *   
    *                                                           *   
    * About :  Solution to NSA's CodeBreaker Challenge 2016     *   
    *          I'd love to hear from you if: You just want to   * 
    *          say hi, if you found this useful, if you         *
    *          use my code, if you solved task 6, or if         * 
    *          you want to team up for 2017's challenge ;)      *    
    *                                                           *
    * Contact me:                                               *
    *          www.dreamersion.com                              *
    *          anishdev@ufl.edu                                 *   
    ************************************************************/

namespace CodeShifter
{
    class c_OTP_Generator
    {
        public void CalculateOtpValues(string masterKeyFile, int serialNumber, ref int[] OTP_N)
        {
            int i = 0;
            var masterKey = File.ReadAllBytes(masterKeyFile);

            using (var hmacsha256 = new HMACSHA256(masterKey))
            {
                var secret = hmacsha256.ComputeHash(BitConverter.GetBytes(serialNumber));

                var unixTime = GetUnixTime(DateTime.Now) / 30;

                for (var timeFrame = unixTime - 1; timeFrame <= unixTime + 1; timeFrame++)
                {
                    using (var hmacsha1 = new HMACSHA1(secret))
                    {
                        var secret2 = hmacsha1.ComputeHash(new byte[] { 0x00, 0x00, 0x00, 0x00 }.Concat(BitConverter.GetBytes(timeFrame).Reverse()).ToArray());

                        var indexer = secret2[secret2.Length - 1] & 0xf;
                        var otp = ((secret2[indexer + 2] << 8 | secret2[indexer + 1] << 16 | secret2[indexer] << 24 | secret2[indexer + 3]) & 0x7FFFFFFF) % 0xF4240;

                        OTP_N[i]=(otp);
                    }
                    i += 1;
                }
            }
        }


        private static int GetUnixTime(DateTime dateTime)
        {
            return
                (int)
                Math.Floor(
                    (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }
    }
}
