using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

namespace CodeShift
{
    //##################### RUNNING IN 64 BIT WILL CAUSE THIS TO OVERLOOK THE SOLUTION
    //I was fortunate to find this out by sheer chance when I was working on the task.
    //If you're able to figure out why, do let me know.

    class c_ParellelBrute
    {
        public string currentDate="Ready";

        public static string deviceSerialNumber;

        //You can hard code Bytes for efficiency
        public static byte[] deviceSerial;

        //You can hard code Bytes for efficiency
        public static  byte[] TargetSecretKey;

        //Lookup String taken from asm
        private static readonly string byte_4FF020 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567=";
        
        //For testing with windows
        private static int win_Random(ref int seed)
        {
            seed = seed * 214013 + 2531011;
            return (seed >> 0x10) & 0x7fff;
        }

        public static int GetUnixTime(DateTime dateTime)
        {
            return
                (int)
                Math.Floor(
                    (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }

        private static bool ValidateSecretKey(int currentSeed)
        {
         
            var randomNumbers = new int[8];

            //Make 8 random integers, Unix style
            var tGNUR = new GnuRand((UInt32)currentSeed);
            for (int idx = 0; idx < 8; idx++)
            { randomNumbers[idx] = tGNUR.Next(); }

            //Then, use that to generate a random set of 32 bytes
            byte[] initialByteSeed = new byte[randomNumbers.Length * sizeof(int)];
            Buffer.BlockCopy(randomNumbers, 0, initialByteSeed, 0, initialByteSeed.Length);

            //1024 SHA256 operations on the same set of bytes
            var hasher = System.Security.Cryptography.SHA256Managed.Create();
            for (int idx = 0; idx < 1024; idx++)
                initialByteSeed = hasher.ComputeHash(initialByteSeed);
            hasher.Dispose();

            //Prepare HMAC hash
            HMACSHA256 hmc = new HMACSHA256(initialByteSeed);
            byte[] HMAC_Hash = hmc.ComputeHash(deviceSerial);
            hmc.Dispose();

            //Reverse Dword sections of the HMAC hash (because they're integers?)
            //for (int idx = 0; idx < 32; idx += 4)
            //    Array.Reverse(HMAC_Hash, idx, 4);
            //This isn't needed.

            //SecretKeyBuilder
            int a2 = 32;
            byte[] result = new byte[8 * a2 / 5 + 8];

            int v3 = 0;
            byte v12 = 0;
            byte v11 = 0;
            byte v5;
            byte v7;

            int v4 = 0;
            char v9;
            byte[] a1 = HMAC_Hash;
            byte  v6;
            byte v8;
            int v10;

            do
            {
                v9 = (char) a1[v3]; 
                result[v12] = (byte) byte_4FF020[(byte)((a1[v3])>> 3)]; 
                if (v4 + 1 == a2)
                {
                    result[v12 + 2] = 0;
                    result[v12 + 1] = (byte)byte_4FF020[4 * (v9 & 7)]; 
                }
                else
                {
                    v5 = (a1[v3 + 1]);;
                    result[v12 + 1] = (byte)byte_4FF020[(byte)(a1[v3 + 1] >> 6) | 4 * (v9 & 7)]; 
                    result[v12 + 2] = (byte)byte_4FF020[(v5 >> 1) & 0x1F];
                    if (v4 + 2 == a2)
                    {
                        result[v12 + 4] = 0;
                        result[v12 + 3] = (byte)byte_4FF020[16 * (v5 & 1)];
                    }
                    else
                    {
                        v6 = (a1 [v3 + 2]);
                        result[v12 + 3] = (byte)byte_4FF020[(byte)(a1[v3 + 2] >> 4) | 16 * (v5 & 1)];
                        if (v4 + 3 == a2)
                        {
                            result[v12 + 5] = 0;
                            result[v12 + 4] = (byte) byte_4FF020[2 * (v6 & 0xF)]; 
                        }
                        else
                        {
                            v7 = (a1[v3 + 3]);
                            result[v12 + 4] = (byte) byte_4FF020[(byte)(a1[v3 + 3] >> 7) | 2 * (v6 & 0xF)]; 
                            result[v12 + 5] = (byte) byte_4FF020[(v7 >> 2) & 0x1F]; 
                            if (v4 + 4 == a2)
                            {
                                result[v12 + 7] = 0;
                                result[v12 + 6] = (byte) byte_4FF020[8 * (v7 & 3)]; 
                            }
                            else
                            {
                                v8 = (byte) byte_4FF020[(byte)(a1[v3 + 4] >> 5) | 8 * (v7 & 3)]; 
                                result[v12 + 7] = (byte) byte_4FF020[(a1[v3 + 4] & 0x1F)]; 
                                result[v12 + 6] = v8;
                            }
                        }
                    }
                }
                v11 += 5;
                v12 += 8;
                v4 = v11;
                v3 = v11;
            }
            while (a2 > v11);
            v10 = v12;
            result[v10] = 0;
            //SecretKeyBuilder Ends here

            for (int idx = 0; idx < 52; idx++)
            {
                if (TargetSecretKey[idx] == result[idx])
                    continue;
                else
                    return false;
            }
            return true;          
        }


        public void doBruteParallel(DateTime startDate)
        {
            bool result = false;
            int seed=0;

            for (var idx = 0; ; idx--)
            {
                var testDate = startDate.AddDays(idx);

                var fromSeed = GetUnixTime(testDate);
                var toSeed = GetUnixTime(testDate.AddDays(1)) - 1;

                currentDate = "Testing: " + testDate;

                Parallel.For(fromSeed, toSeed, (currentSeed, loopState) =>
                {
                    var secretKey = ValidateSecretKey(currentSeed);
                    if (secretKey==false)
                        return;

                    result = true;
                    seed = currentSeed;

                    loopState.Stop();
                });


                if (result == true)
                {
                    writeMasterKey(ref seed);
                    break;
                }

            }
        }

        public  void writeMasterKey(ref int parr_seed)
        {
            var randomNumbers = new int[8];

            //Generate random numbers
            var tGNUR = new GnuRand((UInt32)parr_seed);
            for (int idx = 0; idx < 8; idx++)
            { randomNumbers[idx] = tGNUR.Next(); }
            
            //Convert them to bytes
            byte[] masterBytes = new byte[randomNumbers.Length * sizeof(int)];
            Buffer.BlockCopy(randomNumbers, 0, masterBytes, 0, masterBytes.Length);

            //Compute SHA256 1024 times
            var hasher = System.Security.Cryptography.SHA256Managed.Create();
            for (int idx = 0; idx < 1024; idx++)
                masterBytes = hasher.ComputeHash(masterBytes);

            //Write to file
            ByteArrayToFile(deviceSerialNumber, masterBytes);

            Console.WriteLine();
            Console.WriteLine("Master File written");
            currentDate = "Master file written. Password cracked: " + parr_seed;
         }

        private static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }
            return false;
        }


    }
}
